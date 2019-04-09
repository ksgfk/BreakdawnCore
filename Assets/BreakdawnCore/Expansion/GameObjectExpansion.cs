using UnityEngine;

namespace Breakdawn.Expansion
{
	public static class GameObjectExpansion
	{
		public static void Hide(this GameObject gameObject)
		{
			gameObject.SetActive(false);
		}

		public static void Show(this GameObject gameObject)
		{
			gameObject.SetActive(true);
		}

		public static void Reset(this GameObject gameObject)
		{
			gameObject.transform.ResetLocal();
		}
	}
}
