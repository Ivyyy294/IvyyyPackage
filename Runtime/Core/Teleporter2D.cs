using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.Core
{
	[RequireComponent (typeof (BoxCollider2D))]
	public class Teleporter2D : MonoBehaviour
	{
		//Editor Values
		[SerializeField] Transform destination;

		[Header ("Lara Values")]
		[SerializeField] List <string> targetTags;
	
		//Private Values
		private BoxCollider2D triggerCollider;
		// Start is called before the first frame update
		void Start()
		{
			triggerCollider = GetComponent <BoxCollider2D>();
			triggerCollider.isTrigger = true;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (targetTags.Contains (collision.tag))
				collision.transform.position = destination.position;
		}
	}
}

