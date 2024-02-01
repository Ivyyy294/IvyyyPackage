using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
 #endif

namespace Ivyyy.Network
{
	[ExecuteInEditMode]
    public class NetworkObject : MonoBehaviour
    {
		private NetworkBehaviour[] networkBehaviours;
		private NetworkPackage package = new NetworkPackage();

		//Keeps track of all guids
		static Dictionary <string, string> guidMap = new Dictionary<string, string>();

		private void Start()
		{
			if (!Application.isPlaying)
				return;

			networkBehaviours = GetComponents <NetworkBehaviour>();

			foreach (NetworkBehaviour i in networkBehaviours)
				NetworkBehaviour.guidMap.Add (i.GUID, i);
		}

		void OnDestroy()
		{
			// Don't do anything when running the game or in prefab mode
			if (Application.isPlaying)
			{
				if (networkBehaviours != null)
				{
					foreach (NetworkBehaviour i in networkBehaviours)
						NetworkBehaviour.guidMap.Remove(i.GUID);
				}
			}
		}

#if UNITY_EDITOR
		private void Update()
		{
			// Don't do anything when running the game or in prefab mode
			if (Application.isPlaying)
				return;

			networkBehaviours = GetComponents <NetworkBehaviour>();

			if (gameObject.scene.path.Length == 0)
				ClearGUIDs();
			else
				RefreshGUIDs();
		}
		
		public bool IsGuidValid(NetworkBehaviour networkBehaviour)
		{
			return networkBehaviour.GUID != null && networkBehaviour.GUID.Length > 0;
		}

		public bool GuidContracted (NetworkBehaviour networkBehaviour, string name)
		{
			return IsGuidValid (networkBehaviour)
				&& guidMap.ContainsKey (networkBehaviour.GUID)
				&& guidMap[networkBehaviour.GUID] != name;
		}

		private string GetParentString()
		{
			string parentStr = "";
			Transform parent = transform.parent;

			while (parent != null)
			{
				parentStr += parent.name;
				parent = parent.parent;
			}

			return parentStr;
		}

		private void ClearGUIDs()
		{
			for (int i = 0; i < networkBehaviours.Length; ++i)
			{
				NetworkBehaviour networkBehaviour = networkBehaviours[i];
				networkBehaviour.ResetGUID();
			}
		}

		private void RefreshGUIDs()
		{
			for (int i = 0; i < networkBehaviours.Length; ++i)
			{
				NetworkBehaviour networkBehaviour = networkBehaviours[i];

				string name = gameObject.scene.name 
					+ GetParentString()
					+ gameObject.name + i;

				if (!IsGuidValid(networkBehaviour)/* || GuidContracted(networkBehaviour, name)*/)
				{
					Debug.Log ("Refresh GUID: " + name);

					networkBehaviour.GenerateGuid();

					EditorUtility.SetDirty(networkBehaviour);
					EditorSceneManager.MarkSceneDirty(gameObject.scene);
				}

				if (!guidMap.ContainsKey (networkBehaviour.GUID))
					guidMap.Add (networkBehaviour.GUID, name);
			}
		}
#endif
	}
}
