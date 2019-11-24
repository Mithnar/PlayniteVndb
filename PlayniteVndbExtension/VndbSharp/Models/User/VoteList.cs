﻿using System;
using Newtonsoft.Json;
using VndbSharp.Attributes;

namespace VndbSharp.Models.User
{
    public class VoteList
    {
		[JsonProperty("vn")]
	    public UInt32 VisualNovelId { get; private set; }
	    [JsonProperty("uid")]
	    public UInt32 UserId { get; private set; }
	    [JsonProperty("added"), IsUnixTimestamp]
	    public DateTime AddedOn { get; private set; }
    }
}
