namespace Breakdawn.Core
{
	/// <summary>
	/// 能力提供者,有能力组件的类实现该接口
	/// </summary>
	public interface ICapabilityProvider
	{
		T GetCapability<T>() where T : class, ICapability<T>;

		void SetCustomCapability<T>(T capability) where T : class, ICapability<T>;
	}
}
