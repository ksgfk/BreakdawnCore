using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Breakdawn.Core;
using UnityEngine;

namespace Breakdawn.Unity
{
    public class AssetBundleRef
    {
        public AssetBundle assetBundle;
        internal int refCount;

        public AssetBundleRef(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
            refCount = 0;
        }
    }

    public class AssetBundleManager : Singleton<AssetBundleManager>
    {
        public string configName = "/AssetConfig.config";

        /// <summary>
        /// 资源配置表，key:导出时资源路径CRC32，value:资源信息
        /// </summary>
        private readonly Dictionary<uint, AssetInfo> _crcDict = new Dictionary<uint, AssetInfo>();

        /// <summary>
        /// 资源配置表，key:资源名，value:资源信息
        /// </summary>
        private readonly Dictionary<string, AssetInfo> _nameDict = new Dictionary<string, AssetInfo>();

        /// <summary>
        /// 已加载的AB包，key:AB包名，value:实例
        /// </summary>
        private readonly Dictionary<string, AssetBundleRef> _abDict = new Dictionary<string, AssetBundleRef>();

        private AssetBundleManager()
        {
            //_abRefPool.OnRecycling += abRef => abRef.Reset();
        }

        public bool LoadConfig(string path)
        {
            var fileStream = new FileStream(path + configName, FileMode.Open, FileAccess.Read, FileShare.Read);
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
                if (_crcDict.ContainsKey(list.crc))
                {
                    Debug.LogWarning($"重复CRC!:ab[{list.abName}],assetName[{list.assetName}],crc[{list.crc}]");
                }
                else
                {
                    _crcDict.Add(list.crc, list);
                }

                if (_nameDict.ContainsKey(list.assetName))
                {
                    Debug.LogWarning($"重复资源!:ab[{list.abName}],assetName[{list.assetName}],crc[{list.crc}]");
                }
                else
                {
                    _nameDict.Add(list.assetName, list);
                }
            }

            return true;
        }

        /// <summary>
        /// 获取AB包
        /// </summary>
        /// <param name="name">包名</param>
        /// <param name="isRefAsset">是否有资源引用该包</param>
        /// <returns></returns>
        public AssetBundle GetAssetBundle(string name, bool isRefAsset = false)
        {
            AssetBundleRef result;
            if (_abDict.TryGetValue(name, out var abRef))
            {
                result = abRef;
            }
            else
            {
                var fullABPath = $"{Application.streamingAssetsPath}/{name}";
                var ab = LoadAssetBundle(fullABPath);
                if (ab == null)
                {
                    Debug.LogError($"无法加载AB包:{name}");
                    return null;
                }

                result = new AssetBundleRef(ab);
                _abDict.Add(name, result);
            }

            if (isRefAsset)
            {
                result.refCount++;
            }

            return result.assetBundle;
        }

        public AssetBundle GetAssetBundle(AssetInfo assetInfo, bool isRefAsset = false)
        {
            var abRef = GetAssetBundle(assetInfo.abName, isRefAsset);
            ProcessDepend(assetInfo.dependABs);
            return abRef;
        }

        /// <summary>
        /// 获取资源所在的AssetBundle
        /// </summary>
        /// <param name="crc">该资源的CRC32值</param>
        /// <returns>AB引用</returns>
        public AssetBundle GetAssetDependAB(uint crc)
        {
            if (_crcDict.TryGetValue(crc, out var info))
            {
                return GetAssetBundle(info);
            }

            Debug.LogError($"不存在资源:CRC[{crc}]");
            return null;
        }

        /// <summary>
        /// 获取资源所在的AssetBundle
        /// </summary>
        /// <param name="name">该资源的完整名称，带后缀</param>
        /// <returns>AB引用</returns>
        public AssetBundle GetAssetDependAB(string name)
        {
            if (_nameDict.TryGetValue(name, out var info))
            {
                return GetAssetBundle(info);
            }

            Debug.LogError($"不存在资源:name[{name}]");
            return null;
        }

        private void ProcessDepend(IEnumerable<string> depends)
        {
            foreach (var depend in depends)
            {
                if (!_abDict.TryGetValue(depend, out _))
                {
                    GetAssetBundle(depend, true);
                }
            }
        }

        public void ReleaseAsset(AssetInfo assetInfo)
        {
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