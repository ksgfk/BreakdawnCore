using UnityEngine;

namespace Breakdawn.Factory
{
	public class GameObjectFactory : TemplateFactory<GameObject>
	{
		public GameObjectFactory(string name, GameObject template)
		{
			base.template = template;
			this.name = name;
		}

		public override GameObject Create()
		{
			return GameObject.Instantiate(template);
		}

		public GameObject Create(Vector3 t, Quaternion q, Transform p)
		{
			return GameObject.Instantiate(template, t, q, p);
		}
	}
}
