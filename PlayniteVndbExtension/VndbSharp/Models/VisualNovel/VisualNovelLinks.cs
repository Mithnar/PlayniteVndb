﻿using System;
using Newtonsoft.Json;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.VisualNovel
{
	public class VisualNovelLinks : CommonLinks
	{
		[JsonProperty("encubed")]
		public String Encubed { get; private set; }
		[JsonProperty("renai")]
		public String Renai { get; private set; }
		[JsonProperty("wikidata")]
		public String Wikidata { get; private set; }
	}
}
