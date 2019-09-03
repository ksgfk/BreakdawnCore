using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        internal Object resource;
        private int _refCount;

        public AssetInfo Info { get; }

        internal virtual Object Resource
        {
            get
            {
                if (resource != null)
                {
                    return resource;
                }

                resource = ResourceManager.Instance.LoadObject(this, IsSprite);
                return resource;
            }
        }

        internal DateTime LastUseTime { get; set; }

        public bool IsSprite { get; }

        internal int RefCount
        {
            get => _refCount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidOperationException($"引用计数不能小于0，name:{Info.assetName}，refCount:{value}");
                }

                _refCount = value;
            }
        }

        internal Asset(AssetInfo info)
        {
            Info = info;
            if (info.assetName.EndsWith(".png") ||
                info.assetName.EndsWith(".jpg") ||
                info.assetName.EndsWith(".bmp"))
            {
                IsSprite = true;
            }
            else
            {
                IsSprite = false;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Asset asset && Info.Equals(asset.Info);
        }

        public override int GetHashCode()
        {
            return Info.GetHashCode();
        }
    }

    public class ResourceManager : Singleton<ResourceManager>
    {
        /// <summary>
        /// 正在使用的资源
        /// </summary>
        [Obsolete] private readonly Dictionary<string, Asset> _nameDict = new Dictionary<string, Asset>();

        /// <summary>
        /// 缓存没有被引用的资源
        /// </summary>
        private readonly FastLinkedList<Asset> _noRef = new FastLinkedList<Asset>();

        /// <summary>
        /// 正在使用的资源
        /// </summary>
        private readonly Dictionary<AssetInfo, Asset> _assetDict = new Dictionary<AssetInfo, Asset>();

        /// <summary>
        /// 资源查询
        /// </summary>
        private readonly Dictionary<Object, Asset> _objInquiryDict = new Dictionary<Object, Asset>();

        private ResourceManager()
        {
        }

        #region 同步加载

        /// <summary>
        /// 初始化资源管理
        /// </summary>
        /// <param name="fileName">配置文件完整名称</param>
        /// <param name="paths">配置文件路径</param>
        public void Init(string fileName, params string[] paths)
        {
            AssetBundleManager.Instance.Init(new PathBuilder(string.Empty, fileName, paths).Get());
        }

        public T GetAsset<T>(string name) where T : Object
        {
            var info = AssetBundleManager.Instance.GetAssetInfo(name);
            var asset = GetAssetFromCache(info) ?? new Asset(info);
            asset.RefCount++;
            _assetDict.Add(info, asset);
            return asset.Resource as T;
        }

        [CanBeNull]
        private Asset GetAssetFromCache(AssetInfo info)
        {
            if (_assetDict.TryGetValue(info, out var asset))
            {
                return asset;
            }

            foreach (var ass in _noRef)
            {
                if (!ass.Info.Equals(info))
                {
                    continue;
                }

                asset = ass;
                break;
            }

            if (asset != null)
            {
                _noRef.Remove(asset);
            }

            return asset;
        }

        internal Object LoadObject(Asset asset, bool isSprite)
        {
            var info = asset.Info;
            var ab = AssetBundleManager.Instance.GetAssetBundle(info, true);
            var resource = isSprite
                ? ab.LoadAsset<Sprite>(GetRealNameFromAssetName(info.assetName))
                : ab.LoadAsset(GetRealNameFromAssetName(info.assetName));
            if (resource == null)
            {
                throw new ArgumentException($"无法加载{info}");
            }

            _objInquiryDict.Add(resource, asset);
            return resource;
        }

        #endregion

        #region 异步加载

        private MonoBehaviour _mono;
        private Coroutine _loadCoroutine;
        private readonly Queue<LoadAssetAsyncRequest> _waiting = new Queue<LoadAssetAsyncRequest>();

        private readonly Dictionary<string, LoadAssetAsyncRequest> _waitOrLoad =
            new Dictionary<string, LoadAssetAsyncRequest>();

        private bool _isInitAsync;

        public delegate void LoadComplete(LoadAssetAsyncRequest obj);

        public void InitAsync(MonoBehaviour mono)
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

                var proc = _waiting.Dequeue();
                if (proc.Request == null)
                {
                    proc.Request = proc.IsSprite
                        ? proc.Bundle.LoadAssetAsync<Sprite>(GetRealNameFromAssetName(proc.AssetName))
                        : proc.Bundle.LoadAssetAsync(GetRealNameFromAssetName(proc.AssetName));
                }

                yield return proc.Request;
                if (proc.Request.isDone)
                {
                    proc.LastUseTime = DateTime.Now;
                    foreach (var callback in proc.callbacks)
                    {
                        callback?.Invoke(proc);
                    }

                    _waitOrLoad.Remove(proc.AssetName);
                    proc.resource = proc.Request.asset;
                    proc.isDone = proc.Request.isDone;
                    proc.Request = null;
                    proc.callbacks = null;
                    if (proc.isCached)
                    {
                        continue;
                    }

                    _nameDict.Add(proc.AssetName, proc);
                }
                else
                {
                    _waiting.Enqueue(proc); //我觉得这句永远不会执行QwQ
                    //throw new InvalidOperationException("不可能过了yield,资源却没加载完毕");
                }
            }
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="onComplete">加载完成后的行为</param>
        /// <param name="fileName">资源全名</param>
        /// <param name="paths">资源路径</param>
        /// <returns>异步加载请求</returns>
        [CanBeNull]
        public LoadAssetAsyncRequest GetAssetAsync(LoadComplete onComplete, string fileName, params string[] paths)
        {
            return GetAssetAsync(new PathBuilder(string.Empty, fileName, paths).Get(), onComplete);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="name"></param>
        /// <param name="onComplete">加载完成后的行为</param>
        /// <returns>异步加载请求</returns>
        [CanBeNull]
        public LoadAssetAsyncRequest GetAssetAsync(string name, LoadComplete onComplete)
        {
            CheckInit();
            var res = GetAssetFromPools<LoadAssetAsyncRequest>(name);
            if (res != null)
            {
                onComplete?.Invoke(res);
                //res.RefCount++;
                if (!_nameDict.ContainsKey(name))
                {
                    _nameDict.Add(name, res);
                }

                return res;
            }

            if (!_waitOrLoad.TryGetValue(name, out var request))
            {
                request = GetNewAsyncLoadRequest(name, false);
            }

            request.callbacks.Add(onComplete);
            return request;
        }

        private LoadAssetAsyncRequest GetNewAsyncLoadRequest(string name, bool isCache)
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

            var req = new LoadAssetAsyncRequest(info)
            {
                Request = null,
                Bundle = ab,
                isCached = isCache
            };

            _waiting.Enqueue(req);
            _waitOrLoad.Add(name, req);

            if (_loadCoroutine == null)
            {
                _loadCoroutine = _mono.StartCoroutine(Load());
            }

            return req;
        }

        /// <summary>
        /// 异步缓存资源
        /// </summary>
        /// <returns>异步请求</returns>
        [CanBeNull]
        public LoadAssetAsyncRequest CacheAssetAsync(string name)
        {
            var result = Cache(name);

            if (result == null)
            {
                return null;
            }

            var req = GetNewAsyncLoadRequest(name, true);
            req.callbacks.Add(request => _noRef.AddLast(request));
            return req;
        }

        /// <summary>
        /// 异步缓存资源
        /// </summary>
        /// <returns>异步请求</returns>
        [CanBeNull]
        public LoadAssetAsyncRequest CacheAssetAsync(string fileName, params string[] paths)
        {
            return CacheAssetAsync(new PathBuilder(string.Empty, fileName, paths).Get());
        }

        private void CheckInit()
        {
            if (!_isInitAsync)
            {
                throw new InvalidOperationException($"ResourceManager异步加载功能未初始化!");
            }
        }

        #endregion

        public void RecycleAsset(Object obj, bool isCache = true)
        {
            if (!_objInquiryDict.TryGetValue(obj, out var asset))
            {
                throw new ArgumentException($"该资源未初始化{obj}");
            }

            RecycleAsset(asset, isCache);
        }

        private void RecycleAsset(Asset res, bool isCache)
        {
            res.RefCount--;
            if (res.RefCount > 0)
            {
                return;
            }

            if (!_assetDict.Remove(res.Info))
            {
                throw new ArgumentException($"资源池中无该资源{res}");
            }

            if (isCache)
            {
                _noRef.AddFirst(res);
            }
            else
            {
                AssetBundleManager.Instance.ReleaseAsset(res.Info);
            }

            _objInquiryDict.Remove(res.resource);
            res.resource = null;
        }

        [CanBeNull]
        private Asset Cache(string name)
        {
            CheckInit();
            var result = GetAssetFromPools<Asset>(name);
            if (result != null)
            {
                return null;
            }

            var info = AssetBundleManager.Instance.GetAssetInfo(name);
            if (info != null)
            {
                return new Asset(info);
            }

            Debug.LogError($"不存在资源:name[{name}]");
            return null;
        }

        [CanBeNull]
        private T GetAssetFromPools<T>(string name) where T : Asset
        {
            if (_nameDict.TryGetValue(name, out var result))
            {
                return Utility.TypeCast<Asset, T>(result);
            }

            foreach (var ass in _noRef)
            {
                if (ass.Info.assetName == name)
                {
                    result = ass;
                }
            }

            if (result == null)
            {
                return null;
            }

            if (!_noRef.Remove(result))
            {
                throw new InvalidOperationException("不可能删除失败的，除非FastLinkedList有bug");
            }

            return Utility.TypeCast<Asset, T>(result);
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void ClearCache()
        {
            _noRef.Clear();
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