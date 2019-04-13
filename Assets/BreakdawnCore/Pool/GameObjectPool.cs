using Breakdawn.Expansion;
using UnityEngine;

namespace Breakdawn.Pool
{
	public class GameObjectPool : TemplatePool<GameObject>
	{
		public GameObjectPool(GameObject t,int count) : base(t, count)
		{

		}

		public override void Recycling(GameObject @object)
		{
			@object.Hide();
			base.Recycling(@object);
		}

		public override GameObject Create()
		{
			return GameObject.Instantiate(template).Hide();
		}
	}
}
