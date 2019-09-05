using System;
using System.Collections.Generic;
using Breakdawn.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    /// <summary>
    /// TODO:WIP异步
    /// </summary>
    public class ObjectManager : Singleton<ObjectManager>
    {
        private GameObject _poolGo;
        private bool _isInit;

        private readonly Dictionary<string, ObjectPool<GameObject>> _pools =
            new Dictionary<string, ObjectPool<GameObject>>();

        private readonly Dictionary<GameObject, string> _objQueryDict = new Dictionary<GameObject, string>();

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
                _objQueryDict.Add(obj, name);
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
            return _pools.TryGetValue(name, out var pool) ? pool.Get() : CreatePool(name, 1).Get();
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
            if (!_objQueryDict.TryGetValue(obj, out var name))
            {
                throw new ArgumentException($"{obj}不是由对象池生成的");
            }

            return _pools[name].Recycle(obj);
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