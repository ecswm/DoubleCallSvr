using System;
using System.Net;
using System.Collections.Concurrent;

namespace HTTP2RPCServer
{
	public enum Step
	{
		INCOMING,ABANDON,TOAGENT
	}

	public struct CDRInfo
	{
		Step   step;
		String connid;
		String ani;
		String time;

		public CDRInfo(Step _step, String _connid,String _ani,String _time)
		{
			step = _step;
			connid = _connid;
			ani = _ani;
			time = DateTime.Now.ToString();
		}
	}

	public struct DoubleCallObj
	{
		public HttpListenerContext ctx;
		public String appkey;
		public String caller_number;
		public String called_number;

		public DoubleCallObj(HttpListenerContext _ctx,String _appkey,String _caller_number,String _called_number)
		{
			ctx = _ctx;
			appkey = _appkey;
			caller_number = _caller_number;
			called_number = _called_number;
		}
	}

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

