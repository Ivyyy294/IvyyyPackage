using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace Ivyyy
{
	public class PlacementTool : EditorWindow, ISupportsOverlays
	{
		#region Overlay
		[Overlay(defaultDisplay = true)]
		class InstanceOverlay : Overlay
		{
			PlacementTool m_Window;
			public InstanceOverlay(PlacementTool win) => m_Window = win;
			public override VisualElement CreatePanelContent()
			{
				var root = new VisualElement();
				root.Add(new Label() { text = $"Placement Controlls" });
				root.Add(new Button(() => { m_Window.OnToggleVisibility(); }) { text = "Hide [ESC]" });
				root.Add(new Button(() => { m_Window.OnToggleRuler(); }) { text = "Ruler [R]" });
				root.Add(new Button(() => { m_Window.OnRotateY(); }) { text = "RotateY [Q]" });
				root.Add(new Button(() => { m_Window.OnRotateX(); }) { text = "RotateX [W]" });
				root.Add(new Button(() => { m_Window.OnRotateZ(); }) { text = "RotateZ [E]" });
				root.Add(new Button(() => { m_Window.ResetRotation(); }) { text = "Reset\nRotation" });
				root.Add(new Button(() => { m_Window.OnToggleSnapToGrid(); }) { text = "Snap [Shift]" });
				root.Add(new Button(() => { m_Window.OnNextPrefab(); }) { text = "Prefab [➡]" });
				root.Add(new Button(() => { m_Window.OnPreviousPrefab(); }) { text = "Prefab [⬅]" });
				root.Add(new Button(() => { m_Window.OnNextBrush(); }) { text = "Brush [B]" });
				root.Add(new Button(() => { m_Window.OnNextPalette(); }) { text = "Palette [⬆]" });
				root.Add(new Button(() => { m_Window.OnPreviousPalette(); }) { text = "Palette [⬇]" });
				root.Add(new Button(() => { m_Window.OnCreateGroup(); }) { text = "New Group [G]" });
				return root;
			}
		}

		#endregion
		[MenuItem("Tools/Ivyyy/Placement Tool")]
		private static void OpenWindow() => GetWindow<PlacementTool>("Placement Tool");

		//Editor values
		[SerializeField] PlacementToolData m_data;
		Editor m_toolDataEditor;
		bool m_showToolDataEditor;

		private SerializedObject _so;
		private SerializedProperty _data;

		private PlacementToolMarker m_placementMarker;
		private List<GameObject[]> m_prefabs;
		private GameObject m_currentParent;

		private InstanceOverlay m_Overlay;
		private string m_statusBrush;
		private string m_statusPalette;
		private string m_statusPrefab;

		private int m_indexCurrentPool;
		private int m_indexCurrentBrush;
		private int m_indexCurrentPrefab;

		private Quaternion m_rotationOffset = Quaternion.identity;

		private bool m_isMarkerActive;
		private bool m_showRuler;
		private bool m_cntrlPressed;
		private bool m_shiftPressed;

		private bool m_isInitiated = false;

		Stack<Vector3> m_posLastPlaced = new Stack<Vector3>();

		//Public Methods
		public void OnToggleRuler()
		{
			m_showRuler = !m_showRuler;
		}

		public void OnToggleVisibility()
		{
			m_isMarkerActive = !m_isMarkerActive;
			m_placementMarker?.SetActive(m_isMarkerActive);

			if (!m_isMarkerActive)
				m_posLastPlaced.Clear();
		}

		public void OnToggleSnapToGrid()
		{
			m_shiftPressed = !m_shiftPressed;
		}

		public void OnRotateX()
		{
			m_placementMarker.Rotate(Vector3.right, m_shiftPressed ? -15f : -90f);
		}

		public void OnRotateY()
		{
			m_placementMarker.Rotate(Vector3.up, m_shiftPressed ? 15f : 90f);
		}

		public void OnRotateZ()
		{
			m_placementMarker.Rotate(Vector3.forward, m_shiftPressed ? 15f : 90f);
		}

		public void ResetRotation()
		{
			m_placementMarker.ResetRotation();
		}

		public void OnNextPrefab()
		{
			if (m_prefabs == null)
				return;

			m_indexCurrentPrefab++;

			if (m_indexCurrentPrefab >= m_prefabs[m_indexCurrentPool].Length)
				m_indexCurrentPrefab = 0;

			m_placementMarker.SetTemplate(GetCurrentPrefab());
			UpdateStatusText();
		}

		public void OnPreviousPrefab()
		{
			if (m_prefabs == null)
				return;

			m_indexCurrentPrefab--;

			if (m_indexCurrentPrefab < 0)
				m_indexCurrentPrefab = m_prefabs[m_indexCurrentPool].Length - 1;

			m_placementMarker.SetTemplate(GetCurrentPrefab());
			UpdateStatusText();
		}

		public void OnNextBrush()
		{
			if (m_data == null)
				return;

			m_indexCurrentBrush++;

			if (m_indexCurrentBrush >= m_data.m_brushList.Length)
				m_indexCurrentBrush = 0;

			m_placementMarker.SetBrush(GetCurrentBrush());
			UpdateStatusText();
		}

		public void OnPreviousBrush()
		{
			m_indexCurrentBrush--;

			if (m_indexCurrentBrush < 0)
				m_indexCurrentBrush = m_data.m_brushList.Length - 1;

			m_placementMarker.SetBrush(GetCurrentBrush());
			UpdateStatusText();
		}

		public void OnNextPalette()
		{
			if (m_data == null)
				return;

			m_indexCurrentPool++;

			if (m_indexCurrentPool >= m_data.m_prefabPoolList.Length)
				m_indexCurrentPool = 0;

			if (m_indexCurrentPrefab >= m_prefabs.Count)
				m_indexCurrentPrefab = 0;

			m_placementMarker.SetTemplate(GetCurrentPrefab());
			UpdateStatusText();
		}
		public void OnPreviousPalette()
		{
			if (m_data == null)
				return;

			m_indexCurrentPool--;

			if (m_indexCurrentPool < 0)
				m_indexCurrentPool = m_data.m_prefabPoolList.Length - 1;

			if (m_indexCurrentPrefab >= m_prefabs.Count)
				m_indexCurrentPrefab = 0;

			m_placementMarker.SetTemplate(GetCurrentPrefab());
			UpdateStatusText();
		}

		public void OnCreateGroup()
		{
			m_currentParent = new GameObject("NewGroup");
			m_currentParent.transform.position = m_placementMarker.GetPosition();
		}

		//Private Methods
		private void OnEnable()
		{
			EditorPrefs.HasKey("PlacementToolDataGridSize");

			_so = new SerializedObject(this);
			_data = _so.FindProperty("m_data");
			SceneView.duringSceneGui += OnSceneGUI;

			m_Overlay = new InstanceOverlay(this);
			SceneView.AddOverlayToActiveView(m_Overlay);

			InitPrefabPools();
			InitMarker();

			m_isMarkerActive = false;
		}

		private void OnDisable()
		{
			m_placementMarker.DestroyPrefabs();

			SceneView.duringSceneGui -= OnSceneGUI;
			SceneView.RemoveOverlayFromActiveView(m_Overlay);
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField("Prefab palete");
			EditorGUILayout.Space(8);

			_so.Update();
			var check = new EditorGUI.ChangeCheckScope();
			EditorGUILayout.PropertyField(_data);

			if (check.changed)
			{
				_so.ApplyModifiedProperties();
				OnPlacementToolDataChanged();
			}

			EditorGUILayout.Space(8);

			if (m_data != null)
			{
				DrawSettingsEditor(m_data, OnPlacementToolDataChanged, ref m_showToolDataEditor, ref m_toolDataEditor);
				UpdateStatusText();
			}
		}

		private void OnSceneGUI(SceneView sceneView)
		{
			if (m_data == null)
				return;

			if (Event.current.type == EventType.KeyDown)
				OnKeyDown();
			else if (Event.current.type == EventType.KeyUp)
				OnKeyUp();

			if (m_isMarkerActive)
			{
				if (Event.current.type == EventType.MouseMove
					|| (Event.current.type == EventType.MouseDrag && Event.current.button == 0))
					UpdateMarker();

				m_placementMarker.DrawHandle();

				if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
				{
					m_placementMarker.SpawnPrefab(m_currentParent);
					m_posLastPlaced.Push(m_placementMarker.GetPosition());
					Event.current.Use();
				}
				if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
				{
					Vector3 lastPos = m_posLastPlaced.Count > 0 ? m_posLastPlaced.Peek() : Vector3.negativeInfinity;

					if (m_placementMarker.IsSpawnDragAllowed(lastPos))
					{
						m_placementMarker.SpawnPrefab(m_currentParent);
						m_posLastPlaced.Push(m_placementMarker.GetPosition());
						Event.current.Use();
					}
				}

				ShowStatusText(sceneView);
			}

			if (m_showRuler && m_posLastPlaced.Count > 0)
				ShowRuler();
		}

		private void OnKeyDown()
		{
			//Toggle Marker
			if (Event.current.keyCode == KeyCode.Escape)
			{
				OnToggleVisibility();
				Event.current.Use();
			}

			if (m_isMarkerActive)
			{
				//Toggle Shift
				if (Event.current.keyCode == KeyCode.LeftShift)
				{
					m_shiftPressed = true;
					Event.current.Use();
				}

				//dont use event
				if (Event.current.keyCode == KeyCode.LeftControl
					|| Event.current.keyCode == KeyCode.RightControl)
					m_cntrlPressed = true;

				//Toggle ruler
				if (Event.current.keyCode == KeyCode.R)
				{
					OnToggleRuler();
					Event.current.Use();
				}

				//Brush
				if (Event.current.keyCode == KeyCode.B)
				{
					OnNextBrush();
					Event.current.Use();
				}

				//Palette
				if (Event.current.keyCode == KeyCode.UpArrow)
				{
					OnNextPalette();
					Event.current.Use();
				}
				if (Event.current.keyCode == KeyCode.DownArrow)
				{
					OnPreviousPalette();
					Event.current.Use();
				}

				//Rotation
				if (Event.current.keyCode == KeyCode.W)
				{
					OnRotateX();
					Event.current.Use();
				}
				if (Event.current.keyCode == KeyCode.Q)
				{
					OnRotateY();
					Event.current.Use();
				}
				if (Event.current.keyCode == KeyCode.E)
				{
					OnRotateZ();
					Event.current.Use();
				}

				//Undo
				if (Event.current.keyCode == KeyCode.Z
					&& m_cntrlPressed)
				{
					if (m_posLastPlaced.Count > 0)
						m_posLastPlaced.Pop();
				}

				//Select Prefab
				if (Event.current.keyCode == KeyCode.LeftArrow)
				{
					OnPreviousPrefab();
					Event.current.Use();
				}
				if (Event.current.keyCode == KeyCode.RightArrow)
				{
					OnNextPrefab();
					Event.current.Use();
				}

				if (Event.current.keyCode == KeyCode.G)
				{
					OnCreateGroup();
					Event.current.Use();
				}
			}
		}

		private void OnKeyUp()
		{
			if (Event.current.keyCode == KeyCode.LeftShift)
			{
				m_shiftPressed = false;
				Event.current.Use();
			}
			if (Event.current.keyCode == KeyCode.LeftControl
				|| Event.current.keyCode == KeyCode.RightControl)
				m_cntrlPressed = false;
		}

		private void ShowRuler()
		{
			Vector3 lastPos = m_posLastPlaced.Peek();
			Vector3 markerPos = m_placementMarker.GetPosition();
			float distance = Vector3.Distance(lastPos, markerPos);
			Vector3 direction = markerPos - lastPos;

			Handles.DrawLine(lastPos, markerPos);
			Handles.Label(lastPos + (direction * 0.5f), distance.ToString("0.00"));
		}

		private GameObject[] LoadAssetList(string path)
		{
			GameObject[] gameObjects = null;

			string[] assetPaths = AssetDatabase.FindAssets("t:prefab", new[] { "Assets/" + path })
					.Select(AssetDatabase.GUIDToAssetPath).ToArray();

			gameObjects = assetPaths.Select(AssetDatabase.LoadAssetAtPath<GameObject>).ToArray();

			return gameObjects;
		}

		private void InitPrefabPools()
		{
			if (EditorPrefs.HasKey("PlacementToolData"))
			{
				string data = EditorPrefs.GetString("PlacementToolData");

				m_data = AssetDatabase.LoadAssetAtPath<PlacementToolData>(AssetDatabase.GUIDToAssetPath(data));

				m_prefabs = new List<GameObject[]>();

				for (int i = 0; i < m_data.m_prefabPoolList.Length; ++i)
					m_prefabs.Add(LoadAssetList(m_data.m_prefabPoolList[i].m_path));
			}
		}

		private void InitMarker()
		{
			if (m_placementMarker == null)
				m_placementMarker = new PlacementToolMarker();

			m_placementMarker.Refresh(GetCurrentBrush(), GetCurrentPrefab());
			m_placementMarker.SetActive(m_isMarkerActive);
		}

		private void UpdateMarker()
		{
			m_placementMarker.UpdatePosition(m_shiftPressed);
			SceneView.RepaintAll();
		}

		private GameObject GetCurrentPrefab()
		{
			if (m_prefabs == null || m_prefabs[m_indexCurrentPool].Length == 0)
				return null;

			GameObject prefab = m_prefabs[m_indexCurrentPool][m_indexCurrentPrefab];
			return prefab;
		}

		private PlacementToolBrush GetCurrentBrush()
		{
			if (m_indexCurrentBrush >= 0 && m_data != null && m_indexCurrentBrush < m_data.m_brushList.Length)
				return m_data.m_brushList[m_indexCurrentBrush];
			else
				return null;
		}

		void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
		{
			if (settings == null)
				return;

			foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

			using (var check = new EditorGUI.ChangeCheckScope())
			{
				if (foldout)
				{
					Editor.CreateCachedEditor(settings, null, ref editor);
					editor.OnInspectorGUI();

					if (check.changed)
					{
						if (onSettingsUpdated != null)
							onSettingsUpdated();
					}
				}
			}
		}

		private void OnPlacementToolDataChanged()
		{
			if (m_data == null)
				EditorPrefs.DeleteKey("PlacementToolData");
			else
				EditorPrefs.SetString("PlacementToolData", m_data.GetAssetGUID());

			InitPrefabPools();
			m_placementMarker.Refresh(GetCurrentBrush(), GetCurrentPrefab());
		}

		private void ShowStatusText(SceneView sceneView)
		{
			Handles.BeginGUI();

			Vector2 size = new Vector2(400, 128);
			Rect rect = new Rect((sceneView.position.width - size.x) / 2, 10, size.x, size.y);
			EditorGUI.DrawRect(rect, Color.gray);

			float width = 128f;

			GUIStyle style = new GUIStyle { fontSize = 12, normal = new GUIStyleState { textColor = Color.white }, alignment = TextAnchor.MiddleCenter };

			//Prefab selection
			size = Vector2.one * width;
			rect = new Rect((sceneView.position.width - size.x) / 2, 10, size.x, size.y);
			var preview = AssetPreview.GetAssetPreview(GetCurrentPrefab());
			GUI.DrawTexture(rect, preview);

			rect = new Rect((sceneView.position.width - size.x) / 2, 118, width, 10);

			GUI.Label(rect, m_statusPrefab, style);


			style = new GUIStyle { fontSize = 18, normal = new GUIStyleState { textColor = Color.white }, alignment = TextAnchor.MiddleCenter };

			float offset = width * 2f;
			//Brush
			rect = new Rect((sceneView.position.width - size.x - offset) / 2, 64, width, 20);
			GUI.Label(rect, m_statusBrush, style);

			//Palette
			rect = new Rect((sceneView.position.width - size.x + offset) / 2, 64, width, 20);
			GUI.Label(rect, m_statusPalette, style);

			Handles.EndGUI();
		}

		private void UpdateStatusText()
		{
			m_statusBrush = "Brush:\n" + GetCurrentBrush().name;
			m_statusPalette = "Palette:\n" + m_data.m_prefabPoolList[m_indexCurrentPool].m_name;

			GameObject gameObject = GetCurrentPrefab();

			m_statusPrefab = gameObject != null ? GetCurrentPrefab().name : "";
		}
	}
}

