using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy.StateMachine
{
   public interface IState
	{
		public void Enter (GameObject obj);
		public void Update (GameObject obj);
	}
}
