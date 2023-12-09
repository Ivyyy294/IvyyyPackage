using System;
using System.Net.Sockets;

namespace Ivyyy.Network
{
	public abstract class NetworkManagerState
	{
		//Public Values
		public abstract bool Start();
		public abstract void Update();
		public abstract void ShutDown();

		//Protected Values
		protected const int idOffset = sizeof (int);
		protected NetworkPackage networkPackage = new NetworkPackage();

		//Public Methods
		//Reconstructs a NetworkObject from the given NetworkPackageValue
		public static  void SetNetObjectFromValue (NetworkPackageValue netValue)
		{
			byte[] payload = netValue.GetBytes();
			int id = BitConverter.ToInt32 (payload, 0);

			if (id < NetworkManager.Me.NetworkObjects.Count)
			{
				NetworkObject netObject = NetworkManager.Me.NetworkObjects[id];

				//Only Update not owned objects
				if (!netObject.Owner)
				{
					byte[] data = new byte[payload.Length - idOffset];
					Array.Copy (payload, idOffset, data, 0, data.Length);

					netObject.DeserializeData (data);
				}
			}
		}

		//Protected Methods
		protected void CloseSocket (Socket socket)
		{
			if (socket != null)
			{
				if (socket.Connected) 
					socket.Shutdown(SocketShutdown.Both);

				socket.Close();
				socket.Dispose();
				socket = null;
			}
		}

		//encapsules the given NetworkObject with the given index into a NetworkPackageValue
		protected NetworkPackageValue GetNetObjectAsValue (int index, NetworkObject netObject)
		{
			//Add NetworkObject id to payload
			byte[] objectData = netObject.GetSerializedData();
			byte[] id = BitConverter.GetBytes (index);
			byte[] payload = new byte [idOffset + objectData.Length];

			Array.Copy (id, 0, payload, 0, idOffset);
			Array.Copy (objectData, 0, payload, idOffset, objectData.Length);

			return new NetworkPackageValue (payload);
		}
	}
}
