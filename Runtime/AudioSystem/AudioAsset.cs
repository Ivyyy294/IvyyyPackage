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
			public AudioClip m_clip;
			public string m_subtitle;
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
		[SerializeField] AudioMixerGroup m_mixerGroup;
		[Space]
		[SerializeField] PlayStyle m_playStyle = PlayStyle.RANDOM;
		[Space]
		public AudioTyp m_audioTyp = AudioTyp.SFX;
		[HideInInspector] public bool m_loop = false;
		[HideInInspector] public Vector2 m_volume = new Vector2(0.5f, 0.5f);
		[HideInInspector] public Vector2 m_pitch = new Vector2(1f, 1f);
		[HideInInspector] public bool m_spatial = false;
		[HideInInspector] public float m_minDistance = 0.5f;
		[HideInInspector] public float m_maxDistance = 500f;

		private Stack<ClipData> m_clipBuffer = new Stack<ClipData>();
		private PlayStyle m_oldPlayStyle;

#if UNITY_EDITOR
		private AudioSource m_preview;

		private void OnEnable()
		{
			m_preview = CreateAudioSource();
		}

		private void OnDisable()
		{
			DestroyImmediate(m_preview);
		}

		public void PlayPreview()
		{
			//Play preview without spatial
			Play(m_preview).spatialBlend = 0f; ;
		}

		public void StopPreview()
		{
			m_preview.Stop();
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
				if (m_clipBuffer.Count == 0 || m_oldPlayStyle != m_playStyle)
					ShuffleAudioClips();

				ClipData clip = m_clipBuffer.Pop();

				if (clip.m_clip != null)
				{
					if (source == null)
						source = CreateAudioSource();

					source.clip = clip.m_clip;
					source.outputAudioMixerGroup = m_mixerGroup;
					//Only Allow loop with externen AudioSource
					source.loop = audioSource != null && m_loop;
					source.volume = Random.Range(m_volume.x, m_volume.y) * GetVolumeFactor();
					source.pitch = Random.Range(m_pitch.x, m_pitch.y);
					source.spatialBlend = m_spatial ? 1f : 0f;
					source.minDistance = m_minDistance;
					source.maxDistance = m_maxDistance;
					source.rolloffMode = AudioRolloffMode.Linear;
					source.Play();

#if UNITY_EDITOR
					//Prevents stable source from being deleted
					if (audioSource == m_preview)
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

			if (m_audioTyp == AudioAsset.AudioTyp.SFX)
				factor *= GameSettings.Me().audioSettings.sfxVolume;
			else if (m_audioTyp == AudioAsset.AudioTyp.MUSIC)
				factor *= GameSettings.Me().audioSettings.musicVolume;
			else if (m_audioTyp == AudioAsset.AudioTyp.AMBIENT)
				factor *= GameSettings.Me().audioSettings.ambientVolume;
			else if (m_audioTyp == AudioAsset.AudioTyp.VOICE_LINE)
				factor *= GameSettings.Me().audioSettings.voiceLine;
			else if (m_audioTyp == AudioAsset.AudioTyp.UI)
				factor *= GameSettings.Me().audioSettings.uiVolume;

			return factor;
		}

		public int ClipCount() { return clipData.Length; }

		public void ShuffleAudioClips()
		{
			m_clipBuffer.Clear();

			if (m_playStyle == PlayStyle.RANDOM)
			{
				while (m_clipBuffer.Count < clipData.Length)
				{
					int index = clipData.Length > 1 ? Random.Range(0, clipData.Length) : 0;

					if (!m_clipBuffer.Contains(clipData[index]))
						m_clipBuffer.Push(clipData[index]);
				}
			}
			else if (m_playStyle == PlayStyle.REVERSE)
			{
				foreach (ClipData i in clipData)
					m_clipBuffer.Push(i);
			}
			else
			{
				for (int i = clipData.Length - 1; i >= 0; --i)
					m_clipBuffer.Push(clipData[i]);
			}

			m_oldPlayStyle = m_playStyle;
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


