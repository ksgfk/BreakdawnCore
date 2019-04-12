using Breakdawn.Event;
using Breakdawn.Expansion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Test
{
	public class MonoEvent : MonoMessage<Action<object>>
	{
		private void Start()
		{
			Register("Hello", Hello);
			StringPool.MonoMessageToString = ToString();
			this.InvokeCoroutine(() => { Destroy(gameObject); }, 5F);
		}

		private void Hello(object a)
		{
			Debug.Log($"跨脚本通信:{a}");
		}

		public override void OnBeforeDestroy()
		{

		}
	}
}
