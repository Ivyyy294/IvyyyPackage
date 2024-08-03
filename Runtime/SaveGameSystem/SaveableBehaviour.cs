using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	[RequireComponent(typeof(SaveableObject))]
	public abstract class SaveableBehaviour : MonoBehaviour, ISerializable
	{
		//### Values ###
		//Public
		public string GUID { get { return m_guid; } }
		static public Dictionary<string, SaveableBehaviour> m_guidMap = new Dictionary<string, SaveableBehaviour>();

		//Editor
		[SerializeField, HideInInspector] string m_guid = null;

		//Protected
		protected SerializedPackage m_serializedPackage = new SerializedPackage();

		//public methods
		public void ClearSerializedPackage() { m_serializedPackage.Clear(); }
		public string GetSerializedDataAsString() { return m_serializedPackage.GetSerializedDataAsString(); }
		public byte[] GetSerializedData() { return m_serializedPackage.GetSerializedData(); }
		public bool DeserializeData(string data) { return m_serializedPackage.DeserializeData(data); }
		public bool DeserializeData(byte[] bytes) { return m_serializedPackage.DeserializeData(bytes); }

		public void ReadSerializedPackageData(byte[] data)
		{
			m_serializedPackage.Clear();
			m_serializedPackage.DeserializeData(data);
			ReadSerializedPackageData();
		}

		public void ResetGUID() { m_guid = ""; }
		public void GenerateGuid() { m_guid = System.Guid.NewGuid().ToString(); }

		//public abstract methods
		public abstract void SetSerializedPackageData();
		public abstract void ReadSerializedPackageData();
	}
}


