using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Factory
{
	public class ResourceFactory<T> : ISeriesFactory<string, T> where T : Object
	{
		private string path;
		private Dictionary<string, T> pool = new Dictionary<string, T>();

		public ResourceFactory(string path)
		{
			this.path = path;
		}

		public T GetElement(string name)
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
				pool.Add(name, obj);
			}
			return obj;
		}

		[System.Obsolete("DestroyImmediate立即删资源,编辑器里会导致资源暂时无法使用")]
		public void DestoryResource(string name)
		{
			if (pool.TryGetValue(name, out var v))
			{
				GameObject.DestroyImmediate(v, true);
				pool.Remove(name);
			}
			else
			{
				throw new System.Exception($"资源池异常:没有名为{name}的资源");
			}
		}
	}
}
