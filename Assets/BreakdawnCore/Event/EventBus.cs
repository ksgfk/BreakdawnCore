using System;
using System.Collections.Generic;

namespace Breakdawn.Core
{
	/// <summary>
	/// 事件总线，管理所有事件
	/// </summary>
	[Singleton]
	public class EventBus : TemplateSingleton<EventBus>
	{
		private readonly Dictionary<string, object> factory = new Dictionary<string, object>();//TODO:感觉用string来储存还是太emm,但是又想不到更好的

		private EventBus() { }

		/// <summary>
		/// 新建一个委托集合实例。
		/// </summary> 
		public static IEvent<T, ACT> CreateEvents<T, ACT>(string name) where ACT : Delegate
		{
			var n = new TemplateEvents<T, ACT>(name);
			Instance.factory.Add(name, n);
			return n;
		}
		/// <summary>
		/// 添加一个委托集合
		/// </summary>
		public static IEvent<T, ACT> AddEvents<T, ACT>(IEvent<T, ACT> @event, string name) where ACT : Delegate
		{
			Instance.factory.Add(name, @event);
			return @event;
		}
		/// <summary>
		/// 获取一个委托集合
		/// </summary>
		public static IEvent<T, ACT> GetEvents<T, ACT>(string name) where ACT : Delegate
		{
			if (Instance.factory.TryGetValue(name, out var v))
			{
				return v as IEvent<T, ACT>;
			}
			throw new Exception($"事件异常:名为{name}的委托集合没有对应值");
		}
		/// <summary>
		/// 向委托集合添加委托
		/// </summary>
		public static void AddEvent<T, ACT>(string name, T key, ACT @event) where ACT : Delegate
		{
			var a = GetEvents<T, ACT>(name);
			a.Register(key, @event);
		}
		/// <summary>
		/// 获取委托集合中的委托
		/// </summary>
		public static ACT GetExecuteEvent<T, ACT>(IEvent<T, ACT> events, T key) where ACT : Delegate
		{
			return events.GetEvent(key);
		}
		/// <summary>
		/// 删除委托集合
		/// </summary>
		public static void RemoveEvents(string name)
		{
			Instance.factory.Remove(name);
		}
	}
}
