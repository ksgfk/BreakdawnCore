using System;

namespace Breakdawn.Core
{
	/// <summary>
	/// 创建单例时使用该特性
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class SingletonAttribute : Attribute { }
}
