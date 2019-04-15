using System;
using UnityEngine;

namespace Breakdawn.Manager
{
	/// <summary>
	/// 管理游戏所有入口和启动流程
	/// </summary>
	public abstract class TemplateMainManager : MonoBehaviour
	{
		protected Environment environment = Environment.Develop;
		public Environment Environment { get => environment; }

		private static Environment mSharedENV;
		private static bool mIsENVSetted = false;

		private void Awake()
		{
			if (!mIsENVSetted)
			{
				mSharedENV = environment;
				mIsENVSetted = true;
			}

			switch (mSharedENV)
			{
				case Environment.Develop:
					LunchDevelop();
					break;
				case Environment.Production:
					LunchProduction();
					break;
				case Environment.Test:
					LunchTest();
					break;
				default:
					throw new Exception($"初始化错误:How can it be!{environment}");
			}
		}

		protected abstract void LunchDevelop();

		protected abstract void LunchProduction();

		protected abstract void LunchTest();
	}
}
