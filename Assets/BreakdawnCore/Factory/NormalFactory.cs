namespace Breakdawn.Factory
{
	public class NormalFactory<T> : IFactory<T> where T : new()
	{
		public T Create()
		{
			return new T();
		}
	}
}
