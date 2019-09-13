using System.Runtime.CompilerServices;
#if UNITY_2019
using Unity.Mathematics;
#endif
using UnityEngine;

namespace Breakdawn.Unity
{
    public static class VectorExtension
    {
#if UNITY_2019
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RotateDegree(float degrees, float x, float y)
        {
            var angle = math.PI * degrees / 180.0F;
            return RotateRadian(angle, x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RotateRadian(float radian, float x, float y)
        {
            math.sincos(radian, out var sinA, out var cosA);
            var resX = x * cosA - y * sinA;
            var resY = x * sinA + y * cosA;
            return new Vector2(resX, resY);
        }

        /// <summary>
        /// 旋转（单位：度）
        /// </summary>
        public static Vector2 RotateDegree(this Vector2 v, float degrees)
        {
            return RotateDegree(degrees, v.x, v.y);
        }

        /// <summary>
        /// 旋转（单位：弧度）
        /// </summary>
        public static Vector2 RotateRadian(this Vector2 v, float angle)
        {
            return RotateRadian(angle, v.x, v.y);
        }

        /// <summary>
        /// 旋转（单位：度）
        /// </summary>
        public static float2 RotateDegree(this float2 v, float degrees)
        {
            return RotateDegree(degrees, v.x, v.y);
        }

        /// <summary>
        /// 旋转（单位：弧度）
        /// </summary>
        public static float2 RotateRadian(this float2 v, float angle)
        {
            return RotateRadian(angle, v.x, v.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CoordinateRotateDegree(float degree, float x, float y)
        {
            var radian = math.PI * degree / 180.0F;
            return CoordinateRotateRadian(radian, x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CoordinateRotateRadian(float radian, float x, float y)
        {
            math.sincos(radian, out var sinA, out var cosA);
            var resX = x * cosA + y * sinA;
            var resY = y * cosA - x * sinA;
            return new Vector2(resX, resY);
        }

        /// <summary>
        /// 坐标系旋转后，该点在坐标系中的位置（单位：度）
        /// </summary>
        public static Vector2 CoordinateRotateDegree(this Vector2 v, float angle)
        {
            return CoordinateRotateDegree(angle, v.x, v.y);
        }

        /// <summary>
        /// 坐标系旋转后，该点在坐标系中的位置（单位：弧度）
        /// </summary>
        public static Vector2 CoordinateRotateRadian(this Vector2 v, float angle)
        {
            return CoordinateRotateRadian(angle, v.x, v.y);
        }
        
        /// <summary>
        /// 坐标系旋转后，该点在坐标系中的位置（单位：度）
        /// </summary>
        public static Vector2 CoordinateRotateDegree(this float2 v, float angle)
        {
            return CoordinateRotateDegree(angle, v.x, v.y);
        }

        /// <summary>
        /// 坐标系旋转后，该点在坐标系中的位置（单位：弧度）
        /// </summary>
        public static Vector2 CoordinateRotateRadian(this float2 v, float angle)
        {
            return CoordinateRotateRadian(angle, v.x, v.y);
        }
#endif
    }
}