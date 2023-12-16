using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Ivyyy.Network
{
	class NetworkUdpClientThread : NetworkWorkerThread
	{
		IPEndPoint serverEndPoint = null;
		IPEndPoint localEndPoint = null;
		UdpClient udpClient = null;
		NetworkPackage networkPackage = new NetworkPackage();

		public NetworkUdpClientThread (Socket socket)
		{
			serverEndPoint = (IPEndPoint) socket.RemoteEndPoint;
			localEndPoint = (IPEndPoint) socket.LocalEndPoint;
			udpClient = new UdpClient(localEndPoint.Port);
		}

		protected override void ReceiveData()
		{
			while (!shutdown)
			{
				if (udpClient.Available > 0)
				{
					byte[] data = udpClient.Receive (ref serverEndPoint);
					networkPackage.DeserializeData (data);

					//For each Value in networkPackage
					for (int i = 0; i < networkPackage.Count; ++i)
						NetworkManagerState.SetNetObjectFromValue (networkPackage.Value(i));
				}
			}
		}

		public override bool SendData (byte[] data)
		{
			int length = data.Length;

			try
			{
				int byteSend = udpClient.Send (data, data.Length, serverEndPoint);
				return length == byteSend;
			}
			catch (Exception e)
			{
				Debug.Log (e);
			}

			return false;
		}
	}
}