using System;

namespace Breakdawn.Singleton
{
	/// <summary>
	/// 创建单例时使用该特性,用于开发环境下检查单例创建是否正确
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class SingletonAttribute : Attribute { }
}
