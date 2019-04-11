using System;
using System.Collections.Generic;

namespace Breakdawn.Event
{
	/// <summary>
	/// 事件基类
	/// </summary>
	/// <typeparam name="T">事件索引类型</typeparam>
	/// <typeparam name="ACT">委托类型</typeparam>
	public abstract class AbstractEvents<T, ACT>
	{
		protected Dictionary<T, ACT> events;

		protected AbstractEvents()
		{
			events = new Dictionary<T, ACT>();
		}
		/// <summary>
		/// 注册之前调用
		/// </summary>
		/// <param name="eventKey">事件索引</param>
		/// <param name="event">委托</param>
		protected void OnRegister(T eventKey, ACT @event)
		{
			if (!events.ContainsKey(eventKey))
				events.Add(eventKey, default);
			var d = events[eventKey];
			if (d != null && d.GetType() != @event.GetType())
				throw new Exception($"添加监听异常:尝试为{eventKey}添加不同类型委托.当前事件对应委托{d.GetType()},要添加委托{@event.GetType()}");
		}
		/// <summary>
		/// 删除委托之前调用
		/// </summary>
		/// <param name="eventKey">事件索引</param>
		/// <param name="event">委托</param>
		protected void OnRemoveing(T eventKey, ACT @event)
		{
			if (events.TryGetValue(eventKey, out var d))
			{
				if (d == null)
					throw new Exception($"移除监听异常:没有事件{@event}对应委托");
				if (d.GetType() != @event.GetType())
					throw new Exception($"移除监听异常:尝试为事件{eventKey}移除不同类型委托{@event.GetType()},应为{d.GetType()}");
			}
			else
				throw new Exception($"移除监听异常:没有事件{eventKey}");
		}
		/// <summary>
		/// 删除委托之后调用
		/// </summary>
		/// <param name="eventKey">事件索引</param>
		/// <param name="event">委托</param>
		protected void OnRemoved(T eventKey)
		{
			if (events[eventKey] == null)
				events.Remove(eventKey);
		}
	}
}
