using System;
using System.Collections.Generic;

namespace Breakdawn.Event
{
	public class EventBus
	{
		public static EventBus Instance = new EventBus();

		private Dictionary<string, object> factory = new Dictionary<string, object>();

		public TempletEvents<T, ACT> CreateEvents<T, ACT>(string name) where ACT : Delegate
		{
			var n = new TempletEvents<T, ACT>();
			factory.Add(name, n);
			return n;
		}

		public TempletEvents<T, ACT> GetEvents<T, ACT>(string name) where ACT : Delegate
		{
			if (factory.TryGetValue(name, out var v))
			{
				return v as TempletEvents<T, ACT>;
			}
			throw new Exception($"事件异常:名为{name}的委托集合没有对应值");
		}

		public void AddEvent<T,ACT>(string name, T key,ACT @event) where ACT : Delegate
		{
			var a = GetEvents<T, ACT>(name);
			a.Register(key, @event);
		}

		public ACT GetExecuteEvent<T, ACT>(TempletEvents<T, ACT> events, T key) where ACT : Delegate
		{
			return events.GetEvent(key);
		}

		public void RemoveEvents(string name)
		{
			factory.Remove(name);
		}
	}
}
