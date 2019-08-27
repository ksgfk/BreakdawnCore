using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Breakdawn.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Breakdawn.Unity
{
    /// <summary>
    /// 包含AB包实例的AB包信息
    /// </summary>
    [Serializable]
    public class AssetBundleInstance
    {
        public AssetBundle assetBundle;
        public AssetBundleBase baseInfo;

        public AssetBundleInstance(AssetBundleBase @baseInfo)
        {
            this.baseInfo = baseInfo;
        }
    }

    /// <summary>
    /// AB包实例的引用信息
    /// </summary>
    [Serializable]
    public class AssetBundleRef
    {
        public AssetBundle assetBundle;
        public int refCount;

        public AssetBundleRef(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
        }

        public void Reset()
        {
            assetBundle = null;
            refCount = 0;
        }
    }

    public class AssetBundleManager : Singleton<AssetBundleManager>
    {
        public string configName = "/AssetBundleConfig.config";

        /// <summary>
        /// 资源配置表，根据CRC查找
        /// </summary>
        private readonly Dictionary<uint, AssetBundleInstance> _resourceDict =
            new Dictionary<uint, AssetBundleInstance>();

        private readonly Dictionary<uint, AssetBundleRef> _abRefs = new Dictionary<uint, AssetBundleRef>();

        private readonly ObjectPool<AssetBundleRef> _abRefPool =
            ObjectManager.Instance.AddPool(new ObjectFactory<AssetBundleRef>(() => new AssetBundleRef(null)));

        private AssetBundleManager()
        {
            _abRefPool.OnRecycling += abRef => abRef.Reset();
        }

        public bool LoadConfig(string path)
        {
            var fileStream = new FileStream(path + configName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var binary = new BinaryFormatter();
            var config = binary.Deserialize(fileStream) as AssetBundleConfig;
            fileStream.Close();

            if (config == null)
            {
                Debug.LogError($"AB配置加载失败");
                return false;
            }

            foreach (var list in config.abList)
            {
                if (_resourceDict.ContainsKey(list.crc))
                {
                    Debug.LogWarning($"重复CRC!:ab[{list.name}],assetName[{list.assetName}],crc[{list.crc}]");
                }
                else
                {
                    _resourceDict.Add(list.crc, new AssetBundleInstance(list));
                }
            }

            return true;
        }

        [CanBeNull]
        public AssetBundleInstance GetAssetBundleInstance(uint crc)
        {
            if (!_resourceDict.TryGetValue(crc, out var abInst) || abInst == null)
            {
                Debug.LogError($"无法获取AB包信息，crc:{crc}");
                return null;
            }

            if (abInst.assetBundle != null)
            {
                return abInst;
            }

            abInst.assetBundle = LoadAssetBundle(abInst.baseInfo);
            return abInst;
        }

        [CanBeNull]
        private AssetBundle LoadAssetBundle(AssetBundleBase baseInfo)
        {
            if (!_resourceDict.TryGetValue(baseInfo.crc, out var abInst)) //获取AB信息
            {
                Debug.LogError($"无法找到AB包信息，crc:{baseInfo.crc}");
                return null; //不存在AB信息的话就没办法了
            }

            var abPath = $"{Application.streamingAssetsPath}/{baseInfo.name}";
            var nowPathCrc = CRC32.Get(abPath);
            if (!_abRefs.TryGetValue(nowPathCrc, out var abRef)) //获取AB包引用信息
            {
                abRef = _abRefPool.Get(); //不存在的话从池中取一个
                abRef.assetBundle = abInst.assetBundle;
                _abRefs.Add(nowPathCrc, abRef);
            }

            var result = abRef.assetBundle;
            if (result == null) //AB包引用信息的AB引用是null，就尝试加载
            {
                var ab = AssetBundle.LoadFromFile(abPath);
                if (ab == null)
                {
                    Debug.LogError($"无法加载AB包，crc:{baseInfo.crc}");
                }

                abRef.assetBundle = ab;
                abInst.assetBundle = ab;
            }

            abRef.refCount++;

            foreach (var depend in baseInfo.dependence) //处理依赖
            {
                var dependABPath = $"{Application.streamingAssetsPath}/{depend}";
                var dependCrc = CRC32.Get(dependABPath);
                if (!_abRefs.TryGetValue(dependCrc, out var dependRef))
                {
                    dependRef = _abRefPool.Get();
                    _abRefs.Add(dependCrc, dependRef);
                }

                var ab = AssetBundle.LoadFromFile(dependABPath);
                if (ab == null)
                {
                    Debug.LogError($"无法加载AB包，crc:{baseInfo.crc}");
                }

                dependRef.assetBundle = ab;
                dependRef.refCount++;
            }

            return abRef.assetBundle;
        }
    }
}