using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ivyyy
{
	[CustomEditor(typeof(GameEventListener))]
	public class GameEventListenerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			CreatePropertyField("m_gameEvent");

			GameEventListener gameEventListener = (GameEventListener)target;
			
			if (gameEventListener.m_gameEvent != null)
			{
				GUILayout.Space (8);

				if (gameEventListener.m_gameEvent.m_eventTyp == GameEvent.GameEventTyp.Void)
					CreatePropertyField ("m_response");
				if (gameEventListener.m_gameEvent.m_eventTyp == GameEvent.GameEventTyp.Bool)
					CreatePropertyField("m_responseBool");
				if (gameEventListener.m_gameEvent.m_eventTyp == GameEvent.GameEventTyp.Int)
					CreatePropertyField("m_responseInt");
				if (gameEventListener.m_gameEvent.m_eventTyp == GameEvent.GameEventTyp.Float)
					CreatePropertyField("m_responseFloat");
				if (gameEventListener.m_gameEvent.m_eventTyp == GameEvent.GameEventTyp.String)
					CreatePropertyField("m_responseString");
			}
		}

		public void CreatePropertyField (string name)
		{
			this.serializedObject.Update();
			EditorGUILayout.PropertyField (this.serializedObject.FindProperty (name), true);
			this.serializedObject.ApplyModifiedProperties();
		}
	}
}
