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
            a = a.RotateDegree(45);
            Debug.Log($"({a.x},{a.y})");

            a = a.CoordinateRotateDegree(45);
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
    }
}