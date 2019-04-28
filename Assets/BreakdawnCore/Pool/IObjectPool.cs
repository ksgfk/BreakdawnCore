namespace Breakdawn.Core
{
	public interface IObjectPool<T>
	{
		T Get();

		bool Recycling(T @object);
	}
}
