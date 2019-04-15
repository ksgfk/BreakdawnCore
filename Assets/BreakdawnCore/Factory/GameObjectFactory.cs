using UnityEngine;

namespace Breakdawn.Factory
{
	public class GameObjectFactory : TemplateFactory<GameObject>
	{
		private Transform parent;

		public GameObjectFactory(GameObject template)
		{
			this.template = template;
		}

		public GameObjectFactory(GameObject template, Transform parent) : this(template)
		{
			this.parent = parent;
		}

		public override GameObject Create()
		{
			return parent == null ? GameObject.Instantiate(template) : GameObject.Instantiate(template, parent);
		}

		public GameObject SetGameObject(Vector3 t, Quaternion q, Transform p)
		{
			return GameObject.Instantiate(template, t, q, p);
		}

		public void SetParentOnGameObjectInit(Transform transform)
		{
			parent = transform;
		}
	}
}
