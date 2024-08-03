using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Ivyyy
{
	public class SerializedPackage : ISerializable
	{
		private List<SerializedPackageValue> valueList = new List<SerializedPackageValue>();

		//Public Values

		public void Clear() { valueList.Clear(); }

		//Returns the count of NetworkPackageValues
		public int Count { get { return valueList.Count; } }
		public bool Available { get { return valueList.Count > 0; } }

		//Sets the value of an existing NetworkPackageValue

		public void AddValue(SerializedPackageValue val)
		{
			valueList.Add(val);
		}

		public void AddValue(bool val)
		{
			valueList.Add(new SerializedPackageValue(val));
		}

		public void AddValue(short val)
		{
			valueList.Add(new SerializedPackageValue(val));
		}

		public void AddValue(ushort val)
		{
			valueList.Add(new SerializedPackageValue(val));
		}

		public void AddValue(int val)
		{
			valueList.Add(new SerializedPackageValue(val));
		}

		public void AddValue(uint val)
		{
			valueList.Add(new SerializedPackageValue(val));
		}

		public void AddValue(float val)
		{
			valueList.Add(new SerializedPackageValue(val));
		}

		public void AddValue(char val)
		{
			valueList.Add(new SerializedPackageValue(val));
		}

		public void AddValue(string val)
		{
			valueList.Add(new SerializedPackageValue(val));
		}

		public void AddValue(byte val)
		{
			valueList.Add(new SerializedPackageValue(val));
		}

		public void AddValue(byte[] val)
		{
			valueList.Add(new SerializedPackageValue(val));
		}

		public void AddValue(Vector3 val)
		{
			valueList.Add(new SerializedPackageValue(val));
		}

		public void AddValue(Quaternion val)
		{
			valueList.Add(new SerializedPackageValue(val));
		}

		//Returns the value for the given index
		public SerializedPackageValue Value(int index)
		{
			if (index < valueList.Count)
				return valueList[index];
			else
			{
				Debug.LogError("Invalid NetworkPackageValue index!");
				return default(SerializedPackageValue);
			}
		}

		//Returns the sum of the size of the contents in bytes
		public int Size()
		{
			int size = 0;

			foreach (var i in valueList)
			{
				if (i != null)
					size += i.Size();
			}

			return size;
		}

		public string GetSerializedDataAsString() { return Encoding.BigEndianUnicode.GetString(GetSerializedData()); }
		//public string GetSerializedDataAsString() { return Encoding.Unicode.GetString(GetSerializedData());}

		public byte[] GetSerializedData()
		{
			//Init byte array with package size
			byte[] value = new byte[Size()];

			int index = 0;

			foreach (var i in valueList)
			{
				if (i != null)
				{
					int size = i.Size();

					//copy content bytes to the correct position in value byte array
					Buffer.BlockCopy(i.GetSerializedData(), 0, value, index, size);

					//Adding current size to index offset to get the memmory position for the next entry 
					index += size;
				}
			}

			return value;
		}

		public bool DeserializeData(string data) { return DeserializeData(Encoding.BigEndianUnicode.GetBytes(data)); }

		public bool DeserializeData(byte[] bytes)
		{
			int index = 0;
			valueList.Clear();

			try
			{
				while (index < bytes.Length)
				{
					//Read value size from memmory
					int size = BitConverter.ToInt32(bytes, index);

					size += SerializedPackageValue.StartIndex;

					//Read value from memmory
					SerializedPackageValue tmp = new SerializedPackageValue();

					if (size > 0)
					{
						byte[] buffer = new byte[size];
						Buffer.BlockCopy(bytes, index, buffer, 0, size);
						tmp.DeserializeData(buffer);
					}

					valueList.Add(tmp);

					//Adding current size to index offset to get the memmory position for the next entry 
					index += size;
				}

				return true;
			}
			catch (Exception excp)
			{
				Debug.Log(excp);
				return false;
			}
		}
	}
}