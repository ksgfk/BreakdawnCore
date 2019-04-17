﻿using Breakdawn.Core;
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
				int z;
				Stopwatch sw = new Stopwatch();
				sw.Start();
				IList<int> c = Probability.GetRandomElements(a, 25000, true);
				foreach (var item in c)
				{
					z = a[item];
				}
				sw.Stop();
				Debug.Log($"结果的元素量{c.Count}");
				Debug.Log($"耗时{sw.ElapsedMilliseconds}");

				//Stopwatch sw1 = new Stopwatch();
				//sw.Start();
				//var d = Probability.GetRandomNumbers(0, 50000, 25000);
				//sw.Stop();
				//Debug.Log($"结果的元素量{d.Count}");
				//Debug.Log($"耗时{sw1.ElapsedMilliseconds}");

				//Debug.Log($"{d[0]} {d[1]} {d[2]} {d[3]} {d[4]}");

				key = false;
			}
		}
	}
}
