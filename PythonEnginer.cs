using System;
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

		public static String[] VoiceIdentCall(String called_number,String ident_code)
		{
			String[] ret = new String[10];
			try{
				var voiceidentcall = scope.GetVariable<Func<Object,Object,Object>> ("voice_ident_call");
				IronPython.Runtime.List result =(IronPython.Runtime.List) voiceidentcall (called_number,ident_code);
				if(result!=null)
				{
					result.CopyTo(ret,0);
				}
			}
			catch(Exception ex) {
				Logger.Fatal ("PythonEnginer", "VoiceIdentCall", "exception was happended!!!", ex);
			}
			return ret;
		}
	}
}

