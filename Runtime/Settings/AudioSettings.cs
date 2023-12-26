using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : ISettingContainer
{
	public float sfxVolume = 1f;
	public float musicVolume = 1f;
	public float ambientVolume = 1f;
	public float uiVolume = 1f;
	public float voiceLine = 1f;
	public bool subtitle = true;

	public void SaveSettings()
	{
		PlayerPrefs.SetFloat("IvyyySfxVolume", sfxVolume);
		PlayerPrefs.SetFloat("IvyyyMusicVolume", musicVolume);
		PlayerPrefs.SetFloat("IvyyyAmbientVolume", ambientVolume);
		PlayerPrefs.SetFloat("IvyyyUiVolume", uiVolume);
		PlayerPrefs.SetFloat("IvyyyVoiceLine", voiceLine);
		PlayerPrefs.SetFloat ("IvyyySubtitle", subtitle ? 1f : 0f);
        PlayerPrefs.Save();
	}

    public AudioSettings()
    {
		LoadSettings();
    }

    //Private Functions
	public void LoadSettings()
	{
		sfxVolume = LoadValue ("IvyyySfxVolume");
		musicVolume = LoadValue ("IvyyyMusicVolume");
		ambientVolume = LoadValue ("IvyyyAmbientVolume");
		uiVolume = LoadValue ("IvyyyUiVolume");
		voiceLine = LoadValue ("IvyyyVoiceLine");
		subtitle = LoadValue ("IvyyySubtitle") > 0f ? true: false;
	}

	float LoadValue (string key)
	{
		if (PlayerPrefs.HasKey (key))
			return PlayerPrefs.GetFloat (key);
		else
			return 1f;
	}
}
