using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	public class AudioPlayer : MonoBehaviour
	{
		[SerializeField] AudioAsset m_audioAsset;
		[SerializeField] bool m_playOnAwake = false;
		private AudioSource m_audioSource;
		private float m_baseVolume;
		bool m_fadeOut = false;
		bool m_fadeIn = false;

		//Public Functions
		public void Play()
		{
			if (m_audioSource && !m_audioSource.isPlaying)
			{
				m_audioAsset?.Play(m_audioSource);
				m_baseVolume = m_audioSource.volume;
				m_fadeOut = false;
			}
		}

		public void Play(AudioAsset newAudioAsset)
		{
			m_audioAsset = newAudioAsset;

			if (m_audioSource.isPlaying)
				m_audioSource.Stop();

			Play();
		}

		public void Stop()
		{
			m_audioSource.Stop();
		}

		public void FadeOut(float time)
		{
			if (!m_fadeOut && enabled)
			{
				m_fadeOut = true;
				StartCoroutine(FadeOutTask(time));
			}
		}

		public void FadeIn(float time)
		{
			if (!m_fadeIn && enabled)
			{
				m_fadeIn = true;
				StartCoroutine(FadeInTask(m_audioAsset, time));
			}
		}

		public void FadeIn(AudioAsset newAudioAsset, float time)
		{
			if (!m_fadeIn && enabled)
			{
				m_audioAsset = newAudioAsset;
				FadeIn(time);
			}
		}

		public void Transition(AudioAsset newAudioAsset, float transitionTime)
		{
			StartCoroutine(TransitionTask(newAudioAsset, transitionTime));
		}

		public bool IsPlaying() { return m_audioSource.isPlaying; }

		public AudioAsset AudioAsset() { return m_audioAsset; }

		//Private Functions
		private void Awake()
		{
			m_audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
			m_baseVolume = m_audioSource.volume;
			m_audioSource.playOnAwake = false;
			m_audioSource.Stop();

			if (m_playOnAwake)
				Play();
		}

		private void Update()
		{
			if (!m_fadeOut && !m_fadeIn && m_audioAsset != null && m_audioSource.isPlaying)
				m_audioSource.volume = m_baseVolume * m_audioAsset.GetVolumeFactor();
		}

		IEnumerator FadeInTask(AudioAsset newAudioAsset, float time)
		{
			Play(newAudioAsset);
			m_audioSource.volume = 0f;

			while (m_audioSource.volume < m_baseVolume)
			{
				float volumeOffset = m_baseVolume * Time.unscaledDeltaTime / time;
				m_audioSource.volume += volumeOffset;
				yield return null;
			};

			m_fadeIn = false;
		}

		IEnumerator FadeOutTask(float time)
		{
			while (m_audioSource.volume > 0f)
			{
				float volumeOffset = m_baseVolume * Time.unscaledDeltaTime / time;
				m_audioSource.volume -= volumeOffset;
				yield return null;
			};

			m_audioSource.Stop();
			m_fadeOut = false;
		}

		IEnumerator TransitionTask(AudioAsset newAmbient, float transitionTime)
		{
			float fade = transitionTime * 0.5f;

			if (IsPlaying())
				FadeOut(fade);

			while (m_fadeOut)
				yield return null;

			FadeIn(newAmbient, fade);
		}

		private void OnDrawGizmosSelected()
		{
			if (m_audioAsset != null && m_audioAsset.m_spatial)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(transform.position, m_audioAsset.m_minDistance);
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(transform.position, m_audioAsset.m_maxDistance);
			}
		}
	}
}

