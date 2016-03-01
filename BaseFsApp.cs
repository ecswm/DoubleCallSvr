using System;
using System.Net;

namespace HTTP2RPCServer
{
	public class BaseFsApp:IFsApp
	{
		Byte[] rsp;
		String appname;
		HttpListenerContext ctx;

		public BaseFsApp()
		{
		}

		public BaseFsApp(HttpListenerContext _ctx,String _appname)
		{
			ctx = _ctx;
			appname = _appname;
		}

		public string AppName{
			get { return appname;}
			set {appname = value;}
		}

		public byte[] Result{
			get { return rsp;}
			set { rsp = value; }
		}

		public HttpListenerContext Ctx{
			get { return ctx;}
			set { ctx = value;}
		}

		public virtual void Execute (){}
		public virtual Byte[] GenerateJson(String callid,String errcode,String msg){
			return null;
		}

		public void SendResult()
		{
			ctx.Response.OutputStream.Write (rsp, 0, rsp.Length);
			ctx.Response.OutputStream.Close ();
			ctx.Response.Close ();
		}
	}
}

