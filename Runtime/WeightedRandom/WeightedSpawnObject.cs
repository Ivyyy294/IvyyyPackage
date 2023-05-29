using UnityEngine;

namespace Ivyyy.WeightedRandom
{
	[System.Serializable]
	public class WeightedSpawnObject <T>
	{
		public T objectToSpawn;
		[Range (0f, 1f)]
		public float weight;
	}
}
