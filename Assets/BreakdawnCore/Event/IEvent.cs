using System;

namespace Breakdawn.Event
{
	public interface IEvent<T, A> where A : Delegate
	{
		/// <summary>
		/// 添加事件
		/// </summary>
		void Register(T key, A @event);
		/// <summary>
		/// 获取事件
		/// </summary>
		A GetEvent(T @event);
		/// <summary>
		/// 移除事件
		/// </summary>
		void Remove(T Key, A @event);
	}
}
