using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ivyyy
{
	[CustomEditor(typeof(AudioAsset))]
	public class AudioAssetEditor : Editor
	{
		private void OnDisable()
		{
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			serializedObject.Update();

			AudioAsset audioAsset = (AudioAsset)target;

			if (audioAsset.m_audioTyp == AudioAsset.AudioTyp.SFX
				|| audioAsset.m_audioTyp == AudioAsset.AudioTyp.UI)
				SFXEditor(audioAsset);
			else
				MusicEditor(audioAsset);

			SpatialEditor(audioAsset);

			serializedObject.Update();
			serializedObject.ApplyModifiedProperties();

			if (GUILayout.Button("Play Preview"))
				audioAsset.PlayPreview();

			EditorGUILayout.Space();

			if (GUILayout.Button("Stop Preview"))
				audioAsset.StopPreview();

			EditorUtility.SetDirty(target);
		}

		void SFXEditor(AudioAsset audioAsset)
		{
			//Loop
			EditorGUILayout.Space();
			SerializedProperty yourBoolVariable = serializedObject.FindProperty("loop");
			EditorGUILayout.PropertyField(yourBoolVariable, new GUIContent("loop"));

			//Volume
			EditorGUILayout.Space();
			string labelVolume = ("Volume \t[" + audioAsset.m_volume.x.ToString("0.00") + " - " + audioAsset.m_volume.y.ToString("0.00") + "]");
			EditorGUILayout.MinMaxSlider(labelVolume, ref audioAsset.m_volume.x, ref audioAsset.m_volume.y, 0f, 1f);

			//Pitch
			EditorGUILayout.Space();
			string labelPitch = ("Pitch \t [" + audioAsset.m_pitch.x.ToString("0.00") + " - " + audioAsset.m_pitch.y.ToString("0.00") + "]");
			EditorGUILayout.MinMaxSlider(labelPitch, ref audioAsset.m_pitch.x, ref audioAsset.m_pitch.y, 0f, 2f);
			EditorGUILayout.Space();
		}

		void MusicEditor(AudioAsset audioAsset)
		{
			//Loop
			EditorGUILayout.Space();
			SerializedProperty yourBoolVariable = serializedObject.FindProperty("loop");
			EditorGUILayout.PropertyField(yourBoolVariable, new GUIContent("loop"));

			//Volume
			EditorGUILayout.Space();
			string labelVolume = ("Volume \t[" + audioAsset.m_volume.x.ToString("0.00") + " - " + audioAsset.m_volume.y.ToString("0.00") + "]");

			float volume = audioAsset.m_volume.x;
			volume = EditorGUILayout.Slider(labelVolume, volume, 0f, 1f);
			audioAsset.m_volume.x = volume;
			audioAsset.m_volume.y = volume;

			//Pitch
			EditorGUILayout.Space();
			string labelPitch = ("Pitch \t [" + audioAsset.m_pitch.x.ToString("0.00") + " - " + audioAsset.m_pitch.y.ToString("0.00") + "]");

			float pitch = audioAsset.m_pitch.x;
			pitch = EditorGUILayout.Slider(labelPitch, pitch, 0f, 2f);
			audioAsset.m_pitch.x = pitch;
			audioAsset.m_pitch.y = pitch;
		}

		void SpatialEditor(AudioAsset audioAsset)
		{
			EditorGUILayout.Space();
			SerializedProperty yourBoolVariable = serializedObject.FindProperty("spatial");
			EditorGUILayout.PropertyField(yourBoolVariable, new GUIContent("spatial"));
			serializedObject.ApplyModifiedProperties();

			if (audioAsset.m_spatial)
			{
				SerializedProperty minDistance = serializedObject.FindProperty("minDistance");
				SerializedProperty maxDistance = serializedObject.FindProperty("maxDistance");

				float minDis = EditorGUILayout.Slider("minDistance", audioAsset.m_minDistance, 0f, 500f);
				float maxDis = EditorGUILayout.Slider("maxDistance", audioAsset.m_maxDistance, 0f, 500f);

				audioAsset.m_minDistance = minDis;
				audioAsset.m_maxDistance = maxDis;
			}

			EditorGUILayout.Space();
		}
	}
}

