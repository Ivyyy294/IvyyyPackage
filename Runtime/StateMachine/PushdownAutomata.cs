using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.StateMachine
{
	public class PushdownAutomata : StateMachine
	{
		Stack <IState> stateStack = new Stack <IState>();

		public override void EnterState (IState newState)
		{
			if (stateStack.Count > 0)
				stateStack.Peek().Exit(gameObject);

			stateStack.Push (newState);
			stateStack.Peek().Enter(gameObject);
		}

		public void PopState()
		{
			if (stateStack.Count > 1)
			{
				stateStack.Pop().Exit(gameObject);
				stateStack.Peek().Enter(gameObject);
			}
			else
				Debug.LogError ("Unable to Pop State!");
		}

		protected override void Update ()
		{
			if (stateStack.Count > 0)
				stateStack.Peek().Update(gameObject);
		}
	}
}
