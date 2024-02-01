using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Ivyyy.Network
{
	[AttributeUsage(AttributeTargets.Method)]
	public class RPCAttribute : Attribute { }

	[RequireComponent(typeof(NetworkObject))]
	public abstract class NetworkBehaviour : MonoBehaviour
	{
		//### Values ###
		//Public
		public string GUID { get { return guid; } }
		public bool Owner { get; set; }
		static public Dictionary<string, NetworkBehaviour> guidMap = new Dictionary<string, NetworkBehaviour>();

		//Editor
		[SerializeField] string guid = null;

		//Protected
		protected NetworkPackage networkPackage;
		protected bool Host { get { return !NetworkManager.Me || NetworkManager.Me.Host; } }

		//RPC
		public delegate void RPCDelegateSimple ();
		public delegate void RPCDelegateParameter (byte[] data);

		private Dictionary<string, Tuple <Delegate, bool>> delegateDictionary = new Dictionary<string, Tuple <Delegate, bool>>();

		//### Methods ###
		//Public
		public NetworkBehaviour()
		{
			networkPackage = backBuffer1;
			AddMethodsWithAttribute();
		}

		public byte[] GetSerializedData()
		{
			//Clear Package
			networkPackage.Clear();

			//Call abstract SetPackageData
			SetPackageData();

			return networkPackage.GetSerializedData();
		}

		public bool DeserializeData(byte[] rawData)
		{
			NetworkPackage backBuffer = GetBackBuffer();
			bool ok = backBuffer.DeserializeData(rawData);

			SwapBuffer();

			return ok;
		}

		public void ResetGUID()
		{
			guid = "";
		}

		public void GenerateGuid()
		{
			guid = System.Guid.NewGuid().ToString();
		}

		//Protected
		protected abstract void SetPackageData();

		//### Back Buffer ###
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

		//### RPC ###
		protected void InvokeRPC(string methodeName, byte[] data = null)
		{
			if (Owner)
				NetworkRPC.AddOutgoingPendingRPC(guid, methodeName, data);
		}

		void AddMethodsWithAttribute()
		{
			Type type = GetType();
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (MethodInfo method in methods)
			{
				if (Attribute.IsDefined(method, typeof(RPCAttribute)))
				{
					bool hasAttributes = HasAttributes(method);

					// Create a delegate for the method and add it to the list
					Delegate methodDelegate = hasAttributes 
						? Delegate.CreateDelegate(typeof(RPCDelegateParameter), this, method)
						: Delegate.CreateDelegate(typeof(RPCDelegateSimple), this, method);

					delegateDictionary[method.Name] = new Tuple <Delegate, bool> (methodDelegate, hasAttributes);
				}
			}
		}

		public bool HasAttributes (MethodInfo methodInfo)
		{
			ParameterInfo[] parameters = methodInfo.GetParameters();

			if (parameters.Length == 0)
				return false;
			else if (parameters.Length != 1)
				throw new InvalidOperationException($"Method {methodInfo.Name} has an invalid number of parameters.");

			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].ParameterType != typeof (byte[]))
					throw new InvalidOperationException($"Parameter {i + 1} of method {methodInfo.Name} has an invalid type.");
			}

			return true;
		}

		public bool ExecuteRPCCall(string rpcName, byte[] data)
		{
			if (delegateDictionary.TryGetValue(rpcName, out var methodDelegate))
			{
				if (methodDelegate.Item2)
					((RPCDelegateParameter) methodDelegate.Item1).Invoke (data);
				else
					((RPCDelegateSimple) methodDelegate.Item1).Invoke ();

				return true;
			}

			return false;
		}
	}
}
