using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Mono.Options;

namespace HTTP2RPCServer
{
	class program
	{
		static void Main(string[] args)
		{
			String url = "";
			var p = new OptionSet () {
				{ "url=",v => url = v }
			};
			p.Parse(args);
		
			Logger.Init ();

			RPCThread thread = new RPCThread ();
			thread.Start ();

			HttpSvr svr = new HttpSvr (url);
			svr.Run ();
		}

	public class HttpSvr
	{
		//process webrequest for incoming 
		static void ProcessRequest(object context)
		{
			HttpListenerContext ctx = context as HttpListenerContext;
				String app_key = ctx.Request.QueryString ["app_key"];
				String caller_number = ctx.Request.QueryString ["caller_number"];
				String called_number = ctx.Request.QueryString ["called_number"];
				Logger.Info("HttpSvr","ProcessRequest",String.Format("Recive WebRequest:[app_key is {0},caller_number is {1},called_number is {2}]",
																	 app_key,
																	 caller_number,
																	 called_number));
				Queue<DoubleCallObj>.GetInstance().Enqueue(new DoubleCallObj(
					ctx,
				app_key,
				caller_number,
				called_number
			));
		}

		private HttpListener httplistener;
		private static String prefixes;

		private String ip;
		public String Ip {
			get {
				return ip;
			}
		}

		private String port;
		public String Port {
			get {
				return port;
			}
		}
		
		public HttpSvr(String url)
		{
			prefixes = url;
			if (prefixes == null || prefixes.Length == 0){
				Logger.Fatal("HttpSvr","Init",String.Format("Init HttpServer Error,Cause:{0}",prefixes));
				throw new ArgumentException ();
			}
			if(!HttpListener.IsSupported){
				Logger.Fatal("HttpSvr","Init","Current OS Can Not Support The Feature!!!");
				throw new InvalidOperationException("Current OS Can Not Support The Feature!!!");
			}
		}

		public HttpSvr (String _ip,Int64 _port)
		{
			ip = _ip;
			port = _port.ToString();
			prefixes = String.Format ("http://{0}:{1}/", _ip, _port);
			if (prefixes == null || prefixes.Length == 0){
					Logger.Fatal("HttpSvr","Init",String.Format("Init HttpServer Error,Cause:{0}",prefixes));
					throw new ArgumentException ();
			}
			if(!HttpListener.IsSupported){
					Logger.Fatal("HttpSvr","Init","Current OS Can Not Support The Feature!!!");
					throw new InvalidOperationException("Current OS Can Not Support The Feature!!!");
			}
		}

		public void Run()
		{
			httplistener = new HttpListener ();
			httplistener.Prefixes.Add (prefixes + "voicecodecall/");
			httplistener.Start();
			
			Logger.Info ("HttpSvr", "Start", "HttpSvr was Started!!!");
			while(httplistener.IsListening)
			{
				HttpListenerContext ctx = httplistener.GetContext ();
 				Task task = Task.Factory.StartNew (ProcessRequest, ctx);    
			}
		}

		public void Stop()
		{
			Logger.Info ("HttpSvr", "Stoping", "HttpSvr is stopping!!!");
			Task.WaitAll();
			httplistener.Stop();
			Logger.Info ("HttpSvr", "Stoped", "HttpSvr was stopped!!!");
		}

	}
}
}

