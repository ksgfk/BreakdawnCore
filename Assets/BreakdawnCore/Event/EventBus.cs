using System;
using System.Collections.Generic;

namespace Breakdawn.Event
{
	public class EventBus
	{
		public static EventBus Instance = new EventBus();

		private Dictionary<object, Delegate> mEventDict;

		private EventBus()
		{
			mEventDict = new Dictionary<object, Delegate>();
		}

		private void Register<K, V>(IEventType<K, V> eventType, K key, Delegate callBack)
		{
			if (!mEventDict.ContainsKey(eventType.GetEventType(key)))
				mEventDict.Add(eventType.GetEventType(key), null);
			Delegate d = mEventDict[eventType.GetEventType(key)];
			if (d != null && d.GetType() != callBack.GetType())
				throw new Exception($"添加监听异常:尝试为{eventType.GetEventType(key)}添加不同类型委托.当前事件对应委托{d.GetType()},要添加委托{callBack.GetType()}");
		}

		private void OnRemoving<K, V>(IEventType<K, V> eventType, K key, Delegate callBack)
		{
			if (mEventDict.TryGetValue(eventType.GetEventType(key), out var d))
			{
				if (d == null)
					throw new Exception($"移除监听异常:没有事件{eventType.GetEventType(key)}对应委托");
				if (d.GetType() != callBack.GetType())
					throw new Exception($"移除监听异常:尝试为事件{eventType.GetEventType(key)}移除不同类型委托{callBack.GetType()},应为{d.GetType()}");
			}
			else
				throw new Exception($"移除监听异常:没有事件{eventType.GetEventType(key)}");
		}

		private void OnRemoved<K, V>(IEventType<K, V> eventType, K key)
		{
			if (mEventDict[eventType.GetEventType(key)] == null)
				mEventDict.Remove(eventType.GetEventType(key));
		}

		public void Add<K, V>(IEventType<K, V> eventType, K key, CallBack callBack)
		{
			Register(eventType, key, callBack);
			mEventDict[eventType.GetEventType(key)] = mEventDict[eventType.GetEventType(key)] as CallBack + callBack;
		}

		public void Remove<K, V>(IEventType<K, V> eventType, K key, CallBack callBack)
		{
			OnRemoving(eventType, key, callBack);
			mEventDict[eventType.GetEventType(key)] = mEventDict[eventType.GetEventType(key)] as CallBack - callBack;
			OnRemoved(eventType, key);
		}

		public void Execute<K, V>(IEventType<K, V> eventType, K key)
		{
			if (!mEventDict.TryGetValue(eventType.GetEventType(key), out Delegate d))
				return;
			if (d is CallBack c)
				c();
			else
				throw new Exception($"广播事件异常:事件{eventType.GetEventType(key)}对应委托有不同类型");
		}
	}
}
