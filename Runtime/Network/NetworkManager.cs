using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Ivyyy.Network
{
	public class NetworkManager : MonoBehaviour
	{
		//Serialized Values
		[SerializeField] int port = 23000;
		//Host + 3 clients = 4  players
		[SerializeField] int maxClients = 3;
		[SerializeField] int tickRate = 30;

		//Public Values
		public static NetworkManager Me {get; private set;}
		public int Port { get {return port;} }
		public int MaxClients {get {return maxClients;}}
		public bool Host { get {return host;} }

		public delegate void OnClientConnected (int clientNumber, Socket socket);
		public OnClientConnected onClientConnected = null;

		public delegate void OnConnectedToHost (Socket socket);
		public OnConnectedToHost onConnectedToHost = null;

		//Private Values
		bool host = false;
		private NetworkManagerState managerState;
		private float timer = 0f;

		//Public Functions
		public bool StartHost (int _port)
		{
			if (managerState == null)
			{
				port = _port;
				host = true;
				Debug.Log("Started Host Session");
				managerState = new NetworkManagerHostState();
				bool ok = managerState.Start();

				if (!ok)
					managerState = null;

				return ok;
			}
			else
				return false;
		}

		public bool StartClient (string ip, int _port)
		{
			if (managerState == null)
			{
				port = _port;
				host = false;
				Debug.Log("Started Client Session");
				managerState = new NetworkManagerClientState(ip);
				bool ok = managerState.Start();

				if (!ok)
					managerState = null;

				return ok;
			}
			else
				return false;
		}

		private void Awake()
		{
			if (Me == null)
			{
				Me = this;
				DontDestroyOnLoad (this);
			}
			else
				Destroy (this);
		}

		private void OnDestroy()
		{
			managerState?.ShutDown();
		}

		private void Update()
		{
			if (timer < (1f / tickRate))
				timer += Time.deltaTime;
			else
			{
				managerState?.Update();
				timer = 0f;
			}
		}
	}
}

