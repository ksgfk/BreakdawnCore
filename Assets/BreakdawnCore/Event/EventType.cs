using System;
using System.Collections.Generic;

namespace Breakdawn.Event
{
	public enum EventKeys
	{
		A
	}

	public enum EventValues
	{
		A
	}

	public class EventType : IEventType<EventKeys, EventValues>
	{
		public static EventType Instance = new EventType();

		private Dictionary<EventKeys, EventValues> types;

		public EventType()
		{
			types = new Dictionary<EventKeys, EventValues>();
		}

		public void AddEventType(EventKeys eventKey, EventValues value)
		{
			types.Add(eventKey, value);
		}

		public EventValues GetEventType(EventKeys eventKey)
		{
			if (types.TryGetValue(eventKey, out var d))
			{
				return d;
			}

			throw new Exception("no");
		}
	}
}
