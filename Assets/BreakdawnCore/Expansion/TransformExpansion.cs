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
		public static void SetLocalPos(this Transform transform, float x, float y, float z)
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

		public static Transform SetLocalPosition(this Transform transform, Vector3 vector3)
		{
			transform.localPosition = vector3;
			return transform;
		}

		public static Transform SetLocalRotate(this Transform transform, Vector3 axis, float angle)
		{
			transform.Rotate(axis, angle, Space.Self);
			return transform;
		}

		public static Transform SetLocalScale(this Transform transform, Vector3 vector3)
		{
			transform.localScale = vector3;
			return transform;
		}
	}
}
