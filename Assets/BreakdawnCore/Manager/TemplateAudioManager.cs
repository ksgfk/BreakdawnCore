using System;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Core
{
	/// <summary>
	/// 管理所有声音
	/// </summary>
	/// <typeparam name="T">继承本类的类</typeparam>
	/// <typeparam name="K">声音源索引的类型</typeparam>
	public abstract class TemplateAudioManager<T, K> : TemplateSingleton<T> where T : class
	{
		protected ISeriesFactory<string, AudioClip> audioClips;
		protected Dictionary<K, AudioSource> audioSources;

		protected TemplateAudioManager(ISeriesFactory<string, AudioClip> audioClipsFactory)
		{
			audioClips = audioClipsFactory;
			audioSources = new Dictionary<K, AudioSource>();
		}

		public void PlaySound(K source, string audioName, ulong delay = 0, bool loop = false)
		{
			if (audioSources.TryGetValue(source, out var v))
			{
				v.clip = audioClips.GetElement(audioName);
				v.Play(delay);
				v.loop = loop;
			}
			else
			{
				throw new Exception($"Audio Manager异常:无法找到{source}声音源,请先添加");
			}
		}

		public void PlaySound(AudioSource source, string audioName, ulong delay = 0, bool loop = false)
		{
			source.clip = audioClips.GetElement(audioName);
			source.Play(delay);
			source.loop = loop;
		}

		public void AddAudioSource(K audioSourceName, AudioSource source)
		{
			if (!audioSources.ContainsKey(audioSourceName))
			{
				audioSources.Add(audioSourceName, source);
			}
			else
			{
				throw new Exception($"Audio Manager异常:已有名为{audioSourceName}的声音源");
			}
		}
	}
}
