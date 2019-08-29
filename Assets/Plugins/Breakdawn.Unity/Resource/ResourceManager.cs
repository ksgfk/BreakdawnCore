using System;
using System.Collections;
using System.Collections.Generic;
using Breakdawn.Core;
using JetBrains.Annotations;
using UnityEditor.PackageManager;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    internal class Asset
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

    public class ResourceManager : Singleton<ResourceManager>
    {
        /// <summary>
        /// 缓存没有被引用的资源
        /// </summary>
        private readonly FastLinkedList<Asset> _noRefAssets = new FastLinkedList<Asset>();

        /// <summary>
        /// 正在使用的资源
        /// </summary>
        private readonly Dictionary<string, Asset> _nameDict = new Dictionary<string, Asset>();

        private MonoBehaviour _script;
        private bool _isInitAsync;
        private Coroutine _asyncThread;
        private List<AsyncLoadData>[] _data;
        private Dictionary<string, AsyncLoadData> _loading;

        public delegate void OnLoadAssetAsyncFinish(string path, Object obj);

        private ResourceManager()
        {
        }

        public void InitAsync(MonoBehaviour script)
        {
            if (script == null)
            {
                Debug.LogWarning($"参数{nameof(script)}为空");
                return;
            }

            _loading = new Dictionary<string, AsyncLoadData>();
            var priorityCount = Enum.GetNames(typeof(AsyncLoadPriority)).Length;
            _data = new List<AsyncLoadData>[priorityCount];
            for (var i = 0; i < priorityCount; i++)
            {
                _data[i] = new List<AsyncLoadData>();
            }

            _script = script;
            _asyncThread = _script.StartCoroutine(AsyncLoad());
            _isInitAsync = true;
        }

        [CanBeNull]
        private Asset GetAssetFromPools(string name)
        {
            if (_nameDict.TryGetValue(name, out var result))
            {
                return result;
            }

            foreach (var ass in _noRefAssets)
            {
                if (ass.assetInfo.assetName == name)
                {
                    result = ass;
                }
            }

            return result;
        }

        #region 同步加载

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

                var start = res.assetInfo.assetName.LastIndexOf('/');
                var end = res.assetInfo.assetName.LastIndexOf('.');
                var name = res.assetInfo.assetName.Substring(start + 1, end - start - 1);
                res.asset = ab.LoadAsset(name);
                if (res.asset == null)
                {
                    Debug.LogError($"资源加载失败，name[{res.assetInfo.assetName}]");
                    return default;
                }
            }

            var obj = res.asset;
            if (!(obj is T result))
            {
                Debug.LogError($"类型错误，T:{typeof(T).FullName}，应为{obj.GetType().FullName}");
                return default;
            }

            res.RefCount += refCount;
            res.lastUseTime = DateTime.Now;
            return new UnityObjectInfo<T>(result, res.assetInfo.assetName);
        }

        [CanBeNull]
        private Asset GetAsset(string name)
        {
            var result = GetAssetFromPools(name);
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

            var asset = new Asset(info);
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

        #endregion

        #region 异步加载

        private IEnumerator AsyncLoad()
        {
            while (true)
            {
                yield return null;
            }
        }

        public void GetAssetAsync(string name, OnLoadAssetAsyncFinish finish, AsyncLoadPriority priority)
        {
            var asset = GetAssetFromPools(name);
            if (asset != null)
            {
                finish?.Invoke(name, asset.asset);
            }

            if (_loading.ContainsKey(name))
            {
                Debug.LogWarning($"资源{name}已经在加载了");
                return;
            }
        }

        private void CheckAsyncFeatureInit()
        {
            if (!_isInitAsync)
            {
                throw new InvalidOperationException($"ResourceManager异步加载功能未初始化!");
            }
        }

        #endregion

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

        private static bool CheckParameter<T>(UnityObjectInfo<T> param) where T : Object
        {
            return param.obj == null && string.IsNullOrEmpty(param.rawName);
        }
    }
}