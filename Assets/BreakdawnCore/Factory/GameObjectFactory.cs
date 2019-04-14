using UnityEngine;

namespace Breakdawn.Factory
{
	public class GameObjectFactory : TemplateFactory<GameObject>
	{
		public GameObjectFactory(GameObject template)
		{
			base.template = template;
		}

		public override GameObject Create()
		{
			return GameObject.Instantiate(template);
		}

		public GameObject Set(Vector3 t, Quaternion q, Transform p)
		{
			return GameObject.Instantiate(template, t, q, p);
		}
	}
}
