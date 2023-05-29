using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
	using UnityEditor;
	using UnityEditor.SceneManagement;
 #endif

namespace Ivyyy.Core
{
	[ExecuteInEditMode]
	public class GUID : MonoBehaviour
	{
		[HideInInspector]
		public string uniqueId;

		//list of all active ids
		public static Dictionary <string, GUID> allGuids = new Dictionary <string, GUID>();

	#if UNITY_EDITOR
		void Update()
		{
			// Don't do anything when running the game
			if (Application.isPlaying)
				return;

			// if we are not part of a scene then we are a prefab so do not attempt to set the id
			if  (gameObject.scene.name == null)
				return;

			bool hasValidValue = uniqueId != null && uniqueId.Length > 0;
			bool anotherComponentAlreadyHasThisID = (hasValidValue && allGuids.ContainsKey (uniqueId) && allGuids[uniqueId] != this);

			if (!hasValidValue || anotherComponentAlreadyHasThisID)
			{
				uniqueId = System.Guid.NewGuid().ToString();
				EditorUtility.SetDirty (this);
				EditorSceneManager.MarkSceneDirty (gameObject.scene);
			}

			// We can be sure that the key is unique - now make sure we have 
			// it in our list
			if (!allGuids.ContainsKey (uniqueId))
				allGuids.Add(uniqueId, this);
		}

		void OnDestroy(){
			allGuids.Remove(uniqueId);
		 }
	#endif
	}
}

