using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Breakdawn.Unity
{
    public static class VectorExtension
    {
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
            return math.mul(new float2(x, y), new float2x2(cosA, sinA, -sinA, cosA));
        }

        public static Vector2 RotateDegree(this Vector2 v, float degrees)
        {
            return RotateDegree(degrees, v.x, v.y);
        }

        public static Vector2 RotateRadian(this Vector2 v, float angle)
        {
            return RotateRadian(angle, v.x, v.y);
        }

        public static float2 RotateDegree(this float2 v, float degrees)
        {
            return RotateDegree(degrees, v.x, v.y);
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RotateDegree(float degree, Vector3 v, Vector3 axis)
        {
            var radian = math.PI * degree / 180.0F;
            return RotateRadian(radian, v, axis);
        }

        public static Vector3 RotateRadian(float radian, Vector3 v, Vector3 axis)
        {
            var x = axis.x;
            var y = axis.y;
            var z = axis.z;
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
    }
}