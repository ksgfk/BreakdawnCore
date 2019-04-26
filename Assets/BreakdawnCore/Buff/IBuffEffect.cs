namespace Breakdawn.Core
{
	public interface IBuffEffect
	{
		IBuff Buff { get; }

		float TotalTime { get; }

		float Duration { get; set; }

		void BuffUpdate();

		void BuffFixedUpdate();
	}
}
