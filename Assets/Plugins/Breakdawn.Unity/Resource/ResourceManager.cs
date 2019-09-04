using System;
using System.Collections;
using System.Collections.Generic;
using Breakdawn.Core;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    public delegate void LoadComplete(AssetAsync obj);

    public class ResourceManager : Singleton<ResourceManager>
    {
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

        /// <summary>
        /// 初始化资源管理
        /// </summary>
        /// <param name="fileName">配置文件完整名称</param>
        /// <param name="paths">配置文件路径</param>
        public void Init(string fileName, params string[] paths)
        {
            AssetBundleManager.Instance.Init(new PathBuilder(string.Empty, fileName, paths).Get());
        }

        #region 同步加载

        public T GetAsset<T>(string name) where T : Object
        {
            var info = AssetBundleManager.Instance.GetAssetInfo(name);
            var asset = GetAssetFromCache(info) ?? new Asset(info);
            _assetDict.Add(info, asset);
            return asset.Resource as T;
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
        private bool _isInitAsync;
        private readonly Queue<AssetAsync> _waitingQueue = new Queue<AssetAsync>();
        private readonly Dictionary<AssetInfo, AssetAsync> _asyncInquiryDict = new Dictionary<AssetInfo, AssetAsync>();

        public void InitAsync(MonoBehaviour mono)
        {
            _mono = mono != null ? mono : throw new ArgumentNullException();
            _isInitAsync = true;
        }

        private void CheckInitAsync()
        {
            if (_isInitAsync)
            {
                throw new InvalidOperationException("异步加载未初始化");
            }
        }

        private IEnumerator LoadAssetAsync()
        {
            while (!_waitingQueue.IsEmpty())
            {
                var process = _waitingQueue.Dequeue();
                var ab = AssetBundleManager.Instance.GetAssetBundle(process.Info, true);
                if (process.Request == null)
                {
                    process.Request = process.IsSprite
                        ? ab.LoadAssetAsync<Sprite>(GetRealNameFromAssetName(process.AssetName))
                        : ab.LoadAssetAsync(GetRealNameFromAssetName(process.AssetName));
                }

                yield return process.Request;
                process.LastUseTime = DateTime.Now;
                foreach (var callback in process.callbacks.GetInvocationList())
                {
                    if (callback is LoadComplete complete)
                    {
                        complete(process);
                    }
                    else
                    {
                        throw new InvalidCastException("回调方法类型错误");
                    }
                }

                process.resource = process.Request.asset;
                process.isDone = true;
                process.Request = null;
                process.callbacks = null;

                _asyncInquiryDict.Remove(process.Info);
                AddAssetToDict(process.Info, process);
            }

            _mono.StopCoroutine(_loadCoroutine);
            _loadCoroutine = null;
        }

        public AssetAsync GetAssetAsync(string name, LoadComplete onComplete)
        {
            var info = AssetBundleManager.Instance.GetAssetInfo(name);
            var asset = GetAssetFromCacheAndLoadQueue(info);
            if (asset != null)
            {
                if (asset.IsDone) //资源已加载完毕，直接执行委托并返回请求
                {
                    onComplete(asset);
                    AddAssetToDict(info, asset);
                    return asset;
                }
            }
            else //缓存、正在加载列表都没有请求时新建一个
            {
                asset = new AssetAsync(info);
                _waitingQueue.Enqueue(asset);
                _asyncInquiryDict.Add(info, asset);
            }

            asset.callbacks += onComplete; //资源未加载完毕，添加委托
            if (_loadCoroutine == null)
            {
                _loadCoroutine = _mono.StartCoroutine(LoadAssetAsync());
            }

            return asset;
        }

        [CanBeNull]
        private AssetAsync GetAssetFromCacheAndLoadQueue(AssetInfo info)
        {
            var asset = GetAssetFromCache(info);
            switch (asset)
            {
                case null:
                    return _asyncInquiryDict.TryGetValue(info, out var @async) ? @async : null;
                case AssetAsync assetAsync:
                    return assetAsync;
                default:
                    throw new InvalidOperationException($"该资源已由同步加载，请使用GetAsset而不是GetAssetAsync，{info}");
            }
        }

        #endregion

        public void RecycleAsset(Object obj, bool isCache)
        {
            if (obj == null)
            {
                throw new ArgumentNullException($"回收{obj}不能为null");
            }

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

            _objInquiryDict.Remove(res.resource);
            if (isCache)
            {
                _noRef.AddFirst(res);
            }
            else
            {
                AssetBundleManager.Instance.ReleaseAsset(res.Info);
            }
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void ClearCache()
        {
            _noRef.Clear();
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

        private void AddAssetToDict(AssetInfo info, Asset asset)
        {
            _assetDict.Add(info, asset);
            _objInquiryDict.Add(asset.Resource, asset);
        }

        private static string GetRealNameFromAssetName(string assetName)
        {
            var start = assetName.LastIndexOf('/');
            var end = assetName.LastIndexOf('.');
            return assetName.Substring(start + 1, end - start - 1);
        }
    }
}