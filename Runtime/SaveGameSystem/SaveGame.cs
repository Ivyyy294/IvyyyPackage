using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Ivyyy.SaveGameSystem
{
	public class SaveGame
	{
		public static string Load (string path, string fileName)
		{
			string fullPath = Path.Combine (path, fileName);
			string dataToLoad = "";

			if (File.Exists (fullPath))
			{
				try
				{
					using (FileStream stream = new FileStream (fullPath, FileMode.Open))
					{
						using (StreamReader reader = new StreamReader (stream))
							dataToLoad = reader.ReadToEnd();
					}
				}
				catch (Exception e)
				{
					Debug.LogError ("Failed to load Data!\n" + fullPath + "\n" + e);
				}
			}

			return dataToLoad;
		}

		public static void Save (string path, string fileName, string data)
		{
			string fullPath = Path.Combine (path, fileName);

			try
			{
				Directory.CreateDirectory (Path.GetDirectoryName (fullPath));

				using (FileStream stream = new FileStream (fullPath, FileMode.OpenOrCreate))
				{
					using (StreamWriter writer = new StreamWriter (stream))
						writer.Write (data);
				}
			}
			catch (Exception e)
			{
				Debug.LogError ("Failed to save data!\n" + fullPath + "\n" + e);
			}
		}
	}
}
