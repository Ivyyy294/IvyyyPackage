using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.Utils
{
    public class Utils
    {
		public static Vector3 GetMouseWorldPosition()
		{
			Vector3 vec = Input.mousePosition;
			vec.z = Camera.main.nearClipPlane;
			vec = GetMouseWorldPositionWithZ (vec, Camera.main);
			return vec;
		}

		public static Vector3 GetMouseWorldPositionWithZ()
		{
			return GetMouseWorldPositionWithZ (Input.mousePosition, Camera.main);
		}

		public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
		{
			return GetMouseWorldPositionWithZ (Input.mousePosition, worldCamera);
		}

		public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
		{
			Vector3 worldPosition = worldCamera.ScreenToWorldPoint (screenPosition);
			return worldPosition;
		}
    }
}
