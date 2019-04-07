using Breakdawn.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Breakdawn
{
	public class TestProbability : MonoBehaviour
	{
		private void Start()
		{
			//var c = new List<int>(20000);
			//for (int a = 0; a < 20000; a++)
			//{
			//	c.Add(a);
			//}

			//Stopwatch sw = new Stopwatch();
			//sw.Start();
			//var b = Probability.GetRandomElements(c, 10000);
			//sw.Stop();
			//Debug.Log(sw.ElapsedMilliseconds);

			//var b = Probability.GetRandomNumbers(0, 6, 4);
			//foreach (var item in b)
			//{
			//	Debug.Log(item);
			//}
			//Debug.Log(b.Count);

			//foreach (var item in b)
			//{
			//	Debug.Log(item);
			//}

			var a = Probability.GetRandomNumbers(0, 10, 2);
			Debug.Log(a.Count);

			var b = Probability.Percent(2, Precision.Super);

			//foreach (var item in a)
			//{
			//	Debug.Log(item);
			//}
		}
	}
}
