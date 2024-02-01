	using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Ivyyy.Network
{
	class NetworkManagerHostState : NetworkManagerState
	{
		Task clientAcceptTask;
		Socket clientAcceptSocket;

		public override bool Start()
		{
			clientAcceptSocket = GetHostSocket();

			udpReceiveTask = Task.Run(()=>{UDPReceive(NetworkManager.Me.Port);});

			if (clientAcceptSocket != null)
				clientAcceptTask = Task.Run(()=>{AcceptCLients (clientAcceptSocket);});

			return clientAcceptSocket != null;
		}

		public override void Update()
		{
			//CheckClientStatus();
			CheckTcpSocketStatus();
			SendUPDData();
			SendTCPData();
		}

		public override void ShutDown()
		{
			CloseSocket (clientAcceptSocket);
			base.ShutDown();
		}

		//Private Methods
		void SendUPDData()
		{
			 networkPackage.Clear();

			//Create combined NetworkPackage of all NetworkObjects
			foreach (KeyValuePair<string, NetworkBehaviour> entry in NetworkBehaviour.guidMap)
			{
				NetworkBehaviour netObj = entry.Value;
				networkPackage.AddValue (GetNetObjectAsValue (netObj));
			}

			byte[] data = networkPackage.GetSerializedData();

			SendUDPData (data);
		}

		void SendTCPData()
		{
			//Send TCP Data
			if (NetworkRPC.outgoingRpcStack.Count > 0 || NetworkRPC.pendingRpcStack.Count > 0)
			{
				networkPackage.Clear();

				//Add outgoing Rpc to stack
				while (NetworkRPC.outgoingRpcStack.Count > 0)
					networkPackage.AddValue (new NetworkPackageValue (NetworkRPC.outgoingRpcStack.Dequeue().GetSerializedData()));

				//Execute pendingRpc and add it to package
				while (NetworkRPC.pendingRpcStack.Count > 0)
				{
					NetworkRPC pendingRPC = (NetworkRPC.pendingRpcStack.Dequeue());
					NetworkRPC.ExecutePendingRPC (pendingRPC);
					networkPackage.AddValue (new NetworkPackageValue (pendingRPC.GetSerializedData()));
				}

				//Sent the data of all NetworkObjects to all clients
				SendTCPData (networkPackage.GetSerializedData());
			}
		}

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
		void AcceptCLients (Socket clientAcceptSocket)
		{
			try
			{
				while (!shutDown)
				{
					Socket client = clientAcceptSocket.Accept();
					client.ReceiveTimeout = 5000;

					Debug.Log ("Client connected. " + client.ToString()
							+ ", IPEndpoint: " + client.RemoteEndPoint.ToString());
				
					//Start client thread
					if (HandShake (client))
					{
						if (NetworkManager.Me.onClientConnected != null)
							NetworkManager.Me.onClientConnected (client);

						udpEndPoints.Add ((IPEndPoint)client.RemoteEndPoint);
						AddTcpSocket (client);
						Debug.Log("Init complete!");
					}
					else
						CloseSocket (client);
				}
			}
			catch (Exception e)
			{
				Debug.Log(e);
			}
		}

		bool HandShake (Socket client)
		{
			//Check if server accepts client
			bool accepted = NetworkManager.Me.acceptClient == null || NetworkManager.Me.acceptClient (client);
			client.Send(BitConverter.GetBytes (accepted));

			if (accepted)
				Debug.Log ("Client Accepted!");
			else
				Debug.Log ("Client rejected!");

			return accepted;
		}
	}
}

