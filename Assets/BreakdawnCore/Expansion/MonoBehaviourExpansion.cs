using System;
using System.Collections;
using UnityEngine;

namespace Breakdawn.Core
{
	public static class MonoBehaviourExpansion
	{
		/// <summary>
		/// 开启可延迟执行委托的协程
		/// </summary>
		/// <param name="behaviour">继承MonoBehaviour的类</param>
		/// <param name="action">委托</param>
		/// <param name="seconds">延迟时间</param>
		/// <returns>协程</returns>
		public static Coroutine InvokeCoroutine(this MonoBehaviour behaviour, Action action, float seconds)
		{
			return behaviour.StartCoroutine(DelayInvoke(action, seconds));
		}
		/// <summary>
		/// 开启可延迟执行委托的协程
		/// </summary>
		/// <param name="behaviour">继承MonoBehaviour的类</param>
		/// <param name="action">委托</param>
		/// <param name="param">要执行方法的参数</param>
		/// <param name="seconds">延迟时间</param>
		/// <returns>协程</returns>
		public static Coroutine InvokeCoroutine<T>(this MonoBehaviour behaviour, Action<T> action, T param, float seconds)
		{
			return behaviour.StartCoroutine(DelayInvoke(action, param, seconds));
		}
		/// <summary>
		/// 开启可延迟执行委托的协程
		/// </summary>
		/// <param name="behaviour">继承MonoBehaviour的类</param>
		/// <param name="action">委托</param>
		/// <param name="result">要执行方法的返回值</param>
		/// <param name="seconds">延迟时间</param>
		/// <returns>协程</returns>
		public static Coroutine InvokeCoroutine<T>(this MonoBehaviour behaviour, Func<T> action, CoroutineResult<T> result, float seconds)
		{
			return behaviour.StartCoroutine(DelayInvoke(action, result, seconds));
		}
		/// <summary>
		/// 开启可延迟执行委托的协程
		/// </summary>
		/// <param name="behaviour">继承MonoBehaviour的类</param>
		/// <param name="action">委托</param>
		/// <param name="parma">要执行方法的参数</param>
		/// <param name="result">要执行方法的返回值</param>
		/// <param name="seconds">延迟时间</param>
		/// <returns>协程</returns>
		public static Coroutine InvokeCoroutine<T, TA>(this MonoBehaviour behaviour, Func<TA, T> action, TA parma, CoroutineResult<T> result, float seconds)
		{
			return behaviour.StartCoroutine(DelayInvoke(action, parma, result, seconds));
		}

		private static IEnumerator DelayInvoke(Action action, float seconds)
		{
			yield return new WaitForSeconds(seconds);
			action();
		}

		private static IEnumerator DelayInvoke<T>(Action<T> action, T param, float seconds)
		{
			yield return new WaitForSeconds(seconds);
			action(param);
		}

		private static IEnumerator DelayInvoke<T>(Func<T> action, CoroutineResult<T> result, float seconds)
		{
			yield return new WaitForSeconds(seconds);
			result.Result = action();
		}

		private static IEnumerator DelayInvoke<T, TA>(Func<TA, T> action, TA parma, CoroutineResult<T> result, float seconds)
		{
			yield return new WaitForSeconds(seconds);
			result.Result = action(parma);
		}
	}
}
