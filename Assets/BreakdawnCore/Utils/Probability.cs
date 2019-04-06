using UnityEngine;

namespace Breakdawn.Utils
{
	public static class Probability
	{
		/// <summary>
		/// 计算百分制概率
		/// </summary>
		/// <param name="percent">概率，应该是个0-100间的浮点数</param>
		/// <returns>是否中奖</returns>
		public static bool Percent(float percent)
		{
			if (percent < 0 || percent > 100)
				Debug.LogWarning($"概率:{percent}永远返回true或false");
			return Random.Range(0F, 100F) < percent;
		}
		/// <summary>
		/// 计算百分制概率
		/// </summary>
		/// <param name="percent">概率，应该是个0-100间的浮点数</param>
		/// <param name="ran">返回随机出来的数</param>
		/// <returns>是否中奖</returns>
		public static bool Percent(float percent, out float ran)
		{
			if (percent < 0 || percent > 100)
				Debug.LogWarning($"概率:{percent}永远返回true或false");
			ran = Random.Range(0F, 100F);
			return ran < percent;
		}

		public static bool Percent(Precision precision, float percent)//闲的蛋疼
		{
			if (percent < 0 || percent > 100)
				Debug.LogWarning($"概率:{percent}永远返回true或false");
			var p = SwitchMode(precision);
			var ran = Random.Range(0F, 100F * p);
			return ran < percent * p;
		}

		public static bool Percent(Precision precision, float percent, out float ran)
		{
			if (percent < 0 || percent > 100)
				Debug.LogWarning($"概率:{percent}永远返回true或false");
			var p = SwitchMode(precision);
			ran = Random.Range(0F, 100F * p);
			return ran < percent * p;
		}

		private static int SwitchMode(Precision precision)
		{
			int p;
			switch (precision)
			{
				case Precision.Super:
					p = 1000000;
					break;
				case Precision.Lighting:
					p = 100000;
					break;
				case Precision.VeryHigh:
					p = 10000;
					break;
				case Precision.High:
					p = 1000;
					break;
				case Precision.Medium:
					p = 100;
					break;
				case Precision.Low:
					p = 10;
					break;
				case Precision.VeryLow:
					p = 1;
					break;
				default:
					throw new System.Exception("到底是什么黑魔法");
			}
			return p;
		}
	}

	public enum Precision
	{
		/// <summary>
		/// 最高
		/// </summary>
		Super,
		/// <summary>
		/// 最高-1
		/// </summary>
		Lighting,
		VeryHigh,
		High,
		Medium,
		Low,
		VeryLow
	}
}
