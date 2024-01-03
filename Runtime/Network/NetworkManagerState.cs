using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Ivyyy.Network
{
	public abstract class NetworkManagerState
	{
		public NetworkManagerState()
		{
			udpSendSocket = new UdpClient();
			udpEndPoints = new List<IPEndPoint>();
		}

		//Protected Values
		protected const int idOffset = 16; //size of guid
		protected NetworkPackage networkPackage = new NetworkPackage();
		protected bool shutDown = false;
		
		//UDP
		protected Task udpReceiveTask;
		protected List <IPEndPoint> udpEndPoints;
		private UdpClient udpSendSocket;

		//Public Methods
		public abstract bool Start();
		public abstract void Update();

		public virtual void ShutDown()
		{
			shutDown = true;
			Debug.Log ("Waiting for udpReceiveTask to exit...");
			udpReceiveTask.Wait();
		}

		//Reconstructs a NetworkObject from the given NetworkPackageValue
		public static  void SetNetObjectFromValue (NetworkPackageValue netValue)
		{
			byte[] payload = netValue.GetBytes();
			byte[] tmp = new byte[16];
			Array.Copy (payload, 0, tmp, 0, idOffset);
			string id = new Guid (tmp).ToString();

			if (NetworkBehaviour.guidMap.ContainsKey (id))
			{
				NetworkBehaviour netObject = NetworkBehaviour.guidMap[id];

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
		//encapsules the given NetworkObject with the given index into a NetworkPackageValue
		protected NetworkPackageValue GetNetObjectAsValue (NetworkBehaviour netObject)
		{
			//Add NetworkObject id to payload
			byte[] objectData = netObject.GetSerializedData();
			byte[] id = new System.Guid (netObject.GUID).ToByteArray();
			byte[] payload = new byte [idOffset + objectData.Length];

			Array.Copy (id, 0, payload, 0, idOffset);
			Array.Copy (objectData, 0, payload, idOffset, objectData.Length);

			return new NetworkPackageValue (payload);
		}

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

		protected bool SendUDPData (byte[] data)
		{
			try
			{
				foreach (IPEndPoint i in udpEndPoints)
					udpSendSocket.Send (data, data.Length, i);
			}
			catch (Exception e)
			{
				Debug.Log (e);
			}

			return false;
		}

		protected void UDPReceive (int port)
		{
			NetworkPackage buffer = new NetworkPackage();
			UdpClient udpClient = new UdpClient(port);
			IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

			while (!shutDown)
			{
				//UDP Packages
				if (udpClient.Available > 0)
				{
					byte[] data = udpClient.Receive (ref remoteIpEndPoint);
					buffer.DeserializeData (data);

					//For each Value in networkPackage
					for (int i = 0; i < buffer.Count; ++i)
						SetNetObjectFromValue (buffer.Value(i));
				}
			}
		}
	}
}
