using System;
using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class TimingTest : MonoBehaviour
    {
        private void Start()
        {
            TimingManager.Instance.AddTask(TimeSpan.FromSeconds(2), () => Debug.Log("hello"), 4);
        }
    }
}