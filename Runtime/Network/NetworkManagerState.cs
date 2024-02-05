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
			tcpSockets = new List<Socket>();
			tcpReceiveTask = new List<Task>();
		}

		//Protected Values
		protected const int idOffset = 16; //size of guid
		protected NetworkPackage networkPackage = new NetworkPackage();
		protected bool shutDown = false;
		
		//UDP
		protected Task udpReceiveTask;
		protected List <IPEndPoint> udpEndPoints;
		private UdpClient udpSendSocket;

		//TCP
		private List<Socket> tcpSockets;
		private List <Task> tcpReceiveTask;

		protected void AddTcpSocket (Socket tcpSocket)
		{
			if (tcpSocket != null)
			{
				tcpSockets.Add (tcpSocket);
				tcpReceiveTask.Add(Task.Run(()=>{TCPReceive(tcpSocket);}));
			}
			else
				Debug.LogError("Invalid tcpSocket!");
		}

		//Public Methods
		public abstract bool Start();
		public abstract void Update();

		public virtual void ShutDown()
		{
			shutDown = true;

			if (udpReceiveTask != null)
			{
				udpReceiveTask.Wait();
				Debug.Log ("Waiting for udpReceiveTask to exit...");
			}

			Debug.Log ("waiting for udpReceiveTasks to exit");
			foreach (Task i in tcpReceiveTask)
				i.Wait();

			Debug.Log("ShutDown done!");
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
			Debug.Log ("close socket");
			if (socket != null)
			{
				if (socket.Connected) 
					socket.Shutdown(SocketShutdown.Both);

				socket.Close();
				socket.Dispose();
				socket = null;
			}
		}

		//UDP
		protected bool SendUDPData (byte[] data)
		{
			try
			{
				byte[] buffer = AddSizeHeaderToData(data);

				foreach (IPEndPoint i in udpEndPoints)
					udpSendSocket.Send (buffer, buffer.Length, i);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}

			return false;
		}

		protected void UDPReceive (int port)
		{
			NetworkPackage buffer = new NetworkPackage();
			UdpClient udpClient = new UdpClient();
			udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
			IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
			byte[] sizeBuffer = new byte[sizeof(int)];

			while (!shutDown)
			{
				//UDP Packages
				if (udpClient.Available > 0)
				{
					int index = 0;
					//Get all stored data from socket
					byte[] data = udpClient.Receive (ref remoteIpEndPoint);

					while (index < data.Length)
					{
						//Get package size
						int packageSize = BitConverter.ToInt32 (data, index);
						index += sizeBuffer.Length;

						//Get package data
						byte[] byteBuffer = new byte[packageSize];
						Buffer.BlockCopy (data, index, byteBuffer, 0, packageSize);
						index += packageSize;

						buffer.DeserializeData (byteBuffer);

						//For each Value in networkPackage
						for (int i = 0; i < buffer.Count; ++i)
							SetNetObjectFromValue (buffer.Value(i));
					}
				}
			}

			udpClient.Close();
		}

		//TCP
		protected bool SendTCPData (byte[] data)
		{
			byte[] buffer = AddSizeHeaderToData(data);

			foreach (Socket i in tcpSockets)
			{
				try
				{
					i.Send (buffer);
				}
				catch (Exception e)
				{
					Debug.LogError (e);
				}
			}

			return false;
		}

		protected void TCPReceive (Socket socket)
		{
			//float lastPackageTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			byte[] sizeBuffer = new byte[sizeof(int)];

			while (!shutDown && socket.Connected)
			{
				try
				{
					//TCP Packages
					if (socket.Available > 0)
					{
						//Get package size
						socket.Receive (sizeBuffer);
						int packageSize = BitConverter.ToInt32 (sizeBuffer, 0);

						while (socket.Available < packageSize)
							Debug.Log ("TCPReceive: Waiting for missing package data...");

						//Get package data
						//lastPackageTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
						Debug.Log("TCPReceive: Package data complete!");

						byte[] buffer = new byte[packageSize];
						int bytesReceived = socket.Receive (buffer);

						networkPackage.DeserializeData (buffer);

						for (int i = 0; i < networkPackage.Count; ++i)
							NetworkRPC.AddFromSerializedData (networkPackage.Value(i).GetBytes());
					}
				}
				catch (Exception e)
				{
					Debug.LogError (e);
				}
			}

			//Remove Socket from list
			CloseSocket (socket);
		}

		protected void CheckTcpSocketStatus()
		{
			Queue <Socket> disconnectedSockets = new Queue<Socket>();
			int socketCount = tcpSockets.Count;

			for (int i = 0; i < socketCount; ++i)
			{
				Socket socket = tcpSockets[i];

				if (socket.Available == 0)
				{
					try
					{
						bool disconnected = socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0;

						if (disconnected)
						{
							Debug.LogError ("Socket timed out!");
							disconnectedSockets.Enqueue (socket);
						}
					}
					catch (Exception e)
					{
						Debug.LogError (e);
						disconnectedSockets.Enqueue (socket);
					}
				}
			}

			while (disconnectedSockets.Count > 0)
			{
				Socket socket = disconnectedSockets.Dequeue();
				RemoveClient (socket);
			}
		}

		private void RemoveClient(Socket socket)
		{
			Debug.Log ("Client disconnected!");

			if (NetworkManager.Me.Host && NetworkManager.Me.onClientDisonnected != null)
				NetworkManager.Me.onClientDisonnected(socket);
			else if (!NetworkManager.Me.Host && NetworkManager.Me.onHostDisonnected != null)
				NetworkManager.Me.onHostDisonnected(socket);

			Debug.Log("Remove udpEndPoint");
			udpEndPoints.Remove ((IPEndPoint)socket.RemoteEndPoint);
			Debug.Log("Remove tcpSocket");
			tcpSockets.Remove (socket);
			CloseSocket (socket);
		}

		private byte[] AddSizeHeaderToData (byte[] data)
		{
			byte[] buffer = new byte[sizeof (int) + data.Length];
			Buffer.BlockCopy (BitConverter.GetBytes(data.Length), 0, buffer, 0, sizeof (int));
			Buffer.BlockCopy (data, 0, buffer, sizeof (int), data.Length);
			return buffer;
		}
	}
}
