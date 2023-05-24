using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ivyyy.Core
{
	[RequireComponent (typeof (Rigidbody))]
	public class TriggerZone : MonoBehaviour
	{
		[SerializeField] UnityEvent onTriggerEnter;
		[SerializeField] UnityEvent onTriggerExit;
		[SerializeField] List <string> tagList;

		private void OnTriggerEnter(Collider other)
		{
			if (onTriggerEnter != null && tagList.Contains (other.tag))
				onTriggerEnter.Invoke();
		}

		private void OnTriggerExit(Collider other)
		{
			if (onTriggerExit != null && tagList.Contains (other.tag))
				onTriggerExit.Invoke();
		}
	}
}
