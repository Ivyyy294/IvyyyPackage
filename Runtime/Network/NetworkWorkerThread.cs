using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Ivyyy.Network
{
	public abstract class NetworkWorkerThread
	{
		//Public Values
		private Thread thread = null;
		protected bool shutdown = false;
		public bool IsRunning { get { return !shutdown;} }

		public void Start()
		{
			thread = new Thread (ReceiveData);
			thread.IsBackground = true;
			thread.Start();
		}

		public void Shutdown()
		{
			shutdown = true;
			thread.Join();
		}

		protected abstract void ReceiveData();
	}
}
