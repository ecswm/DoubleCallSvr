﻿using System;

namespace HTTP2RPCServer
{
	public interface IFsApp
	{
		void Execute();
		void SendResult();
		Byte[] GenerateJson();
		String AppName{ get; set;}
		Byte[] Result{ get; set;}
	}
}

