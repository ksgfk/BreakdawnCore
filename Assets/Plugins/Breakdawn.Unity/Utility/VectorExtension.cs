using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Breakdawn.Unity
{
    public static class VectorExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (float, float) RotateDegree(float degrees, float x, float y)
        {
            var angle = math.PI * degrees / 180.0F;
            return RotateAngle(angle, x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (float, float) RotateAngle(float angle, float x, float y)
        {
            math.sincos(angle, out var sinA, out var cosA);
            var resX = x * cosA + y * sinA;
            var resY = -x * sinA + y * cosA;
            return (resX, resY);
        }

        /// <summary>
        /// 旋转（单位：度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RotateDegree(ref this Vector2 v, float degrees)
        {
            var (x, y) = RotateDegree(degrees, v.x, v.y);
            return new Vector2(x, y);
        }

        /// <summary>
        /// 旋转（单位：弧度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RotateAngle(ref this Vector2 v, float angle)
        {
            var (x, y) = RotateAngle(angle, v.x, v.y);
            return new Vector2(x, y);
        }

        /// <summary>
        /// 旋转（单位：度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 RotateDegree(ref this float2 v, float degrees)
        {
            var (x, y) = RotateDegree(degrees, v.x, v.y);
            return new float2(x, y);
        }

        /// <summary>
        /// 旋转（单位：弧度）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 RotateAngle(ref this float2 v, float angle)
        {
            var (x, y) = RotateAngle(angle, v.x, v.y);
            return new float2(x, y);
        }
    }
}