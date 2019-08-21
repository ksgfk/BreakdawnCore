using System;
using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class TimingTest : MonoBehaviour
    {
        private Guid taskId;

        private void Start()
        {
            taskId = TimingManager.Instance.AddTask(TimeSpan.FromSeconds(2), () => Debug.Log("hello"), -1);
            taskId = TimingManager.Instance.AddTask(TimeSpan.FromSeconds(3), () => Debug.Log("emm"), 3);
        }

        public void ClickDelButton()
        {
            TimingManager.Instance.RemoveTask(taskId);
        }
    }
}