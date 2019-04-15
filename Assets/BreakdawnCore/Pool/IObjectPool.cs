namespace Breakdawn.Pool
{
	public interface IObjectPool<T>
	{
		T Get();

		void Recycling(T @object);
	}
}
