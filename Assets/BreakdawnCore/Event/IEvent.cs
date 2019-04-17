using System;

namespace Breakdawn.Core
{
	public interface IEvent<T, A> where A : Delegate
	{
		/// <summary>
		/// 添加委托
		/// </summary>
		void Register(T key, A @event);
		/// <summary>
		/// 获取委托
		/// </summary>
		A GetEvent(T @event);
		/// <summary>
		/// 移除委托
		/// </summary>
		void Remove(T Key, A @event);
	}
}
