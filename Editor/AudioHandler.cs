using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioHandler : MonoBehaviour
	{
		public static AudioHandler Me;

		[SerializeField] private AudioSource source;
		[SerializeField] private List <string> soundIds;
		[SerializeField] private List <AudioClip> sounds;
		private Dictionary <string, AudioClip> audioList = new Dictionary<string, AudioClip>();

		private void Start()
		{
			Me = this;

			audioList.Clear();

			source = GetComponent <AudioSource>();

			for (int i = 0; i < soundIds.Count && i < sounds.Count; ++i)
				audioList.Add (soundIds[i], sounds[i]);
		}

		public void PlayOneShot (string name, float volume = 1f)
		{
			if (source != null)
			{
				if (audioList.ContainsKey (name) && audioList[name] != null)
					source.PlayOneShot (audioList[name], volume);
			}
		}
	}
}
