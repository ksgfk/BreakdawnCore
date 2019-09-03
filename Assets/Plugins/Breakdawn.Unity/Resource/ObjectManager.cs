using System;
using System.Collections.Generic;
using Breakdawn.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Breakdawn.Unity
{
    /// <summary>
    /// TODO:WIP
    /// </summary>
    public class ObjectManager : Singleton<ObjectManager>
    {
        private GameObject _recycle;
        private bool _isInit;

        private readonly Dictionary<string, ObjectPool<UnityObjectInfo<GameObject>>> _pools =
            new Dictionary<string, ObjectPool<UnityObjectInfo<GameObject>>>();

        private ObjectManager()
        {
        }

        public void Init(GameObject pool)
        {
            _recycle = pool;
            _isInit = true;
        }

        #region 同步加载的对象池

        public UnityObjectInfo<GameObject> GetObject(string name)
        {
            CheckInit();
            if (TryGetObjectFromPool(name, out var obj))
            {
                return obj;
            }

            var pool = GetNewPool(GetObjInfo(name), 1);
            _pools.Add(name, pool);
            return pool.Get();
        }

        public void InitPool(string name, int initCount)
        {
            CheckInit();
            if (_pools.ContainsKey(name))
            {
                return;
            }

            _pools.Add(name, GetNewPool(GetObjInfo(name), initCount));
        }

        private static UnityObjectInfo<GameObject> GetObjInfo(string name)
        {
            UnityObjectInfo<GameObject> objInfo = default;
//            ResourceManager.Instance.GetAsset(name, ref objInfo);//TODO:WIP
            if (!objInfo.IsValid())
            {
                throw new ArgumentException($"无法找到资源{name}");
            }

            return objInfo;
        }

        #endregion

        private void CheckInit()
        {
            if (!_isInit)
            {
                throw new InvalidOperationException($"对象池管理未初始化");
            }
        }

        private bool TryGetObjectFromPool(string name, out UnityObjectInfo<GameObject> obj)
        {
            if (!_pools.TryGetValue(name, out var pool))
            {
                obj = default;
                return false;
            }

            obj = pool.Get();
            return true;
        }

        private ObjectPool<UnityObjectInfo<GameObject>> GetNewPool(UnityObjectInfo<GameObject> objInfo, int initCount)
        {
            var pool = new ObjectPool<UnityObjectInfo<GameObject>>(
                new ObjectFactory<UnityObjectInfo<GameObject>>(() =>
                {
                    var instance = Object.Instantiate(objInfo.obj);
                    var info = new UnityObjectInfo<GameObject>(instance, objInfo.rawName);
                    HideObject(info);
                    return info;
                }), initCount);
            pool.OnGetObject += info => info.obj.Show();
            pool.OnRecycling += HideObject;
            pool.OnRelease += info => Object.Destroy(info.obj);
            return pool;
        }

        private void HideObject(UnityObjectInfo<GameObject> info)
        {
            info.obj.Hide();
            info.obj.transform.parent = _recycle.transform;
        }
    }
}