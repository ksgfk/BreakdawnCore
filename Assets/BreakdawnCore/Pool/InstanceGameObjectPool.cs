using UnityEngine;

namespace Breakdawn.Core
{
	[System.Obsolete("用EntrustPool")]
	public class InstanceGameObjectPool : TemplatePool<GameObject>
	{
		public InstanceGameObjectPool(int count, InstanceGameObjectFactory factory) : base(count, factory) { }

		public override bool Recycling(GameObject @object)
		{
			@object.Hide();
			return base.Recycling(@object);
		}

		public override GameObject Create()
		{
			return factory.Create().Hide();
		}

		public override GameObject Get()
		{
			return base.Get().Show();
		}
	}
}
