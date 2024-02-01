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
				Debug.Log("Conntected to Host!");

				if (NetworkManager.Me.onConnectedToHost != null)
					NetworkManager.Me.onConnectedToHost (socket);

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
			CheckTcpSocketStatus();
			NetworkRPC.ExecutePendingRPC();
			SendData();
		}

		//Private Methods
		bool HandShake(Socket socket)
		{
			byte[] buffer = new byte[sizeof(bool)];

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

				if (networkObject.Owner)
					networkPackage.AddValue (GetNetObjectAsValue (networkObject));
			}

			SendUDPData (networkPackage.GetSerializedData());

			//Send TCP Data
			if (NetworkRPC.outgoingRpcStack.Count > 0)
			{
				networkPackage.Clear();

				while (NetworkRPC.outgoingRpcStack.Count > 0)
					networkPackage.AddValue (new NetworkPackageValue (NetworkRPC.outgoingRpcStack.Dequeue().GetSerializedData()));

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

				if (clientSocket != null)
					clientSocket.ReceiveTimeout = 5000;

				return clientSocket;
			}
			catch (Exception excp)
			{
				Debug.LogError (excp);
				return null;
			}
		}
	}
}

