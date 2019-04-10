using UnityEngine;
using Breakdawn.Expansion;
using System;

namespace Breakdawn.Test
{
	public class TransformCube : MonoBehaviour
	{
		private CoroutineResult<string> test;
		private Coroutine coroutine;
		private void Start()
		{
			//transform.SetLocalPos(0, 2, 0);
			Func<string> hello = new Func<string>(Hello);
			test = new CoroutineResult<string>();
			coroutine = MonoBehaviourExpansion.InvokeCoroutine(this, hello, test, 2.5F);
		}

		private void Update()
		{
			StopCoroutine(coroutine);
			if (test.TryGetResult(out var res))
			{
				Debug.Log(res);
			}
		}

		private string Hello()
		{
			return "Hello,Coroutine";
		}
	}
}
