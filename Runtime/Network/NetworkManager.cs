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
		[SerializeField] int tickRate = 30;
		[SerializeField] long timeout = 5000; //Timeout after 5 seconds

		//Public Values
		public static NetworkManager Me {get; private set;}
		public int Port { get {return port;} }
		public bool Host { get {return host;} }
		public long Timeout { get { return timeout;} }

		//Delegates
		public delegate void SocketDelegate (Socket socket);
		public delegate bool AcceptClient (Socket socket);

		//Client
		public SocketDelegate onConnectedToHost = null;
		public SocketDelegate onHostDisonnected = null;
		public SocketDelegate onHostTimeOut = null;

		//Host
		public AcceptClient acceptClient = null; //True accept / False reject
		public SocketDelegate onClientConnected = null;
		public SocketDelegate onClientDisonnected = null;
		public SocketDelegate onClientTimeOut = null;

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

