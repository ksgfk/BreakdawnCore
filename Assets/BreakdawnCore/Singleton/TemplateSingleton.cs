using System;
using System.Linq;
using System.Reflection;

namespace Breakdawn.Singleton
{
	/// <summary>
	/// 单例模板
	/// </summary>
	/// <typeparam name="T">单例类型</typeparam>
	public class TemplateSingleton<T> : ISingleton<T>
	{
		private static readonly Lazy<T> instance = new Lazy<T>(() =>
		{
			var constructors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if (constructors.Count() != 1)
				throw new Exception($"单例异常:{typeof(T)}只能有一个构造函数!");
			var constructor = constructors.SingleOrDefault(c => c.GetParameters().Count() == 0 && c.IsPrivate);
			if (constructor == null)
				throw new Exception($"单例异常:{typeof(T)}的构造函数必须是私有且无参");
			return (T)constructor.Invoke(null);
		}, true);

		public static T Instance { get => instance.Value; }

		public T GetInstance()
		{
			return instance.Value;
		}
	}
}
