using UnityEngine;

namespace Breakdawn.Expansion
{
	public static class TransformExpansion
	{
		/// <summary>
		/// 设置坐标
		/// </summary>
		/// <param name="x">x轴</param>
		/// <param name="y">y轴</param>
		/// <param name="z">z轴</param>
		public static void SetLocalPos(this Transform transform, int x, int y, int z)
		{
			var pos = transform.localPosition;
			pos.x = x;
			pos.y = y;
			pos.z = z;
			transform.localPosition = pos;
		}
		/// <summary>
		/// 重置坐标，缩放和旋转
		/// </summary>
		/// <param name="transform"></param>
		public static void ResetLocal(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			transform.localRotation = Quaternion.identity;
		}
	}
}
