using Breakdawn.Unity;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

namespace Breakdawn.Tests
{
    public class MathTest
    {
        [Test]
        public void TestRotate()
        {
            var a = new Vector2(1, 0);
            a = a.RotateDegree(90);
            Debug.Log($"({a.x},{a.y})");

            a = a.RotateDegree(-90);
            Debug.Log($"({a.x},{a.y})");

            a = a.RotateRadian(math.PI / 2);
            Debug.Log($"({a.x},{a.y})");

            a = a.CoordinateRotateDegree(90);
            Debug.Log($"({a.x},{a.y})");
        }

        [Test]
        public void TestMatrix()
        {
            var a = new float3x2(1, 2, 1, 2, 2, 1);
            var b = new float2x3(2, 2, 1, 3, 1, 2);
            var c = math.mul(a, b);
            Debug.Log($"{c}");
        }

        [Test]
        public void TestRotateV3()
        {
            var a = new Vector3(1, 0, 0);
            a = a.RotateDegree(90, new Vector3(0, 1, 0));
            Debug.Log($"({a.x},{a.y},{a.z})");
        }

        [Test]
        public void TestProjection()
        {
            var a = new Vector2(0.5F, 1);
            a = VectorExtension.Projection(a, new Vector2(1, 1));
            Debug.Log($"({a.x},{a.y})");

            var b = new Vector2(0.5F, 1);
            b = VectorExtension.Reflection(b, new Vector2(1, -1));
            Debug.Log($"({b.x},{b.y})");

            var c = new float3(0, 0, 1);
            Debug.Log(VectorExtension.Projection(c, new float3(1, 1, 1)));
        }
    }
}