using UnityEngine;

namespace Ivyyy.StateMachine
{ 
	public abstract class StateMachine: MonoBehaviour
	{
		public abstract void EnterState (IState newState);
		protected abstract void Update ();
	}
}
