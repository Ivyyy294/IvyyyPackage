using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ivyyy.GameEvent
{
	[CustomEditor(typeof(GameEvent))]
	public class GameEventEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Raise Event"))
				((GameEvent)target).Raise();
		}
	}
}
