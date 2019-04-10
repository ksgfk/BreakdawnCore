namespace Breakdawn.Expansion
{
	public class CoroutineResult<T>
	{
		private T[] resultList;

		public T Result
		{
			get => resultList[0];
			set => resultList[0] = value;
		}

		public CoroutineResult()
		{
			resultList = new T[1];
		}

		public bool TryGetResult(out T result)
		{
			result = resultList[0];
			return result != null;
		}
	}
}
