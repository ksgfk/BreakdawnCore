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
            a = VectorExtension.RotateDegree(90, a, Vector3.forward);
            Debug.Log($"({a.x},{a.y},{a.z})");
        }
    }
}