using System;
using System.Threading;
using System.Collections.Generic;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace HTTP2RPCServer
{
	public static class PythonEnginer
	{
		public static ScriptEngine engine = Python.CreateEngine();
		public static ScriptScope scope = engine.CreateScope();
		public static ScriptSource source = engine.CreateScriptSourceFromFile("call_fs.py");

		public static void Init()
		{
			source.Execute (scope);
		}
			
		public static String[] OriginateCall(String caller_number,String called_number)
		{
			String[] ret = new String[10];
			try{
				var originatecall = scope.GetVariable<Func<Object,Object,Object>> ("double_call");
				IronPython.Runtime.List result =(IronPython.Runtime.List) originatecall (caller_number,called_number);
				if(result!=null)
				{
					result.CopyTo(ret,0);
				}
			}
			catch(Exception ex) {
				Logger.Fatal ("PythonEnginer", "DoubleCall", "exception was happended!!!", ex);
			}
			return ret;
		}
	}

	public class RPCThread
	{
		public static void Execute(Object stateinfo)
		{
			DoubleCallObj obj = (DoubleCallObj)stateinfo;
			Byte[] rsp = null;
			Logger.Debug("RPCThread","InvokeFs",String.Format("caller_number:[{0},called_number:[{1}]]",obj.caller_number,obj.called_number));

			try{
				if (obj.caller_number == null||
					obj.called_number == null||
					obj.appkey == null) {
					rsp = Tools.GenerateJson ("", "404", "params can not empty");
					return;
				}
				String[] ret = PythonEnginer.OriginateCall (obj.caller_number, obj.called_number);
				if (ret [0].ToString().Equals ("+OK")) {
					rsp = Tools.GenerateJson (ret [1].ToString (), "0", "OK");
				}
				else{
					rsp = Tools.GenerateJson ("", "", ret [1].ToString ());
				}
				Logger.Debug ("RPCThread", "InvokeFs", String.Format ("fs return is {0}, callid is {1}", ret[0].ToString(),ret[1].ToString()));
			}
			catch(Exception ex) {
					Logger.Fatal ("RPCThread", "InvokeFs", "exception was happened!!!", ex);
					rsp = System.Text.Encoding.UTF8.GetBytes (ex.Message);
			}
			finally{
				obj.ctx.Response.OutputStream.Write (rsp, 0, rsp.Length);
				obj.ctx.Response.OutputStream.Close ();
				obj.ctx.Response.Close ();
			}
		}

		public static void InvokeFs()
		{
			DoubleCallObj obj = new DoubleCallObj();
			while (!_stop) {
				while (Queue<DoubleCallObj>.GetInstance ().Dequeue (ref obj)) {
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

