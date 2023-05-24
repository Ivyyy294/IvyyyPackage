using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.Core
{
	//[System.Serializable]
	//public class AudioClipList
	//{
	//	public string name;
	//	public List <AudioClip> audioClips;
	//}

	[RequireComponent(typeof(AudioSource))]
	public class AudioHandler : MonoBehaviour
	{
		public static AudioHandler Me;

		[SerializeField] private AudioSource source;

		private void Start()
		{
			if (Me == null)
			{
				source = GetComponent <AudioSource>();
				Me = this;
			}
			else
				Debug.Log ("Trying to create a new AudioHandler instance!");
		}

		public void PlayOneShot (AudioClip clip, float volume = 1f)
		{
			if (source != null && clip != null)
				source.PlayOneShot (clip, volume);
		}

		public void PlayOneShotFromList (List <AudioClip> list, float volume = 1f)
		{
			if (source != null && list.Count > 0)
			{
				int i = Random.Range (0, list.Count);
				source.PlayOneShot (list[i], volume);
			}
		}
	}
}
