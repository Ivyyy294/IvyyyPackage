using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ivyyy.Network
{
	public class NetworkPackageValue
	{
		byte[] value;
		public static int StartIndex {get {return sizeof (int);}}

		public int Size () {return (value != null ? value.Length : 0);}

		public byte[] GetSerializedData() { return value;}
		public void DeserializeData (byte[] bytes) { value = bytes;}

		public NetworkPackageValue() {}
		public NetworkPackageValue (short val) {SetValue (BitConverter.GetBytes (val));}
		public NetworkPackageValue (ushort val) {SetValue (BitConverter.GetBytes (val));}
		public NetworkPackageValue (int val) {SetValue (BitConverter.GetBytes (val));}
		public NetworkPackageValue (uint val) {SetValue (BitConverter.GetBytes (val));}
		public NetworkPackageValue (float val) {SetValue (BitConverter.GetBytes (val));}
		public NetworkPackageValue (char val) {SetValue (BitConverter.GetBytes (val));}
		public NetworkPackageValue (bool val) {SetValue (BitConverter.GetBytes (val));}
		public NetworkPackageValue (string val) {SetValue (Encoding.ASCII.GetBytes (val));}
		public NetworkPackageValue (byte val) {SetValue (new byte[1]{val});}
		public NetworkPackageValue (byte[] val) {SetValue (val);}
		public NetworkPackageValue (Vector3 val)
		{
			byte[] buffer = new byte [sizeof (float) * 3];
			Buffer.BlockCopy (BitConverter.GetBytes(val.x), 0, buffer, 0, 4);
			Buffer.BlockCopy (BitConverter.GetBytes(val.y), 0, buffer, 4, 4);
			Buffer.BlockCopy (BitConverter.GetBytes(val.z), 0, buffer, 8, 4);
			SetValue (buffer);
		}

		public NetworkPackageValue (Quaternion val)
		{
			byte[] buffer = new byte [sizeof (float) * 4];
			Buffer.BlockCopy (BitConverter.GetBytes(val.x), 0, buffer, 0, 4);
			Buffer.BlockCopy (BitConverter.GetBytes(val.y), 0, buffer, 4, 4);
			Buffer.BlockCopy (BitConverter.GetBytes(val.z), 0, buffer, 8, 4);
			Buffer.BlockCopy (BitConverter.GetBytes(val.w), 0, buffer, 12, 4);
			SetValue (buffer);
		}

		public short GetShort() {return BitConverter.ToInt16 (value, StartIndex);}
		public ushort GetUShort() {return BitConverter.ToUInt16 (value, StartIndex);}
		public int GetInt32() {return BitConverter.ToInt32 (value, StartIndex);}
		public uint GetUInt32() {return BitConverter.ToUInt32 (value, StartIndex);}
		public float GetFloat() {return BitConverter.ToSingle (value, StartIndex);}
		public char GetChar() {return BitConverter.ToChar (value, StartIndex);}
		public bool GetBool() {return BitConverter.ToBoolean (value, StartIndex);}
		public string GetString() {return Encoding.ASCII.GetString (value, StartIndex, Size() - StartIndex);}
		public byte GetByte() {return value[0];}
		public byte[] GetBytes()
		{
			byte[] tmp = new byte[value.Length - StartIndex];
			Array.Copy (value, StartIndex, tmp, 0, tmp.Length);
			return tmp;
		}

		public Vector3 GetVector3()
		{
			float x = BitConverter.ToSingle (value, StartIndex);
			float y = BitConverter.ToSingle (value, StartIndex + 4);
			float z = BitConverter.ToSingle (value, StartIndex + 8);

			return new Vector3 (x, y, z);
		}

		public Quaternion GetQuaternion()
		{
			float x = BitConverter.ToSingle (value, StartIndex);
			float y = BitConverter.ToSingle (value, StartIndex + 4);
			float z = BitConverter.ToSingle (value, StartIndex + 8);
			float w = BitConverter.ToSingle (value, StartIndex + 12);

			return new Quaternion (x, y, z, w);
		}

		//Reserves requiered memory
		private void SetValue (byte[] val)
		{
			value = new byte[StartIndex + val.Length];
			//Write val size into memory
			Buffer.BlockCopy (BitConverter.GetBytes(val.Length), 0, value, 0, StartIndex);
			//Write cal into memory
			Buffer.BlockCopy (val, 0, value, StartIndex, val.Length);
		}
	}
}

