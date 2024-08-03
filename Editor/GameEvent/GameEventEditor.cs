using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Ivyyy.GameEvent
{
	[CustomEditor(typeof(GameEvent))]
	public class GameEventEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			CreateEnumSelection();

			GUILayout.Space(16);

			GameEvent gameEvent = (GameEvent)target;
			switch (gameEvent.m_eventTyp)
			{
				case GameEvent.GameEventTyp.Bool:
					CreateRaiseButton <bool>();
					break;
				case GameEvent.GameEventTyp.Int:
					CreateRaiseButton <int>();
					break;
				case GameEvent.GameEventTyp.Float:
					CreateRaiseButton <float>();
					break;
				case GameEvent.GameEventTyp.String:
					CreateRaiseButton <string>();
					break;
				default:
					CreateRaiseButton ();
					break;
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void CreateEnumSelection()
		{
			EditorGUILayout.BeginHorizontal();

			GUILayout.Label ("GameEventTyp", GUILayout.Width(100));

			GameEvent gameEvent = (GameEvent)target;
			gameEvent.m_eventTyp = (GameEvent.GameEventTyp)EditorGUILayout.EnumPopup(gameEvent.m_eventTyp, GUILayout.Width(100));

			EditorGUILayout.EndHorizontal();
		}

		private void CreateRaiseButton<T> ()
		{
			GameEvent gameEvent = (GameEvent) target;

			EditorGUILayout.BeginHorizontal();

			// Create an input field for the local boolean
			bool pressed = GUILayout.Button("Raise", GUILayout.Width(200));

			if (typeof(T) == typeof (bool))
				gameEvent.m_bool = EditorGUILayout.Toggle(gameEvent.m_bool);
			else if (typeof(T) == typeof(int))
				gameEvent.m_int = EditorGUILayout.IntField(gameEvent.m_int);
			else if (typeof(T) == typeof(float))
				gameEvent.m_float = EditorGUILayout.FloatField(gameEvent.m_float);
			else if (typeof(T) == typeof(string))
				gameEvent.m_string = EditorGUILayout.TextField(gameEvent.m_string);

			// Add a button next to the input field
			if (pressed)
			{
				if (typeof(T) == typeof(void))
					((GameEvent)target).Raise();
				else if (typeof(T) == typeof(bool))
					((GameEvent)target).Raise(gameEvent.m_bool);
				else if (typeof(T) == typeof(int))
					((GameEvent)target).Raise(gameEvent.m_int);
				else if (typeof(T) == typeof(float))
					((GameEvent)target).Raise(gameEvent.m_float);
				else if (typeof(T) == typeof(string))
					((GameEvent)target).Raise(gameEvent.m_string);
			}

			// End the horizontal group
			EditorGUILayout.EndHorizontal();
		}

		private void CreateRaiseButton()
		{
			EditorGUILayout.BeginHorizontal();

			GUILayout.Label("Void Event", GUILayout.Width(100));

			// Create an input field for the local boolean
			bool pressed = GUILayout.Button("Raise", GUILayout.Width(100));

			// Add a button next to the input field
			if (pressed)
				((GameEvent)target).Raise();

			// End the horizontal group
			EditorGUILayout.EndHorizontal();
		}
	}
}

#endif