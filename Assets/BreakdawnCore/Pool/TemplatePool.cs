using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Core
{
	[System.Obsolete("没有用")]
	public abstract class TemplatePool<T> : IObjectPool<T> where T : new()
	{
		protected readonly Stack<T> pool;
		protected readonly IFactory<T> factory;

		public TemplatePool(int count)
		{
			//factory = new NormalFactory<T>();
			pool = new Stack<T>(count);
			Init(count);
			throw new System.Exception("已弃用");
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

		public virtual bool Recycling(T @object)
		{
			if (@object == null)
			{
				Debug.LogWarning($"对象池:对象{@object}为空");
				return false;
			}
			pool.Push(@object);
			return true;
		}

		public virtual T Create()
		{
			return factory.Create();
		}
	}
}
