namespace Breakdawn.Expansion
{
	/// <summary>
	/// 协程执行需要返回值的方法时实例化
	/// </summary>
	/// <typeparam name="T">返回值的类型</typeparam>
	public class CoroutineResult<T>
	{
		private T[] resultList;
		/// <summary>
		/// 最好不要给Result赋值
		/// </summary>
		public T Result
		{
			get => resultList[0];
			set => resultList[0] = value;
		}

		public CoroutineResult()
		{
			resultList = new T[1];
		}
		/// <summary>
		/// 尝试获取返回值
		/// </summary>
		/// <param name="result">返回值</param>
		/// <returns>当result是null时,返回false,否则返回true</returns>
		public bool TryGetResult(out T result)
		{
			result = resultList[0];
			return result != null;
		}
	}
}
