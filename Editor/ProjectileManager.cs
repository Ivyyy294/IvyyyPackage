using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	public class ProjectileManager : MonoBehaviour
	{
		protected float timerFire = 0f;
		protected float timerReload = 0f;
		//float timerFireSalvo = 0f;
		uint pFired = 0;
		List <GameObject> pooledObjects;
		[SerializeField] protected uint magSize;

		[Header ("Fire settings")]
		[SerializeField] GameObject projectile;
		[SerializeField] protected KeyCode fireKey = KeyCode.None;
		[SerializeField] protected bool isMouseKey = false;
		[SerializeField] protected int fireMouseKey = 0;
		[SerializeField] float coolDownLaser = 0f;
		[SerializeField] GameObject parentTransform = null;
		[SerializeField] protected uint salvoSize = 1;
		[Space]
		[Header ("Reload settings")]
		[SerializeField] protected KeyCode reloadKey = KeyCode.None;
		[SerializeField] protected bool onKeyDown = true;
		[SerializeField] protected float reloadTime = 0f;

		//[SerializeField] uint salvoSize = 1;
		//[SerializeField] float coolDownSalvo = 0f;

		protected virtual void Start()
		{
			pooledObjects = new List <GameObject>();
			GameObject tmp;

			for (uint i = 0; i < magSize; ++i)
			{
				//if (parentTransform == null)
				tmp = Instantiate (projectile);
				//else
					//tmp = Instantiate (projectile, parentTransform.transform);

				if (tmp != null)
				{
					tmp.SetActive (false);
					pooledObjects.Add (tmp);
				}
			}

			timerFire = coolDownLaser;
			pFired = 0;
		}

		public void ShootAtPos (Vector3 targetPos)
		{
			ShootAtPos (gameObject.transform.position, targetPos);
		}

		public void ShootAtPos (Vector3 spawnPos, Vector3 targetPos)
		{
			if (timerFire >= coolDownLaser && pFired < salvoSize)
			{
				GameObject obj = GetPooledObject();

				if (obj != null)
				{
					Projectile p = obj.GetComponent <Projectile> ();

					if (p != null)
					{
						Line tmp = new Line (spawnPos, targetPos);
						p.trajectory.Angle = tmp.Angle;
					}

					obj.transform.position = spawnPos;
					obj.transform.rotation = gameObject.transform.rotation;
					obj.SetActive (true);
					pFired++;
					Ivyyy.AudioHandler.Me.PlayOneShot ("Projectile");
				}
				
				timerFire = 0f;
			}
		}

		GameObject GetPooledObject()
		{
			for (int i = 0; i < pooledObjects.Count; ++i)
			{
				if (!pooledObjects[i].activeInHierarchy)
					return pooledObjects[i];
			}
			
			return null;
		}

		protected virtual Vector3 GetTargetPos ()
		{
			return transform.position + new Vector3 (0f, 1f);
		}

		protected virtual void Update()
		{
			if (onKeyDown && Input.GetKeyDown(reloadKey)
			|| !onKeyDown && Input.GetKeyUp(reloadKey))
				Reload();
			else if (Input.GetKeyDown (fireKey)
			|| (isMouseKey && Input.GetMouseButton (fireMouseKey)))
				ShootAtPos (GetTargetPos());
			
			timerFire += Time.deltaTime;
			timerReload += Time.deltaTime;
		}

		protected void Reload ()
		{
			Reload (pFired);
		}

		protected void Reload (uint anz)
		{
			if (timerReload >= reloadTime)
			{
				if (pFired >= anz)
					pFired -= anz;
				else
					pFired = 0;

				timerReload = 0;
			}
		}

		public uint GetRoundsLeft ()
		{
			return salvoSize - pFired;
		}
	}
}
