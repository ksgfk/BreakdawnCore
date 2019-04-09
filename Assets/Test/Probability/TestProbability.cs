using Breakdawn.Utils;
using Breakdawn.Expansion;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Breakdawn.Test
{
	public class TestProbability : MonoBehaviour
	{
		private bool key = true;
		private void Start()
		{
			//var c = new Dictionary<int,int>(20000);
			//for (int a = 0; a < 20000; a++)
			//{
			//	c.Add(a, a);
			//}
			//Stopwatch sw = new Stopwatch();
			//sw.Start();
			//var b = Probability.GetRandomElements(c, 10000);
			//sw.Stop();
			//Debug.Log(sw.ElapsedMilliseconds);


		}

		private void Update()
		{
			if (key)
			{
				var a = new Dictionary<int, int>(50000);
				for (int b = 0; b < 50000; b++)
				{
					a.Add(b, b);
				}
				//var a = new List<int>(50000);
				//for (int b = 0; b < 50000; b++)
				//{
				//	a.Add(b);
				//}
				Stopwatch sw = new Stopwatch();
				sw.Start();
				var c = Probability.GetRandomElements(a, 25000);
				sw.Stop();
				Debug.Log($"结果的元素量{c.Count}");
				Debug.Log($"耗时{sw.ElapsedMilliseconds}");

				key = false;
			}
		}
	}
}
