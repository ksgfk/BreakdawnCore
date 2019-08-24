using System;
using System.Collections.Concurrent;
using Breakdawn.Core;
using UnityEngine;

namespace Breakdawn.Test
{
    public class TimingTest : MonoBehaviour
    {
        private Guid taskId;
        private Timing _t;
        private ConcurrentQueue<(Action<Guid>, Guid)> act = new ConcurrentQueue<(Action<Guid>, Guid)>();

        private void Start()
        {
//            var a = TimingManager.Instance.AddTask(TimeSpan.FromSeconds(2), (_) => Debug.Log("hello"), -1);
//            if (a.HasValue)
//            {
//                taskId = a.Value;
//            }
//
//            TimingManager.Instance.AddTask(TimeSpan.FromSeconds(3), (_) => Debug.Log("emm"), 3);

            _t = new Timing();
            _t.AddTask(TimeSpan.FromSeconds(1), (_) => Debug.Log("wowow"), -1);
            _t.TaskHandler += (a, t) => act.Enqueue((a, t));
            _t.Start();
        }

        private void Update()
        {
            var a = 0;
            while (act.Count != a)
            {
                var r = act.TryDequeue(out var t);
                if (!r)
                {
                    a++;
                    continue;
                }

                t.Item1(t.Item2);
            }
        }

        public void ClickDelButton()
        {
//            TimingManager.Instance.ReplaceTask(taskId,
//                TimeSpan.FromSeconds(4),
//                (_) => Debug.Log("ahh"),
//                3);
        }

        private void OnDestroy()
        {
            _t.Close();
        }
    }
}