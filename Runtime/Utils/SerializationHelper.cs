using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ivyyy.Utils
{
	public class SerializationHelper
	{
		public static byte[] Vector3ToBytes (Vector3 data)
		{
			byte[] buffer = new byte [sizeof (float) * 3];
			Buffer.BlockCopy (BitConverter.GetBytes(data.x), 0, buffer, 0, 4);
			Buffer.BlockCopy (BitConverter.GetBytes(data.y), 0, buffer, 4, 4);
			Buffer.BlockCopy (BitConverter.GetBytes(data.z), 0, buffer, 8, 4);
			return buffer;
		}

		public static Vector3 BytesToVector3 (byte[] data)
		{
			float x = BitConverter.ToSingle (data, 0);
			float y = BitConverter.ToSingle (data, 4);
			float z = BitConverter.ToSingle (data, 8);
			return new Vector3 (x, y, z);
		}
	}
}
