using System;
using UnityEngine;

namespace Breakdawn.Core
{
	public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		protected static T instance;

		public static T Instance
		{
			get
			{
				T i;
				i = instance ?? FindObjectOfType<T>();
				if (i == null)
				{
					throw new Exception($"单例异常:无法找到挂载在了{typeof(T)}脚本的物体");
				}
				return i;
			}
		}
	}
}
