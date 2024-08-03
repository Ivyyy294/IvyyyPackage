using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif

namespace Ivyyy
{
	[System.Serializable]
	public struct PrefabPoolData
	{
		public string m_name;
		public string m_path;
	}

	[CreateAssetMenu(fileName = "NewPlacementToolData", menuName = "PlacementToolData")]
	public class PlacementToolData : ScriptableObject
	{
		public PrefabPoolData[] m_prefabPoolList;
		public PlacementToolBrush[] m_brushList;

	#if UNITY_EDITOR
		public string GetAssetGUID()
		{
			return AssetDatabase.AssetPathToGUID (AssetDatabase.GetAssetPath(this));
		}
	#endif
	}
}
