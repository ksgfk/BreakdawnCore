using System;

namespace Breakdawn.Core
{
	public abstract class TemplateBuff : IBuff
	{
		public string Name { get; }
		public bool IsHarm { get; }

		protected TemplateBuff(string name, bool isHarm)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			IsHarm = isHarm;
		}

		public virtual void BuffLogic() { }

		public virtual void BuffPhysicLogic() { }

		public virtual bool IsReady(float duration) { return false; }
	}
}
