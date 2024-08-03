using System;
using System.Text;
using UnityEngine;

namespace Ivyyy
{

	public class SerializedPackageValue
	{
		byte[] value;
		public static int StartIndex {get {return sizeof (int);}}

		public int Size () {return (value != null ? value.Length : 0);}
		public bool IsEmpty() {  return Size() == 0;}

		//As Byte[]
		public byte[] GetSerializedData() { return value;}
		public void DeserializeData (byte[] bytes) { value = bytes;}

		public SerializedPackageValue() {}
		public SerializedPackageValue (short val) {SetValue (BitConverter.GetBytes (val));}
		public SerializedPackageValue (ushort val) {SetValue (BitConverter.GetBytes (val));}
		public SerializedPackageValue (int val) {SetValue (BitConverter.GetBytes (val));}
		public SerializedPackageValue (uint val) {SetValue (BitConverter.GetBytes (val));}
		public SerializedPackageValue (float val) {SetValue (BitConverter.GetBytes (val));}
		public SerializedPackageValue (char val) {SetValue (BitConverter.GetBytes (val));}
		public SerializedPackageValue (bool val) {SetValue (BitConverter.GetBytes (val));}
		public SerializedPackageValue (byte val) {SetValue (new byte[1]{val});}
		public SerializedPackageValue (byte[] val) {SetValue (val);}
		public SerializedPackageValue (string val)
		{
			if (val.Length == 0)
				val = "NULL";

			SetValue (Encoding.Unicode.GetBytes (val));
		}
		public SerializedPackageValue (Vector3 val)
		{
			byte[] buffer = new byte [sizeof (float) * 3];
			Buffer.BlockCopy (BitConverter.GetBytes(val.x), 0, buffer, 0, 4);
			Buffer.BlockCopy (BitConverter.GetBytes(val.y), 0, buffer, 4, 4);
			Buffer.BlockCopy (BitConverter.GetBytes(val.z), 0, buffer, 8, 4);
			SetValue (buffer);
		}

		public SerializedPackageValue (Quaternion val)
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
		public byte GetByte() {return value[0];}
		public string GetString()
		{
			string data = Encoding.Unicode.GetString(value, StartIndex, Size() - StartIndex);

			if (data == "NULL")
				data = "";

			return data;
		}
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
			int length = val != null ? val.Length : 0;

			value = new byte[StartIndex + length];
			//Write val size into memory
			Buffer.BlockCopy (BitConverter.GetBytes(length), 0, value, 0, StartIndex);

			//Write cal into memory
			if (length > 0)
				Buffer.BlockCopy (val, 0, value, StartIndex, length);
		}
	}
}


