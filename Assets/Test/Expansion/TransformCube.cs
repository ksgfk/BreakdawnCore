using UnityEngine;
using Breakdawn.Expansion;
using System;
using Breakdawn.Event;

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
			coroutine = this.InvokeCoroutine(hello, "233", test, 2.5F);

			Event.EventType.Instance.AddEventType(EventKeys.A, EventValues.A);
			EventBus.Instance.Add(Event.EventType.Instance, EventKeys.A, () => { Debug.Log("emmm"); });
		}

		private void Update()
		{
			//StopCoroutine(coroutine);
			if (test.TryGetResult(out var res))
			{
				//Debug.Log(res);
			}

			EventBus.Instance.Execute(Event.EventType.Instance, EventKeys.A);
		}

		private string PrintString(string a)
		{
			return a;
		}
	}
}
