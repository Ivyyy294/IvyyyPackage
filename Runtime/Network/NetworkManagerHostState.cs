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
		List <NetworkClientThread> clientList = new List<NetworkClientThread>();
		Socket clientAcceptSocket = null;
		Task udpReceiveTask;
		int updPort = 23001;

		public override bool Start()
		{
			clientAcceptSocket = GetHostSocket();

			udpReceiveTask = Task.Run(()=>{UDPReceive(NetworkManager.Me.Port);});

			clientAcceptSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

			return clientAcceptSocket != null;
		}

		public override void Update()
		{
			CheckClientStatus();
			SendUPDData();
			SendTCPData();
		}

		public override void ShutDown()
		{
			//Close all client sockets
			foreach (NetworkClientThread client in clientList)
			{
				client.Shutdown();
				CloseSocket (client.TcpSocket);
			}
		}

		//Private Methods
		void SendUPDData()
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
		}

		void SendTCPData()
		{
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
		
		void CheckClientStatus()
		{
			List <NetworkClientThread> invalidClientList = new List<NetworkClientThread>();

			//Find invalid client connection
			foreach (NetworkClientThread client in clientList)
			{
				if (client.Status != NetworkClientThread.ConnectionStatus.CONNECTED)
					invalidClientList.Add (client);
			}

			foreach (NetworkClientThread client in invalidClientList)
			{
				if (client.Status == NetworkClientThread.ConnectionStatus.DISCONNECTED)
				{
					NetworkManager.Me.onClientDisonnected?.Invoke(client.TcpSocket);
					Debug.Log ("Client disconnected!");
				}
				else if (client.Status == NetworkClientThread.ConnectionStatus.TIME_OUT)
				{
					NetworkManager.Me.onClientTimeOut?.Invoke(client.TcpSocket);
					Debug.Log ("Client timed-out!");
				}

				clientList.Remove (client);
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
		void AcceptCallback (IAsyncResult ar)
		{
			try
			{
				Socket client = clientAcceptSocket.EndAccept(ar);

				Debug.Log ("Client connected. " + client.ToString()
						+ ", IPEndpoint: " + client.RemoteEndPoint.ToString());
				
				//Notify client
				NetworkClientThread.ConnectionData connectionData = HandShake (client);

				//Start client thread
				if (connectionData.accepted)
				{
					NetworkManager.Me.onClientConnected?.Invoke(client);

					NetworkClientThread handleClient = new NetworkClientThread(connectionData);
					handleClient.Start();
					clientList.Add (handleClient);
				}
				else
					CloseSocket (client);
			}
			catch (Exception e)
			{
				Debug.Log(e);
			}

			// Start accepting the next connection asynchronously
			clientAcceptSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
		}

		NetworkClientThread.ConnectionData HandShake (Socket client)
		{
			NetworkClientThread.ConnectionData connectionData = new NetworkClientThread.ConnectionData();
			connectionData.socket = client;

			//Step1 send accept flag to client

			//Check if server accepts client
			connectionData.accepted = NetworkManager.Me.acceptClient == null || NetworkManager.Me.acceptClient (client);
			client.Send(BitConverter.GetBytes (connectionData.accepted));

			if (connectionData.accepted)
				Debug.Log ("Client Accepted!");
			else
			{
				Debug.Log ("Client rejected!");
				return connectionData;
			}

			//Step2 send server upd port number to client
			connectionData.localUDPPort = updPort++;
			client.Send(BitConverter.GetBytes (connectionData.localUDPPort));
			Debug.Log("Server UDP Port: " + connectionData.localUDPPort);

			//Step2 get client upd port number
			byte[] buffer = new byte[sizeof(int)];
			client.Receive (buffer);
			connectionData.remoteUDPPort = BitConverter.ToInt32 (buffer, 0);
			Debug.Log ("Client UDP Port: " + connectionData.remoteUDPPort);

			return connectionData;
		}
	}
}

