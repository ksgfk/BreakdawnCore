using Breakdawn.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Test
{
	public class HangOnObject : MonoBehaviour
	{
		private TestMyEvent myEvent;

		private void Start()
		{
			myEvent = new TestMyEvent();
			EventBus.Add(myEvent, MyEventTest.A, () => { Debug.Log("C# is the best language!"); });
		}

		private void Update()
		{
			EventBus.Execute(myEvent, MyEventTest.A);
		}
	}
}
