using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.Network
{
	public abstract class NetworkObject : MonoBehaviour
	{
		public NetworkObject()
		{
			networkPackage = backBuffer1;
		}

		public bool Owner {get; set;}

		public byte[] GetSerializedData()
		{
			//Clear Package
			networkPackage.Clear();

			//Call abstract SetPackageData
			SetPackageData();

			//Return Serialized Data
			return networkPackage.GetSerializedData();
		}
		public bool DeserializeData (byte[] rawData)
		{
			NetworkPackage backBuffer = GetBackBuffer();
			bool ok = backBuffer.DeserializeData(rawData);
			SwapBuffer();
			return ok;
		}

		protected abstract void SetPackageData();
		protected bool Host {get {return NetworkManager.Me.Host;}}

		protected NetworkPackage networkPackage;

		//Private values
		//Double buffer is probably unessescary at this point and a relict of iterations
		private NetworkPackage backBuffer1 = new NetworkPackage();
		private NetworkPackage backBuffer2 = new NetworkPackage();

		private void SwapBuffer()
		{
			networkPackage = GetBackBuffer();
		}

		NetworkPackage GetBackBuffer()
		{
			if (networkPackage == backBuffer1)
				return backBuffer2;
			else
				return backBuffer1;
		}
	}
}
