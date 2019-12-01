﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Newtonsoft.Json;
using VndbSharp.Attributes;
using VndbSharp.Json.Converters;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.Staff
{
    public class Staff
    {
		public UInt32 Id { get; private set; }
        public String Name { get; private set; }
        [JsonProperty("original")]
        public String OriginalName { get; private set; }
        public Gender? Gender { get; private set; }
		public String Language { get; private set; }
        [JsonProperty("links")]
		public StaffLinks StaffLinks { get; private set; }
        public String Description { get; private set; }
        public ReadOnlyCollection<StaffAliases> Aliases { get; private set; }
		[JsonProperty("main_alias")]
        public String MainAlias { get; private set; }
		public StaffVns[] Vns { get; private set; }
		public StaffVoiced[] Voiced { get; private set; }
	}
}
