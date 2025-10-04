using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	[CreateAssetMenu (menuName = "Ivyyy/GameEvent")]
	public class GameEvent : ScriptableObject
	{
		[System.Serializable]
		public enum GameEventTyp
		{
			Void,
			Bool,
			Int,
			Float,
			String,
            GameObject
		}

		public GameEventTyp m_eventTyp;

		//Custom Inspector values
		[HideInInspector] public bool m_bool;
		[HideInInspector] public int m_int;
		[HideInInspector] public float m_float;
		[HideInInspector] public string m_string;
		[HideInInspector] public GameObject m_gameObject;

		private List <IGameEventListener> m_listeners = new List<IGameEventListener>();

		public void Raise()
		{
			if (IsEventTypValid(GameEventTyp.Void))
			{
				Debug.Log ("Raise Event: " + name);
				m_listeners.ForEach (x=>x.OnEventRaised());
			}
		}

		public void Raise (bool val)
		{
			if (IsEventTypValid(GameEventTyp.Bool))
			{
				Debug.Log("Raise Event: " + name + " " + val);
				m_listeners.ForEach(x => x.OnEventRaisedBool(val));
			}
		}

		public void Raise(int val)
		{
			if (IsEventTypValid(GameEventTyp.Int))
			{
				Debug.Log("Raise Event: " + name + " " + val);
				m_listeners.ForEach(x => x.OnEventRaisedInt(val));
			}
		}

		public void Raise(float val)
		{
			if (IsEventTypValid(GameEventTyp.Float))
			{
				Debug.Log("Raise Event: " + name + " " + val);
				m_listeners.ForEach(x => x.OnEventRaisedFloat(val));
			}
		}
		
		public void Raise(string val)
		{
			if (IsEventTypValid(GameEventTyp.String))
			{
				Debug.Log("Raise Event: " + name + " " + val);
				m_listeners.ForEach(x => x.OnEventRaisedString(val));
			}
		}

        public void Raise(GameObject val)
        {
            if (IsEventTypValid(GameEventTyp.GameObject))
            {
                Debug.Log("Raise Event: " + name + " " + val);
                m_listeners.ForEach(x => x.OnEventRaisedGameObject(val));
            }
        }

        public void RegisterListener(IGameEventListener listener)
		{
			m_listeners.Add(listener); 
		}

		public void UnregisterListener(IGameEventListener listener)
		{
			m_listeners.Remove(listener); 
		}

		private bool IsEventTypValid (GameEventTyp eventTyp)
		{
			if (m_eventTyp == eventTyp)
				return true;
			else
			{
				Debug.LogError ("Invalid GameEvent parameter! GameEventTyp is: " + m_eventTyp.ToString());
				return false;
			}
		}
	}
}
