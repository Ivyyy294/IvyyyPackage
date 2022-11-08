using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	public class Projectile : MonoBehaviour
	{
		[SerializeField] float speed = 0f;
		[SerializeField] float maxRange = 0f;
		[SerializeField] List <string> TagWhiteList;
		public Line trajectory = new Line();
		Line path = new Line ();

		void Start()
		{
			trajectory.Length = speed;
		}

		private void OnEnable()
		{
			path.SetP1 (transform.position);
		}

		private void OnTriggerEnter(Collider other)
		{
			foreach (string element in TagWhiteList)
			{
				if (element == other.tag)
					return;
			}

			gameObject.SetActive (false);
		}

		// Update is called once per frame
		void Update()
		{
			transform.position += trajectory.P2 * Time.deltaTime;
			path.P2 = transform.position;

 			if (maxRange > 0f && path.Length > maxRange)
				gameObject.SetActive (false);
		}
	}
}
