﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Models.Character
{
	public class VoiceActorMetadata
	{
		[JsonProperty("id")]
		public Int32 StaffId { get; private set; }
		[JsonProperty("aid")]
		public Int32 AliasId { get; private set; }
		[JsonProperty("vid")]
		public Int32 VisualNovelId { get; private set; }
		public String Note { get; private set; }
	}
}
