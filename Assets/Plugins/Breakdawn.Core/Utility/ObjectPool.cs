using System;
using System.Collections.Generic;

namespace Breakdawn.Core
{
    public class ObjectPool<T>
    {
        private readonly Stack<T> _pool;
        private readonly IFactory<T> _factory;
        private readonly int _initCount;

        /// <summary>
        /// 初始化对象池时触发
        /// </summary>
        public event Action<T> OnInit;

        /// <summary>
        /// 从池中获取对象时触发事件
        /// </summary>
        public event Action<T> OnGetObject;

        /// <summary>
        /// 开始回收时触发事件，用于判断该对象是否可被回收，默认不检查
        /// </summary>
        public event Func<T, bool> OnPreRecycle;

        /// <summary>
        /// 回收时触发事件，可用于重置正在回收的对象
        /// </summary>
        public event Action<T> OnRecycling;

        /// <summary>
        /// 释放对象时触发事件，可用于自定义释放资源的方法
        /// </summary>
        public event Action<T> OnRelease;

        public int InstanceCount { get; private set; }
        public bool IsInit { get; private set; }

        public ObjectPool(IFactory<T> factory, int initCount = 0)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _initCount = initCount >= 0 ? initCount : throw new ArgumentException($"{nameof(initCount)}不能小于0");
            _pool = new Stack<T>(initCount);
            InstanceCount = 0;
            IsInit = false;
        }

        public void Init()
        {
            for (var i = 0; i < _initCount; i++)
            {
                var instance = GetObjectFromFactory();
                OnInit?.Invoke(instance);
                _pool.Push(instance);
            }

            IsInit = true;
        }

        public T Get()
        {
            var result = _pool.Count > 0 ? _pool.Pop() : GetObjectFromFactory();
            OnGetObject?.Invoke(result);
            return result;
        }

        private T GetObjectFromFactory()
        {
            var get = _factory.Get();
//            OnInit?.Invoke(get);
            InstanceCount++;
            return get;
        }

        public void Recycle(T @object)
        {
            var checkResult = OnPreRecycle?.Invoke(@object);
            if (!checkResult.HasValue)
            {
                ResetAndPush(@object);
                return;
            }

            if (!checkResult.Value)
            {
                return;
            }

            ResetAndPush(@object);
        }

        private void ResetAndPush(T @object)
        {
            OnRecycling?.Invoke(@object);
            _pool.Push(@object);
        }

        public void Release()
        {
            Release(_initCount);
        }

        public void Release(int remain)
        {
            for (var i = 0; i < _pool.Count - remain; i++)
            {
                OnRelease?.Invoke(_pool.Peek());
                _pool.Pop();
            }

            _pool.TrimExcess();
            InstanceCount = _pool.Count;
        }
    }
}