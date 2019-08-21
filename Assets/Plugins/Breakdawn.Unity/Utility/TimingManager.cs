using System;
using Breakdawn.Core;

namespace Breakdawn.Unity
{
    public class TimingManager : MonoSingleton<TimingManager>
    {
        private readonly Timing _timing = new Timing();

        private void Awake()
        {
            InitInstance();
        }

        private void Update()
        {
            _timing.OnUpdate();
        }

        private void OnDestroy()
        {
            DisposeInstance();
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
        public bool ReplaceTask(Guid taskId, TimeSpan delay, Action task, int loopCount = 1)
        {
            return _timing.ReplaceTask(taskId, delay, task, loopCount);
        }
    }
}