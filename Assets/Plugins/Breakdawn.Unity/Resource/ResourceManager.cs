using System;
using System.Collections;
using System.Collections.Generic;
using Breakdawn.Core;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    /// <summary>
    /// 资源实例
    /// </summary>
    public class Asset
    {
        public readonly AssetInfo assetInfo;
        internal Object asset;
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

        internal Asset(AssetInfo assetInfo)
        {
            this.assetInfo = assetInfo;
        }
    }

    public class ResourceManager : Singleton<ResourceManager>
    {
        private ResourceManager()
        {
        }

        #region 同步加载

        /// <summary>
        /// 缓存没有被引用的资源
        /// </summary>
        private readonly FastLinkedList<Asset> _noRef = new FastLinkedList<Asset>();

        /// <summary>
        /// 正在使用的资源
        /// </summary>
        private readonly Dictionary<string, Asset> _nameDict = new Dictionary<string, Asset>();

        [CanBeNull]
        private Asset GetAssetFromPools(string name)
        {
            if (_nameDict.TryGetValue(name, out var result))
            {
                return result;
            }

            foreach (var ass in _noRef)
            {
                if (ass.assetInfo.assetName == name)
                {
                    result = ass;
                }
            }

            return result;
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="name">资源名</param>
        /// <param name="result">返回的资源本体</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void GetAsset<T>(string name, ref UnityObjectInfo<T> result) where T : Object
        {
            if (CheckParameter(result))
            {
                result = GetAsset<T>(GetAsset(name), 1);
            }
            else
            {
                throw new ArgumentException($"参数{nameof(result)}已经有值了，不可以重复赋值");
            }
        }

        public void GetAsset<T>(ref UnityObjectInfo<T> result, string fileName, params string[] paths) where T : Object
        {
            GetAsset(new PathBuilder(string.Empty, fileName, paths).Get(), ref result);
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

                res.asset = ab.LoadAsset(GetRealNameFromAssetName(res.assetInfo.assetName));
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
            if (result == null)
            {
                return CacheAsset(name);
            }

            _noRef.Remove(result);
            _nameDict.Add(name, result);
            return result;
        }

        #endregion

        #region 异步加载

        private MonoBehaviour _mono;
        private Coroutine _loadCoroutine;
        private readonly Queue<AsyncLoadRequest> _waiting = new Queue<AsyncLoadRequest>();

        private readonly Dictionary<string, AsyncLoadRequest> _waitOrLoad =
            new Dictionary<string, AsyncLoadRequest>();

        private readonly FastLinkedList<AsyncLoadRequest> _noRefAsync = new FastLinkedList<AsyncLoadRequest>();

        private readonly Dictionary<string, AsyncLoadRequest> _nameDictAsync =
            new Dictionary<string, AsyncLoadRequest>();

        private bool _isInitAsync;

        public delegate void LoadComplete(AsyncLoadRequest obj);

        public void Init(MonoBehaviour mono)
        {
            _mono = mono != null ? mono : throw new ArgumentNullException();
            _isInitAsync = true;
        }

        private IEnumerator Load()
        {
            while (true)
            {
                if (_waiting.IsEmpty())
                {
                    _mono.StopCoroutine(_loadCoroutine);
                    _loadCoroutine = null;
                    break;
                }

                var request = _waiting.Dequeue();

                if (request.Request == null)
                {
                    request.Request =
                        request.Bundle.LoadAssetAsync(GetRealNameFromAssetName(request.AssetName));
                }

                yield return request.Request;
                if (request.IsDone)
                {
                    request.Asset.asset = request.Request.asset;
                    request.Asset.lastUseTime = DateTime.Now;
                    foreach (var callback in request.callbacks)
                    {
                        callback?.Invoke(request);
                    }

                    request.Asset.RefCount += request.callbacks.Count;
                    _nameDictAsync.Add(request.AssetName, request);
                    _waitOrLoad.Remove(request.AssetName);
                }
                else
                {
                    Debug.LogError("这不可能");
                }
            }
        }

        [CanBeNull]
        public AsyncLoadRequest GetAssetAsync(string name, LoadComplete onComplete)
        {
            CheckInit();
            var res = GetAsyncAssetFromPools(name);
            if (res != null)
            {
                onComplete?.Invoke(res);
                res.Asset.RefCount++;
                return res;
            }

            if (!_waitOrLoad.TryGetValue(name, out var request))
            {
                var info = AssetBundleManager.Instance.GetAssetInfo(name);
                if (info == null)
                {
                    return null;
                }

                var ab = AssetBundleManager.Instance.GetAssetBundle(info, true);
                if (ab == null)
                {
                    return null;
                }

                request = new AsyncLoadRequest(new Asset(info)
                {
                    asset = null,
                    lastUseTime = default,
                    RefCount = 0
                })
                {
                    Request = null,
                    Bundle = ab
                };

                _waiting.Enqueue(request);
                _waitOrLoad.Add(name, request);

                if (_loadCoroutine == null)
                {
                    _loadCoroutine = _mono.StartCoroutine(Load());
                }
            }

            request.callbacks.Add(onComplete);
            return request;
        }

        [CanBeNull]
        private AsyncLoadRequest GetAsyncAssetFromPools(string name)
        {
            if (_nameDictAsync.TryGetValue(name, out var result))
            {
                return result;
            }

            foreach (var ass in _noRefAsync)
            {
                if (ass.Asset.assetInfo.assetName == name)
                {
                    result = ass;
                }
            }

            return result;
        }

        private void CheckInit()
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
                _noRef.AddFirst(res);
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

        private Asset CacheAsset(string name)
        {
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

        private static bool CheckParameter<T>(UnityObjectInfo<T> param) where T : Object
        {
            return param.obj == null && string.IsNullOrEmpty(param.rawName);
        }

        private static string GetRealNameFromAssetName(string assetName)
        {
            var start = assetName.LastIndexOf('/');
            var end = assetName.LastIndexOf('.');
            return assetName.Substring(start + 1, end - start - 1);
        }
    }
}