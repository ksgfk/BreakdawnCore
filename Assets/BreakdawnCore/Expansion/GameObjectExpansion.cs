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
	}
}
