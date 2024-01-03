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

			if (HandShake (socket))
			{
				if (NetworkManager.Me.onConnectedToHost != null)
					NetworkManager.Me.onConnectedToHost (socket);

				Debug.Log("Conntected to Host!");

				//Add server to udpEndPoints
				udpEndPoints.Add ((IPEndPoint)socket.RemoteEndPoint);

				//Add tcp socket to list
				AddTcpSocket (socket);

				return true;
			}
			else
				CloseSocket (socket);

			return false;;
		}

		public override void Update()
		{
			NetworkRPC.ExecutePendingRPC();
			SendData();
			//if (clientThread.IsRunning)
			//{
			//	if (clientThread.Status == NetworkClientThread.ConnectionStatus.CONNECTED)
			//	{
			//		NetworkRPC.ExecutePendingRPC();
			//		SendData();
			//	}
			//	else
			//		CheckHostStatus();
			//}
		}

		//Private Methods
		bool HandShake(Socket socket)
		{
			NetworkClientThread.ConnectionData connectionData = new NetworkClientThread.ConnectionData();
			connectionData.socket = socket;

			byte[] buffer = new byte[sizeof(bool) + sizeof (int)];

			//Step1 get accept flag from Server
			socket.Receive (buffer);
			bool accepted = BitConverter.ToBoolean (buffer, 0);

			if (accepted)
				Debug.Log("Server accepted!");
			else
				Debug.Log("Server rejected!");

			return accepted;
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

				SendTCPData (networkPackage.GetSerializedData());
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

		//void CheckHostStatus()
		//{
		//	if (clientThread.Status == NetworkClientThread.ConnectionStatus.DISCONNECTED)
		//	{
		//		NetworkManager.Me.onClientDisonnected?.Invoke(clientThread.TcpSocket);
		//		clientThread.Shutdown();
		//		Debug.Log ("Client disconnected!");
		//	}
		//	else if (clientThread.Status == NetworkClientThread.ConnectionStatus.TIME_OUT)
		//	{
		//		NetworkManager.Me.onClientTimeOut?.Invoke(clientThread.TcpSocket);
		//		clientThread.Shutdown();
		//		Debug.Log ("Client timed-out!");
		//	}
		//}
	}
}

