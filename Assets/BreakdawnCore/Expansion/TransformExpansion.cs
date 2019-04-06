using UnityEngine;

namespace Breakdawn.Expansion
{
	public static class TransformExpansion
	{
		public static void SetLocalPos(this Transform transform, int x, int y, int z)
		{
			var pos = transform.localPosition;
			pos.x = x;
			pos.y = y;
			pos.z = z;
			transform.localPosition = pos;
		}
	}
}
