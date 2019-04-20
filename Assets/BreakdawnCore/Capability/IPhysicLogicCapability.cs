namespace Breakdawn.Core
{
	public interface IPhysicLogicCapability
	{
		string CapabilityName { get; }

		int Priority { get; }

		void PhysicLogicCapability();
	}
}
