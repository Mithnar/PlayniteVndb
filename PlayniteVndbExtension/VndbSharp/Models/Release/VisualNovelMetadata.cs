﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Models.Release
{
	public class VisualNovelMetadata
	{
		[JsonProperty("title")]
		public String Name { get; private set; }
		[JsonProperty("original")]
		public String OriginalName { get; private set; }
	}
}
