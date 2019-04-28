using System;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Core
{
	public class SeriesFactory<K, V> : ISeriesFactory<K, V>
	{
		protected readonly Dictionary<K, Func<V>> factory = new Dictionary<K, Func<V>>();
		protected readonly List<K> keys = new List<K>();

		public ISeriesFactory<K, V> Add(K key, Func<V> func)
		{
			if (!factory.ContainsKey(key))
			{
				Debug.LogError($"对象池异常:池中已包含{key}");
				return null;
			}
			if (func == null)
			{
				Debug.LogWarning($"对象池警告:将要添加null委托");
			}
			factory.Add(key, func);
			keys.Add(key);
			return this;
		}

		public V GetElement(K name)
		{
			return factory[name]();
		}

		public IEnumerable<K> GetKeyList()
		{
			return keys;
		}
	}
}
