namespace Breakdawn.Core
{
	[System.Obsolete("没啥用的")]
	public class NormalFactory<T> : IFactory<T> where T : new()
	{
		public T Create()
		{
			return new T();
		}
	}
}
