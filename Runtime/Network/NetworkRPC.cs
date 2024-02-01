using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ivyyy.Network
{
	class NetworkRPC
	{
		public static Queue <NetworkRPC> outgoingRpcStack = new Queue<NetworkRPC>();
		public static Queue <NetworkRPC> pendingRpcStack = new Queue<NetworkRPC>();
		public static Queue <NetworkRPC> fallbackRpcStack = new Queue<NetworkRPC>();
		public string guid;
		public string methodName;
		public byte[] data;

		public byte[] GetSerializedData()
		{
			NetworkPackage networkPackage = new NetworkPackage();
			networkPackage.AddValue (new NetworkPackageValue (guid));
			networkPackage.AddValue (new NetworkPackageValue (methodName));

			if (data != null)
				networkPackage.AddValue (new NetworkPackageValue (data));

			return networkPackage.GetSerializedData();
		}

		public static void AddOutgoingPendingRPC (string _guid, string _methodName, byte[] data)
		{
			NetworkRPC rpc = new NetworkRPC();
			rpc.guid = _guid;
			rpc.methodName = _methodName;
			rpc.data = data;

			outgoingRpcStack.Enqueue (rpc);
		}

		public static void AddFromSerializedData (byte[] data)
		{
			NetworkPackage networkPackage = new NetworkPackage();
			networkPackage.DeserializeData (data);

			NetworkRPC rpc = new NetworkRPC();

			if (networkPackage.Count >= 2)
			{
				rpc.guid = networkPackage.Value (0).GetString();
				rpc.methodName = networkPackage.Value (1).GetString();
			}

			if (networkPackage.Count >= 3)
				rpc.data = networkPackage.Value(2).GetBytes();

			if (rpc.guid != null)
				pendingRpcStack.Enqueue (rpc);
			else
				Debug.LogError ("Invalid RPC Package!");
		}

		public static void ExecutePendingRPC()
		{
			while (pendingRpcStack.Count > 0)
				ExecutePendingRPC (pendingRpcStack.Dequeue());

			while (fallbackRpcStack.Count > 0)
				pendingRpcStack.Enqueue (fallbackRpcStack.Dequeue());
		}

		public static void ExecutePendingRPC (NetworkRPC currentRpc)
		{
			if (NetworkBehaviour.guidMap.ContainsKey (currentRpc.guid))
			{
				NetworkBehaviour networkBehaviour = NetworkBehaviour.guidMap[currentRpc.guid];

				if (!networkBehaviour.Owner && !networkBehaviour.ExecuteRPCCall (currentRpc.methodName, currentRpc.data))
					Debug.LogError ("Invalid RPC Method! " + currentRpc.guid + " " + currentRpc.methodName);
			}
			else if (currentRpc.guid != null)
			{
				Debug.Log ("Stashed RPC for later");
				NetworkRPC.fallbackRpcStack.Enqueue (currentRpc);
			}
			else
			{
				Debug.LogError ("Invalid RPC Method! " + currentRpc.guid + " " + currentRpc.methodName);
			}
		}
	}
}
