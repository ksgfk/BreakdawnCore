using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Event
{
	public class TempletEvents<T, ACT> : IEvent<T, ACT> where ACT : Delegate
	{
		private Dictionary<T, ACT> events = new Dictionary<T, ACT>();

		/// <summary>
		/// 注册之前调用
		/// </summary>
		/// <param name="eventKey">事件索引</param>
		/// <param name="event">委托</param>
		private void OnRegister(T eventKey, ACT @event)
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
		private void OnRemoveing(T eventKey, ACT @event)
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
		private void OnRemoved(T eventKey)
		{
			if (events[eventKey] == null)
				events.Remove(eventKey);
		}

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
			OnRegister(key, @event);
			if (@event is Action)
			{
				events[key] = (ACT)Delegate.Combine(events[key], @event);
			}
		}

		public void Remove(T key, ACT @event)
		{
			OnRemoveing(key, @event);
			events[key] = (ACT)Delegate.Remove(events[key], @event);
			OnRemoved(key);
		}
	}
}
