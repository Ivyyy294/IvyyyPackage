using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Ivyyy.Network
{
	[AttributeUsage(AttributeTargets.Method)]
	public class RPCAttribute : Attribute { }

	[RequireComponent (typeof(NetworkObject))]
	public abstract class NetworkBehaviour : MonoBehaviour
	{
		[SerializeField] string guid = null;
		public string GUID { get { return guid;} }
		static public Dictionary <string, NetworkBehaviour> guidMap = new Dictionary<string, NetworkBehaviour>();

		//RPC
		private Dictionary<string, Delegate> delegateDictionary = new Dictionary<string, Delegate>();
		private Stack <string> rpcStack = new Stack<string>();

		public NetworkBehaviour()
		{
			networkPackage = backBuffer1;
			AddMethodsWithAttribute();
		}

		public bool Sync ()
		{
			//return gameObject.activeInHierarchy;
			return true;
		}

		public bool Owner {get; set;}

		public byte[] GetSerializedData()
		{
			//Clear Package
			networkPackage.Clear();

			//Call abstract SetPackageData
			SetPackageData();

			if(Owner)
				SetRPCCalls();

			//Return Serialized Data
			return networkPackage.GetSerializedData();
		}
		public bool DeserializeData (byte[] rawData)
		{
			NetworkPackage backBuffer = GetBackBuffer();
			bool ok = backBuffer.DeserializeData(rawData);

			if (!Owner)
				ExecuteRPCCalls();

			SwapBuffer();

			return ok;
		}

		public void AddMethodsWithAttribute()
		{
			Type type = GetType();
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (MethodInfo method in methods)
			{
				if (Attribute.IsDefined(method, typeof(RPCAttribute)))
				{
					// Create a delegate for the method and add it to the list
					Delegate methodDelegate = Delegate.CreateDelegate(typeof(Action), this, method);
					delegateDictionary[method.Name] = methodDelegate;
				}
			}
		}

		public void GenerateGuid()
		{
			guid = System.Guid.NewGuid().ToString();
		}

		//Protected
		protected abstract void SetPackageData();
		protected bool Host {get {return NetworkManager.Me.Host;}}

		protected NetworkPackage networkPackage;

		protected void Invoke (string methodeName)
		{
			rpcStack.Push (methodeName);
		}

		//Private values
		//Double buffer is probably unessescary at this point and a relict of iterations
		private NetworkPackage backBuffer1 = new NetworkPackage();
		private NetworkPackage backBuffer2 = new NetworkPackage();

		private void SwapBuffer()
		{
			networkPackage = GetBackBuffer();
		}

		NetworkPackage GetBackBuffer()
		{
			if (networkPackage == backBuffer1)
				return backBuffer2;
			else
				return backBuffer1;
		}

		private void SetRPCCalls()
		{
			//while (rpcStack.Count > 0)
			//{
			//	string tmp = "RPC" + rpcStack.Pop();
			//	networkPackage.AddValue (new NetworkPackageValue (tmp));
			//}
		}

		private void ExecuteRPCCalls()
		{
			//for (int i = 0; i < networkPackage.Count; ++i)
			//{
			//	string tmp = networkPackage.Value (i).GetString();

			//	if (tmp.Contains ("RPC"))
			//	{
			//		string name = tmp.Replace ("RPC", "");

			//		if (delegateDictionary.TryGetValue(name, out var methodDelegate))
			//		{
			//			// Invoke the delegate if found
			//			((Action)methodDelegate).Invoke();
			//		}
			//	}
			//}
		}
	}
}
