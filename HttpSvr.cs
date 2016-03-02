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
			Tools.InitSecretKey ();

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
			BaseFsApp fsapp = null;
			Byte[] rsp = null;
			if (!Tools.DecodeSigParams (ctx.Request.QueryString ["SigParameter"], ctx.Request.Headers ["Authorization"])) {
				if (ctx.Request.Url.AbsolutePath.Contains ("DoubleCall")) {
					fsapp = new DoubleCallApp (ctx, "DoubleCallApp");
				} else if (ctx.Request.Url.AbsolutePath.Contains ("VoiceIdentCall")) {
					fsapp = new VoiceIdentCallApp (ctx, "VoiceIdentCall");
				}
				Queue<IFsApp>.GetInstance ().Enqueue (fsapp);
				return;
			}
			ctx.Response.StatusCode = 503;
			ctx.Response.OutputStream.Write (rsp, 0, 0);
			ctx.Response.OutputStream.Close ();
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
			httplistener.Prefixes.Add (prefixes + "VoiceIdentCall/");
			httplistener.Prefixes.Add (prefixes + "DoubleCall/");
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

