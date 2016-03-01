using System;
using System.IdentityModel.Tokens;
using System.IdentityModel.Claims;
using System.IdentityModel.Protocols.WSTrust;


namespace HTTP2RPCServer
{
	public static class AuthToken
	{
		public static String GenerateToken(String user)
		{
			JwtHeader jwtheader = new JwtHeader();
			jwtheader.Add ("typ", "JWT");
			jwtheader.Add ("alg", JwtAlgorithms.HMAC_SHA256);


			JwtPayload playload = new JwtPayload ();
			var utc0 = new DateTime (1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			var lifetime = new Lifetime (DateTime.Now, DateTime.Now.AddMinutes (24*60));
			
			
			playload.Add ("iss", user);
			playload.Add ("sub", "voice");
			playload.Add ("iat", DateTime.Now.Subtract(utc0).TotalSeconds);
			playload.Add ("exp", DateTime.Now.AddMinutes (24 * 60).Subtract (utc0).TotalSeconds);

			//var token = new JwtSecurityToken(user,null,playload.Claims,DateTime.Now,DateTime.Now.AddMinutes(5),new SigningCredentials();
			var token = new JwtSecurityToken(jwtheader,playload);
			var tokenhandler = new JwtSecurityTokenHandler ();

			String a =  tokenhandler.WriteToken (token);
			String[] b = a.Split ('.');
			//var b = tokenhandler.ReadToken (a);
			return a;
		}


	}
		

}

