using System.Collections;
using System.Collections.Generic;
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

	public class GameEventListener : MonoBehaviour, IGameEventListener
	{
		public GameEvent gameEvent;
		public UnityEvent response;
		public UnityEvent <bool> responseBool;
		public UnityEvent <int> responseInt;
		public UnityEvent <float> responseFloat;
		public UnityEvent <string> responseString;

		private void OnEnable()
		{
			if (gameEvent != null)
				gameEvent.RegisterListener(this);
		}

		private void OnDisable()
		{
			if (gameEvent != null)
				gameEvent.UnregisterListener(this);
		}

		public void OnEventRaised()
		{
			if (response != null)
				response.Invoke();
		}

		public void OnEventRaisedBool (bool val)
		{
			if (responseBool != null)
				responseBool.Invoke(val);
		}

		public void OnEventRaisedInt(int val)
		{
			if (responseInt != null)
				responseInt.Invoke(val);
		}

		public void OnEventRaisedFloat(float val)
		{
			if (responseFloat != null)
				responseFloat.Invoke(val);
		}
		
		public void OnEventRaisedString(string val)
		{
			if (responseString != null)
				responseString.Invoke(val);
		}
	}
}
