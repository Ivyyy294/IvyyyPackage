using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.Utils
{
	public class Line
	{
	//Public
		public Line ()
		{
			p1 = new Vector3 (0, 0);
			p2 = new Vector3 (0, 0);
			angle = 90f * Mathf.Deg2Rad;
		}
		public Line (Vector3 _p1, Vector3 _p2)
		{
			p1 = _p1;
			p2 = _p2;
			CalculateLength();

			if (p1 != p2)
				CalculateAngle();
			else
				angle = 90f * Mathf.Deg2Rad;

		}

		public Vector3 P1 { get { return p1;} set {SetP1 (value);}}
		public Vector3 P2 { get { return p2;} set {SetP2(value);} }
		//Angle in Deg
		public float Angle { get { return angle * Mathf.Rad2Deg;} set {SetAngle (value);}}
		public float Length { get { return length;} set {SetLength (value);}}


	//Private;
		//Start Point
		private Vector3 p1;
		//End Point
		private Vector3 p2;
		//Angle between P1 and P2
		private float angle;
		private float length;

		//Recalculates P2
		void SetLength (float l)
		{
			length = l;
			CalculateP2();
		}

		//Recalculates length, angle
		public void SetP1 (Vector3 p)
		{
			p1 = p;
			CalculateLength();
			CalculateAngle();
		}

		//Recalculates length, angle
		void SetP2 (Vector3 p)
		{
			p2 = p;
			CalculateLength();
			CalculateAngle();
		}

		//Angle in Deg, recalculates P2
		void SetAngle (float _angle)
		{
			angle = _angle * Mathf.Deg2Rad;
			CalculateP2();
		}

		private void CalculateP2()
		{
			p2.y = p1.y + length * Mathf.Sin (angle);
			p2.x = p1.x + length * Mathf.Cos (angle);
		}

		private void CalculateAngle ()
		{
			float xDis = p2.x - p1.x;
			float yDis = p2.y - p1.y;

			float tmp3 = Mathf.Atan2 (yDis, xDis);
			angle = tmp3;
		}

		private void CalculateLength()
		{
			float a = Mathf.Abs (p1.x - p2.x);
			float b = Mathf.Abs (p1.y - p2.y);
			length = Mathf.Sqrt (Mathf.Pow (a, 2) + Mathf.Pow (b, 2));
		}
	}
}
