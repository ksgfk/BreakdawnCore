using UnityEngine;

namespace Breakdawn.Core
{
	public static class GameObjectExpansion
	{
		public static GameObject Hide(this GameObject gameObject)
		{
			gameObject.SetActive(false);
			return gameObject;
		}

		public static GameObject Show(this GameObject gameObject)
		{
			gameObject.SetActive(true);
			return gameObject;
		}

		public static GameObject Reset(this GameObject gameObject)
		{
			gameObject.transform.ResetLocal();
			return gameObject;
		}

		public static T AddOrGetComponent<T>(this GameObject component) where T : Component
		{
			var c = component.GetComponent<T>();
			if (c == null)
			{
				c = component.AddComponent<T>();
			}
			return c;
		}
	}
}
