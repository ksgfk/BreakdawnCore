using System;

namespace Breakdawn.Event
{
	public static class EventBus
	{
		#region NoParma
		public static void Add<T>(IEventType<T> eventType, T key, Action action)
		{
			eventType.Register(key, action);
		}

		public static void Remove<T>(IEventType<T> eventType, T key, Action action)
		{
			eventType.RemoveEvent(key, action);
		}

		public static void Execute<T>(IEventType<T> eventType, T key)
		{
			var action = eventType.GetEvent(key);
			action();
		}
		#endregion
	}
}
