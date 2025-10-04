using UnityEngine;

namespace Ivyyy
{
   public interface IState
	{
        //Public Methods
		public void Enter (GameObject obj);
		public void Update ();
        public void FixedUpdate();
		public void Exit();
	}
}
