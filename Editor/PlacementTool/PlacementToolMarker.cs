using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ivyyy
{
	public class PlacementToolMarker
	{
		private GameObject m_marker;
		private List<GameObject> m_currentPrefabs = new List<GameObject>();

		private PlacementToolBrush m_brush;
		private GameObject m_currentTemplate;

		private int m_circleId;

		private float m_currentRadius;
		private Vector3 m_currentRotAxis;

		//Public Methods
		public PlacementToolMarker()
		{
			m_marker = new GameObject();
			m_marker.hideFlags = HideFlags.HideAndDontSave;
			m_circleId = GUIUtility.GetControlID(FocusType.Passive);
		}

		public void SetActive(bool active)
		{
			m_marker.SetActive(active);

			if (active)
			{
				UpdatePosition(false);
				SwapPrefab();
			}
		}

		public void SetBrush(PlacementToolBrush brush)
		{
			m_brush = brush;
			UpdatePosition(false);
			SwapPrefab();
		}

		public void SetTemplate(GameObject template)
		{
			m_currentTemplate = template;
			SwapPrefab();
		}

		public void Refresh(PlacementToolBrush brush, GameObject template)
		{
			m_brush = brush;
			UpdatePosition(false);
			m_currentTemplate = template;
			SwapPrefab();
		}

		public void DestroyPrefabs()
		{
			foreach (GameObject i in m_currentPrefabs)
				Object.DestroyImmediate(i);

			m_currentPrefabs.Clear();
		}

		public Vector3 GetPosition()
		{
			return m_marker.transform.position;
		}

		public Quaternion GetRotation()
		{
			return m_marker.transform.rotation;
		}

		public void UpdatePosition(bool snapToGrid)
		{
			if (Event.current == null || m_brush == null)
				return;

			if (m_currentRotAxis != m_brush.m_randomRotationAxis
				|| m_currentRadius != m_brush.m_randomRadius)
				SwapPrefab();

			RaycastHit hitInfo;
			int mask = m_brush.m_layerMask.value;

			if (!Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hitInfo, Mathf.Infinity, mask))
				return;
			else
			{
				Vector3 pos = hitInfo.point;

				if (snapToGrid)
				{
					pos.x -= pos.x % m_brush.m_gridSize;
					pos.z -= pos.z % m_brush.m_gridSize;
					pos.y = m_marker.transform.position.y;
				}

				m_marker.transform.position = pos;
			}
		}

		public void ResetRotation()
		{
			m_marker.transform.rotation = Quaternion.identity;
		}

		public void Rotate(Vector3 axis, float angle)
		{
			m_marker.transform.Rotate(axis, angle, Space.Self);
		}

		public bool IsSpawnDragAllowed(Vector3 lastPos)
		{
			if (m_brush.m_allowDrag == false
				|| lastPos == Vector3.negativeInfinity)
				return false;

			float distance = Mathf.Abs(Vector3.Distance(lastPos, GetPosition()));

			return distance >= m_brush.m_dragSpacing;
		}

		public void SpawnPrefab(GameObject parent = null)
		{
			foreach (GameObject i in m_currentPrefabs)
			{
				GameObject newGameObject = PrefabUtility.InstantiatePrefab(m_currentTemplate) as GameObject;

				if (newGameObject is null) return;

				Transform t = newGameObject.transform;
				t.position = i.transform.position;
				t.rotation = i.transform.rotation;
				t.localScale = Vector3.one;

				if (parent != null)
					newGameObject.transform.SetParent(parent.transform);

				Undo.RegisterCreatedObjectUndo(newGameObject, "Paint Prefab");
			}

			if (m_brush.m_randomRadius > 0f)
				SwapPrefab();
		}

		public void DrawHandle()
		{
			if (Event.current.type == EventType.Layout
				|| Event.current.type == EventType.Repaint
				|| Event.current.type == EventType.MouseMove)
			{
				Handles.PositionHandle(GetPosition(), GetRotation());

				if (m_brush.m_randomRadius > 0f)
					Handles.CircleHandleCap(m_circleId, GetPosition(), Quaternion.Euler(90f, 0f, 0f), m_brush.m_randomRadius, Event.current.type);
			}
		}

		//Private Methods
		private void SwapPrefab()
		{
			if (m_brush == null)
				return;

			DestroyPrefabs();

			if (m_currentTemplate == null)
				return;

			m_currentRadius = m_brush.m_randomRadius;
			m_currentRotAxis = m_brush.m_randomRotationAxis;

			for (int i = 0; i < m_brush.m_size; ++i)
			{
				GameObject obj = Object.Instantiate(m_currentTemplate, m_marker.transform);
				SetLayer(obj, LayerMask.NameToLayer("Ignore Raycast"));

				Vector3 positionOffset = Random.insideUnitSphere * m_currentRadius;
				positionOffset.y = 0f;

				obj.transform.position += positionOffset;

				Vector3 rotationOffset = Random.insideUnitCircle;
				rotationOffset = new Vector3(rotationOffset.x * m_brush.m_randomRotationAxis.x
					, rotationOffset.y * m_brush.m_randomRotationAxis.y
					, rotationOffset.z * m_brush.m_randomRotationAxis.z);

				rotationOffset *= 360f;

				obj.transform.localRotation = Quaternion.Euler(rotationOffset);

				m_currentPrefabs.Add(obj);
			}
		}

		private void SetLayer(GameObject obj, int layer)
		{
			obj.layer = layer;

			Transform[] transforms = obj.GetComponentsInChildren<Transform>();

			foreach (Transform i in transforms)
				i.gameObject.layer = layer;

		}
	}
}