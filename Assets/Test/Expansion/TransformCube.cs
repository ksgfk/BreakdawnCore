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
			Func<string, string> hello = new Func<string, string>(PrintString);
			test = new CoroutineResult<string>();
			coroutine = MonoBehaviourExpansion.InvokeCoroutine(this, hello, "233", test, 2.5F);
		}

		private void Update()
		{
			//StopCoroutine(coroutine);
			if (test.TryGetResult(out var res))
			{
				Debug.Log(res);
				//gameObject.Hide();
			}
		}

		private string PrintString(string a)
		{
			return a;
		}
	}
}
