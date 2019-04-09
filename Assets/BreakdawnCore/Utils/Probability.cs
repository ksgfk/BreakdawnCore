using System;
using System.Collections.Generic;
using System.Linq;
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
				Debug.LogWarning($"想随机的数量{count}比数组内含有的个数{list.Count}多");
				return list;
			}
			if (count == 1)
				return new T[1] { list[Random.Range(0, list.Count)] };
			if (count <= 0)
				throw new ArgumentException($"count不能小于等于0，count值:{count}", "count");

			var result = new List<T>(list.Count);
			foreach (var item in list)
				result.Add(item);
			if (count >= list.Count / 2)
			{
				for (int a = 0; a < list.Count - count; a++)
				{
					var willRemove = Random.Range(0, result.Count);
					result.RemoveAt(willRemove);
				}
			}
			else
			{
				var temp = new List<T>(count);
				for (int a = 0; a < count; a++)
				{
					var willAdd = Random.Range(0, result.Count);
					temp.Add(result[willAdd]);
					result.RemoveAt(willAdd);
				}
				result = temp;
				//GC.Collect();//我好像用了很多内存
			}
			return result;
		}
		/// <summary>
		/// 获取字典中随机元素
		/// </summary>
		/// <param name="count">数量</param>
		/// <returns>随机的元素Keys</returns>
		public static IList<K> GetRandomElements<K, V>(IDictionary<K, V> dict, int count)
		{
			if (count >= dict.Count)
			{
				Debug.LogWarning($"想随机的数量({count})比数组内含有的个数({dict.Count})多");
				return dict.Keys.ToArray();
			}
			if (count == 1)
				return new K[1] { dict.ElementAt(Random.Range(0, dict.Count)).Key };
			if (count <= 0)
				throw new ArgumentException($"count不能小于等于0，count值:{count}", "count");

			var keys = dict.Keys.ToArray();
			var selectedKey = GetRandomElements(keys, count);
			return selectedKey;
		}
		/// <summary>
		/// 获取随机数字
		/// </summary>
		/// <param name="min">数字最小值(能取到最小值)</param>
		/// <param name="max">数字最大值(能取到最大值)</param>
		/// <param name="count">数量</param>
		/// <returns>随机的数字数组</returns>
		public static IList<int> GetRandomNumbers(int min, int max, int count)
		{
			var all = max - min;

			if (count == 1)
				return new int[1] { Random.Range(min, max + 1) };
			if (all <= 0)
				throw new ArgumentException($"count不能小于等于0，count值:{count}", "count");

			var result = new List<int>(all);
			for (int a = min; a < max; a++)
				result.Add(a);

			if (count >= all)
			{
				Debug.LogWarning($"想随机的数量{count}比最小值{min}到最大值{max}的整数数量多{all}");
				return result;
			}

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
			var result = new Dictionary<float, object>(count);
			for (int i = 0; i < count; i++)
			{
				var a = Random.Range(min, max);
				if (i == 0)
					result.Add(a, null);
				else
				{
					if (result.ContainsKey(a))
						i -= 1;
					else
						result.Add(a, null);
				}
			}
			return result.Keys.ToArray();
		}
	}
}
