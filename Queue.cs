using System;
using System.Net;
using System.Collections.Concurrent;

namespace HTTP2RPCServer
{
	public class Queue<T>
	{
		private static ConcurrentQueue<T> _cq;
		private static Queue<T> _instance = null;
		private static readonly object _lockHelper = new object();

		public static Queue<T> GetInstance()
		{
			if (_instance == null) {
				lock (_lockHelper) {
					if (_instance == null)
						_instance = new Queue<T> ();
				}
			}
			return _instance;
		}

		private Queue()
		{
			_cq = new ConcurrentQueue<T> ();
		}

		public void Enqueue(T obj)
		{
			_cq.Enqueue (obj);
		}

		public bool Dequeue(ref T obj)
		{
			return _cq.TryDequeue(out obj);
		}
	}
}

