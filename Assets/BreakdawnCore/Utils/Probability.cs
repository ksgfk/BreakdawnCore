using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Breakdawn.Utils
{
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
		/// <summary>
		/// 获取数组中随机元素
		/// </summary>
		/// <param name="count">数量</param>
		/// <returns>随机的元素数组</returns>
		public static IList<T> GetRandomElements<T>(IList<T> t, int count)
		{
			if (count >= t.Count)
			{
				Debug.LogWarning($"想随机的数量({count})比数组内含有的个数({t.Count})多");
				return t;
			}
			var arr = new T[count];
			var selected = GetRandomNumbers(0, t.Count, count);
			for (int i = 0; i < selected.Count; i++)
			{
				arr[i] = t[selected[i]];
			}
			return arr;
		}
		/// <summary>
		/// 获取字典中随机元素
		/// </summary>
		/// <param name="count">数量</param>
		/// <returns>随机的元素数组</returns>
		public static IList<V> GetRandomElements<K, V>(IDictionary<K, V> dict, int count)//好像写了一堆辣鸡
		{
			if (count >= dict.Count)
			{
				Debug.LogWarning($"想随机的数量({count})比数组内含有的个数({dict.Count})多");
				var par = from v in dict select v.Value;
				return par.ToArray();
			}
			var vs = from v in dict select v.Value;
			var val = vs.ToArray();
			var arr = new V[count];
			var selected = GetRandomNumbers(0, dict.Count, count);
			for (int a = 0; a < count; a++)
			{
				arr[a] = val[selected[a]];
			}
			return arr.ToArray();
		}
		/// <summary>
		/// 获取随机数字
		/// </summary>
		/// <param name="min">数字最小值</param>
		/// <param name="max">数字最大值</param>
		/// <param name="count">数量</param>
		/// <returns>随机的数字数组</returns>
		public static IList<int> GetRandomNumbers(int min, int max, int count)
		{
			var result = new List<int>();
			for (int i = 0; i < count; i++)
			{
				var a = Random.Range(min, max);
				if (i == 0)
					result.Add(a);
				else
				{
					if (result.Contains(a))
						i -= 1;
					else
						result.Add(a);
				}
			}
			return result;
		}
		/// <summary>
		/// 获取随机数字
		/// </summary>
		/// <param name="min">数字最小值</param>
		/// <param name="max">数字最大值</param>
		/// <param name="count">数量</param>
		/// <returns>随机的数字数组</returns>
		public static IList<float> GetRandomNumbers(float min, float max, int count)
		{
			var result = new List<float>();
			for (int i = 0; i < count; i++)
			{
				var a = Random.Range(min, max);
				if (i == 0)
					result.Add(a);
				else
				{
					if (result.Contains(a))
						i -= 1;
					else
						result.Add(a);
				}
			}
			return result;
		}
	}
}
