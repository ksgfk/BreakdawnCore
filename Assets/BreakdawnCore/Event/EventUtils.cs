using System;
using System.Collections.Generic;

namespace Breakdawn.Core
{
	public static class EventUtils
	{
		/// <summary>
		/// 注册之前调用
		/// </summary>
		public static void OnRegister<T, ACT>(Dictionary<T, ACT> events, T eventKey, ACT @event) where ACT : Delegate
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
		public static void OnRemoveing<T, ACT>(Dictionary<T, ACT> events, T eventKey, ACT @event) where ACT : Delegate
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
		public static void OnRemoved<T, ACT>(Dictionary<T, ACT> events, T eventKey) where ACT : Delegate
		{
			if (events[eventKey] == null)
				events.Remove(eventKey);
		}
	}
}
