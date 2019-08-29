using System;
using System.Collections.Generic;
using Breakdawn.Core;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    public class Asset
    {
        public readonly AssetInfo assetInfo;
        public Object asset;
        internal DateTime lastUseTime;
        private int _refCount;

        public int RefCount
        {
            get => _refCount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidOperationException($"引用计数不能小于0，name:{assetInfo.assetName}，refCount:{_refCount}");
                }

                _refCount = value;
            }
        }

        public Asset(AssetInfo assetInfo)
        {
            this.assetInfo = assetInfo;
        }
    }

    /// <summary>
    /// TODO:清理资源时要清理_nameDict和_noRefAssets
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager>
    {
        /// <summary>
        /// 缓存没有被引用的资源
        /// </summary>
        private readonly FastLinkedList<Asset> _noRefAssets = new FastLinkedList<Asset>();

        private readonly Dictionary<string, Asset> _nameDict = new Dictionary<string, Asset>();

        private ResourceManager()
        {
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="name">资源名</param>
        /// <param name="refCount">引用该资源的数量</param>
        /// <typeparam name="T">资源类型</typeparam>
        [CanBeNull]
        public T GetAsset<T>(string name, int refCount = 1) where T : Object
        {
            return GetAsset<T>(GetAsset(name), refCount);
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="crc">资源CRC32值</param>
        /// <param name="refCount">引用该资源的数量</param>
        /// <typeparam name="T">资源类型</typeparam>
        [CanBeNull]
        public T GetAsset<T>(uint crc, int refCount = 1) where T : Object
        {
            return GetAsset<T>(GetAsset(crc), refCount);
        }

        [CanBeNull]
        private static T GetAsset<T>(Asset res, int refCount) where T : Object
        {
            if (res == null)
            {
                return null;
            }

            if (refCount < 1)
            {
                Debug.LogWarning($"Are you sure Reference count is {refCount}? :(");
            }

            if (res.asset == null)
            {
                var ab = AssetBundleManager.Instance.GetAssetBundle(res.assetInfo, true);
                if (ab == null)
                {
                    return null;
                }

                res.asset = ab.LoadAsset(res.assetInfo.assetName.Split('.')[0]);
            }

            var obj = res.asset;
            if (!(obj is T result))
            {
                Debug.LogError($"类型错误，{typeof(T).FullName}");
                return null;
            }

            res.RefCount += refCount;
            res.lastUseTime = DateTime.Now;
            return result;
        }

        [CanBeNull]
        private Asset GetAsset(string name)
        {
            if (_nameDict.TryGetValue(name, out var asset))
            {
                return asset;
            }

            var info = AssetBundleManager.Instance.GetAssetInfo(name);
            if (info == null)
            {
                Debug.LogError($"不存在资源:name[{name}]");
                return null;
            }

            asset = new Asset(info);
            _nameDict.Add(name, asset);
            return asset;
        }

        [CanBeNull]
        private Asset GetAsset(uint crc)
        {
            var info = AssetBundleManager.Instance.GetAssetInfo(crc);
            if (info != null)
            {
                return GetAsset(info.assetName);
            }

            Debug.LogError($"不存在资源:crc[{crc}]");
            return null;
        }
    }
}