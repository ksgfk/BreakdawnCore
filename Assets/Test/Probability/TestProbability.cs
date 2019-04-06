using Breakdawn.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn
{
	public class TestProbability : MonoBehaviour
	{
		private void Start()
		{
			//List<bool> yes = new List<bool>();
			//List<bool> no = new List<bool>();
			//for (int a = 0; a < 1000000; a++)
			//{
			//	if (Probability.Percent(51.25141F))
			//	{
			//		yes.Add(true);
			//	}
			//	else
			//	{
			//		no.Add(false);
			//	}
			//}
			//Debug.Log($"{yes.Count}/1000000");
			//Debug.Log($"{no.Count}/1000000");
			List<int> c = new List<int>(20000);
			for (int a = 0; a < 20000; a++)
			{
				c.Add(a);
			}

			var b = Probability.GetRandomElements(c, 18000);
			//var b = Probability.GetRandomNumbers(0, 6, 4);
			//foreach (var item in b)
			//{
			//	Debug.Log(item);
			//}
			Debug.Log(b.Count);
		}
	}
}
