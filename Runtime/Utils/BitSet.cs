using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivyyy.Utils
{
	public class BitSet
	{
		private byte[] data;

		public BitSet (int byteSize)
		{
			data = new byte[byteSize];
		}

		public void SetBit (int bitNr, bool val)
		{
			int byteIndex = bitNr / 8;
			bitNr = bitNr % 8;			
			byte code = (byte) (1 << bitNr);

			if (val)
				data[byteIndex] = (byte) (data[byteIndex] | code);
			else if ((data[byteIndex] & code) == code)
				data[byteIndex] = (byte) (data[byteIndex] ^ code);
		}

		public bool Check (int bitNr)
		{
			int byteIndex = bitNr / 8;
			bitNr = bitNr % 8;			
			byte code = (byte) (1 << bitNr);

			return (data[byteIndex] & code) == code;
		}

		public byte[] GetRawData()
		{
			return data;
		}

		public void SetRawData (byte[] _data)
		{
			data = _data;
		}
	}
}
