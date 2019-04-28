using System;
using UnityEngine;

namespace Breakdawn.Core
{
	public abstract class TemplateBuffEffect : IBuffEffect
	{
		public IBuff Buff { get; }

		public float TotalTime { get; }

		public float Duration { get; set; }

		protected TemplateBuffEffect(IBuff buff, float totalTime, float duration)
		{
			Buff = buff ?? throw new ArgumentNullException(nameof(buff));
			TotalTime = totalTime;
			Duration = duration;
		}

		public void BuffFixedUpdate()
		{
			//if (Duration >= 0)
			//{
			//	if (Buff.IsReady(Duration))
			//	{
			//		Buff.BuffPhysicLogic();
			//	}
			//}
			throw new NotImplementedException("Buff异常:暂时不实现物理Buff");
		}

		public virtual void BuffUpdate()
		{
			if (Duration >= 0)
			{
				if (Buff.IsReady(Duration))
				{
					BuffUpdate();
				}
				Duration -= Time.deltaTime;
			}
		}
	}
}
