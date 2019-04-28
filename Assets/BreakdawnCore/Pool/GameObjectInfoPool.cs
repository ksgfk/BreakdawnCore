using System;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Core
{
	public class GameObjectInfoPool : ISeriesPool<GameObject, string>
	{
		private readonly Dictionary<string, Stack<GameObject>> gameObjectsPools;
		private readonly Dictionary<GameObject, Rigidbody> rigidbodys;
		private readonly ISeriesFactory<string, GameObject> prefabs;
		private Transform parent;

		public GameObjectInfoPool(ISeriesFactory<string, GameObject> prefabs, Transform parent, int count, params string[] prefabsName)
		{
			gameObjectsPools = new Dictionary<string, Stack<GameObject>>(count);
			rigidbodys = new Dictionary<GameObject, Rigidbody>(count);
			this.prefabs = prefabs;
			this.parent = parent;
			foreach (var prefab in prefabsName)
			{
				var p = prefabs.GetElement(prefab);
				var stack = new Stack<GameObject>();
				Init(p, stack, count);
				gameObjectsPools.Add(prefab, stack);
			}
		}

		private void Init(GameObject prefab, Stack<GameObject> stack, int count)
		{
			for (int i = 0; i < count; i++)
			{
				var go = InstantiateObject(prefab).Hide();
				go.transform.SetParent(parent);
				stack.Push(go);
				rigidbodys.Add(go, go.GetComponent<Rigidbody>());
			}
		}

		public GameObject Get(string name)
		{
			GameObject initGo;
			if (gameObjectsPools.TryGetValue(name, out var v))
			{
				if (v.Count == 0)
				{
					var p = prefabs.GetElement(name);
					var go = InstantiateObject(p);
					go.transform.SetParent(parent);
					rigidbodys.Add(go, go.GetComponent<Rigidbody>());
					initGo = go;
				}
				else
				{
					initGo = v.Pop();
				}
			}
			else
			{
				var p = prefabs.GetElement(name);
				var go = InstantiateObject(p);
				go.transform.SetParent(parent);
				var stack = new Stack<GameObject>();
				gameObjectsPools.Add(name, stack);
				rigidbodys.Add(go, go.GetComponent<Rigidbody>());
				initGo = go;
			}
			return initGo.Show();
		}

		public void Recycling(string name, GameObject @object)
		{
			@object.Hide();
			gameObjectsPools[name].Push(@object);
		}

		private GameObject InstantiateObject(GameObject prefabs)
		{
			var init = GameObject.Instantiate(prefabs).Hide();
			return init;
		}

		public Rigidbody GetRigidbody(GameObject g)
		{
			if (rigidbodys.TryGetValue(g, out var v))
			{
				return v;
			}
			throw new Exception($"对象池异常:不存在游戏物体{g}");
		}
	}
}
