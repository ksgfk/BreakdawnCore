using System;
using System.Collections.Generic;
using Breakdawn.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Breakdawn.Unity
{
    public class ObjectManager : Singleton<ObjectManager>
    {
        private readonly Dictionary<Type, object> _poolDict = new Dictionary<Type, object>();

        private ObjectManager()
        {
        }

        [CanBeNull]
        public ObjectPool<T> GetPool<T>() where T : class
        {
            return !_poolDict.TryGetValue(typeof(T), out var pool) ? null : pool as ObjectPool<T>;
        }

        [CanBeNull]
        public ObjectPool<T> AddPool<T>(IFactory<T> factory, int initCount = 0) where T : class
        {
            var type = typeof(T);
            if (_poolDict.ContainsKey(type))
            {
                return null;
            }

            var newPool = new ObjectPool<T>(factory, initCount);
            _poolDict.Add(type, newPool);
            return newPool;
        }

        public bool RemovePool<T>() where T : class
        {
            var type = typeof(T);
            if (!_poolDict.ContainsKey(type))
            {
                return false;
            }

            _poolDict.Remove(type);
            return true;
        }

        [CanBeNull]
        public T GetObject<T>() where T : class
        {
            return GetPool<T>()?.Get();
        }

        public bool RecycleObject<T>(T @object) where T : class
        {
            var result = GetPool<T>()?.Recycle(@object);
            return result.HasValue && result.Value;
        }

        public void ReleasePools()
        {
            foreach (var pool in _poolDict.Values)
            {
                var type = pool.GetType();
                var method = type.GetMethod("Release");
                if (method != null)
                {
                    method.Invoke(pool, null);
                }
                else
                {
                    Debug.LogWarning($"调用方法失败:{type.FullName}");
                }
            }
        }
    }
}