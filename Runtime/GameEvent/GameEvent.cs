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

		public void Raise (bool val)
		{
			Debug.Log("Raise Event: " + name + " " + val);
			listeners.ForEach(x => x.OnEventRaisedBool(val));
		}

		public void Raise(int val)
		{
			Debug.Log("Raise Event: " + name + " " + val);
			listeners.ForEach(x => x.OnEventRaisedInt(val));
		}

		public void Raise(float val)
		{
			Debug.Log("Raise Event: " + name + " " + val);
			listeners.ForEach(x => x.OnEventRaisedFloat(val));
		}
		
		public void Raise(string val)
		{
			Debug.Log("Raise Event: " + name + " " + val);
			listeners.ForEach(x => x.OnEventRaisedString(val));
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
