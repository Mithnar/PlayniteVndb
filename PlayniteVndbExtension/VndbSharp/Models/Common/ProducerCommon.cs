﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Models.Common
{
	public class ProducerCommon
	{
		public UInt32 Id { get; private set; }
		public String Name { get; private set; }
		[JsonProperty("original")]
		public String OriginalName { get; private set; }
		[JsonProperty("type")]
		public String ProducerType { get; private set; } // Enum? Valid values - https://g.blicky.net/vndb.git/tree/util/sql/all.sql#n20 , real values???
	}
}
