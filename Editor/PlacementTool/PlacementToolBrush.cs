using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	[CreateAssetMenu(fileName = "NewPlacementToolBrush", menuName = "PlacementTool/Brush")]
	public class PlacementToolBrush : ScriptableObject
	{
		[Header("Object settings")]
		[Min(1)] public int m_size = 1;
		[Space]
		[Header("Placement settings")]
		public LayerMask m_layerMask;
		[Min (0.1f)] public float m_gridSize = 0.25f;
		[Space]
		[Header ("Random settings")]
		[Min(0)] public float m_randomRadius = 0f;
		public Vector3 m_randomRotationAxis;
		[Space]
		[Header ("Drag settings")]
		public bool m_allowDrag;
		[Min(0.1f)] public float m_dragSpacing;
		//public bool m_randomObject;
	}
}
