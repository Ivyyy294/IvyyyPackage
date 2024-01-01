using System;
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
		UdpClient udpClient = null;
		Socket tcpSocket = null;
		NetworkPackage networkPackage = new NetworkPackage();
		long lastPackageTimestamp;

		public ConnectionStatus Status {get; private set; }
		public Socket TcpSocket {get{return tcpSocket; } }

		public NetworkClientThread (Socket socket, int udpPortLocal, int updPortRemote)
		{
			Debug.Log("udpPortLocal: " + udpPortLocal + " updPortRemote: " + updPortRemote);

			tcpSocket = socket;
			serverEndPoint = (IPEndPoint) socket.RemoteEndPoint;
			serverEndPoint.Port = updPortRemote;
			udpClient = new UdpClient(udpPortLocal);
			Status = socket.Connected ? ConnectionStatus.CONNECTED : ConnectionStatus.DISCONNECTED;
			lastPackageTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		}

		protected override void ReceiveData()
		{
			try
			{
				IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

				while (!shutdown)
				{
					//UDP Packages
					if (udpClient.Available > 0)
					{
						lastPackageTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

						byte[] data = udpClient.Receive (ref remoteIpEndPoint);
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

					if (TimeOut())
					{
						Status = ConnectionStatus.TIME_OUT;
						Debug.Log ("Time out!");
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

		private bool TimeOut()
		{
			if (NetworkManager.Me.Timeout <= 0)
				return false;
			
			return Ping() > NetworkManager.Me.Timeout;
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