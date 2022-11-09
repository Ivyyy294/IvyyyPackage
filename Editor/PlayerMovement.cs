using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	// PlayerScript requires the GameObject to have a Rigidbody component
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerMovement : MonoBehaviour
	{
		protected Rigidbody m_Rigidbody;
		protected Line trajectory = new Line();
		[SerializeField] float maxSpeed = 0f;
		[SerializeField] float acceleration = 0f;
		[SerializeField] float deacceleration = 0.9f;

		//Start is called before the first frame update
		void Start()
		{
			m_Rigidbody = gameObject.GetComponent <Rigidbody>();
		}

		//Update is called once per frame
		protected virtual void Update()
		{
			MovePlayer();
		}

		protected virtual void FixedUpdate()
		{
			if (m_Rigidbody != null)
			{
				m_Rigidbody.velocity = new Vector3 (0f, 0f);
				m_Rigidbody.MovePosition (transform.position + trajectory.P2 * Time.deltaTime);

				//Deacceleration
				if (trajectory.Length > 0f)
				{
					if (trajectory.Length > deacceleration)
						trajectory.Length -= deacceleration;
					else
						trajectory.Length = 0f;
				}
			}
		}

		void MovePlayer()
		{
			Vector3 tmpP = trajectory.P2;

			if (Input.GetKey(KeyCode.D))
				tmpP.x += acceleration;

			if (Input.GetKey(KeyCode.A))
				tmpP.x -= acceleration;

			if (Input.GetKey(KeyCode.W))
				tmpP.y += acceleration;

			if (Input.GetKey(KeyCode.S))
				tmpP.y -= acceleration;

			trajectory.P2 = tmpP;

			if (maxSpeed > 0f && trajectory.Length > maxSpeed)
				trajectory.Length = maxSpeed;
		}
	}
}
