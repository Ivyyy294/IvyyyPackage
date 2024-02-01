using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISettingContainer
{
	void SaveSettings();
	void LoadSettings();
}

public class GameSettings
{
	private static GameSettings me;
	public AudioSettings audioSettings;
	public GraphicSettings graphicSettings;

	private GameSettings()
	{
		audioSettings = new AudioSettings();
		//graphicSettings = new GraphicSettings();
	}

	//Public Functions
	public static GameSettings Me()
	{
		if (me == null)
			me = new GameSettings();

		return me;
	}
}
