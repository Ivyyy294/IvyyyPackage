using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Ivyyy
{
	public abstract class SaveGame
	{
		//private
		string m_path;
		string m_fileName;
		byte[] m_data;

		//public Methods
		public SaveGame() { }

		public SaveGame(string fileName)
		{
			m_fileName = fileName;
			m_path = Application.persistentDataPath;
		}

		public SaveGame(string path, string fileName)
		{
			m_fileName = fileName;
			m_path = path;
		}

		public SaveGame(byte[] data)
		{
			ReadSerializedData(data);
			m_path = Application.persistentDataPath;
		}

		public SaveGame(byte[] data, string path)
		{
			ReadSerializedData(data);
			m_path = path;
		}

		public void SetSaveGameData(byte[] data) { m_data = data; }
		public byte[] GetSaveGameData()
		{
			return m_data;
		}

		public string GetFileName() { return m_fileName; }
		public string GetPath() { return m_path; }
		public string GetFullPath() { return Path.Combine(m_path, m_fileName); }

		public bool IsFilePathValid() { return File.Exists(GetFullPath()); }

		public bool Load(string path, string fileName)
		{
			m_path = path;
			m_fileName= fileName;

			return Load();
		}

		public bool Load()
		{
			bool ok = IsFilePathValid();

			if (ok)
			{
				string fullPath = GetFullPath();

				byte[] data = File.ReadAllBytes(fullPath);

				ok = ReadSerializedData(data);

				if (!ok)
				{
					Debug.LogError("Failed to load Data!\n" + fullPath);
					ok = false;
				}
			}
			else
				Debug.LogError ($"Invalid file path {GetFullPath()}");

			return ok;
		}

		public bool Save()
		{
			bool ok = true;
			string fullPath = GetFullPath();

			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
				byte[] data = GetSerializedData();
				File.WriteAllBytes(fullPath, data);
			}
			catch (Exception e)
			{
				Debug.LogError("Failed to save data!\n" + fullPath + "\n" + e);
				ok = false;
			}

			return ok;
		}

		public void DeleteLocalFile()
		{
			if (IsFilePathValid())
			{
				string fullPath = GetFullPath();
				File.Delete(fullPath);
			}
		}

		static public bool Compare(SaveGame left, SaveGame right)
		{
			if (left == null && right == null)
				return true;
			else if (left == null || right == null)
				return false;
			else
				return left.m_fileName == right.m_fileName;
		}

		//Protected Methods
		protected abstract bool ReadSerializedData(byte[] data);
		protected abstract byte[] GetSerializedData();
	}
}
