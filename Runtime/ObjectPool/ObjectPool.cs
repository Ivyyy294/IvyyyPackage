using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	public class ObjectPool : MonoBehaviour
	{
        public delegate bool CheckCondition (GameObject obj);
		protected List <GameObject> pooledObjects = new List<GameObject>();

        //Public Methods
		public GameObject GetPooledObject()
		{
			return GetPooledObjectWithCondition (null);
        }

        public GameObject GetPooledObjectWithCondition (CheckCondition condition)
        {
            for (int i = 0; i < pooledObjects.Count; ++i)
            {
                GameObject obj = pooledObjects[i];

                if (!obj.activeInHierarchy && (condition == null || condition (obj)))
                    return obj;
            }

            return null;
        }

        //Private Methods
        protected void Spawn (uint anz, GameObject template, Action<GameObject> initCallback)
		{
            if (pooledObjects == null)
                pooledObjects = new List<GameObject>();

            GameObject tmp;

			for (int i = 0; i < anz; ++i)
			{
				tmp = Instantiate (template, gameObject.transform);

                tmp.AddComponent<PooledObject>();

                if (initCallback != null)
                    initCallback.Invoke (tmp);

                tmp.SetActive (false);
				pooledObjects.Add (tmp);
			}
		}

        protected GameObject Spawn(GameObject template, Action<GameObject> initCallback)
        {
            if (pooledObjects == null)
                pooledObjects = new List<GameObject>();

            GameObject tmp;
            tmp = Instantiate(template, gameObject.transform);

            tmp.AddComponent<PooledObject>();

            if (initCallback != null)
                initCallback.Invoke(tmp);

            tmp.SetActive(false);
            pooledObjects.Add(tmp);

            return tmp;
        }
    }
}
