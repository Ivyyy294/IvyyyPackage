using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	public class AudioPlayer : MonoBehaviour
	{
		[SerializeField] AudioAsset audioAsset;
		[SerializeField] bool playOnAwake = false;
		private AudioSource audioSource;
		private float baseVolume;
		bool fadeOut = false;
		bool fadeIn = false;

		//Public Functions
		public void Play()
		{
			if (audioSource && !audioSource.isPlaying)
			{
				audioAsset?.Play(audioSource);
				baseVolume = audioSource.volume;
				fadeOut = false;
			}
		}

		public void Play(AudioAsset newAudioAsset)
		{
			audioAsset = newAudioAsset;

			if (audioSource.isPlaying)
				audioSource.Stop();

			Play();
		}

		public void Stop()
		{
			audioSource.Stop();
		}

		public void FadeOut(float time)
		{
			if (!fadeOut && enabled)
			{
				fadeOut = true;
				StartCoroutine(FadeOutTask(time));
			}
		}

		public void FadeIn(float time)
		{
			if (!fadeIn && enabled)
			{
				fadeIn = true;
				StartCoroutine(FadeInTask(audioAsset, time));
			}
		}

		public void FadeIn(AudioAsset newAudioAsset, float time)
		{
			if (!fadeIn && enabled)
			{
				audioAsset = newAudioAsset;
				FadeIn(time);
			}
		}

		public void Transition(AudioAsset newAudioAsset, float transitionTime)
		{
			StartCoroutine(TransitionTask(newAudioAsset, transitionTime));
		}

		public bool IsPlaying() { return audioSource.isPlaying; }

		public AudioAsset AudioAsset() { return audioAsset; }

		//Private Functions
		private void Awake()
		{
			audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
			baseVolume = audioSource.volume;
			audioSource.playOnAwake = false;
			audioSource.Stop();

			if (playOnAwake)
				Play();
		}

		private void Update()
		{
			if (!fadeOut && !fadeIn && audioAsset != null && audioSource.isPlaying)
				audioSource.volume = baseVolume * audioAsset.GetVolumeFactor();
		}

		IEnumerator FadeInTask(AudioAsset newAudioAsset, float time)
		{
			Play(newAudioAsset);
			audioSource.volume = 0f;

			while (audioSource.volume < baseVolume)
			{
				float volumeOffset = baseVolume * Time.unscaledDeltaTime / time;
				audioSource.volume += volumeOffset;
				yield return null;
			};

			fadeIn = false;
		}

		IEnumerator FadeOutTask(float time)
		{
			while (audioSource.volume > 0f)
			{
				float volumeOffset = baseVolume * Time.unscaledDeltaTime / time;
				audioSource.volume -= volumeOffset;
				yield return null;
			};

			audioSource.Stop();
			fadeOut = false;
		}

		IEnumerator TransitionTask(AudioAsset newAmbient, float transitionTime)
		{
			float fade = transitionTime * 0.5f;

			if (IsPlaying())
				FadeOut(fade);

			while (fadeOut)
				yield return null;

			FadeIn(newAmbient, fade);
		}

		private void OnDrawGizmosSelected()
		{
			if (audioAsset != null && audioAsset.spatial)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(transform.position, audioAsset.minDistance);
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(transform.position, audioAsset.maxDistance);
			}
		}
	}
}

