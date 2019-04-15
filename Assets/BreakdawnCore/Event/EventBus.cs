using Breakdawn.Singleton;
using System;
using System.Collections.Generic;

namespace Breakdawn.Event
{
	/// <summary>
	/// 事件总线，管理所有事件
	/// </summary>
	public class EventBus : TemplateSingleton<EventBus>
	{
		private readonly Dictionary<string, object> factory = new Dictionary<string, object>();
		/// <summary>
		/// 新建一个委托集合实例。
		/// </summary>
		public IEvent<T, ACT> CreateEvents<T, ACT>(string name) where ACT : Delegate
		{
			var n = new TemplateEvents<T, ACT>();
			factory.Add(name, n);
			return n;
		}
		/// <summary>
		/// 添加一个委托集合
		/// </summary>
		public IEvent<T, ACT> AddEvents<T, ACT>(IEvent<T, ACT> @event, string name) where ACT : Delegate
		{
			factory.Add(name, @event);
			return @event;
		}
		/// <summary>
		/// 获取一个委托集合
		/// </summary>
		public IEvent<T, ACT> GetEvents<T, ACT>(string name) where ACT : Delegate
		{
			if (factory.TryGetValue(name, out var v))
			{
				return v as IEvent<T, ACT>;
			}
			throw new Exception($"事件异常:名为{name}的委托集合没有对应值");
		}
		/// <summary>
		/// 向委托集合添加委托
		/// </summary>
		public void AddEvent<T, ACT>(string name, T key, ACT @event) where ACT : Delegate
		{
			var a = GetEvents<T, ACT>(name);
			a.Register(key, @event);
		}
		/// <summary>
		/// 获取委托集合中的委托
		/// </summary>
		public ACT GetExecuteEvent<T, ACT>(IEvent<T, ACT> events, T key) where ACT : Delegate
		{
			return events.GetEvent(key);
		}
		/// <summary>
		/// 删除委托集合
		/// </summary>
		public void RemoveEvents(string name)
		{
			factory.Remove(name);
		}
	}
}
