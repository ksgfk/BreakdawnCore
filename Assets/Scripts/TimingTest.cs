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
            var a = TimingManager.Instance.AddTask(TimeSpan.FromSeconds(2), () => Debug.Log("hello"), -1);
            if (a.HasValue)
            {
                taskId = a.Value;
            }
            TimingManager.Instance.AddTask(TimeSpan.FromSeconds(3), () => Debug.Log("emm"), 3);
        }

        public void ClickDelButton()
        {
            TimingManager.Instance.ReplaceTask(taskId, 
                TimeSpan.FromSeconds(4), 
                () => Debug.Log("ahh"), 
                3);
        }
    }
}