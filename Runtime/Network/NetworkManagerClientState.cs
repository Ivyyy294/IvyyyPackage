using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

		public override bool Start()
		{
			//Create client Socket
			Socket socket = GetClientSocket (ip);

			bool ok = socket != null && socket.Connected;

			if (ok)
			{
				Debug.Log("Conntected to Host!");
		
				if (NetworkManager.Me.onConnectedToHost != null)
					NetworkManager.Me.onConnectedToHost (socket);

				int serverUdpPort = GetUDPPortFromServer (socket);

				//Start listener thread
				clientThread = new NetworkClientThread (socket, ((IPEndPoint) socket.LocalEndPoint).Port, serverUdpPort);
				clientThread.Start();
			}

			return ok;
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
			if (clientThread != null)
				clientThread.Shutdown();
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

			clientThread.SendUDPData (networkPackage.GetSerializedData());

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

		int GetUDPPortFromServer(Socket server)
		{
			byte[] buffer = new byte[sizeof(int)];
			server.Receive (buffer);
			return BitConverter.ToInt32 (buffer, 0);
		}
	}
}

