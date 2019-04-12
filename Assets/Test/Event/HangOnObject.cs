using Breakdawn.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Test
{
	public class HangOnObject : MonoBehaviour
	{
		private void Start()
		{
			EventBus.Instance.CreateEvents<MyEventTest, Action>("myNoParma").Register(MyEventTest.A, () => { Debug.Log("233"); });
			EventBus.Instance.CreateEvents<MyEventTest, Action<int>>("myOneParma").Register(MyEventTest.A, Hello);
		}

		private void Update()
		{
			var a = EventBus.Instance.GetEvents<MyEventTest, Action>("myNoParma").GetEvent(MyEventTest.A);
			a();
			var b = EventBus.Instance.GetEvents<MyEventTest, Action<int>>("myOneParma").GetEvent(MyEventTest.A);
			b(233);
		}

		private void Hello(int a)
		{
			Debug.Log($"HEllo,{a}");
		}
	}
}
