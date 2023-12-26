﻿using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Ivyyy.Network
{
	class NetworkClientThread : NetworkWorkerThread
	{
		IPEndPoint serverEndPoint = null;
		IPEndPoint localEndPoint = null;
		UdpClient udpClient = null;
		Socket tcpSocket = null;
		NetworkPackage networkPackage = new NetworkPackage();

		public bool Connected {get; private set; }

		public NetworkClientThread (Socket socket)
		{
			tcpSocket = socket;
			serverEndPoint = (IPEndPoint) socket.RemoteEndPoint;
			localEndPoint = (IPEndPoint) socket.LocalEndPoint;
			udpClient = new UdpClient(localEndPoint.Port);
			Connected = true;
		}

		protected override void ReceiveData()
		{
			try
			{
				while (!shutdown)
				{
					//UDP Packages
					if (udpClient.Available > 0)
					{
						byte[] data = udpClient.Receive (ref serverEndPoint);
						networkPackage.DeserializeData (data);

						//For each Value in networkPackage
						for (int i = 0; i < networkPackage.Count; ++i)
							NetworkManagerState.SetNetObjectFromValue (networkPackage.Value(i));
					}

					//TCP Packages
					if (tcpSocket.Available > 0)
					{
						byte[] buffer = new byte[4096];
						int byteReceived = tcpSocket.Receive (buffer);
						byte[] data = new byte [byteReceived];
						Buffer.BlockCopy (buffer, 0, data, 0, byteReceived);
						networkPackage.DeserializeData (data);

						for (int i = 0; i < networkPackage.Count; ++i)
							NetworkRPC.AddFromSerializedData (networkPackage.Value(i).GetBytes());
					}
				}
			}
			catch (Exception e)
			{
				Debug.Log (e);
			}

			Connected = false;
		}

		public override bool SendUDPData (byte[] data)
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

		public bool SendTCPData (byte[] data)
		{
			int length = data.Length;

			try
			{
				int byteSend = tcpSocket.Send (data);
				return length == byteSend;
			}
			catch (Exception e)
			{
				Debug.Log (e);
				return false;
			}
		}
	}
}