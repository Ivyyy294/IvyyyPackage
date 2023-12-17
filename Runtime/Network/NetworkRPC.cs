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
		public static Stack <NetworkRPC> outgoingRpcStack;
		public static Stack <NetworkRPC> pendingRpcStack;
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
			{
				NetworkRPC currentRpc = pendingRpcStack.Pop();

				if (NetworkBehaviour.guidMap.ContainsKey (currentRpc.guid))
				{
					NetworkBehaviour networkBehaviour = NetworkBehaviour.guidMap[currentRpc.guid];

					if (!networkBehaviour.ExecuteRPCCall (currentRpc.methodName))
						Debug.LogError ("Invalid RPC Method!");
				}
				else
					Debug.LogError ("Invalid RPC GUID!");
			}
		}
	}
}
