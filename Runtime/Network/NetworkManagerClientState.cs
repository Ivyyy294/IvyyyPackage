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
		Socket socket = null;
		NetworkUdpClientThread udpClientThread = null;

		public NetworkManagerClientState (string _ip) {ip = _ip;}
		~NetworkManagerClientState()
		{
			ShutDown();
		}

		public override bool Start()
		{
			//Create client Socket
			socket = GetClientSocket (ip);

			bool ok = socket != null && socket.Connected;

			if (ok)
			{
				Debug.Log("Conntected to Host!");
		
				if (NetworkManager.Me.onConnectedToHost != null)
					NetworkManager.Me.onConnectedToHost (socket);

				//Start listener thread
				udpClientThread = new NetworkUdpClientThread (socket);
				udpClientThread.Start();
			}

			return ok;
		}

		public override void Update()
		{
			if (socket != null && socket.Connected)
				SendData();
		}

		public override void ShutDown()
		{
			CloseSocket (socket);

			if (udpClientThread != null)
				udpClientThread.Shutdown();
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

			udpClientThread.SendData (networkPackage.GetSerializedData());
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

	}
}

