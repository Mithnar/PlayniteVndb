﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace VndbSharp.Models.Staff
{
    public class StaffVoiced
    {
        [JsonProperty("id")]
        public UInt32 StaffId { get; private set; }
        [JsonProperty("aid")]
        public UInt32 AliasId { get; private set; }
        [JsonProperty("cid")]
        public UInt32 CharacterId { get; private set; }
		public string Note { get; private set; }
	}
}
