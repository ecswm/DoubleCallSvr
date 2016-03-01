using System;

namespace HTTP2RPCServer
{
	public interface IFsApp
	{
		void Execute();
		void SendResult();
		Byte[] GenerateJson(String callid,String errcode,String msg);

		String AppName{ get; set;}
		Byte[] Result{ get; set;}
	}
}

