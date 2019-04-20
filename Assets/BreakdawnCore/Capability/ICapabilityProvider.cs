namespace Breakdawn.Core
{
	/// <summary>
	/// 能力提供者,有能力组件的类实现该接口
	/// </summary>
	public interface ICapabilityProvider
	{
		T GetProperty<T>() where T : class, IPropertyCapability<T>;

		void AddCustomProperty<T>(T capability) where T : class, IPropertyCapability<T>;
	}
}
