using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Factory
{
	public class ResourceFactory<T> : IResourcesFactory<T> where T : Object
	{
		private string path;
		private Dictionary<string, T> pool = new Dictionary<string, T>();

		public ResourceFactory(string path)
		{
			this.path = path;
		}

		public T GetResource(string name)
		{
			T obj;
			if (pool.TryGetValue(name, out var v))
			{
				obj = v;
			}
			else
			{
				var resPath = $"{path}/{name}";
				var res = Resources.Load<T>(resPath);
				obj = res ?? throw new System.Exception($"资源池异常:无法找到路径为{resPath}的资源");
			}
			pool.Add(name, obj);
			return obj;
		}
	}
}
