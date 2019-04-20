namespace Breakdawn.Core
{
	public interface ILogicCapability
	{
		void LogicCapability();

		int Priority { get; }

		string CapabilityName { get; }
	}
}
