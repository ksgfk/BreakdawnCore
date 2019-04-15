using System;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Event
{
	public abstract class MonoMessage<ACT> : MonoBehaviour, IEvent<string, ACT> where ACT : Delegate
	{
		protected Dictionary<string, ACT> events = new Dictionary<string, ACT>();

		private void Awake()
		{
			EventBus.Instance.AddEvents(this, ToString());
		}

		private void OnDestroy()
		{
			OnBeforeDestroy();
			EventBus.Instance.RemoveEvents(ToString());
			events.Clear();
		}
		/// <summary>
		/// 用来代替OnDestroy，子类中最好不要使用OnDestroy
		/// </summary>
		public abstract void OnBeforeDestroy();

		public ACT GetEvent(string @event)
		{
			if (events.TryGetValue(@event, out var action))
			{
				return action;
			}
			throw new Exception($"获取事件异常:未找到委托{@event},检查是否未注册");
		}

		public void Register(string key, ACT @event)
		{
			EventUtils.OnRegister(events, key, @event);
			events[key] = (ACT)Delegate.Combine(events[key], @event);
		}

		public void Remove(string key, ACT @event)
		{
			EventUtils.OnRemoveing(events, key, @event);
			events[key] = (ACT)Delegate.Remove(events[key], @event);
			EventUtils.OnRemoved(events, key);
		}

		public override string ToString()
		{
			return $"{base.ToString()}_{GetHashCode()}";
		}
	}
}
