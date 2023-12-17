	using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Ivyyy.Network
{
	class NetworkManagerHostState : NetworkManagerState
	{
		List <NetworkClientThread> clientList = new List<NetworkClientThread>();
		Socket clientAcceptSocket = null;
		Thread clientAcceptThread;

		public override bool Start()
		{
			clientAcceptSocket = GetHostSocket();

			//Start accept thread
			clientAcceptThread = new Thread (AcceptClients);
			clientAcceptThread.Start();

			return clientAcceptSocket != null;
		}

		public override void Update()
		{
			 networkPackage.Clear();

			//Create combined NetworkPackage of all NetworkObjects
			foreach (KeyValuePair<string, NetworkBehaviour> entry in NetworkBehaviour.guidMap)
			{
				NetworkBehaviour netObj = entry.Value;

				if (netObj.Sync())
					networkPackage.AddValue (GetNetObjectAsValue (netObj));
			}

			byte[] data = networkPackage.GetSerializedData();

			//Sent the data of all NetworkObjects to all clients
			foreach (NetworkClientThread client in clientList)
				client.SendUDPData (data);

			//Send TCP Data
			if (NetworkRPC.outgoingRpcStack.Count > 0 || NetworkRPC.pendingRpcStack.Count > 0)
			{
				networkPackage.Clear();

				//Add outgoing Rpc to stack
				while (NetworkRPC.outgoingRpcStack.Count > 0)
					networkPackage.AddValue (new NetworkPackageValue (NetworkRPC.outgoingRpcStack.Pop().GetSerializedData()));

				//Execute pendingRpc and add it to package
				while (NetworkRPC.pendingRpcStack.Count > 0)
				{
					NetworkRPC pendingRPC = (NetworkRPC.pendingRpcStack.Pop());
					NetworkRPC.ExecutePendingRPC (pendingRPC);
					networkPackage.AddValue (new NetworkPackageValue (pendingRPC.GetSerializedData()));
				}

				//Sent the data of all NetworkObjects to all clients
				foreach (NetworkClientThread client in clientList)
					client.SendTCPData (networkPackage.GetSerializedData());
			}
		}

		public override void ShutDown()
		{
			//Close accept socket
			CloseSocket (clientAcceptSocket);

			//Close all client sockets
			foreach (NetworkClientThread client in clientList)
				client.Shutdown();

			//Wait for Threads to finish
			clientAcceptThread.Join();
		}


		//Private Methods

		Socket GetHostSocket ()
		{
			//Create TCP Socket
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			
			//Accepts connection fromm any IP address
			IPAddress iPAddress = IPAddress.Any;

			//Listens to port 23000
			IPEndPoint iPEndPoint = new IPEndPoint (iPAddress, NetworkManager.Me.Port);
			socket.Bind (iPEndPoint);

			//Allows up to 5 incomming connections
			socket.Listen (5);

			return socket;
		}

		//Method of clientAcceptThread
		//Creates a new HandleClient Thread for each Client
		void AcceptClients ()
		{
			try
			{
				while (clientList.Count < NetworkManager.Me.MaxClients)
				{
					Socket client = clientAcceptSocket.Accept();
					Debug.Log ("Client connected. " + client.ToString()
							+ ", IPEndpoint: " + client.RemoteEndPoint.ToString());

					NetworkClientThread handleClient = new NetworkClientThread(client);
					handleClient.Start();
					clientList.Add (handleClient);

					//clientList is 1 short of player list so clientList.Count is equal to client index
					if (NetworkManager.Me.onClientConnected != null)
						NetworkManager.Me.onClientConnected (clientList.Count, client);
				}
			}
			catch (Exception e)
			{
				Debug.Log (e);
			}
		}
	}
}

