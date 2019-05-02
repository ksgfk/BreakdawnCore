using System;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Core
{
	public class SeriesPool<K, V> : ISeriesPool<K, V>
	{
		protected readonly ISeriesFactory<K, V> factory;
		protected readonly Dictionary<K, Stack<V>> pool = new Dictionary<K, Stack<V>>();
		protected readonly Dictionary<K, Action<V>> resetMethods = new Dictionary<K, Action<V>>();

		public SeriesPool(ISeriesFactory<K, V> factory, int count = 0)
		{
			this.factory = factory;

			Init(count);
		}

		private void Init(int count)
		{
			foreach (var key in factory.GetKeyList())
			{
				var s = new Stack<V>(count);
				for (int i = 0; i < count; i++)
				{
					s.Push(factory.GetElement(key));
				}
				pool.Add(key, s);
			}
		}

		public V Get(K key)
		{
			if (pool.TryGetValue(key, out var v))
			{
				return v.Count == 0 ? factory.GetElement(key) : v.Pop();
			}
			throw new Exception($"对象池异常:不存在{key}");
		}

		public void Recycling(K key, V @object)
		{
			if (pool.TryGetValue(key, out var v))
			{
				if (resetMethods.TryGetValue(key, out var m))
				{
					m?.Invoke(@object);
				}
				v.Push(@object);
			}
			else
			{
				throw new Exception($"对象池异常:不存在{key}");
			}
		}

		public SeriesPool<K, V> SetResetMethod(K key, Action<V> action)
		{
			if (!pool.ContainsKey(key))
			{
				Debug.LogWarning($"对象池警告:尝试修改池中没有包含的对象{key}的重置方法");
			}

			if (resetMethods.ContainsKey(key))
			{
				resetMethods[key] = action;
			}
			else
			{
				resetMethods.Add(key, action);
			}
			return this;
		}
	}
}
