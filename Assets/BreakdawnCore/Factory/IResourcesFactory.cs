using UnityEngine;

namespace Breakdawn.Factory
{
	public interface IResourcesFactory<T> where T : Object
	{
		T GetResource(string name);
	}
}
