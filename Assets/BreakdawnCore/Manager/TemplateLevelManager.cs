using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Breakdawn.Core
{
	/// <summary>
	/// (不知道有啥用
	/// </summary>
	public abstract class TemplateLevelManager<T> : TemplateSingleton<T> where T : class
	{
		protected readonly Dictionary<string, int> sceneName4Index = new Dictionary<string, int>();
		protected readonly List<string> sceneName;

		public TemplateLevelManager(List<string> sceneName)
		{
			this.sceneName = sceneName;
			for (int i = 0; i < sceneName.Count; i++)
			{
				sceneName4Index.Add(sceneName[i], i);
			}
		}

		public T LoadScene(int index)
		{
			SceneManager.LoadScene(index);
			return this as T;
		}

		public T LoadScene(string name)
		{
			SceneManager.LoadScene(name);
			return this as T;
		}
	}
}
