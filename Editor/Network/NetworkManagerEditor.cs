using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Ivyyy.Network
{
	[CustomEditor(typeof(NetworkManager))]
	public class NetworkManagerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (Application.isPlaying)
			{
				NetworkManager manager = (NetworkManager)target;

				if (GUILayout.Button("Start Host"))
					manager.StartHost(manager.Port);

				if (GUILayout.Button("Start Client"))
					manager.StartClient("127.0.0.1", manager.Port);
			}

			base.OnInspectorGUI();
		}
	}
}

#endif