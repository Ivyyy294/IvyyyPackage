using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.Network
{
	[RequireComponent (typeof(NetworkObject))]
	public abstract class NetworkBehaviour : MonoBehaviour
	{
		[SerializeField] string guid = null;
		public string GUID { get { return guid;} }
		static public Dictionary <string, NetworkBehaviour> guidMap = new Dictionary<string, NetworkBehaviour>();

		public NetworkBehaviour()
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

		public bool IsGuidValid()
		{
			return guid != null && guid.Length > 0;
		}

		public bool IsGuidUnique()
		{
			return !guidMap.ContainsKey (guid) || (guidMap.ContainsKey (guid) && guidMap[guid] == this);
		}

		public void GenerateGuid()
		{
			guid = System.Guid.NewGuid().ToString();
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
