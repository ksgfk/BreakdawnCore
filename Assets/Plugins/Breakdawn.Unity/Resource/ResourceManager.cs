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

        internal int RefCount
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
    /// TODO:基本可以清理资源了，但有个小问题，若用户没有释放资源，又向同一个字段GetAsset，可能会有问题...
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
        /// <param name="refCount">引用该资源的次数</param>
        /// <typeparam name="T">资源类型</typeparam>
        [Obsolete("如果你对一个没有被RecycleAsset方法回收过的字段，调用该方法赋值，可能会造成资源无法回收")]
        public UnityObjectInfo<T> GetAsset<T>(string name, int refCount = 1) where T : Object
        {
            return GetAsset<T>(GetAsset(name), refCount);
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="name">资源名</param>
        /// <param name="result">返回的资源本体</param>
        /// <param name="refCount">引用该资源的次数</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void GetAsset<T>(string name, ref UnityObjectInfo<T> result, int refCount = 1) where T : Object
        {
            if (CheckParameter(result))
            {
                result = GetAsset<T>(GetAsset(name), refCount);
            }
            else
            {
                Debug.LogWarning($"参数{nameof(result)}已经有值了，不可以重复赋值");
            }
        }

        private static bool CheckParameter<T>(UnityObjectInfo<T> param) where T : Object
        {
            return param.obj == null && string.IsNullOrEmpty(param.rawName);
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="crc">资源CRC32值</param>
        /// <param name="refCount">引用该资源的数量</param>
        /// <typeparam name="T">资源类型</typeparam>
        [Obsolete("如果你对一个没有被RecycleAsset方法回收过的字段，调用该方法赋值，可能会造成资源无法回收")]
        public UnityObjectInfo<T> GetAsset<T>(uint crc, int refCount = 1) where T : Object
        {
            return GetAsset<T>(GetAsset(crc), refCount);
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="crc">资源CRC32值</param>
        /// <param name="result">返回的资源本体</param>
        /// <param name="refCount">引用该资源的次数</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void GetAsset<T>(uint crc, ref UnityObjectInfo<T> result, int refCount = 1) where T : Object
        {
            if (CheckParameter(result))
            {
                result = GetAsset<T>(GetAsset(crc), refCount);
            }
            else
            {
                Debug.LogWarning($"参数{nameof(result)}已经有值了，不可以重复赋值");
            }
        }

        private static UnityObjectInfo<T> GetAsset<T>(Asset res, int refCount) where T : Object
        {
            if (res == null)
            {
                return default;
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
                    return default;
                }

                res.asset = ab.LoadAsset(res.assetInfo.assetName.Split('.')[0]);
            }

            var obj = res.asset;
            if (!(obj is T result))
            {
                Debug.LogError($"类型错误，{typeof(T).FullName}");
                return default;
            }

            res.RefCount += refCount;
            res.lastUseTime = DateTime.Now;
            return new UnityObjectInfo<T>(result, res.assetInfo.assetName);
        }

        [CanBeNull]
        private Asset GetAsset(string name)
        {
            if (_nameDict.TryGetValue(name, out var asset))
            {
                return asset;
            }

            Asset result = null;
            foreach (var ass in _noRefAssets)
            {
                if (ass.assetInfo.assetName == name)
                {
                    result = ass;
                }
            }

            if (result != null)
            {
                _noRefAssets.Remove(result);
                _nameDict.Add(name, result);
                return result;
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

        private void RecycleAsset(Asset res, bool isCache)
        {
            res.RefCount--;
            if (res.RefCount > 0)
            {
                return;
            }

            if (!_nameDict.Remove(res.assetInfo.assetName))
            {
                Debug.LogError($"字典中不存在资源:{res.assetInfo.assetName}，可能多次释放该资源");
                return;
            }

            if (isCache)
            {
                _noRefAssets.AddFirst(res);
            }
            else
            {
                AssetBundleManager.Instance.ReleaseAsset(res.assetInfo);
                res.asset = null;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="obj">Unity资源，如果成功释放，则会将引用的值置空</param>
        /// <param name="isCache">是否缓存，以便下次使用</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <exception cref="ArgumentException">资源不存在时抛出</exception>
        public bool RecycleAsset<T>(ref UnityObjectInfo<T> obj, bool isCache = true) where T : Object
        {
            if (!_nameDict.TryGetValue(obj.rawName, out var ass))
            {
                Debug.LogError($"无法找到资源{obj.rawName}，这可能是Bug！");
                return false;
            }

            RecycleAsset(ass, isCache);
            obj = default;
            return true;
        }
    }
}