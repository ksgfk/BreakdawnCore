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
        public AssetInfo Info { get; }
        internal virtual Object Resource { get; set; }
        internal DateTime LastUseTime { get; set; }
        private int _refCount;

        internal int RefCount
        {
            get => _refCount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidOperationException($"引用计数不能小于0，name:{Info.assetName}，refCount:{_refCount}");
                }

                _refCount = value;
            }
        }

        internal Asset(AssetInfo info)
        {
            Info = info;
        }
    }

    public class ResourceManager : Singleton<ResourceManager>
    {
        /// <summary>
        /// 正在使用的资源
        /// </summary>
        private readonly Dictionary<string, Asset> _nameDict = new Dictionary<string, Asset>();

        /// <summary>
        /// 缓存没有被引用的资源
        /// </summary>
        private readonly FastLinkedList<Asset> _noRef = new FastLinkedList<Asset>();

        private ResourceManager()
        {
        }

        #region 同步加载

        /// <summary>
        /// 初始化资源管理
        /// </summary>
        /// <param name="fileName">配置文件完整名称</param>
        /// <param name="paths">配置文件路径</param>
        public static void Init(string fileName, params string[] paths)
        {
            AssetBundleManager.Instance.Init(new PathBuilder(string.Empty, fileName, paths).Get());
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="name">资源完整路径</param>
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

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="result">返回的资源本体</param>
        /// <param name="fileName">资源全名</param>
        /// <param name="paths">资源路径</param>
        /// <typeparam name="T">资源类型</typeparam>
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
                throw new ArgumentException($"你确定引用数量为{refCount}? :(");
            }

            if (res.Resource == null)
            {
                var ab = AssetBundleManager.Instance.GetAssetBundle(res.Info, true);
                if (ab == null)
                {
                    return default;
                }

                res.Resource = ab.LoadAsset(GetRealNameFromAssetName(res.Info.assetName));
                if (res.Resource == null)
                {
                    Debug.LogError($"资源加载失败，name[{res.Info.assetName}]");
                    return default;
                }
            }

            var obj = res.Resource;

            res.RefCount += refCount;
            res.LastUseTime = DateTime.Now;
            return new UnityObjectInfo<T>(Utility.TypeCast<Object, T>(obj), res.Info.assetName);
        }

        [CanBeNull]
        private Asset GetAsset(string name)
        {
            var result = GetAssetFromPools<Asset>(name);
            if (result != null)
            {
                return result;
            }

            result = CacheAsset(name);
            return result;
        }

        #endregion

        #region 异步加载

        private MonoBehaviour _mono;
        private Coroutine _loadCoroutine;
        private readonly Queue<AsyncAssetRequest> _waiting = new Queue<AsyncAssetRequest>();

        private readonly Dictionary<string, AsyncAssetRequest> _waitOrLoad =
            new Dictionary<string, AsyncAssetRequest>();

        private bool _isInitAsync;

        public delegate void LoadComplete(AsyncAssetRequest obj);

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
                    proc.Request = proc.Bundle.LoadAssetAsync(GetRealNameFromAssetName(proc.AssetName));
                }

                yield return proc.Request;
                if (proc.IsDone)
                {
                    proc.LastUseTime = DateTime.Now;
                    foreach (var callback in proc.callbacks)
                    {
                        callback?.Invoke(proc);
                    }

                    proc.RefCount += proc.callbacks.Count;
                    _nameDict.Add(proc.AssetName, proc);
                    _waitOrLoad.Remove(proc.AssetName);
                    proc.callbacks = null;
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
        public AsyncAssetRequest GetAssetAsync(LoadComplete onComplete, string fileName, params string[] paths)
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
        public AsyncAssetRequest GetAssetAsync(string name, LoadComplete onComplete)
        {
            CheckInit();
            var res = GetAssetFromPools<AsyncAssetRequest>(name);
            if (res != null)
            {
                onComplete?.Invoke(res);
                res.RefCount++;
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

                request = new AsyncAssetRequest(info)
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
        private AsyncAssetRequest GetAsyncAssetFromPools(string name)
        {
            if (_nameDict.TryGetValue(name, out var result))
            {
                if (result is AsyncAssetRequest request)
                {
                    return request;
                }

                throw new InvalidCastException($"资源{name}不是通过异步请求加载的");
            }

//            foreach (var ass in _noRefAsync)
//            {
//                if (ass.assetInfo.assetName == name)
//                {
//                    result = ass;
//                }
//            }

            return null;
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

            if (!_nameDict.Remove(res.Info.assetName))
            {
                Debug.LogError($"字典中不存在资源:{res.Info.assetName}，可能多次释放该资源");
                return;
            }

            if (isCache)
            {
                _noRef.AddFirst(res);
            }
            else
            {
                AssetBundleManager.Instance.ReleaseAsset(res.Info);
                res.Resource = null;
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
            if (CheckParameter(obj))
            {
                return false;
            }

            if (!_nameDict.TryGetValue(obj.rawName, out var ass))
            {
                Debug.LogError($"无法找到资源{obj.rawName}，这可能是Bug！");
                return false;
            }

            RecycleAsset(ass, isCache);
            obj = default;
            return true;
        }

        [CanBeNull]
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