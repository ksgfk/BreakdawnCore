using System;
using System.Collections.Generic;

namespace Breakdawn.Unity
{
    internal struct TimingTask
    {
        internal readonly Action task;
        internal TimeSpan delay;
        internal DateTime executeTime;
        internal readonly int loopCount;

        internal TimingTask(Action task, TimeSpan delay, int loopCount)
        {
            this.task = task;
            this.delay = delay;
            executeTime = DateTime.Now + delay;
            this.loopCount = loopCount;
        }
    }

    public class TimingManager : MonoSingleton<TimingManager>
    {
        private readonly List<TimingTask> _tasks = new List<TimingTask>();
        private readonly Queue<TimingTask> _cache = new Queue<TimingTask>();

        private void Awake()
        {
            InitInstance();
        }

        private void Update()
        {
            while (_cache.Count > 0)
            {
                _tasks.Add(_cache.Dequeue());
            }

            for (var a = 0; a < _tasks.Count; a++)
            {
                var task = _tasks[a];
                if (DateTime.Now < task.executeTime)
                {
                    continue;
                }

                if (task.loopCount > 0)
                {
                    task.task?.Invoke();
                    var count = task.loopCount - 1;
                    _tasks[a] = new TimingTask(task.task, task.delay, count);
                }
                else if (task.loopCount == 0)
                {
                    _tasks.RemoveAt(a);
                }
                else
                {
                    task.task?.Invoke();
                }

                a--;
            }
        }

        private void OnDestroy()
        {
            DisposeInstance();
        }

        public void AddTask(TimeSpan delay, Action task, int loopCount = 1)
        {
            _cache.Enqueue(new TimingTask(task, delay, loopCount));
        }

        public void Release()
        {
            _cache.TrimExcess();
            _tasks.TrimExcess();
        }
    }
}