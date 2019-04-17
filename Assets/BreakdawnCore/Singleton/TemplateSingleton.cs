using System;

namespace Breakdawn.Core
{
	/// <summary>
	/// 单例模板
	/// </summary>
	/// <typeparam name="T">单例类型</typeparam>
	public abstract class TemplateSingleton<T> where T : class
	{
		private static readonly Lazy<T> instance = new Lazy<T>(() =>
		{
			var type = typeof(T);
			if (type.IsDefined(typeof(SingletonAttribute), true))
				return Activator.CreateInstance(type, true) as T;
			throw new Exception($"单例异常:请在单例类{type}上加上SingletonAttribute特性");
		}, true);

		public static T Instance { get => instance.Value; }
	}
}
