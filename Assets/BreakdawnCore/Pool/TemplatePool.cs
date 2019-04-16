using Breakdawn.Factory;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Pool
{
	public abstract class TemplatePool<T> : IObjectPool<T> where T : new()
	{
		protected Stack<T> pool;
		protected IFactory<T> factory;

		public TemplatePool(int count)
		{
			factory = new NormalFactory<T>();
			pool = new Stack<T>(count);
			Init(count);
		}

		public TemplatePool(int count, IFactory<T> factory)
		{
			this.factory = factory;
			pool = new Stack<T>(count);
			Init(count);
		}

		protected virtual void Init(int count)
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

		public virtual T Create()
		{
			return factory.Create();
		}
	}
}
