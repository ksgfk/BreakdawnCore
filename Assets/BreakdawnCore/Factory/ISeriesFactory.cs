using System.Collections.Generic;

namespace Breakdawn.Core
{
	public interface ISeriesFactory<K, V>
	{
		V GetElement(K name);

		IEnumerable<K> GetKeyList();
	}
}
