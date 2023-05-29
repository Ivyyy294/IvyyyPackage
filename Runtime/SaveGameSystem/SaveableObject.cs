using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Ivyyy.Core;

namespace Ivyyy.SaveGameSystem
{
	[RequireComponent (typeof (GUID))]
	public abstract class SaveableObject : MonoBehaviour
	{
		public static Dictionary <string, SaveableObject> allSaveableObject = new Dictionary <string, SaveableObject>();

		//Private
		private GUID guid;
		abstract public Payload GetPayload ();
		abstract public void LoadObject (Payload val);

		public string GetSerializedData()	{return GetPayload().GetSerializedData();}
		public string UniqueId {get {return guid.uniqueId; }}

		//Add Object to Guid list
		private void Start()
		{
			guid = GetComponent <GUID>();		
			allSaveableObject.Add (guid.uniqueId, this);
		}

		void OnDestroy()
		{
			if (allSaveableObject.ContainsKey (guid.uniqueId))
				allSaveableObject.Remove (guid.uniqueId);
		}
	}
}

