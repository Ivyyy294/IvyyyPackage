using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ivyyy.Network
{
	class NetworkManagerClientState : NetworkManagerState
	{
		private string ip;
		NetworkClientThread clientThread = null;

		public NetworkManagerClientState (string _ip) {ip = _ip;}
		~NetworkManagerClientState()
		{
			ShutDown();
		}

		//Public Methods
		public override bool Start()
		{
			//Create client Socket
			Socket socket = GetClientSocket (ip);

			//Start Receive task
			int port = ((IPEndPoint) socket.LocalEndPoint).Port;
			udpReceiveTask = Task.Run(()=>{UDPReceive(port);});

			if (socket == null)
			{
				Debug.Log ("Unable to connect!");
				return false;
			}

			NetworkClientThread.ConnectionData connectionData = HandShake (socket);

			if (connectionData.accepted)
			{
				Debug.Log("Conntected to Host!");

				//Add server to udpEndPoints
				udpEndPoints.Add ((IPEndPoint)socket.RemoteEndPoint);

				NetworkManager.Me.onConnectedToHost?.Invoke (socket);

				//Start listener thread
				clientThread = new NetworkClientThread (connectionData);
				clientThread.Start();
				return true;
			}
			else
				CloseSocket (socket);

			return false;;
		}

		public override void Update()
		{
			if (clientThread.IsRunning)
			{
				if (clientThread.Status == NetworkClientThread.ConnectionStatus.CONNECTED)
				{
					NetworkRPC.ExecutePendingRPC();
					SendData();
				}
				else
					CheckHostStatus();
			}
		}

		public override void ShutDown()
		{
			base.ShutDown();

			if (clientThread != null)
			{
				clientThread.Shutdown();
				CloseSocket (clientThread.TcpSocket);
			}
		}

		//Private Methods
		NetworkClientThread.ConnectionData HandShake(Socket socket)
		{
			NetworkClientThread.ConnectionData connectionData = new NetworkClientThread.ConnectionData();
			connectionData.socket = socket;

			byte[] buffer = new byte[sizeof(bool) + sizeof (int)];

			//Step1 get accept flag from Server
			socket.Receive (buffer);
			connectionData.accepted = BitConverter.ToBoolean (buffer, 0);

			if (connectionData.accepted)
				Debug.Log("Server accepted!");
			else
			{
				Debug.Log("Server rejected!");
				CloseSocket (socket);
				return connectionData;
			}

			//Step2 get server upd port number
			socket.Receive (buffer);
			connectionData.remoteUDPPort = BitConverter.ToInt32 (buffer, 0);
			Debug.Log("Server UDP Port: " + connectionData.remoteUDPPort);

			//Step3 send client upd port number to server
			connectionData.localUDPPort = ((IPEndPoint) socket.LocalEndPoint).Port;
			socket.Send(BitConverter.GetBytes (connectionData.localUDPPort));
			Debug.Log ("Client UDP Port: " + connectionData.localUDPPort);

			return connectionData;
		}

		void SendData()
		{
			//Reset networkPackage
			networkPackage.Clear();

			//Only Update owned NetworkObject
			foreach (KeyValuePair<string, NetworkBehaviour> entry in NetworkBehaviour.guidMap)
			{
				NetworkBehaviour networkObject = entry.Value;

				if (networkObject.Owner && networkObject.Sync())
					networkPackage.AddValue (GetNetObjectAsValue (networkObject));
			}

			SendUDPData (networkPackage.GetSerializedData());

			//Send TCP Data
			if (NetworkRPC.outgoingRpcStack.Count > 0)
			{
				networkPackage.Clear();

				while (NetworkRPC.outgoingRpcStack.Count > 0)
					networkPackage.AddValue (new NetworkPackageValue (NetworkRPC.outgoingRpcStack.Pop().GetSerializedData()));

				clientThread.SendTCPData (networkPackage.GetSerializedData());
			}
		}

		Socket GetClientSocket (string ip)
		{
			//Create TCP Socket
			Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			//Connect with Host
			IPAddress iPAddress = null;
			try
			{
				//Cast input to IPAddress
				iPAddress = IPAddress.Parse (ip);
				clientSocket.Connect (iPAddress, NetworkManager.Me.Port);
				return clientSocket;
			}
			catch (Exception excp)
			{
				return null;
			}
		}

		void CheckHostStatus()
		{
			if (clientThread.Status == NetworkClientThread.ConnectionStatus.DISCONNECTED)
			{
				NetworkManager.Me.onClientDisonnected?.Invoke(clientThread.TcpSocket);
				clientThread.Shutdown();
				Debug.Log ("Client disconnected!");
			}
			else if (clientThread.Status == NetworkClientThread.ConnectionStatus.TIME_OUT)
			{
				NetworkManager.Me.onClientTimeOut?.Invoke(clientThread.TcpSocket);
				clientThread.Shutdown();
				Debug.Log ("Client timed-out!");
			}
		}
	}
}

