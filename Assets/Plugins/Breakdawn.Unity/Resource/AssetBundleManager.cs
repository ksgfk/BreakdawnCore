using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Breakdawn.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Breakdawn.Unity
{
    internal struct AssetBundleRef
    {
        public readonly AssetBundle assetBundle;
        internal int refCount;

        public AssetBundleRef(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
            refCount = 0;
        }
    }

    internal class AssetBundleManager : Singleton<AssetBundleManager>
    {
        /// <summary>
        /// 资源配置表，key:资源名，value:资源信息
        /// </summary>
        private readonly Dictionary<string, AssetInfo> _nameDict = new Dictionary<string, AssetInfo>();

        /// <summary>
        /// 已加载的AB包，key:AB包名，value:实例
        /// </summary>
        private readonly Dictionary<string, AssetBundleRef> _abDict = new Dictionary<string, AssetBundleRef>();

        private bool _isInit;

        private AssetBundleManager()
        {
        }

        public void Init(string path)
        {
            if (_isInit)
            {
                Debug.LogError($"不可重复初始化!");
                return;
            }

            _isInit = LoadConfig(path);
        }
        
        private bool LoadConfig(string path)
        {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var binary = new BinaryFormatter();
            var config = binary.Deserialize(fileStream) as AssetConfig;
            fileStream.Close();

            if (config == null)
            {
                Debug.LogError($"AB配置加载失败");
                return false;
            }

            foreach (var list in config.assetList)
            {
                if (_nameDict.ContainsKey(list.assetName))
                {
                    Debug.LogWarning($"重复资源!:ab[{list.abName}],assetName[{list.assetName}]");
                }
                else
                {
                    _nameDict.Add(list.assetName, list);
                }
            }

            return true;
        }

        private void CheckInit()
        {
            if (!_isInit)
            {
                throw new InvalidOperationException($"AssetBundleManager未初始化!");
            }
        }

        /// <summary>
        /// 获取AB包，不会加载依赖项
        /// </summary>
        /// <param name="name">包名</param>
        /// <param name="isRefAsset">是否有资源引用该包</param>
        [CanBeNull]
        private AssetBundle GetAssetBundle(string name, bool isRefAsset = false)
        {
            CheckInit();
            if (!_abDict.TryGetValue(name, out var abRef))
            {
                var fullABPath = $"{Application.streamingAssetsPath}/{name}";
                var ab = LoadAssetBundle(fullABPath);
                if (ab == null)
                {
                    Debug.LogError($"无法加载AB包:{name}");
                    return null;
                }

                abRef = new AssetBundleRef(ab);
                _abDict.Add(name, abRef);
            }

            if (!isRefAsset)
            {
                return abRef.assetBundle;
            }

            abRef.refCount++;
            _abDict[name] = abRef;
            return abRef.assetBundle;
        }

        /// <summary>
        /// 获取AB包，会加载依赖项
        /// </summary>
        /// <param name="assetInfo">包名</param>
        /// <param name="isRefAsset">是否有资源引用该包</param>
        [CanBeNull]
        internal AssetBundle GetAssetBundle(AssetInfo assetInfo, bool isRefAsset = false)
        {
            var abRef = GetAssetBundle(assetInfo.abName, isRefAsset);
            ProcessDepend(assetInfo.dependABs);
            return abRef;
        }

        /// <summary>
        /// 获取资源所在的AssetBundle
        /// </summary>
        /// <param name="name">该资源的完整名称，带后缀</param>
        /// <param name="info">获取到的资源信息</param>
        /// <returns>AB引用</returns>
        [CanBeNull]
        internal AssetBundle GetAssetAndAB(string name, out AssetInfo info)
        {
            info = GetAssetInfo(name);
            return GetAssetBundle(info);
        }

        [CanBeNull]
        internal AssetInfo GetAssetInfo(string name)
        {
            CheckInit();
            return _nameDict.TryGetValue(name, out var info) ? info : null;
        }

        private void ProcessDepend(IEnumerable<string> depends)
        {
            foreach (var depend in depends)
            {
                if (!_abDict.ContainsKey(depend))
                {
                    GetAssetBundle(depend, true);
                }
            }
        }

        internal void ReleaseAsset(AssetInfo assetInfo)
        {
            CheckInit();
            if (!_abDict.TryGetValue(assetInfo.abName, out var abRef))
            {
                Debug.LogError($"AB包未加载，name:{assetInfo.abName}");
                return;
            }

            if (abRef.assetBundle == null)
            {
                return;
            }

            foreach (var depend in assetInfo.dependABs)
            {
                UnloadAssetBundle(depend);
            }

            UnloadAssetBundle(assetInfo.abName);
        }

        public void ReleaseAsset(string name)
        {
            if (!_nameDict.TryGetValue(name, out var info))
            {
                return;
            }

            ReleaseAsset(info);
        }

        private void UnloadAssetBundle(string name)
        {
            if (!_abDict.TryGetValue(name, out var abRef))
            {
                return;
            }

            abRef.refCount--;
            if (abRef.assetBundle == null || abRef.refCount > 0)
            {
                return;
            }

            abRef.assetBundle.Unload(true);
            _abDict.Remove(name);
        }

        private static AssetBundle LoadAssetBundle(string path)
        {
            return AssetBundle.LoadFromFile(path);
        }
    }
}