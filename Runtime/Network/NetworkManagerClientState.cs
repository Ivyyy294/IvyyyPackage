﻿using System;
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

			bool ok = socket != null;

			if (ok)
			{
				Debug.Log("Conntected to Host!");
		
				if (NetworkManager.Me.onConnectedToHost != null)
					NetworkManager.Me.onConnectedToHost (socket);

				int clientPort = ((IPEndPoint) socket.LocalEndPoint).Port;
				int serverUdpPort = ExchangeUDPPorts (socket, clientPort);

				//Start listener thread
				clientThread = new NetworkClientThread (socket, clientPort, serverUdpPort);
				clientThread.Start();
			}
			else
				Debug.Log ("Unable to connect!");

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
			{
				clientThread.Shutdown();
				CloseSocket (clientThread.TcpSocket);
			}
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

				if (clientSocket.Connected && ServerAcceptsClient (clientSocket))
				{
					Debug.Log("Server accepted!");
					return clientSocket;
				}
				else
				{
					Debug.Log("Server rejected!");
					CloseSocket (clientSocket);
					return null;
				}
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

		//Handshake
		bool ServerAcceptsClient (Socket server)
		{
			//Get confirmation from server
			byte[] buffer = new byte[sizeof(bool)];
			server.Receive (buffer);
			return BitConverter.ToBoolean (buffer, 0);
		}

		int ExchangeUDPPorts(Socket server, int clientPort)
		{
			//Get host port
			byte[] buffer = new byte[sizeof(int)];
			server.Receive (buffer);
			int serverPort = BitConverter.ToInt32 (buffer, 0);

			//send Host Port
			server.Send(BitConverter.GetBytes (clientPort));

			return serverPort;
		}
	}
}

