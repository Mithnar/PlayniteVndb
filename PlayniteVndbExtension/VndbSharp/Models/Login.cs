﻿using System;
#if UserAuth
using System.Security;
#endif
using Newtonsoft.Json;

namespace VndbSharp.Models
{
	internal class Login
	{
		public Login()
		{
			this.ClientName = VndbUtils.ClientName;
			this.ClientVersion = VndbUtils.ClientVersion;
		}

		[JsonProperty("protocol")]
		public UInt32 ProtocolVersion = 1;

		[JsonProperty("client")]
		public String ClientName;

		[JsonProperty("clientver")]
		public String ClientVersion;
	}
}
