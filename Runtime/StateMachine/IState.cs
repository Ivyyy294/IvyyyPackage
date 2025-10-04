using UnityEngine;

namespace Ivyyy
{
   public interface IState
	{
		public void Enter (GameObject obj);
		public void Update (GameObject obj);
		public void Exit(GameObject obj);
	}
}
