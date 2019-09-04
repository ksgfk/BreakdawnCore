using System;
using System.Collections.Concurrent;
using Breakdawn.Core;
using UnityEngine;

namespace Breakdawn.Unity
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Breakdawn/TimingManager")]
    public class TimingManager : MonoSingleton<TimingManager>
    {
        private readonly Timing _timing = new Timing();
        private readonly ConcurrentQueue<(Action<Guid>, Guid)> _queue = new ConcurrentQueue<(Action<Guid>, Guid)>();

        private void Awake()
        {
            InitInstance();
            _timing.TaskHandler += (act, id) => _queue.Enqueue((act, id));
            _timing.Start();
        }

        private void Update()
        {
            var a = 0;
            while (_queue.Count != a)
            {
                var r = _queue.TryDequeue(out var t);
                if (!r)
                {
                    a++;
                    continue;
                }

                t.Item1(t.Item2);
            }
        }

        private void OnDestroy()
        {
            DisposeInstance();
            _timing.Close();
        }

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="delay">延迟多少时间后执行</param>
        /// <param name="task">任务</param>
        /// <param name="loopCount">循环执行次数，若值小于0则一直执行</param>
        /// <returns>该任务的ID</returns>
        public Guid? AddTask(TimeSpan delay, Action<Guid> task, int loopCount)
        {
            return _timing.AddTask(delay, task, loopCount);
        }

        /// <summary>
        /// 释放内部资源
        /// </summary>
        public void Release()
        {
            _timing.Release();
        }

        /// <summary>
        /// 移除一个任务
        /// </summary>
        /// <param name="taskId">任务的GUID</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveTask(Guid taskId)
        {
            return _timing.RemoveTask(taskId);
        }

        /// <summary>
        /// 替换任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="delay">替换后的延时执行时间</param>
        /// <param name="task">任务</param>
        /// <param name="loopCount">循环执行次数，若值小于0则一直执行</param>
        /// <returns>是否替换成功</returns>
        public bool ReplaceTask(Guid taskId, TimeSpan delay, Action<Guid> task, int loopCount)
        {
            return _timing.ReplaceTask(taskId, delay, task, loopCount);
        }
    }
}