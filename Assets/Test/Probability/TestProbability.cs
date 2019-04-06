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
			List<bool> yes = new List<bool>();
			List<bool> no = new List<bool>();
			for (int a = 0; a < 1000000; a++)
			{
				if (Probability.Percent(51.25141F))
				{
					yes.Add(true);
				}
				else
				{
					no.Add(false);
				}
			}
			Debug.Log($"{yes.Count}/1000000");
			Debug.Log($"{no.Count}/1000000");
		}
	}
}
