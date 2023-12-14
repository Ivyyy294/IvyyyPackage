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

		// Start is called before the first frame update
        void Start()
        {
			// Don't do anything when in prefab mode
			if  (gameObject.scene.name == null)
				return;

			networkBehaviours = GetComponents <NetworkBehaviour>();
			
			foreach (NetworkBehaviour i in networkBehaviours)
			{
				if (i.IsGuidValid() && i.IsGuidUnique())
					NetworkBehaviour.guidMap.Add (i.GUID, i);
			}
        }

		void OnDestroy()
		{
			// Don't do anything when in prefab mode
			if  (gameObject.scene.name == null)
				return;

			if (networkBehaviours != null)
			{
				foreach (NetworkBehaviour i in networkBehaviours)
				{
					if (NetworkBehaviour.guidMap.ContainsKey (i.GUID))
						NetworkBehaviour.guidMap.Remove (i.GUID);
				}
			}
		}

#if UNITY_EDITOR
		private void Update()
		{
			// Don't do anything when running the game or in prefab mode
			if (Application.isPlaying)
				return;

			if  (gameObject.scene.name == null)
				return;

			networkBehaviours = GetComponents <NetworkBehaviour>();

			foreach (NetworkBehaviour i in networkBehaviours)
			{
				if (!i.IsGuidValid() || !i.IsGuidUnique())
				{
					do {i.GenerateGuid();}
					while (!i.IsGuidUnique());

					NetworkBehaviour.guidMap.Add (i.GUID, i);
					EditorUtility.SetDirty (this);
					EditorSceneManager.MarkSceneDirty (gameObject.scene);
				}
			}
		}
#endif
	}
}
