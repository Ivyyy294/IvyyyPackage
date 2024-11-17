using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	public class AudioSettings : ISettingContainer
	{
		public float m_masterVolume;
		public float m_sfxVolume = 1f;
		public float m_musicVolume = 1f;
		public float m_ambientVolume = 1f;
		public float m_uiVolume = 1f;
		public float m_voiceLine = 1f;
		public bool m_subtitle = true;

		public void SaveSettings()
		{
			PlayerPrefs.SetFloat("IvyyyMasterVolume", m_masterVolume);
			PlayerPrefs.SetFloat("IvyyySfxVolume", m_sfxVolume);
			PlayerPrefs.SetFloat("IvyyyMusicVolume", m_musicVolume);
			PlayerPrefs.SetFloat("IvyyyAmbientVolume", m_ambientVolume);
			PlayerPrefs.SetFloat("IvyyyUiVolume", m_uiVolume);
			PlayerPrefs.SetFloat("IvyyyVoiceLine", m_voiceLine);
			PlayerPrefs.SetFloat("IvyyySubtitle", m_subtitle ? 1f : 0f);
			PlayerPrefs.Save();
		}

		public AudioSettings()
		{
			LoadSettings();
		}

		//Private Functions
		public void LoadSettings()
		{
			m_masterVolume = LoadValue("IvyyyMasterVolume");
			m_sfxVolume = LoadValue("IvyyySfxVolume");
			m_musicVolume = LoadValue("IvyyyMusicVolume");
			m_ambientVolume = LoadValue("IvyyyAmbientVolume");
			m_uiVolume = LoadValue("IvyyyUiVolume");
			m_voiceLine = LoadValue("IvyyyVoiceLine");
			m_subtitle = LoadValue("IvyyySubtitle") > 0f ? true : false;
		}

		float LoadValue(string key)
		{
			if (PlayerPrefs.HasKey(key))
				return PlayerPrefs.GetFloat(key);
			else
				return 1f;
		}
	}
}


