using Breakdawn.Factory;
using Breakdawn.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Test
{
	public class AB : TemplatePool<CD>
	{
		public AB()
		{
			template.a = 5;
		}

		protected override void Init(int count)
		{
			for (int a = 0; a < count; a++)
			{
				var b = Create();
				b.a = a;
				pool.Push(b);
			}
		}
	}

	public class CD
	{
		public int a;
	}

	public class PoolTest : MonoBehaviour
	{
		public GameObject game;
		private void Start()
		{
			var b = new GameObjectPool(game, 3, new GameObjectFactory(game, transform));


			var c = new AB();

			for (int i = 0; i < 5; i++)
			{
				var d = c.Get();
				Debug.Log(d.a);
				c.Recycling(d);
			}

			//b.Get();

		}

		private void Update()
		{

		}
	}
}
