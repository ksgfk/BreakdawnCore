using System;

namespace Breakdawn.Event
{
	/// <summary>
	/// 单参委托
	/// </summary>
	public class AbstractEventsOne<T, A> : AbstractEvents<T, Action<A>>, IEventType<T, A>
	{
		public void Register(T eventKey, Action<A> @event)
		{
			OnRegister(eventKey, @event);
			events[eventKey] = events[eventKey] + @event;
		}

		public Action<A> GetEvent(T @event)
		{
			if (events.TryGetValue(@event, out var action))
			{
				return action;
			}
			throw new Exception($"获取事件异常:未找到委托{@event},检查是否未注册");
		}

		public void RemoveEvent(T eventKey, Action<A> @event)
		{
			OnRemoveing(eventKey, @event);
			events[eventKey] = events[eventKey] - @event;
			OnRemoved(eventKey);
		}
	}
}
