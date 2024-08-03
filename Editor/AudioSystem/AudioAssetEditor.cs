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

			if (audioAsset.audioTyp == AudioAsset.AudioTyp.SFX
				|| audioAsset.audioTyp == AudioAsset.AudioTyp.UI)
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
			string labelVolume = ("Volume \t[" + audioAsset.volume.x.ToString("0.00") + " - " + audioAsset.volume.y.ToString("0.00") + "]");
			EditorGUILayout.MinMaxSlider(labelVolume, ref audioAsset.volume.x, ref audioAsset.volume.y, 0f, 1f);

			//Pitch
			EditorGUILayout.Space();
			string labelPitch = ("Pitch \t [" + audioAsset.pitch.x.ToString("0.00") + " - " + audioAsset.pitch.y.ToString("0.00") + "]");
			EditorGUILayout.MinMaxSlider(labelPitch, ref audioAsset.pitch.x, ref audioAsset.pitch.y, 0f, 2f);
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
			string labelVolume = ("Volume \t[" + audioAsset.volume.x.ToString("0.00") + " - " + audioAsset.volume.y.ToString("0.00") + "]");

			float volume = audioAsset.volume.x;
			volume = EditorGUILayout.Slider(labelVolume, volume, 0f, 1f);
			audioAsset.volume.x = volume;
			audioAsset.volume.y = volume;

			//Pitch
			EditorGUILayout.Space();
			string labelPitch = ("Pitch \t [" + audioAsset.pitch.x.ToString("0.00") + " - " + audioAsset.pitch.y.ToString("0.00") + "]");

			float pitch = audioAsset.pitch.x;
			pitch = EditorGUILayout.Slider(labelPitch, pitch, 0f, 2f);
			audioAsset.pitch.x = pitch;
			audioAsset.pitch.y = pitch;
		}

		void SpatialEditor(AudioAsset audioAsset)
		{
			EditorGUILayout.Space();
			SerializedProperty yourBoolVariable = serializedObject.FindProperty("spatial");
			EditorGUILayout.PropertyField(yourBoolVariable, new GUIContent("spatial"));
			serializedObject.ApplyModifiedProperties();

			if (audioAsset.spatial)
			{
				SerializedProperty minDistance = serializedObject.FindProperty("minDistance");
				SerializedProperty maxDistance = serializedObject.FindProperty("maxDistance");

				float minDis = EditorGUILayout.Slider("minDistance", audioAsset.minDistance, 0f, 500f);
				float maxDis = EditorGUILayout.Slider("maxDistance", audioAsset.maxDistance, 0f, 500f);

				audioAsset.minDistance = minDis;
				audioAsset.maxDistance = maxDis;
			}

			EditorGUILayout.Space();
		}
	}
}

