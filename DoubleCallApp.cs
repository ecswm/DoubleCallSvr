using System;
using System.Net;

namespace HTTP2RPCServer
{
	public class DoubleCallAppResponse
	{
		public DoubleCallAppResponse(String _callid,String _msg,String _errcode)
		{
			callid = _callid;
			msg = _msg;
			errcode = _errcode;
		}

		public String callid;
		public String msg;
		public String errcode;
		static String appname = "DoubleCall";
	}

	public class DoubleCallApp:BaseFsApp
	{
		public DoubleCallApp(HttpListenerContext ctx,String appname):base(ctx,appname)
		{
		}

		public override Byte[] GenerateJson(String callid,String errcode,String msg)
		{
			DoubleCallAppResponse rsp = new DoubleCallAppResponse (callid,errcode,msg);
			return System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject (rsp));
		}

		public override void Execute()
		{
			Logger.Debug("RPCThread","InvokeFs",
				String.Format("appname: {0},caller_number:{1},called_number:{2}",AppName,Ctx.Request.QueryString["caller_number"],Ctx.Request.QueryString["called_number"])
			);

			Logger.Info("HttpSvr","ProcessRequest",String.Format("Recive WebRequest:[app_key is {0},caller_number is {1},called_number is {2}]",
				Ctx.Request.QueryString["app_key"],
				Ctx.Request.QueryString["caller_number"],
				Ctx.Request.QueryString["called_number"]));
			try{
				if (Ctx.Request.QueryString["caller_number"] == null||
					Ctx.Request.QueryString["called_number"] == null||
					Ctx.Request.QueryString["appkey"] == null) {
					Result = Tools.GenerateJson ("", "404", "params can not empty");
					return;
				}
				String[] ret = PythonEnginer.OriginateCall (Ctx.Request.QueryString["caller_number"], Ctx.Request.QueryString["called_number"]);
				if (ret [0].ToString().Equals ("+OK")) {
					Result = GenerateJson (ret [1].ToString (), "0", "OK");
				}
				else{
					Result = GenerateJson ("", "", ret [1].ToString ());
				}
				Logger.Debug ("RPCThread", "InvokeFs", String.Format ("fs return is {0}, callid is {1}", ret[0].ToString(),ret[1].ToString()));
			}
			catch(Exception ex) {
				Logger.Fatal ("RPCThread", AppName, "exception was happened!!!", ex);
				Result = System.Text.Encoding.UTF8.GetBytes (ex.Message);
			}
		}
	}

}

