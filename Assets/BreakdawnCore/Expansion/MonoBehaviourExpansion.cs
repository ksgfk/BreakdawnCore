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

		private static IEnumerator DelayInvoke(Action action, float seconds)
		{
			yield return new WaitForSeconds(seconds);
			action();
		}

		private static IEnumerator DelayInvoke(Delegate action, float seconds)
		{
			yield return new WaitForSeconds(seconds);
			//emm
		}
	}
}
