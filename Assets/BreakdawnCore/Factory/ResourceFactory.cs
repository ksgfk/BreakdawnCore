using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Breakdawn.Core
{
	public class ResourceFactory<T> : ISeriesFactory<string, T> where T : Object
	{
		private Dictionary<string, T> pool = new Dictionary<string, T>();
		private StringBuilder buffer;

		public ResourceFactory(params string[] paths)
		{
			buffer = new StringBuilder();
			foreach (var path in paths)
			{
				buffer.Append(path).Append("/");
			}
		}

		public T GetElement(string name)
		{
			T obj;
			var resPath = buffer.Append(name).ToString();
			if (pool.TryGetValue(resPath, out var v))
			{
				obj = v;
			}
			else
			{
				var res = Resources.Load<T>(resPath);
				obj = res ?? throw new System.Exception($"资源池异常:无法找到路径为{resPath}的资源");
				pool.Add(resPath, obj);
			}
			buffer.Remove(buffer.Length - name.Length, name.Length);
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
