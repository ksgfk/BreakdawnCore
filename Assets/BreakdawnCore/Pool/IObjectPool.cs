namespace Breakdawn.Core
{
	public interface IObjectPool<T>
	{
		T Get();

		void Recycling(T @object);
	}
}
