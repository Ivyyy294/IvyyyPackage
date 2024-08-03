using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Ivyyy.GameEvent
{
	public interface IGameEventListener
	{
		public void OnEventRaised();
		public void OnEventRaisedBool (bool val);
		public void OnEventRaisedInt (int val);
		public void OnEventRaisedFloat (float val);
		public void OnEventRaisedString (string val);
	}

	[System.Serializable]
	public class UnityEventBool : UnityEvent<bool> {}
	[System.Serializable]
	public class UnityEventInt : UnityEvent<int> {}
	[System.Serializable]
	public class UnityEventFloat : UnityEvent<float> {}
	[System.Serializable]
	public class UnityEventString : UnityEvent<string> {}

	public class GameEventListener : MonoBehaviour, IGameEventListener
	{
		public GameEvent m_gameEvent;
		[SerializeField] UnityEvent m_response;
		[SerializeField] UnityEventBool m_responseBool;
		[SerializeField] UnityEventInt m_responseInt;
		[SerializeField] UnityEventFloat m_responseFloat;
		[SerializeField] UnityEventString m_responseString;

		private void OnEnable()
		{
			if (m_gameEvent != null)
				m_gameEvent.RegisterListener(this);
		}

		private void OnDisable()
		{
			if (m_gameEvent != null)
				m_gameEvent.UnregisterListener(this);
		}

		public void OnEventRaised()
		{
			if (m_response != null)
				m_response.Invoke();
		}

		public void OnEventRaisedBool (bool val)
		{
			if (m_responseBool != null)
				m_responseBool.Invoke(val);
		}

		public void OnEventRaisedInt(int val)
		{
			if (m_responseInt != null)
				m_responseInt.Invoke(val);
		}

		public void OnEventRaisedFloat(float val)
		{
			if (m_responseFloat != null)
				m_responseFloat.Invoke(val);
		}
		
		public void OnEventRaisedString(string val)
		{
			if (m_responseString != null)
				m_responseString.Invoke(val);
		}
	}
}
