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
	public abstract class TemplateAudioManager<T, K> : TemplateSingleton<T> where T : TemplateAudioManager<T, K>
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
			var v = GetSound(source);
			v.clip = audioClips.GetElement(audioName);
			v.Play(delay);
			v.loop = loop;
		}

		public static void PlaySound(AudioSource source, string audioName, ulong delay = 0, bool loop = false)
		{
			source.clip = Instance.audioClips.GetElement(audioName);
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

		public AudioSource GetSound(K audioSourceName)
		{
			if (audioSources.TryGetValue(audioSourceName, out var v))
			{
				return v;
			}
			throw new Exception($"Audio Manager异常:无法找到声音源{audioSourceName},请先添加");
		}

		public void PauseSound(K audioSourceName)
		{
			GetSound(audioSourceName).Pause();
		}

		public void StopSound(K audioSourceName)
		{
			GetSound(audioSourceName).Stop();
		}

		public void UnPauseSound(K audioSourceName)
		{
			var s = GetSound(audioSourceName);
			if (s.isPlaying)
			{
				Debug.LogWarning($"Audio Manager警告:声音源{audioSourceName}正在播放,不需要恢复");
			}
			else
			{
				s.UnPause();
			}
		}

		public void CloseMusic(K audioSourceName)
		{
			var s = GetSound(audioSourceName);
			s.Pause();
			s.mute = true;
		}

		public void OpenMusic(K audioSourceName)
		{
			var s = GetSound(audioSourceName);
			s.UnPause();
			s.mute = false;
		}

		public void CloseAllMusic()
		{
			var allSources = GameObject.FindObjectsOfType<AudioSource>();
			foreach (var source in allSources)
			{
				source.Pause();
				source.mute = true;
			}
		}

		public void OpenAllMusic()
		{
			var allSources = GameObject.FindObjectsOfType<AudioSource>();
			foreach (var source in allSources)
			{
				source.UnPause();
				source.mute = false;
			}
		}
	}
}
