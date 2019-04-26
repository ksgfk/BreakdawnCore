namespace Breakdawn.Core
{
	public interface IBuff
	{
		string Name { get; }

		bool IsHarm { get; }

		bool IsReady(float duration);

		void BuffLogic();

		void BuffPhysicLogic();
	}
}
