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
		Super = 1000000,
		/// <summary>
		/// 最高-1
		/// </summary>
		Lighting = 100000,
		VeryHigh = 10000,
		High = 1000,
		Medium = 100,
		Low = 10,
		VeryLow = 1
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
			var p = (float)precision;
			var ran = Random.Range(0F, 100F * p);
			return ran < percent * p;
		}

		public static bool Percent(float percent, out float ran, Precision precision)
		{
			if (percent < 0 || percent > 100)
				Debug.LogWarning($"概率:{percent}永远返回true或false");
			var p = (float)precision;
			ran = Random.Range(0F, 100F * p);
			return ran < percent * p;
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
			var result = new List<T>(list.Count);
			foreach (var item in list)
			{
				result.Add(item);
			}
			for (int i = 0; i < count; i++)
			{
				var ran = Random.Range(0, result.Count());
				result.RemoveAt(ran);
			}
			return result;
		}
		/// <summary>
		/// 获取字典中随机元素
		/// </summary>
		/// <param name="count">数量</param>
		/// <returns>随机的元素数组</returns>
		public static IList<V> GetRandomElements<K, V>(IDictionary<K, V> dict, int count)
		{
			var par = from v in dict select v.Value;
			if (count >= dict.Count)
			{
				Debug.LogWarning($"想随机的数量({count})比数组内含有的个数({dict.Count})多");
				return par.ToArray();
			}
			var val = par.ToArray();

			return GetRandomElements(val, count);
		}

		[Obsolete("这个性能超差")]
		public static IDictionary<K, V> GetRandomElements<K, V>(IDictionary<K, V> dict, int count, int a = 0)//好像写了一堆辣鸡
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
			var all = max - min;
			var result = new List<int>(all);
			for (int a = min; a < max; a++)
				result.Add(a);
			var remain = all - count;
			if (count >= all / 2)
			{
				for (int a = 0; a < remain; a++)
					result.RemoveAt(Random.Range(0, result.Count));
				return result;
			}
			else
			{
				var rr = new int[count];
				for (int i = 0; i < count; i++)
				{
					int del = Random.Range(0, result.Count);
					rr[i] = result[del];
					result.RemoveAt(del);
				}
				return rr;
			}
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
