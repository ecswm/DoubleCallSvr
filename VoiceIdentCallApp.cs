﻿using System;
using System.Net;
using System.IO;
using RpcXmlClient;

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

	public class VoiceIdentCallAppRequest
	{
		public String called_number;
		public String ident_code;

		public VoiceIdentCallAppRequest(String _called_number,String _ident_code)
		{
			called_number = _called_number;
			ident_code = _ident_code;
		}
	}

	public class VoiceIdentCallApp:BaseFsApp
	{
	    void ParseJson(String body)
		{
			try{
			    req = Newtonsoft.Json.JsonConvert.DeserializeObject<VoiceIdentCallAppRequest> (body);
			}
			catch(Exception ex) {
				Logger.Fatal ("ParseJson", "ParseVoiceIdentCallAppRequest", ex.Message);
			}
		}

		VoiceIdentCallAppRequest req;

		public VoiceIdentCallApp(HttpListenerContext ctx,String appname):base(ctx,appname)
		{
			if (ctx.Request.HttpMethod.Equals ("POST")) {
				ParseJson (new StreamReader (ctx.Request.InputStream).ReadToEnd ());
			}
			if(ctx.Request.HttpMethod.Equals("GET")){
				req = new VoiceIdentCallAppRequest (ctx.Request.QueryString ["called_number"], ctx.Request.QueryString ["ident_code"]);
			}
		}

		public override Byte[] GenerateJson(String callid,String errcode,String msg)
		{
			VoiceIdentCallAppResponse rsp = new VoiceIdentCallAppResponse (callid,msg,errcode);
			return System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject (rsp));
		}

		public override void Execute()
		{
			if (req == null) {
				Logger.Fatal ("RPCThread", AppName, "Revice WebRequest Error,Cause: parse body is err!!!");
				Result = GenerateJson ("", "404", "params can not empty");
				return;
			}
			Logger.Debug("RPCThread","InvokeFs",
						  String.Format("appname: {0},called_number:{1},ident_code:{2}",AppName,req.called_number,req.ident_code)
			);
			
			try{
				RpcEnginer enginer = new RpcEnginer ();

				string[] ret = enginer.VoiceIdentCall("9"+req.called_number,req.ident_code);
				if (ret [0].ToString().Equals ("+OK")) {
					Result = GenerateJson (ret [1].TrimEnd('\n'), "0", "OK");
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

