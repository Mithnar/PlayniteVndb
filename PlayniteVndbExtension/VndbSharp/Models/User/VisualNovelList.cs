﻿using System;
using Newtonsoft.Json;
using VndbSharp.Attributes;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.User
{
	public class VisualNovelList
	{
		[JsonProperty("vn")]
		public UInt32 VisualNovelId { get; private set; }
		[JsonProperty("uid")]
		public UInt32 UserId { get; private set; }
		public Status Status { get; private set; }
		[JsonProperty("added"), IsUnixTimestamp]
		public DateTime AddedOn { get; private set; }
		public String Notes { get; private set; }
	}
}
