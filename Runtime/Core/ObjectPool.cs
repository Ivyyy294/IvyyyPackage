using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.Core
{
	public class ObjectPoolAutoSpawn : ObjectPool
	{
		protected float timer = 0f;
		[SerializeField] protected float spawnCoolDown = 1f;
		[SerializeField] protected Vector3 spawnPosition;

		protected override void Awake()
		{
			timer = spawnCoolDown;
			base.Awake();
		}
		protected virtual Vector3 GetSpawnPosition()	{ return spawnPosition;}
		protected virtual bool CheckSpawnCondition() { return timer >= spawnCoolDown;}

		protected void Update()
		{
			if (CheckSpawnCondition())
			{
				ActivateObject (GetSpawnPosition());
				timer = 0f;
			}
			else
				timer += Time.deltaTime;
		}
	}

	public class ObjectPool : MonoBehaviour
	{
		//Editor Values
		protected List <GameObject> pooledObjects;
		[SerializeField] protected GameObject objectToPool;
		[SerializeField] protected uint anz = 1;
		[SerializeField] uint autoResizeAmount = 0;

		protected virtual void Awake()
		{
			Spawn();
		}

		protected void Spawn ()
		{
			pooledObjects = new List <GameObject>();

			Spawn (anz);
		}

		public void ActivateObject (Vector3 pos)
		{
			GameObject tmp = GetPooledObject();

			if (tmp != null)
			{
				tmp.transform.position = pos;
				tmp.SetActive (true);
			}
			else
				Debug.Log ("No free object in pool!");
		}

		public GameObject GetPooledObject()
		{
			for (int i = 0; i < pooledObjects.Count; ++i)
			{
				if (!pooledObjects[i].activeInHierarchy)
					return pooledObjects[i];
			}
			
			if (autoResizeAmount > 0)
			{
				Debug.Log ("Resizing object pool!");
				Spawn (autoResizeAmount);
				
				//starting with the new spawned objects
				for (int i = pooledObjects.Count - 1; i >= 0; --i)
				{
					if (!pooledObjects[i].activeInHierarchy)
						return pooledObjects[i];
				}
			}

			return null;
		}

		//Private Functions
		void Spawn (uint anz)
		{
			GameObject tmp;

			for (int i = 0; i < anz; ++i)
			{
				tmp = Instantiate (objectToPool, gameObject.transform);
				tmp.SetActive (false);
				pooledObjects.Add (tmp);
			}
		}
	}
}
