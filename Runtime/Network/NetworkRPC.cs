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
		public static Stack <NetworkRPC> outgoingRpcStack = new Stack<NetworkRPC>();
		public static Stack <NetworkRPC> pendingRpcStack = new Stack<NetworkRPC>();
		public string guid;
		public string methodName;

		public byte[] GetSerializedData()
		{
			string data = guid + ";" + methodName;
			return Encoding.ASCII.GetBytes (data);
		}

		public static void AddOutgoingPendingRPC (string _guid, string _methodName)
		{
			NetworkRPC rpc = new NetworkRPC();
			rpc.guid = _guid;
			rpc.methodName = _methodName;

			outgoingRpcStack.Push (rpc);
		}

		public static void AddFromSerializedData (byte[] data)
		{
			string[] tmp = Encoding.ASCII.GetString (data, 0, data.Length).Split (";");
			NetworkRPC rpc = new NetworkRPC();

			rpc.guid = tmp[0];
			rpc.methodName = tmp[1];

			pendingRpcStack.Push (rpc);
		}

		public static void ExecutePendingRPC()
		{
			while (pendingRpcStack.Count > 0)
				ExecutePendingRPC (pendingRpcStack.Pop());
		}

		public static void ExecutePendingRPC (NetworkRPC currentRpc)
		{
			if (NetworkBehaviour.guidMap.ContainsKey (currentRpc.guid))
			{
				NetworkBehaviour networkBehaviour = NetworkBehaviour.guidMap[currentRpc.guid];

				if (!networkBehaviour.Owner && !networkBehaviour.ExecuteRPCCall (currentRpc.methodName))
					Debug.LogError ("Invalid RPC Method! " + currentRpc.guid + " " + currentRpc.methodName);
			}
			else
				Debug.LogError ("Invalid RPC Method! " + currentRpc.guid + " " + currentRpc.methodName);
		}
	}
}
