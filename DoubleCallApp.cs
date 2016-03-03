using System;
using System.IO;
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

	public class DoubleCallAppRequest
	{
		public DoubleCallAppRequest(String _caller_number,String _called_number)
		{
			caller_number = _caller_number;
			called_number = _called_number;
		}

		public String caller_number;
		public String called_number;
	}

	public class DoubleCallApp:BaseFsApp
	{
		void ParseJson(String body)
		{
			try{
				req = Newtonsoft.Json.JsonConvert.DeserializeObject<DoubleCallAppRequest> (body);
			}
			catch(Exception ex) {
			}
		}

		DoubleCallAppRequest req;

		public DoubleCallApp(HttpListenerContext ctx,String appname):base(ctx,appname)
		{
			req = null;
			if (ctx.Request.HttpMethod.Equals ("POST")) {
				 ParseJson(new StreamReader(ctx.Request.InputStream).ReadToEnd());
			}
			if (ctx.Request.HttpMethod.Equals ("GET")) {
				req = new DoubleCallAppRequest (ctx.Request.QueryString ["caller_number"], ctx.Request.QueryString ["called_number"]);
			}
		}

		public override Byte[] GenerateJson(String callid,String errcode,String msg)
		{
			DoubleCallAppResponse rsp = new DoubleCallAppResponse (callid,msg,errcode);
			return System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject (rsp));
		}

		public override void Execute()
		{
			if (req==null) {
				Logger.Fatal ("RPCThread", AppName, "Revice WebRequest Error,Cause: body is null");
				Result = GenerateJson ("", "404", "params can not empty");
				return;
			}
			Logger.Debug("RPCThread","InvokeFs",
				String.Format("appname: {0},caller_number:{1},called_number:{2}",AppName,req.caller_number,req.called_number)
			);
			try{
				String[] ret = PythonEnginer.OriginateCall (req.caller_number,req.called_number);
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

