using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.WeightedRandom
{
	[System.Serializable]
	public class WeightedSpawnManager <T>
	{
		public List <WeightedSpawnObject <T>> spawnObjects = new List <WeightedSpawnObject <T>>();
		private float[] Weights;

		public void Init ()
		{
			Weights = new float [spawnObjects.Count];

			float totalWeight = 0f;

			for (int i = 0; i < spawnObjects.Count; ++i)
			{
				Weights[i] = spawnObjects[i].weight;
				totalWeight += Weights[i];
			}

			for (int i = 0; i < Weights.Length; ++i)
				Weights[i] = Weights[i] / totalWeight;
		}

		public T GetObjectToSpawn ()
		{
			float val = Random.value;

			for (int i = 0; i < Weights.Length; ++i)
			{
				if (val < Weights[i])
					return spawnObjects[i].objectToSpawn;

				val -= Weights[i];
			}

			return default (T);
		}
	}
}
