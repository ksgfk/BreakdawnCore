namespace Breakdawn.Core
{
	public interface ISeriesFactory<K, V>
	{
		V GetElement(K name);
	}
}
