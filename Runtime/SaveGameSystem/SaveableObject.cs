using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Ivyyy
{
	[ExecuteInEditMode]
	public class SaveableObject : MonoBehaviour
	{
		private SaveableBehaviour[] m_saveableBehaviours;

		static Dictionary<string, string> m_guidMap = new Dictionary<string, string>();

		// Start is called before the first frame update
		void Start()
		{
			if (!Application.isPlaying)
				return;

			m_saveableBehaviours = GetComponents<SaveableBehaviour>();

			foreach (SaveableBehaviour i in m_saveableBehaviours)
			{
				if (i.GUID == null || i.GUID.Length == 0)
					Debug.LogWarning("Missing GUID on " + i.gameObject.name);
				else
					SaveableBehaviour.m_guidMap.Add(i.GUID, i);
			}
		}

		void OnDestroy()
		{
			// Don't do anything when running the game or in prefab mode
			if (Application.isPlaying)
			{
				if (m_saveableBehaviours != null)
				{
					foreach (SaveableBehaviour i in m_saveableBehaviours)
						SaveableBehaviour.m_guidMap.Remove(i.GUID);
				}
			}
		}

#if UNITY_EDITOR
		private void Update()
		{
			// Don't do anything when running the game or in prefab mode
			if (Application.isPlaying || gameObject == null)
				return;

			m_saveableBehaviours = GetComponents<SaveableBehaviour>();

			if (gameObject.scene == null || gameObject.scene.path == null || gameObject.scene.path.Length == 0)
				ClearGUIDs();
			else
				RefreshGUIDs();
		}

		public bool IsGuidValid(SaveableBehaviour saveableBehaviour)
		{
			return saveableBehaviour.GUID != null && saveableBehaviour.GUID.Length > 0;
		}

		public bool GuidContracted(SaveableBehaviour saveableBehaviour, string name)
		{
			return IsGuidValid(saveableBehaviour)
				&& m_guidMap.ContainsKey(saveableBehaviour.GUID)
				&& m_guidMap[saveableBehaviour.GUID] != name;
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
			for (int i = 0; i < m_saveableBehaviours.Length; ++i)
			{
				SaveableBehaviour saveableBehaviour = m_saveableBehaviours[i];
				saveableBehaviour.ResetGUID();
			}
		}

		private void RefreshGUIDs()
		{
			for (int i = 0; i < m_saveableBehaviours.Length; ++i)
			{
				SaveableBehaviour saveableBehaviour = m_saveableBehaviours[i];

				string name = gameObject.scene.name
					+ GetParentString()
					+ gameObject.name + i;

				if (!IsGuidValid(saveableBehaviour) || GuidContracted(saveableBehaviour, name))
				{
					Debug.Log("Refresh GUID: " + name);

					saveableBehaviour.GenerateGuid();

					EditorUtility.SetDirty(saveableBehaviour);
					EditorSceneManager.MarkSceneDirty(gameObject.scene);
				}

				if (!m_guidMap.ContainsKey(saveableBehaviour.GUID))
					m_guidMap.Add(saveableBehaviour.GUID, name);
			}
		}
#endif
	}
}