using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.GameEvent
{
	[CreateAssetMenu (menuName = "GameEvent")]
	public class GameEvent : ScriptableObject
	{
		private List <GameEventListener> listeners = new List<GameEventListener>();

		public void Raise()
		{
			Debug.Log ("Raise Event: " + name);
			listeners.ForEach (x=>x.OnEventRaised());
		}

		public void RegisterListener(GameEventListener listener)
		{
			listeners.Add(listener); 
		}

		public void UnregisterListener(GameEventListener listener)
		{
			listeners.Remove(listener); 
		}
	}
}
