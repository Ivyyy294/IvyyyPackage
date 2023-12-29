﻿using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Ivyyy.Network
{
	class NetworkClientThread : NetworkWorkerThread
	{
		public enum ConnectionStatus
		{
			DISCONNECTED,
			CONNECTED,
			TIME_OUT
		}

		IPEndPoint serverEndPoint = null;
		IPEndPoint localEndPoint = null;
		UdpClient udpClient = null;
		Socket tcpSocket = null;
		NetworkPackage networkPackage = new NetworkPackage();
		long lastPackageTimestamp;

		public ConnectionStatus Status {get; private set; }
		public Socket TcpSocket {get{return tcpSocket; } }

		public NetworkClientThread (Socket socket)
		{
			tcpSocket = socket;
			serverEndPoint = (IPEndPoint) socket.RemoteEndPoint;
			localEndPoint = (IPEndPoint) socket.LocalEndPoint;
			udpClient = new UdpClient(localEndPoint.Port);
			Status = socket.Connected ? ConnectionStatus.CONNECTED : ConnectionStatus.DISCONNECTED;
			lastPackageTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
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
						lastPackageTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

						byte[] data = udpClient.Receive (ref serverEndPoint);
						networkPackage.DeserializeData (data);

						//For each Value in networkPackage
						for (int i = 0; i < networkPackage.Count; ++i)
							NetworkManagerState.SetNetObjectFromValue (networkPackage.Value(i));
					}

					//TCP Packages
					if (tcpSocket.Available > 0)
					{
						lastPackageTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

						byte[] buffer = new byte[4096];
						int byteReceived = tcpSocket.Receive (buffer);
						byte[] data = new byte [byteReceived];
						Buffer.BlockCopy (buffer, 0, data, 0, byteReceived);
						networkPackage.DeserializeData (data);

						for (int i = 0; i < networkPackage.Count; ++i)
							NetworkRPC.AddFromSerializedData (networkPackage.Value(i).GetBytes());
					}

					if (Ping() >= NetworkManager.Me.Timeout)
					{
						Status = ConnectionStatus.TIME_OUT;
						break;
					}
				}
			}
			catch (Exception e)
			{
				Debug.Log (e);
				Status = ConnectionStatus.DISCONNECTED;
			}

			CloseSocket();
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

		public long Ping()
		{
			return DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastPackageTimestamp;
		}

		private void CloseSocket ()
		{
			if (tcpSocket != null)
			{
				if (tcpSocket.Connected) 
					tcpSocket.Shutdown(SocketShutdown.Both);

				tcpSocket.Close();
				tcpSocket.Dispose();
			}

			if (udpClient != null)
			{
				udpClient.Close();
				udpClient.Dispose();
			}

			Debug.Log ("Sockets closed!");
		}
	}

}