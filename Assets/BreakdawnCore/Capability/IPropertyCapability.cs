namespace Breakdawn.Core
{
	/// <summary>
	/// 数据能力接口
	/// </summary>
	public interface IPropertyCapability<T> where T : class
	{
		string CapabilityName { get; }

		T PropertyCapability { get; }
	}
}
