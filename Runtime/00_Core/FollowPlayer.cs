using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ivyyy.Core
{
	public class FollowPlayer : MonoBehaviour
	{
		[SerializeField] GameObject playerToFollow;
		[SerializeField] Vector3 followOffset = Vector3.zero;

		//LateUpdate is used to make sure that the player already moved
		void LateUpdate()
		{
			Assert.IsTrue (playerToFollow != null, "Missing player object!");

			if (playerToFollow != null)
				transform.position = playerToFollow.transform.position + followOffset;
		}
	}
}

