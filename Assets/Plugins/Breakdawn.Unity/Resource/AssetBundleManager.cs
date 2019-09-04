using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Breakdawn.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Breakdawn.Unity
{
    internal struct AssetBundleRef : IEquatable<AssetBundleRef>
    {
        public readonly AssetBundle assetBundle;
        private int _refCount;

        public int RefCount
        {
            get => _refCount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidOperationException("引用计数不可为负数");
                }

                _refCount = value;
            }
        }

        public AssetBundleRef(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
            _refCount = 0;
        }

        public override int GetHashCode()
        {
            return assetBundle.GetInstanceID();
        }

        public bool Equals(AssetBundleRef other)
        {
            return assetBundle == other.assetBundle && RefCount == other.RefCount;
        }

        public static bool operator ==(AssetBundleRef x, AssetBundleRef y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(AssetBundleRef x, AssetBundleRef y)
        {
            return !x.Equals(y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return obj is AssetBundleRef abRef && Equals(abRef);
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
                throw new InvalidOperationException($"不可重复初始化!");
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
                throw new ArgumentException($"AB配置加载失败，配置文件路径{path}");
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
        private AssetBundle GetAssetBundle(string name, bool isRefAsset)
        {
            CheckInit();
            if (!_abDict.TryGetValue(name, out var abRef))
            {
                var fullABPath = $"{Application.streamingAssetsPath}/{name}";
                var ab = LoadAssetBundle(fullABPath);
                if (ab == null)
                {
                    throw new ArgumentException($"无法加载AB包:{name}");
                }

                abRef = new AssetBundleRef(ab);
                _abDict.Add(name, abRef);
            }

            if (!isRefAsset)
            {
                return abRef.assetBundle;
            }

            abRef.RefCount++;
            _abDict[name] = abRef;
            return abRef.assetBundle;
        }

        /// <summary>
        /// 获取AB包，会加载依赖项
        /// </summary>
        /// <param name="assetInfo">包名</param>
        /// <param name="isRefAsset">是否有资源引用该包</param>
        internal AssetBundle GetAssetBundle(AssetInfo assetInfo, bool isRefAsset)
        {
            var abRef = GetAssetBundle(assetInfo.abName, isRefAsset);
            ProcessDepend(assetInfo.dependABs);
            return abRef;
        }

        /// <summary>
        /// 获取资源所在的AssetBundle
        /// </summary>
        /// <param name="name">该资源的完整名称，带后缀</param>
        /// <param name="isRefAsset">是否有资源引用该包</param>
        /// <param name="info">获取到的资源信息</param>
        /// <returns>AB引用</returns>
        [CanBeNull]
        internal AssetBundle GetAssetInfoAndAB(string name, bool isRefAsset, out AssetInfo info)
        {
            info = GetAssetInfo(name);
            return GetAssetBundle(info, isRefAsset);
        }

        /// <summary>
        /// 获取资源信息
        /// </summary>
        /// <param name="name">该资源的完整名称，带后缀</param>
        /// <returns>AB信息</returns>
        internal AssetInfo GetAssetInfo(string name)
        {
            CheckInit();
            return _nameDict.TryGetValue(name, out var info) ? info : throw new ArgumentException($"无法获取资源信息:{name}");
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

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <exception cref="InvalidOperationException">AB包未加载时抛出</exception>
        internal void ReleaseAsset(AssetInfo assetInfo)
        {
            CheckInit();
            if (!_abDict.TryGetValue(assetInfo.abName, out var abRef))
            {
                throw new InvalidOperationException($"AB包未加载，name:{assetInfo}");
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

        private void UnloadAssetBundle(string name)
        {
            if (!_abDict.TryGetValue(name, out var abRef))
            {
                return;
            }

            abRef.RefCount--;
            if (abRef.assetBundle == null || abRef.RefCount > 0)
            {
                return;
            }

            abRef.assetBundle.Unload(true);
            _abDict.Remove(name);
        }

        [CanBeNull]
        private static AssetBundle LoadAssetBundle(string path)
        {
            return AssetBundle.LoadFromFile(path);
        }
    }
}