using System.Collections.Generic;

namespace Breakdawn.Core
{
	/// <summary>
	/// 能力提供者,有能力组件的类实现该接口
	/// </summary>
	public interface ICapabilityProvider
	{
		IEnumerable<ILogicCapability> GetLogics();

		IEnumerable<IPhysicLogicCapability> GetPhysicLogics();

		void AddLogic(ILogicCapability logic);

		void AddPhysicLogic(IPhysicLogicCapability physicLogic);

		bool HasLogic(string name);

		bool HasPhysicLogic(string name);
	}
}
