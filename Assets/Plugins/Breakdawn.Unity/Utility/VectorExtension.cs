using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Breakdawn.Unity
{
    public static class VectorExtension
    {
        /// <summary>
        /// 旋转(单位:度)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RotateDegree(float degrees, Vector2 v)
        {
            var angle = math.PI * degrees / 180.0F;
            return RotateRadian(angle, v);
        }

        /// <summary>
        /// 旋转(单位:弧度)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RotateRadian(float radian, Vector2 v)
        {
            math.sincos(radian, out var sinA, out var cosA);
            return math.mul(v, new float2x2(cosA, sinA, -sinA, cosA));
        }

        public static Vector2 RotateDegree(this Vector2 v, float degrees)
        {
            return RotateDegree(degrees, v);
        }

        public static Vector2 RotateRadian(this Vector2 v, float angle)
        {
            return RotateRadian(angle, v);
        }

        public static float2 RotateDegree(this float2 v, float degrees)
        {
            return RotateDegree(degrees, v);
        }

        public static float2 RotateRadian(this float2 v, float angle)
        {
            return RotateRadian(angle, v);
        }

        /// <summary>
        /// 坐标旋转(单位:度)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CoordinateRotateDegree(float degree, float x, float y)
        {
            var radian = math.PI * degree / 180.0F;
            return CoordinateRotateRadian(radian, x, y);
        }

        /// <summary>
        /// 坐标旋转(单位:弧度)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CoordinateRotateRadian(float radian, float x, float y)
        {
            math.sincos(radian, out var sinA, out var cosA);
            return math.mul(new float2(x, y), new float2x2(cosA, -sinA, sinA, cosA));
        }

        public static Vector2 CoordinateRotateDegree(this Vector2 v, float angle)
        {
            return CoordinateRotateDegree(angle, v.x, v.y);
        }

        public static Vector2 CoordinateRotateRadian(this Vector2 v, float angle)
        {
            return CoordinateRotateRadian(angle, v.x, v.y);
        }

        public static Vector2 CoordinateRotateDegree(this float2 v, float angle)
        {
            return CoordinateRotateDegree(angle, v.x, v.y);
        }

        public static Vector2 CoordinateRotateRadian(this float2 v, float angle)
        {
            return CoordinateRotateRadian(angle, v.x, v.y);
        }

        /// <summary>
        /// 旋转(单位:度)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RotateDegree(float degree, Vector3 v, Vector3 axis)
        {
            var radian = math.PI * degree / 180.0F;
            return RotateRadian(radian, v, axis);
        }

        /// <summary>
        /// 旋转(单位:弧度)
        /// </summary>
        public static Vector3 RotateRadian(float radian, Vector3 v, Vector3 axis)
        {
            var normal = math.normalizesafe(axis);
            var x = normal.x;
            var y = normal.y;
            var z = normal.z;
            var xy = x * y;
            var xz = x * z;
            var yz = y * z;
            math.sincos(radian, out var sinA, out var cosA);
            var cosAs = 1 - cosA;
            var sinAx = sinA * x;
            var sinAy = sinA * y;
            var sinAz = sinA * z;
            var f1 = new float3(
                math.pow(x, 2) * cosAs + cosA,
                xy * cosAs - sinAz,
                xz * cosAs + sinAy);
            var f2 = new float3(
                xy * cosAs + sinAz,
                math.pow(y, 2) * cosAs + cosA,
                yz * cosAs - sinAx);
            var f3 = new float3(
                xz * cosAs - sinAy,
                yz * cosAs + sinAx,
                math.pow(z, 2) * cosAs + cosA);
            return math.mul(v, new float3x3(f1, f2, f3));
        }

        public static Vector3 RotateDegree(this Vector3 v, float angle, Vector3 axis)
        {
            return RotateDegree(angle, v, axis);
        }

        public static Vector3 RotateRadian(this Vector3 v, float radian, Vector3 axis)
        {
            return RotateRadian(radian, v, axis);
        }

        public static float3 RotateDegree(this float3 v, float angle, Vector3 axis)
        {
            return RotateDegree(angle, v, axis);
        }

        public static float3 RotateRadian(this float3 v, float radian, Vector3 axis)
        {
            return RotateRadian(radian, v, axis);
        }

        private static Vector2 ProjectionMatrix(Vector2 v, Vector2 axis, int factor)
        {
            var real = axis.RotateDegree(90); //公式的n是垂直于投影线,所以这里旋转90度来强制平行（
            var normal = math.normalizesafe(real);
            var x = normal.x;
            var y = normal.y;
            var xy = x * y;
            var factorResult = factor - 1;
            var f1 = new float2(1 + factorResult * math.pow(x, 2), factorResult * xy);
            var f2 = new float2(factorResult * xy, 1 + factorResult * math.pow(y, 2));
            return math.mul(v, new float2x2(f1, f2));
        }

        /// <summary>
        /// 沿axis轴投影
        /// </summary>
        public static Vector2 Projection(Vector2 v, Vector2 axis)
        {
            return ProjectionMatrix(v, axis, 0);
        }

        /// <summary>
        /// 沿axis轴镜像
        /// </summary>
        public static Vector2 Reflection(Vector2 v, Vector2 axis)
        {
            return ProjectionMatrix(v, axis, -1);
        }

        private static Vector3 ProjectionMatrix(Vector3 v, Vector3 axis, int factor)
        {
            var normal = math.normalizesafe(axis);
            var x = normal.x;
            var y = normal.y;
            var z = normal.z;
            var result = factor - 1;
            var xy = x * y;
            var xz = x * z;
            var yz = y * z;
            var f1 = new float3(1 + result * math.pow(x, 2), result * xy, result * xz);
            var f2 = new float3(result * xy, 1 + result * math.pow(y, 2), result * yz);
            var f3 = new float3(result * xz, result * yz, 1 + result * math.pow(z, 2));
            return math.mul(v, new float3x3(f1, f2, f3));
        }

        /// <summary>
        /// 沿垂直于axis轴的平面投影
        /// </summary>
        public static Vector3 Projection(Vector3 v, Vector3 axis)
        {
            return ProjectionMatrix(v, axis, 0);
        }

        /// <summary>
        /// 沿垂直于axis轴的平面镜像
        /// </summary>
        public static Vector3 Reflection(Vector3 v, Vector3 axis)
        {
            return ProjectionMatrix(v, axis, -1);
        }
    }
}