using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ivyyy.GameEvent
{
	public interface IGameEventListener
	{
		public void OnEventRaised();
	}

	public class GameEventListener : MonoBehaviour, IGameEventListener
	{
		public GameEvent gameEvent;
		public UnityEvent response;

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
	}
}
