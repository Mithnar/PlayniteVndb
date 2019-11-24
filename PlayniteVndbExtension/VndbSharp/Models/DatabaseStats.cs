﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Models
{
	public class DatabaseStats
	{
		[JsonProperty("chars")]
		public UInt32 Characters { get; private set; }
		[JsonProperty("vn")]
		public UInt32 VisualNovels { get; private set; }
	}
}
