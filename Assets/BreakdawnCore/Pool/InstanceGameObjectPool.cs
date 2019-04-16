using Breakdawn.Expansion;
using Breakdawn.Factory;
using UnityEngine;

namespace Breakdawn.Pool
{
	public class InstanceGameObjectPool : TemplatePool<GameObject>
	{
		public InstanceGameObjectPool(GameObject t, int count, InstanceGameObjectFactory factory) : base(t, count, factory) { }

		public override void Recycling(GameObject @object)
		{
			@object.Hide();
			base.Recycling(@object);
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
