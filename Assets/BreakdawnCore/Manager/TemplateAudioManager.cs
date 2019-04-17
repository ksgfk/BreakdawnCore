using UnityEngine;

namespace Breakdawn.Core
{
	public abstract class TemplateAudioManager<T> : TemplateSingleton<T> where T : class
	{
		protected ISeriesFactory<string, AudioClip> audioClips;

		protected TemplateAudioManager(ISeriesFactory<string, AudioClip> audioClipsFactory)
		{
			audioClips = audioClipsFactory;
		}
	}
}
