namespace Breakdawn.Core
{
	/// <summary>
	/// 系列对象池
	/// </summary>
	/// <typeparam name="T">对象</typeparam>
	/// <typeparam name="K">获取对象的key</typeparam>
	public interface ISeriesPool<T, K>
	{
		T Get(K key);

		void Recycling(K key, T @object);
	}
}
