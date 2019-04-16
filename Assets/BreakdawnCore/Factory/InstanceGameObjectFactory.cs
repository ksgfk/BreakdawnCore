using UnityEngine;

namespace Breakdawn.Factory
{
	public class InstanceGameObjectFactory : TemplateFactory<GameObject>
	{
		private Transform parent;

		public InstanceGameObjectFactory(GameObject template)
		{
			this.template = template;
		}

		public InstanceGameObjectFactory(GameObject template, Transform parent) : this(template)
		{
			this.parent = parent;
		}

		public override GameObject Create()
		{
			return parent == null ? GameObject.Instantiate(template) : GameObject.Instantiate(template, parent);
		}

		public GameObject SetGameObject(Vector3 t, Quaternion q, Transform p)
		{
			parent = p;
			return GameObject.Instantiate(template, t, q, p);
		}
	}
}
