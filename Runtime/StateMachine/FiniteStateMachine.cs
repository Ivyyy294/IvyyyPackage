using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	public class FiniteStateMachine : MonoBehaviour
	{
		protected IState m_currentState;

        //Public Methods
		public void EnterState(IState newState)
		{
			if (m_currentState != null)
				m_currentState.Exit();
			
			if (newState != null)
			{
				m_currentState = newState;
				m_currentState.Enter (gameObject);
			}
		}

        public virtual void Update()
		{
			if (m_currentState != null)
				m_currentState.Update();
		}

        public virtual void FixedUpdate()
        {
            if (m_currentState != null)
                m_currentState.FixedUpdate();
        }

	}
}
