using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

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

		public static bool Percent(float percent, Precision precision)//闲的蛋疼
		{
			if (percent < 0 || percent > 100)
				Debug.LogWarning($"概率:{percent}永远返回true或false");
			var p = SwitchMode(precision);
			var ran = Random.Range(0F, 100F * p);
			return ran < percent * p;
		}

		public static bool Percent(float percent, out float ran, Precision precision)
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
		public static IList<T> GetRandomElements<T>(IList<T> list, int count)
		{
			if (count >= list.Count)
			{
				Debug.LogWarning($"想随机的数量({count})比数组内含有的个数({list.Count})多");
				return list;
			}
			var arr = new T[count];
			var selected = GetRandomNumbers(0, list.Count, count);
			for (int i = 0; i < selected.Count; i++)
			{
				arr[i] = list[selected[i]];
			}
			return arr;
		}
		/// <summary>
		/// 获取字典中随机元素
		/// </summary>
		/// <param name="count">数量</param>
		/// <returns>随机的元素数组</returns>
		public static IList<V> GetRandomElements<K, V>(IDictionary<K, V> dict, int count)
		{
			//Stopwatch sw = new Stopwatch();
			//sw.Start();

			var par = from v in dict select v.Value;
			if (count >= dict.Count)
			{
				Debug.LogWarning($"想随机的数量({count})比数组内含有的个数({dict.Count})多");
				return par.ToArray();
			}
			var val = par.ToArray();

			//sw.Stop();
			//Debug.Log(sw.ElapsedMilliseconds);

			return GetRandomElements(val, count);
		}

		[Obsolete("这个性能超差")]
		public static IDictionary<K, V> GetRandomElements<K, V>(IDictionary<K, V> dict, int count,int a = 0)//好像写了一堆辣鸡
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			
			var selected = GetRandomNumbers(0, dict.Count, count);
			var r = new Dictionary<K, V>();
			var par = dict.Select((e) => e.Key).ToArray();
			for (int i = 0; i < count; i++)
			{
				r.Add(par[selected[i]], dict[par[selected[i]]]);
			}
			
			sw.Stop();
			Debug.LogWarning($"时间花费:{sw.ElapsedMilliseconds}");

			return r;
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
			//Stopwatch sw = new Stopwatch();
			//sw.Start();
			int t;
			var res = new List<int>();
			if (count > (max - min) / 2)
			{
				var del = new List<int>(count);
				t = max - min - count;
				for (int i = min; i < max; i++)
					res.Add(i);
				for (int i = 0; i < t; i++)
				{
					var a = Random.Range(min, max);
					if (i == 0)
						del.Add(a);
					else
					{
						if (del.Contains(a))
							i -= 1;
						else
							del.Add(a);
					}
				}
				foreach (var item in del)
					res.Remove(item);
			}
			else
			{
				t = count;
				for (int i = 0; i < t; i++)
				{
					var a = Random.Range(min, max);
					if (i == 0)
						res.Add(a);
					else
					{
						if (res.Contains(a))
							i -= 1;
						else
							res.Add(a);
					}
				}
			}
			//sw.Stop();
			//Debug.Log(sw.ElapsedMilliseconds);
			return res;
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
