using System;
using UnityEngine;

namespace Breakdawn.Core
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		private static T instance;

		public static T Instance
		{
			get
			{
				if (typeof(T).IsDefined(typeof(SingletonAttribute), true))
				{
					if (instance == null)
					{
						instance = FindObjectOfType<T>();
					}
					return instance ?? throw new Exception($"单例异常:Hierarchy面板中可能没有物体挂载了{typeof(T)}");
				}
				else
				{
					throw new Exception($"单例异常:请给{typeof(T)}加上Singleton特性!");
				}
			}
		}

		private void Awake()
		{
			DontDestroyOnLoad(Instance.gameObject);
			OnBeforeAwake();
		}

		/// <summary>
		/// 不可覆写Awake
		/// </summary>
		protected virtual void OnBeforeAwake() { }

		private void OnDestroy()
		{
			OnBeforeDestory();
			instance = null;
		}

		/// <summary>
		/// 不可覆写OnDestroy
		/// </summary>
		protected virtual void OnBeforeDestory() { }
	}
}
