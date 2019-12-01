﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Models
{
	public class DatabaseStats
	{
		public UInt32 Users { get; private set; }
		public UInt32 Threads { get; private set; }
		public UInt32 Tags { get; private set; }
		public UInt32 Releases { get; private set; }
		public UInt32 Producers { get; private set; }
		[JsonProperty("chars")]
		public UInt32 Characters { get; private set; }
		public UInt32 Posts{ get; private set; }
		[JsonProperty("vn")]
		public UInt32 VisualNovels { get; private set; }
		public UInt32 Traits { get; private set; }
	}
}
