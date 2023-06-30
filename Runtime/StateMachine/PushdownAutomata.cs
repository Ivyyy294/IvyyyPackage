using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.StateMachine
{
	public class PushdownAutomata : MonoBehaviour
	{
		Stack <IState> stateStack = new Stack <IState>();

		public IState CurrentState() { return stateStack.Peek();}

		public void PushState (IState newState)
		{
			stateStack.Push (newState);
			stateStack.Peek().Enter(gameObject);
		}

		public void SwapState (IState newState)
		{
			PopState();
			PushState (newState);
		}

		public void PopState()
		{
			stateStack.Pop().Exit(gameObject);
		}

		protected virtual void Update ()
		{
			if (stateStack.Count > 0)
				stateStack.Peek().Update(gameObject);
		}
	}
}
