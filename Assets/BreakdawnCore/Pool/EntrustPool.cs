using System;
using System.Collections.Generic;

namespace Breakdawn.Core
{
	public class EntrustPool<T> : IObjectPool<T>
	{
		protected Action<T> reset;
		protected Stack<T> pool;
		protected IFactory<T> factory;

		public EntrustPool(EntrustFactory<T> factory, int count = 0, Action<T> reset = null)
		{
			this.reset = reset;
			this.factory = factory;
			pool = new Stack<T>();
		}

		public T Get()
		{
			return pool.Count == 0 ? factory.Create() : pool.Pop();
		}

		public bool Recycling(T @object)
		{
			reset?.Invoke(@object);
			pool.Push(@object);
			return true;
		}
	}
}
