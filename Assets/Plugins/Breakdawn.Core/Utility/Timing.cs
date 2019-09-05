using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Timer = System.Timers.Timer;

namespace Breakdawn.Core
{
    internal struct TimingTask : IEquatable<TimingTask>
    {
        internal readonly Guid id;
        internal readonly Action<Guid> task;
        internal readonly TimeSpan delay;
        internal DateTime executeTime;
        internal int loopCount;

        internal TimingTask(Guid id, Action<Guid> task, TimeSpan delay, int loopCount)
        {
            this.id = id;
            this.task = task;
            this.delay = delay;
            executeTime = Timing.GetStandardTime() + delay;
            this.loopCount = loopCount;
        }

        public bool Equals(TimingTask other)
        {
            return id == other.id;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return obj is TimingTask t && Equals(t);
        }
    }

    public class Timing
    {
        private readonly List<TimingTask> _tasks = new List<TimingTask>();
        private readonly ConcurrentQueue<TimingTask> _cache = new ConcurrentQueue<TimingTask>();
        private readonly ConcurrentDictionary<Guid, int> _offset = new ConcurrentDictionary<Guid, int>();
        private readonly Timer _timer;
        private bool _isClose;

        /// <summary>
        /// 如果订阅该事件，则会在订阅事件时的线程处理任务
        /// </summary>
        public event Action<Action<Guid>, Guid> TaskHandler;

        private static readonly object Locker2 = new object();
        private static readonly object Locker1 = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">事件间间隔时间(单位:毫秒)</param>
        public Timing(double interval = 50)
        {
            if (interval < 0)
            {
                return;
            }

            _timer = new Timer(interval);
            _timer.Elapsed += (o, args) => OnUpdate();
            _isClose = false;
        }

        ~Timing()
        {
            if (!_isClose)
            {
                _timer.Close();
            }
        }

        /// <summary>
        /// 开始异步计时并执行任务。
        /// </summary>
        public Timing Start()
        {
            _timer.Start();
            return this;
        }

        /// <summary>
        /// 停止异步执行任务
        /// </summary>
        public Timing Stop()
        {
            _timer.Stop();
            return this;
        }

        /// <summary>
        /// 释放线程资源
        /// </summary>
        public Timing Close()
        {
            _timer.Close();
            _isClose = true;
            return this;
        }

        private void OnUpdate()
        {
            while (_cache.Count > 0)
            {
                var result = _cache.TryDequeue(out var t);
                if (!result)
                {
                    continue;
                }

                _tasks.Add(t);
                var local = _tasks.Count;
                _offset.TryAdd(t.id, --local);
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
                    DOOnTaskHandler(task.task, task.id);
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
                    task.task?.Invoke(task.id);
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
        public Guid? AddTask(TimeSpan delay, Action<Guid> task, int loopCount = 1)
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
            _tasks.TrimExcess();
        }

        private static Guid GetId()
        {
            lock (Locker2)
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

            var tryRemove = _offset.TryRemove(taskId, out _);
            if (!tryRemove)
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
        public bool ReplaceTask(Guid taskId, TimeSpan delay, Action<Guid> task, int loopCount = 1)
        {
            lock (Locker1)
            {
                if (!_offset.TryGetValue(taskId, out var index))
                {
                    return false;
                }

                _tasks[index] = new TimingTask(taskId, task, delay, loopCount);
                return true;
            }
        }

        /// <summary>
        /// 停止执行所有任务并清空
        /// </summary>
        /// <returns>是否成功清除</returns>
        public bool ClearAll()
        {
            _timer.Stop();
            _offset.Clear();
            _tasks.Clear();
            while (!_cache.IsEmpty)
            {
                var result = _cache.TryDequeue(out _);
                if (!result)
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime GetStandardTime()
        {
            return DateTime.UtcNow;
        }

        private void DOOnTaskHandler(Action<Guid> act, Guid id)
        {
            if (TaskHandler == null)
            {
                act(id);
            }
            else
            {
                TaskHandler(act, id);
            }
        }
    }
}