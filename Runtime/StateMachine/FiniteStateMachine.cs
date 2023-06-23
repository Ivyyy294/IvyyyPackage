using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.StateMachine
{
	public class FiniteStateMachine : StateMachine
	{
		private IState currentState;

		public override void EnterState(IState newState)
		{
			if (currentState != null)
				currentState.Exit(gameObject);
			
			currentState = newState;
			currentState.Enter (gameObject);
		}

		protected override void Update()
		{
			if (currentState != null)
				currentState.Update(gameObject);
		}

	}
}
