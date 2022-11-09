using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	public class ObjectPoolAutoSpawn : ObjectPool
	{
		protected float timer = 0f;
		[SerializeField] protected float spawnCoolDown = 1f;
		[SerializeField] protected Vector3 spawnPosition;

		protected override void Start()
		{
			timer = spawnCoolDown;
			base.Start();
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
		protected List <GameObject> pooledObjects;
		[SerializeField] protected GameObject objectToPool;
		[SerializeField] protected uint anz = 1;

		protected virtual void Start()
		{
			Spawn();
		}

		protected void Spawn ()
		{
			pooledObjects = new List <GameObject>();
			GameObject tmp;

			for (uint i = 0; i < anz; ++i)
			{
				tmp = Instantiate (objectToPool, gameObject.transform);
				tmp.SetActive (false);
				pooledObjects.Add (tmp);
			}
		}

		protected void ActivateObject (Vector3 pos)
		{
			GameObject tmp = GetPooledObject();

			if (tmp != null)
			{
				tmp.transform.position = pos;
				tmp.SetActive (true);
			}
		}

		public GameObject GetPooledObject()
		{
			for (int i = 0; i < anz; ++i)
			{
				if (!pooledObjects[i].activeInHierarchy)
					return pooledObjects[i];
			}
			
			return null;
		}
	}
}
