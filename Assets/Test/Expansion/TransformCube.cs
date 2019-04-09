using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breakdawn.Expansion;

namespace Breakdawn.Test
{
	public class TransformCube : MonoBehaviour
	{
		private void Start()
		{
			transform.SetLocalPos(0, 2, 0);
		}

		private void Update()
		{
			//Debug.Log($"{Probability.Percent(Precision.Super, 25)}");
			//Debug.Log($"{Probability.Percent(100,50)}");
		}
	}
}
