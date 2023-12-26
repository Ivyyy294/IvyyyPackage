using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphicSettings : ISettingContainer
{
	public class Setting
	{
		public int width;
		public int height;

		public Setting() {}
		public Setting (Resolution val)
		{
			width = val.width;
			height = val.height;
		}

		public string GetDisplayName() {return width.ToString() + "x" + height.ToString();}

		public bool Compare (Setting val)
		{
			return width == val.width
				&& height == val.height;
		}
	}

	public Setting currentSetting = new Setting();
	public List <Setting> availableSettings = new List<Setting>();
	public bool fullscreen = true;
	public Resolution resolution;

	public GraphicSettings()
	{
		for (int i = Screen.resolutions.Length -1; i > 0; --i)
		{
			Setting tmp = new Setting (Screen.resolutions[i]);
			
			if (!availableSettings.Any(x=>x.Compare(tmp)))
				availableSettings.Add (tmp);
		}

		LoadSettings();
	}

	public void SaveSettings()
	{
		PlayerPrefs.SetInt ("IvyyyFullscreen", fullscreen ? 1 : 0);
		PlayerPrefs.SetInt ("IvyyyresolutionW", currentSetting.width);
		PlayerPrefs.SetInt ("IvyyyresolutionH", currentSetting.height);
	}

	public void LoadSettings()
	{
		currentSetting.width = PlayerPrefs.GetInt("IvyyyresolutionW");
		currentSetting.height = PlayerPrefs.GetInt("IvyyyresolutionH");
		fullscreen = PlayerPrefs.GetInt("IvyyyFullscreen") > 0 ? true : false;

		if (currentSetting.width == 0 || currentSetting.height == 0)
			currentSetting = new Setting (Screen.currentResolution);
		
		Screen.SetResolution (currentSetting.width, currentSetting.height, fullscreen);
	}
}
