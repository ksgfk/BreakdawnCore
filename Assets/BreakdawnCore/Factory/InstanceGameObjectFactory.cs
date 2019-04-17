using UnityEngine;

namespace Breakdawn.Core
{
	public class InstanceGameObjectFactory : IFactory<GameObject>
	{
		private GameObject template;
		private Transform parent;

		public GameObject Template { get => template; }

		public InstanceGameObjectFactory(GameObject template)
		{
			this.template = template;
		}

		public InstanceGameObjectFactory(GameObject template, Transform parent) : this(template)
		{
			this.parent = parent;
		}

		public GameObject Create()
		{
			return parent == null ? GameObject.Instantiate(template) : GameObject.Instantiate(template, parent);
		}
	}
}
