using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Breakdawn.Core
{
	/// <summary>
	/// 管理游戏所有入口和启动流程
	/// </summary>
	public abstract class TemplateMainManager<T> : MonoSingleton<T> where T : MonoSingleton<T>
	{
		public Environment environment = Environment.Develop;

		private static Environment mSharedENV;
		private static bool mIsENVSetted = false;

		private void Awake()
		{
			if (!mIsENVSetted)
			{
				mSharedENV = environment;
				mIsENVSetted = true;
			}

			switch (mSharedENV)
			{
				case Environment.Develop:
					CheckSingleton();
					AwakeLunchDevelop();
					break;
				case Environment.Production:
					AwakeLunchProduction();
					break;
				case Environment.Test:
					AwakeLunchTest();
					break;
				default:
					throw new Exception($"初始化错误:How can it be!{environment}");
			}
		}

		protected abstract void AwakeLunchDevelop();

		protected abstract void AwakeLunchProduction();

		protected abstract void AwakeLunchTest();

		private void CheckSingleton()
		{
			var classes = Assembly.GetExecutingAssembly().GetTypes();
			var types = from need in classes where need.IsDefined(typeof(SingletonAttribute)) select need;
			foreach (var item in types)
			{
				var t = item.BaseType;
				var isCon = false;
				while (true)
				{
					if (t != typeof(MonoBehaviour))
					{
						t = t.BaseType;
						isCon = true;
						break;
					}
					else if (t == null)
					{
						isCon = false;
						break;
					}
					else
					{
						t = t.BaseType;
					}
				}
				if (isCon)
					continue;
				if (item.IsAbstract)
					Debug.LogError($"单例异常:{item}是抽象类,不能实例化,不需要SingletonAttribute特性");
				var constructors = item.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				if (constructors.Count() != 1)
					Debug.LogError($"单例异常:{constructors}只能有一个构造函数!");
				var constructor = constructors.SingleOrDefault(c => c.GetParameters().Count() == 0 && c.IsPrivate);
				if (constructor == null)
					Debug.LogError($"单例异常:{item}的构造函数必须是私有且无参");
			}
		}
	}
}
