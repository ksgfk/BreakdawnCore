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

			//var a = new List<int>(20000);
			//for (int b = 0; b < 20000; b++)
			//{
			//	a.Add(b);
			//}
			//Stopwatch sw = new Stopwatch();
			//sw.Start();
			//var c = Probability.GetRandomElements(a, 10000);
			//sw.Stop();
			//Debug.Log($"结果list的元素量{c.Count}");
			//Debug.Log($"耗时{sw.ElapsedMilliseconds}");

			this.InvokeCoroutine(Hello, 2);
		}

		private void Hello()
		{
			Debug.Log("hello,Coroutine");
		}
	}
}
