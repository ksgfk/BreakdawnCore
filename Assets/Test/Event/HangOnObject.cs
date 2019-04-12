using Breakdawn.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Test
{
	public enum MyEventTest
	{
		A
	}

	public class HangOnObject : MonoBehaviour
	{
		private void Start()
		{
			EventBus.Instance.CreateEvents<MyEventTest, Action>("myNoParma").Register(MyEventTest.A, () => { Debug.Log("233"); });
			EventBus.Instance.CreateEvents<MyEventTest, Action<int>>("myOneParma").Register(MyEventTest.A, Hello);
			EventBus.Instance.CreateEvents<MyEventTest, Func<int>>("myNoParmaR").Register(MyEventTest.A, Hello);
		}

		private void Update()
		{
			//var a = EventBus.Instance.GetEvents<MyEventTest, Action>("myNoParma").GetEvent(MyEventTest.A);
			//a();
			//var b = EventBus.Instance.GetEvents<MyEventTest, Action<int>>("myOneParma").GetEvent(MyEventTest.A);
			//b(233);
			//var c = EventBus.Instance.GetEvents<MyEventTest, Func<int>>("myNoParmaR").GetEvent(MyEventTest.A);
			//Debug.Log(c());
		}

		private void Hello(int a)
		{
			Debug.Log($"HEllo,{a}");
		}

		private int Hello()
		{
			return 666;
		}
	}
}
