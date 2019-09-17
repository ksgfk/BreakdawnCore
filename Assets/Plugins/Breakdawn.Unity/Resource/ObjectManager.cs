using System;
using System.Collections.Generic;
using Breakdawn.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    internal struct CacheObjectInfo : IEquatable<CacheObjectInfo>
    {
        public readonly GameObject obj;
        public readonly string rawName;
        public bool isUse;

        public CacheObjectInfo(GameObject obj, string rawName, bool isUse)
        {
            this.obj = obj;
            this.rawName = rawName;
            this.isUse = isUse;
        }

        public bool Equals(CacheObjectInfo other)
        {
            return obj == other.obj && rawName == other.rawName;
        }
    }

    /// <summary>
    /// TODO:感觉只有Instantiate一大堆GO的时候才需要异步，就先不写了QwQ
    /// </summary>
    public class ObjectManager : Singleton<ObjectManager>
    {
        private GameObject _poolGo;
        private bool _isInit;

        private readonly Dictionary<string, GameObject> _rawAsset = new Dictionary<string, GameObject>();

        private readonly Dictionary<string, ObjectPool<GameObject>> _pools =
            new Dictionary<string, ObjectPool<GameObject>>();

        private readonly Dictionary<GameObject, CacheObjectInfo> _objQueryDict =
            new Dictionary<GameObject, CacheObjectInfo>();

        private ObjectManager()
        {
        }

        public void Init(GameObject pool)
        {
            _poolGo = pool;
            _isInit = true;
        }

        #region 同步加载对象池

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="name">对象完整名称</param>
        /// <param name="count">初始化数量</param>
        public ObjectPool<GameObject> CreatePool(string name, int count)
        {
            return CreatePool(name, count, null);
        }

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="name">对象完整名称</param>
        /// <param name="count">初始化数量</param>
        /// <param name="onInit">初始化时调用</param>
        public ObjectPool<GameObject> CreatePool(string name, int count, Action<GameObject> onInit)
        {
            return CreatePool(name, count, onInit, go => go.Show(), go => go.Hide(), Object.Destroy);
        }

        /// <summary>
        /// 创建对象池，若资源管理内无对应资源则同步加载。
        /// </summary>
        /// <param name="name">对象完整名称</param>
        /// <param name="count">初始化数量</param>
        /// <param name="onInit">初始化时调用</param>
        /// <param name="onGet">获取对象时触发事件</param>
        /// <param name="onRecycle">回收对象时触发事件</param>
        /// <param name="onRelease">释放对象时触发事件</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException">无资源</exception>
        public ObjectPool<GameObject> CreatePool(string name,
            int count,
            Action<GameObject> onInit,
            Action<GameObject> onGet,
            Action<GameObject> onRecycle,
            Action<GameObject> onRelease)
        {
            CheckInit();
            if (_pools.TryGetValue(name, out var pool))
            {
                return pool;
            }

            if (_rawAsset.ContainsKey(name))
            {
                throw new ArgumentException();
            }

            var asset = ResourceManager.Instance.GetAsset<GameObject>(name);
            if (!asset)
            {
                throw new ArgumentNullException();
            }

            _rawAsset.Add(name, asset);
            pool = new ObjectPool<GameObject>(() =>
            {
                var obj = Object.Instantiate(_rawAsset[name], _poolGo.transform).Hide();
                _objQueryDict.Add(obj, new CacheObjectInfo(obj, name, false));
                onInit?.Invoke(obj);
                return obj;
            }, count);
            pool.OnGetObject += onGet;
            pool.OnRecycling += onRecycle;
            pool.OnRelease += onRelease;
            pool.OnRelease += obj => _objQueryDict.Remove(obj);
            _pools.Add(name, pool);

            return pool;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="name">对象完整名称</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException">资源已被使用了</exception>
        public GameObject Get(string name)
        {
            CheckInit();
            var go = _pools.TryGetValue(name, out var pool) ? pool.Get() : CreatePool(name, 1).Get();
            if (!_objQueryDict.TryGetValue(go, out var info))
            {
                throw new ArgumentException();
            }

            if (info.isUse)
            {
                throw new InvalidOperationException($"已经使用该资源了{go}");
            }

            info.isUse = true;
            _objQueryDict[go] = info;
            return go;
        }

        #endregion

        /// <summary>
        /// 释放所有对象池的资源
        /// </summary>
        public void ReleaseAllPools()
        {
            foreach (var pools in _pools.Values)
            {
                pools.Release();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="name">对象名字</param>
        /// <param name="count">剩余对象数量</param>
        /// <exception cref="ArgumentException">没有对象池</exception>
        public void Release(string name, int count = 0)
        {
            if (!_pools.TryGetValue(name, out var pool))
            {
                throw new ArgumentException($"没有对象池{name}");
            }

            pool.Release(count);
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">将回收的实例</param>
        /// <returns>回收成功时返回true</returns>
        /// <exception cref="ArgumentException">实例不是由对象池生成的</exception>
        /// <exception cref="InvalidOperationException">已经回收该资源了</exception>
        public bool Recycle(GameObject obj)
        {
            if (!_objQueryDict.TryGetValue(obj, out var info))
            {
                throw new ArgumentException($"{obj}不是由对象池生成的");
            }

            if (!info.isUse)
            {
                throw new InvalidOperationException($"已经回收该资源了{obj}");
            }

            info.isUse = false;
            _objQueryDict[obj] = info;
            return _pools[info.rawName].Recycle(obj);
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="isCache">是否缓存资源</param>
        /// <exception cref="ArgumentException">没有对象池</exception>
        /// <exception cref="InvalidOperationException">由该池创建的物体未完全回收</exception>
        public void DestroyPool(string name, bool isCache)
        {
            if (!_pools.TryGetValue(name, out var pool))
            {
                throw new ArgumentException($"没有对象池{name}");
            }

            if (!_rawAsset.TryGetValue(name, out var asset))
            {
                throw new ArgumentException($"没有资源{name}");
            }

            if (pool.InstanceCount != pool.Count)
            {
                throw new InvalidOperationException($"由该池创建的物体未完全回收，创建了{pool.InstanceCount}，回收了{pool.Count}");
            }

            pool.Release(0);
            _pools.Remove(name);
            ResourceManager.Instance.RecycleAsset(asset, isCache);
            _rawAsset.Remove(name);
        }

        private void CheckInit()
        {
            if (!_isInit)
            {
                throw new InvalidOperationException($"对象池管理未初始化");
            }
        }
    }
}