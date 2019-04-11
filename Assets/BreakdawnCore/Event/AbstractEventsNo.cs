using System;

namespace Breakdawn.Event
{
	/// <summary>
	/// 无参委托
	/// </summary>
	public abstract class AbstractEventsNo<T> : AbstractEvents<T, Action>, IEventType<T>
	{
		public static AbstractEventsNo<T> Instance;

		public virtual void Register(T eventKey, Action @event)
		{
			OnRegister(eventKey, @event);
			events[eventKey] = events[eventKey] + @event;
		}

		public virtual Action GetEvent(T @event)
		{
			if (events.TryGetValue(@event, out var action))
			{
				return action;
			}
			throw new Exception($"获取事件异常:未找到委托{@event},检查是否未注册");
		}

		public virtual void RemoveEvent(T eventKey, Action @event)
		{
			OnRemoveing(eventKey, @event);
			events[eventKey] = events[eventKey] - @event;
			OnRemoved(eventKey);
		}
	}
}
