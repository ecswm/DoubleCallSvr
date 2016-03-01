using System;
using System.Net;

namespace HTTP2RPCServer
{
	public class VoiceIdentCallAppResponse
	{
		public VoiceIdentCallAppResponse(String _callid,String _msg,String _errcode)
		{
			callid = _callid;
			msg = _msg;
			errcode = _errcode;
		}

		public String callid;
		public String msg;
		public String errcode;
		static String appname = "VoiceIdentCall";
	}

	public class VoiceIdentCallApp:BaseFsApp
	{
		public VoiceIdentCallApp(HttpListenerContext ctx,String appname):base(ctx,appname)
		{
		}

		public override Byte[] GenerateJson(String callid,String errcode,String msg)
		{
			VoiceIdentCallAppResponse rsp = new VoiceIdentCallAppResponse (callid,errcode,msg);
			return System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject (rsp));
		}

		public override void Execute()
		{
			Logger.Debug("RPCThread","InvokeFs",
				String.Format("appname: {0},called_number:{1},ident_code:{2}",AppName,Ctx.Request.QueryString["called_number"],Ctx.Request.QueryString["ident_code"])
			);

			Logger.Info("HttpSvr","ProcessRequest",String.Format("Recive WebRequest:[app_key is {0},called_number is {1},ident_code is {2}]",
				Ctx.Request.QueryString["app_key"],
				Ctx.Request.QueryString["called_number"],
				Ctx.Request.QueryString["ident_code"]));
			
			try{
				if (Ctx.Request.QueryString["called_number"] == null||
					Ctx.Request.QueryString["ident_code"] == null||
					Ctx.Request.QueryString["app_key"] == null) {
					Result = GenerateJson ("", "404", "params can not empty");
					return;
				}
				String[] ret = PythonEnginer.VoiceIdentCall (Ctx.Request.QueryString["called_number"], Ctx.Request.QueryString["ident_code"]);
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

