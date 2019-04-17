namespace Breakdawn.Core
{
	public class NormalFactory<T> : IFactory<T> where T : new()
	{
		public T Create()
		{
			return new T();
		}
	}
}
