using System;
using System.Collections;
using UnityEngine;

namespace Breakdawn.Expansion
{
	public static class MonoBehaviourExpansion
	{
		public static void InvokeCoroutine(this MonoBehaviour behaviour, Action action, float seconds)
		{
			behaviour.StartCoroutine(DelayInvoke(action, seconds));
		}

		public static void InvokeCoroutine<T>(this MonoBehaviour behaviour, Action<T> action, T param, float seconds)
		{
			behaviour.StartCoroutine(DelayInvoke(action, param, seconds));
		}

		public static void InvokeCoroutine<T>(this MonoBehaviour behaviour, Func<T> action, CoroutineResult<T> result, float seconds)
		{
			behaviour.StartCoroutine(DelayInvoke(action, result, seconds));
		}

		public static void InvokeCoroutine<T, TA>(this MonoBehaviour behaviour, Func<TA, T> action, TA parma, CoroutineResult<T> result, float seconds)
		{
			behaviour.StartCoroutine(DelayInvoke(action, parma, result, seconds));
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
