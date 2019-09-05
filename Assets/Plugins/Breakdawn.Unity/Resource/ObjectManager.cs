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
    /// TODO:WIP异步
    /// </summary>
    public class ObjectManager : Singleton<ObjectManager>
    {
        private GameObject _poolGo;
        private bool _isInit;

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

        public ObjectPool<GameObject> CreatePool(string name, int count)
        {
            return CreatePool(name, count, go => go.Show(), go => go.Hide(), Object.Destroy);
        }

        public ObjectPool<GameObject> CreatePool(string name, int count, Action<GameObject> onGet,
            Action<GameObject> onRecycle,
            Action<GameObject> onRelease)
        {
            CheckInit();
            if (_pools.TryGetValue(name, out var pool))
            {
                return pool;
            }

            var asset = ResourceManager.Instance.GetAsset<GameObject>(name);
            if (!asset)
            {
                throw new ArgumentNullException();
            }

            pool = new ObjectPool<GameObject>(() =>
            {
                var obj = Object.Instantiate(asset, _poolGo.transform).Hide();
                _objQueryDict.Add(obj, new CacheObjectInfo(obj, name, false));
                return obj;
            }, count);
            pool.OnGetObject += onGet;
            pool.OnRecycling += onRecycle;
            pool.OnRelease += onRelease;
            pool.OnRelease += obj => _objQueryDict.Remove(obj);
            _pools.Add(name, pool);
            return pool;
        }

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

        public void ReleaseAllPools()
        {
            foreach (var pools in _pools.Values)
            {
                pools.Release();
            }
        }

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

        private void CheckInit()
        {
            if (!_isInit)
            {
                throw new InvalidOperationException($"对象池管理未初始化");
            }
        }
    }
}