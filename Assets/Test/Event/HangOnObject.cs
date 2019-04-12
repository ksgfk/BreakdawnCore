using Breakdawn.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Test
{
	public class HangOnObject : MonoBehaviour
	{
		private TempletEvents<MyEventTest, Action> myEvent;

		private void Start()
		{
			myEvent = new TempletEvents<MyEventTest, Action>();

			//EventBus.Add(myEvent, MyEventTest.A, () => { Debug.Log("C# is the best language!"); });
			myEvent.Register(MyEventTest.A, () => { Debug.Log("C# is the best language!"); });
		}

		private void Update()
		{
			//EventBus.Execute(myEvent, MyEventTest.A);
			var a = myEvent.GetEvent(MyEventTest.A);
			a();
		}
	}
}
