namespace Breakdawn.Factory
{
	public interface ISeriesFactory<K, V>
	{
		V GetElement(K name);
	}
}
