namespace Breakdawn.Core
{
	/// <summary>
	/// 能力
	/// </summary>
	/// <typeparam name="T">实现能力的类</typeparam>
	public interface ICapability<T> where T : class
	{
		string CapabilityName { get; }

		T GetCapability { get; }
	}
}
