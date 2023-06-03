using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.GameEvent
{
	[CreateAssetMenu (menuName = "GameEvent")]
	public class GameEvent : ScriptableObject
	{
		private List <IGameEventListener> listeners = new List<IGameEventListener>();

		public void Raise()
		{
			Debug.Log ("Raise Event: " + name);
			listeners.ForEach (x=>x.OnEventRaised());
		}

		public void RegisterListener(IGameEventListener listener)
		{
			listeners.Add(listener); 
		}

		public void UnregisterListener(IGameEventListener listener)
		{
			listeners.Remove(listener); 
		}
	}
}
