using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Ivyyy
{
	[CreateAssetMenu(fileName = "NewAudioAsset", menuName = "My Assets/AudioAsset")]
	public class AudioAsset : ScriptableObject
	{
		[System.Serializable]
		public struct ClipData
		{
			public AudioClip clip;
			public string subtitle;
		}

		public enum AudioTyp
		{
			SFX,
			MUSIC,
			AMBIENT,
			UI,
			VOICE_LINE
		}

		public enum PlayStyle
		{
			RANDOM,
			IN_ORDER,
			REVERSE,
		}

		public ClipData[] clipData;
		[SerializeField] AudioMixerGroup mixerGroup;
		[Space]
		[SerializeField] PlayStyle playStyle = PlayStyle.RANDOM;
		[Space]
		public AudioTyp audioTyp = AudioTyp.SFX;
		[HideInInspector] public bool loop = false;
		[HideInInspector] public Vector2 volume = new Vector2(0.5f, 0.5f);
		[HideInInspector] public Vector2 pitch = new Vector2(1f, 1f);
		[HideInInspector] public bool spatial = false;
		[HideInInspector] public float minDistance = 0.5f;
		[HideInInspector] public float maxDistance = 500f;

		private Stack<ClipData> clipBuffer = new Stack<ClipData>();
		private PlayStyle oldPlayStyle;

#if UNITY_EDITOR
		private AudioSource preview;

		private void OnEnable()
		{
			preview = CreateAudioSource();
		}

		private void OnDisable()
		{
			DestroyImmediate(preview);
		}

		public void PlayPreview()
		{
			//Play preview without spatial
			Play(preview).spatialBlend = 0f; ;
		}

		public void StopPreview()
		{
			preview.Stop();
		}
#endif

		public void PlayOneShot()
		{
			Play(null);
		}

		public AudioSource Play(AudioSource audioSource = null)
		{
			AudioSource source = audioSource;

			if (clipData.Length > 0)
			{
				if (clipBuffer.Count == 0 || oldPlayStyle != playStyle)
					ShuffleAudioClips();

				ClipData clip = clipBuffer.Pop();

				if (clip.clip != null)
				{
					if (source == null)
						source = CreateAudioSource();

					source.clip = clip.clip;
					source.outputAudioMixerGroup = mixerGroup;
					//Only Allow loop with externen AudioSource
					source.loop = audioSource != null && loop;
					source.volume = Random.Range(volume.x, volume.y) * GetVolumeFactor();
					source.pitch = Random.Range(pitch.x, pitch.y);
					source.spatialBlend = spatial ? 1f : 0f;
					source.minDistance = minDistance;
					source.maxDistance = maxDistance;
					source.rolloffMode = AudioRolloffMode.Linear;
					source.Play();

#if UNITY_EDITOR
					//Prevents stable source from being deleted
					if (audioSource == preview)
						return source;
#endif
					//Delete tmp audio source after playing
					if (audioSource == null)
						Destroy(source.gameObject, source.clip.length / source.pitch);
				}
				else
					Debug.LogError("Invalid CLip!");

				//if (audioTyp != AudioTyp.MUSIC && audioTyp != AudioTyp.AMBIENT && source != null && source.clip != null)
				//	ShowSubtitle (clip.subtitle, source.clip.length / source.pitch);
				//else 
				//	ShowSubtitle (clip.subtitle);
			}
			//Shows the Subtitle even when clip is null as a placeholder



			return source;
		}

		public void PlayAtPos(Vector3 pos)
		{
			AudioSource tmp = Play();

			if (tmp != null)
				tmp.transform.position = pos;
		}

		public float GetVolumeFactor()
		{
			float factor = GameSettings.Me().audioSettings.m_masterVolume;

			if (audioTyp == AudioAsset.AudioTyp.SFX)
				factor *= GameSettings.Me().audioSettings.sfxVolume;
			else if (audioTyp == AudioAsset.AudioTyp.MUSIC)
				factor *= GameSettings.Me().audioSettings.musicVolume;

			return factor;
		}

		public int ClipCount() { return clipData.Length; }

		public void ShuffleAudioClips()
		{
			clipBuffer.Clear();

			if (playStyle == PlayStyle.RANDOM)
			{
				while (clipBuffer.Count < clipData.Length)
				{
					int index = clipData.Length > 1 ? Random.Range(0, clipData.Length) : 0;

					if (!clipBuffer.Contains(clipData[index]))
						clipBuffer.Push(clipData[index]);
				}
			}
			else if (playStyle == PlayStyle.REVERSE)
			{
				foreach (ClipData i in clipData)
					clipBuffer.Push(i);
			}
			else
			{
				for (int i = clipData.Length - 1; i >= 0; --i)
					clipBuffer.Push(clipData[i]);
			}

			oldPlayStyle = playStyle;
		}
		//Private FUnctions

		AudioSource CreateAudioSource()
		{
			var obj = new GameObject("AudioAssetSource", typeof(AudioSource));
			obj.hideFlags = HideFlags.HideAndDontSave;
			return obj.GetComponent<AudioSource>();
		}

		//void ShowSubtitle (string txt, float playTime = 0f)
		//{
		//	if (!string.IsNullOrEmpty(txt))
		//	{
		//		float minTime = 0.75f;
		//		//Making sure the subtile is readable for at lest 1 second
		//		playTime = Mathf.Max (minTime, playTime);

		//		int priority = 0;

		//		if (audioTyp == AudioTyp.VOICE_LINE)
		//			priority = 3;
		//		else if (audioTyp == AudioTyp.SFX)
		//			priority = 2;
		//		else if (audioTyp == AudioTyp.UI)
		//			priority = 1;

		//		Subtitle.Add (txt, playTime, priority);
		//	}
		//}
	}
}


