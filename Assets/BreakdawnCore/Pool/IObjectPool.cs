namespace Breakdawn.Pool
{
	public interface IObjectPool<T>
	{
		void Init(int count);

		T Get();

		void Recycling(T @object);
	}
}
