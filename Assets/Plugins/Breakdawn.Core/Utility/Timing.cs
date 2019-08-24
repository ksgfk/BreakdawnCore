using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Breakdawn.Core
{
    internal struct TimingTask
    {
        internal readonly Guid id;
        internal readonly Action task;
        internal readonly TimeSpan delay;
        internal DateTime executeTime;
        internal int loopCount;

        internal TimingTask(Guid id, Action task, TimeSpan delay, int loopCount)
        {
            this.id = id;
            this.task = task;
            this.delay = delay;
            executeTime = Timing.GetStandardTime() + delay;
            this.loopCount = loopCount;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }

    public class Timing
    {
        private readonly List<TimingTask> _tasks = new List<TimingTask>();
        private readonly Queue<TimingTask> _cache = new Queue<TimingTask>();
        private readonly Dictionary<Guid, int> _offset = new Dictionary<Guid, int>();
        private static readonly object Locker = new object();

        public void OnUpdate()
        {
            while (_cache.Count > 0)
            {
                var t = _cache.Dequeue();
                _tasks.Add(t);
                var local = _tasks.Count;
                _offset.Add(t.id, --local);
            }

            for (var a = 0; a < _tasks.Count; a++)
            {
                var task = _tasks[a];
                if (GetStandardTime() < task.executeTime)
                {
                    continue;
                }

                if (task.loopCount > 0)
                {
                    task.task?.Invoke();
                    task.loopCount -= 1;
                    _tasks[a] = RefreshTaskTime(ref task);
                }
                else if (task.loopCount == 0)
                {
                    _tasks.RemoveAt(a);
                    a--;
                }
                else
                {
                    task.task?.Invoke();
                    _tasks[a] = RefreshTaskTime(ref task);
                }
            }
        }

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="delay">延迟多少时间后执行</param>
        /// <param name="task">任务</param>
        /// <param name="loopCount">循环执行次数，若值小于0则一直执行</param>
        /// <returns>该任务的ID</returns>
        public Guid? AddTask(TimeSpan delay, Action task, int loopCount = 1)
        {
            var id = GetId();
            if (_offset.ContainsKey(id))
            {
                return default;
            }

            _cache.Enqueue(new TimingTask(id, task, delay, loopCount));
            return id;
        }

        /// <summary>
        /// 释放内部资源
        /// </summary>
        public void Release()
        {
            _cache.TrimExcess();
            _tasks.TrimExcess();
        }

        private static Guid GetId()
        {
            lock (Locker)
            {
                return Guid.NewGuid();
            }
        }

        /// <summary>
        /// 移除一个任务
        /// </summary>
        /// <param name="taskId">任务的GUID</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveTask(Guid taskId)
        {
            if (!_offset.TryGetValue(taskId, out var index))
            {
                return false;
            }

            _tasks.RemoveAt(index);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref TimingTask RefreshTaskTime(ref TimingTask task)
        {
            task.executeTime = GetStandardTime() + task.delay;
            return ref task;
        }

        /// <summary>
        /// 替换任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="delay">替换后的延时执行时间</param>
        /// <param name="task">任务</param>
        /// <param name="loopCount">循环执行次数，若值小于0则一直执行</param>
        /// <returns>是否替换成功</returns>
        public bool ReplaceTask(Guid taskId, TimeSpan delay, Action task, int loopCount = 1)
        {
            if (!_offset.TryGetValue(taskId, out var index))
            {
                return false;
            }

            _tasks[index] = new TimingTask(taskId, task, delay, loopCount);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime GetStandardTime()
        {
            return DateTime.UtcNow;
        }
    }
}