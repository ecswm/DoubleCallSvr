using System;
using System.Threading;
using System.Collections.Generic;


namespace HTTP2RPCServer
{
	public class RPCThread
	{
		public static void Execute(Object stateinfo)
		{
			BaseFsApp app = (BaseFsApp)stateinfo;
			app.Execute ();
			app.SendResult ();
		}

		public static void InvokeFs()
		{
			IFsApp obj = new BaseFsApp();
			while (!_stop) {
				while (Queue<IFsApp>.GetInstance ().Dequeue (ref obj)) {
					ThreadPool.QueueUserWorkItem (Execute, obj);
				}
			}
		}

		private Thread _fs;
		private static bool _stop = false;

		public RPCThread()
		{
			PythonEnginer.Init ();
			_fs = new Thread (new ThreadStart(InvokeFs));
		}

		public void Start()
		{
			_fs.Start ();
		}

		public void Stop()
		{
			_stop = true;
			_fs.Join ();
		}
	}
}

