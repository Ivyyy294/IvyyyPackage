using UnityEngine;

namespace Ivyyy.WeightedRandom
{
	class WeightedRandom
	{
		public WeightedRandom () { }
		public WeightedRandom (AnimationCurve _curve)
		{
			SetWeight (_curve);
		}

		public void SetWeight (AnimationCurve _curve)
		{
			curve = _curve;
		}

		public float Value ()
		{
			float val = Random.value;

			if (curve != null)
				val = curve.Evaluate (val);

			return val;
		}

		private AnimationCurve curve;
	}
}
