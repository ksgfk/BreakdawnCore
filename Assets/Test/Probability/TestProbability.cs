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
			var a = new Vector3[] { Vector3.zero, Vector3.up, new Vector3(1, 2, 3), new Vector3(1, 3, 3), new Vector3(1, 5, 2), new Vector3(1, 1, 3), new Vector3(3, 2, 3) };

			var b = Probability.GetRandomElements(a, 7);
			//var b = Probability.GetRandomNumbers(0, 6, 4);
			foreach (var item in b)
			{
				Debug.Log(item);
			}
		}
	}
}
