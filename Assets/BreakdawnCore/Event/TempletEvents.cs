using System;
using System.Collections.Generic;

namespace Breakdawn.Event
{
	public class TempletEvents<T, ACT> : IEvent<T, ACT> where ACT : Delegate
	{
		protected Dictionary<T, ACT> events = new Dictionary<T, ACT>();

		public ACT GetEvent(T @event)
		{
			if (events.TryGetValue(@event, out var action))
			{
				return action;
			}
			throw new Exception($"获取事件异常:未找到委托{@event},检查是否未注册");
		}

		public void Register(T key, ACT @event)
		{
			EventUtils.OnRegister(events, key, @event);
			events[key] = (ACT)Delegate.Combine(events[key], @event);
		}

		public void Remove(T key, ACT @event)
		{
			EventUtils.OnRemoveing(events, key, @event);
			events[key] = (ACT)Delegate.Remove(events[key], @event);
			EventUtils.OnRemoved(events, key);
		}
	}
}
