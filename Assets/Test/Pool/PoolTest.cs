using Breakdawn.Factory;
using Breakdawn.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Test
{
	public class PoolTest : MonoBehaviour
	{
		public GameObject game;
		private void Start()
		{
			var a = new GameObjectFactory("MyTest", game);
			a.Create();

			var b = new GameObjectPool(game, 10);
			b.Get();
		}

		private void Update()
		{

		}
	}
}
