using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
	public class PushdownAutomata : MonoBehaviour
	{
		protected Stack <IState> m_stateStack = new Stack <IState>();

        //Public Methods
		public IState CurrentState() { return m_stateStack.Peek();}

		public void PushState (IState newState)
		{
			m_stateStack.Push (newState);
			m_stateStack.Peek().Enter(gameObject);
		}

		public void SwapState (IState newState)
		{
			PopState();
			PushState (newState);
		}

		public void PopState()
		{
            if (m_stateStack.Count > 0)
                m_stateStack.Pop().Exit();
        }

		protected virtual void Update ()
		{
			if (m_stateStack.Count > 0)
				m_stateStack.Peek().Update();
		}

        public virtual void FixedUpdate()
        {
            if (m_stateStack.Count > 0)
                m_stateStack.Peek().FixedUpdate();
        }
    }
}
