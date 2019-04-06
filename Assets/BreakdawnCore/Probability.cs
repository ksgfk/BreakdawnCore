using UnityEngine;

namespace Breakdawn
{
	public static class Probability
	{
		public static bool Percent(float percent)
		{
			return Random.Range(0F, 100F) < percent;
		}

		public static bool Percent(float percent, out float ran)
		{
			ran = Random.Range(0F, 100F);
			return ran < percent;
		}

		public static bool Percent(Precision precision, float percent)//闲的蛋疼
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
					throw new System.Exception("这怎么可能...");
			}
			var ran = UnityEngine.Random.Range(0F, 100F * p);
			return ran < percent * p;
		}

		public static bool Percent(Precision precision, float percent, out float ran)
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
			ran = UnityEngine.Random.Range(0F, 100F * p);
			return ran < percent * p;
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
