using Breakdawn.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Test
{
	public enum MyEventKey
	{
		A2
	}

	public class EventTest : IEventType<MyEventKey, MyEventKey>
	{
		public static EventTest Instance = new EventTest();

		private Dictionary<MyEventKey, MyEventKey> types;

		public EventTest()
		{
			types = new Dictionary<MyEventKey, MyEventKey>();
		}

		public void AddEventType(MyEventKey eventKey, MyEventKey value)
		{
			types.Add(eventKey, value);
		}

		public MyEventKey GetEventType(MyEventKey eventKey)
		{
			if (types.TryGetValue(eventKey, out var d))
			{
				return d;
			}

			throw new System.Exception("no");
		}
	}
}
