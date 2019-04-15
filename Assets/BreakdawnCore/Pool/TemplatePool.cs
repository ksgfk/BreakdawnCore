using Breakdawn.Factory;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Pool
{
	public abstract class TemplatePool<T> : IObjectPool<T>, IFactory<T>
	{
		protected T template;
		protected Stack<T> pool;

		public TemplatePool()
		{
		}

		public TemplatePool(T t, int count)
		{
			template = t;
			pool = new Stack<T>(count);
			Init(count);
		}

		private void Init(int count)
		{
			for (int a = 0; a < count; a++)
			{
				pool.Push(Create());
			}
		}

		public virtual T Get()
		{
			return pool.Count == 0 ? Create() : pool.Pop();
		}

		public virtual void Recycling(T @object)
		{
			if (@object == null)
			{
				Debug.LogWarning($"对象池:对象{@object}为空");
			}
			pool.Push(@object);
		}

		public abstract T Create();
	}
}
