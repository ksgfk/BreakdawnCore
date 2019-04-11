using System;

namespace Breakdawn.Event
{
	public interface IEventType<T>
	{
		/// <summary>
		/// 添加事件
		/// </summary>
		void Register(T eventKey, Action @event);
		/// <summary>
		/// 获取事件
		/// </summary>
		Action GetEvent(T @event);
		/// <summary>
		/// 
		/// </summary>
		void RemoveEvent(T eventKey, Action @event);
	}

	public interface IEventType<T, A>
	{
		/// <summary>
		/// 添加事件
		/// </summary>
		void Register(T eventKey, Action<A> @event);
		/// <summary>
		/// 获取事件
		/// </summary>
		Action<A> GetEvent(T @event);
		/// <summary>
		/// 移除事件
		/// </summary>
		void RemoveEvent(T eventKey, Action<A> @event);
	}
}
