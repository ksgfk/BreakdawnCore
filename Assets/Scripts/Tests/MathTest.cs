using Breakdawn.Unity;
using NUnit.Framework;
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
    }
}