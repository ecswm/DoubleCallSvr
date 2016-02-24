using System;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using log4net;
using log4net.Config;

[assembly: log4net.Config.XmlConfigurator(Watch=true)]
namespace HTTP2RPCServer
{

	public class Result
	{
		public Result(String _callid,String _msg,String _errcode)
		{
			callid = _callid;
			msg = _msg;
			errcode = _errcode;
		}

		public String callid;
		public String msg;
		public String errcode;
	}

	public class GT_Voice_DoubleCall_Response
	{
		public Result result;
		public GT_Voice_DoubleCall_Response()
		{
			result = null;
		}
	}

	public static class Tools
	{
		public static Byte[] GenerateJson(String callid,String errcode,String msg)
		{
			GT_Voice_DoubleCall_Response gt_voice_doublecall_response = new GT_Voice_DoubleCall_Response ();
			Result result = new Result (callid,msg,errcode);
			gt_voice_doublecall_response.result = result;

			return System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject (gt_voice_doublecall_response));
		}
			
	}
		
	public static  class Logger
	{
		private static XmlDocument Log4netConfig = new XmlDocument();
		private static String XmlString = @"<log4net>
                  <appender name=""UdpAppender_Error"" type=""log4net.Appender.UdpAppender"">
                  <remoteAddress value=""127.0.0.1"" />
                  <remotePort value=""8000"" />
                  <layout type=""log4net.Layout.PatternLayout, log4net"">
                   <conversionPattern value=""[%c] [%date{yyyy-MM-dd HH:mm:ss.fff}] [%level] [Tag:%l] [SESSION:%thread] [MSG:%message]%newline"" />
                  </layout>
                  <filter type=""log4net.Filter.LevelRangeFilter"">
	              <levelMin value=""WARN"" />
                  <levelMax value=""FATAL"" />
	              </filter>
                  </appender>
                  <appender name=""UdpAppender_Debug"" type=""log4net.Appender.UdpAppender"">
                  <remoteAddress value=""127.0.0.1"" />
                  <remotePort value=""8000"" />
                  <layout type=""log4net.Layout.PatternLayout, log4net"">
                  <conversionPattern value=""[%c] [%date{yyyy-MM-dd HH:mm:ss.fff}] [%level] [SESSION:%thread] [MSG:%message]%newline"" />
                  </layout>
                  <filter type=""log4net.Filter.LevelRangeFilter"">
	              <levelMin value=""DEBUG"" />
                  <levelMax value=""INFO"" />
	              </filter>
                  </appender>
                  <appender name=""RollingFileAppender_FS"" type=""log4net.Appender.RollingFileAppender"">
                  <file value=""Http2Rpc.log"" />
                  <appendToFile value=""true"" />
                  <rollingStyle value=""Size"" />
                  <maxSizeRollBackups value=""20"" />
                  <maximumFileSize value=""20MB"" />
                  <staticLogFileName value=""true"" />
                  <layout type=""log4net.Layout.PatternLayout"">
                  <conversionPattern value=""[%c] [%date{yyyy-MM-dd HH:mm:ss.fff}] [%level] [SESSION:%thread] [MSG:%message]%newline"" />
                  </layout>
                  </appender>
                  <logger name=""HTTP2RPCService"">
                  <level value=""DEBUG"" />
                  <appender-ref ref=""RollingFileAppender_FS""/>
                  </logger>
                  </log4net>";

		public static void Init()
		{
			try
			{
				if (File.Exists("./fs-server.config"))
				{
					FileInfo config = new FileInfo("./fs-server.config");
					XmlConfigurator.Configure(config);
				}
				else
				{
					#region Load log4netConfig
					Log4netConfig.LoadXml(XmlString);
					#endregion
					XmlConfigurator.Configure(Log4netConfig["log4net"]);
				}
			}
			catch
			{
				#region Load log4netConfig
				Log4netConfig.LoadXml(XmlString);
				#endregion
				XmlConfigurator.Configure(Log4netConfig["log4net"]);
			} 
			Log = LogManager.GetLogger(@"HTTP2RPCService");

		}

		public static void Init(String loghost,int logport)
		{
			#region Load log4netConfig
			Log4netConfig.LoadXml(XmlString);
			XmlNodeList remoteAddressList = Log4netConfig.SelectNodes("//log4net//appender//remoteAddress/@value");
			XmlNodeList remotePortList = Log4netConfig.SelectNodes("//log4net//appender//remotePort/@value");

			foreach (XmlNode node in remoteAddressList)
			{
				((XmlAttribute)node).Value = loghost;
			}

			foreach (XmlNode node in remotePortList)
			{
				((XmlAttribute)node).Value = Convert.ToString(logport); ;
			}
			#endregion
			XmlConfigurator.Configure(Log4netConfig["log4net"]);
			Log = LogManager.GetLogger(@"HTTP2RPCService");
		}

		public static ILog Log = LogManager.GetLogger(@"HTTP2RPCService");

		public static void Debug(String Tag, String SubTag, String Message)
		{
			if (Log.IsDebugEnabled)
			{
				Log.DebugFormat("[{0}][{1}][{2}]", Tag, SubTag, Message);
			}
		}

		public static void Debug(String Tag,String SubTag,String Message,Exception ex)
		{
			if (Log.IsDebugEnabled)
			{
				Log.Debug(Message, ex);
			}
		}

		public static void Error(String Tag, String SubTag, String Message)
		{
			if (Log.IsErrorEnabled)
			{
				Log.ErrorFormat("[{0}][{1}][{2}]", Tag, SubTag, Message);
			}
		}

		public static void Error(String Tag,String SubTag,String Message,Exception ex)
		{
			if (Log.IsErrorEnabled)
			{
				Log.Error(Message, ex);
			}
		}

		public static void Fatal(String Tag, String SubTag, String Message)
		{
			if (Log.IsFatalEnabled)
			{
				Log.FatalFormat("[{0}][{1}][{2}]", Tag, SubTag, Message);
			}
		}

		public static void Fatal(String Tag,String SubTag,String Message,Exception ex)
		{
			if (Log.IsFatalEnabled)
			{
				Log.Fatal(Message, ex);
			}
		}

		public static void Info(String Tag, String SubTag, String Message)
		{
			if (Log.IsInfoEnabled)
			{
				Log.InfoFormat("[{0}][{1}][{2}]", Tag, SubTag, Message);
			}
		}

		public static void Info(String Tag,String SubTag,String Message,Exception ex)
		{
			if (Log.IsInfoEnabled)
			{
				Log.Info(Message, ex);
			}
		}

		public static void Warn(String Tag, String SubTag, String Message)
		{
			if (Log.IsWarnEnabled)
			{
				Log.WarnFormat("[{0}][{1}][{2}]", Tag, SubTag, Message);
			}
		}

		public static void Warn(String Tag, String SubTag, String Message, Exception ex)
		{
			if (Log.IsWarnEnabled)
			{
				Log.Warn(Message, ex);
			}
		}
	}
}

