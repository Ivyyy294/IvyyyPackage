using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Ivyyy.Network
{
	class NetworkClientThread : NetworkWorkerThread
	{
		//Public Values
		public enum ConnectionStatus
		{
			DISCONNECTED,
			CONNECTED,
			TIME_OUT
		}

		public struct ConnectionData
		{
			public bool accepted;
			public Socket socket;
			public int localUDPPort;
			public int remoteUDPPort;
		}

		//Private Values
		IPEndPoint remoteEndPoint = null;
		IPEndPoint localEndPoint = null;
		UdpClient udpClient = null;
		Socket tcpSocket = null;
		NetworkPackage networkPackage = new NetworkPackage();
		long lastPackageTimestamp;

		//Public Methods
		public ConnectionStatus Status {get; private set; }
		public Socket TcpSocket {get{return tcpSocket; } }

		public NetworkClientThread (ConnectionData connectionData)
		{
			tcpSocket = connectionData.socket;
			remoteEndPoint = (IPEndPoint) tcpSocket.RemoteEndPoint;
			localEndPoint = (IPEndPoint) tcpSocket.LocalEndPoint;
			udpClient = new UdpClient(localEndPoint.Port);
			Status = tcpSocket.Connected ? ConnectionStatus.CONNECTED : ConnectionStatus.DISCONNECTED;
			lastPackageTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		}

		public override bool SendUDPData (byte[] data)
		{
			int length = data.Length;

			try
			{
				int byteSend = udpClient.Send (data, data.Length, remoteEndPoint);
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

		//Protected Methods
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

		//Private Methods
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