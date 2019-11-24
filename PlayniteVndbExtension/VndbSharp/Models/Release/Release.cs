﻿using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.Release
{
	public class Release
	{
		[JsonProperty("title")]
		public String Name { get; private set; }
		[JsonProperty("original")]
		public String OriginalName { get; private set; }
		[JsonProperty("patch")]
		public Boolean IsPatch { get; private set; }
		[JsonProperty("freeware")]
		public Boolean IsFreeware { get; private set; }
		[JsonProperty("doujin")]
		public Boolean IsDoujin { get; private set; }
		[JsonProperty("minage")]
		public UInt32? MinimumAge { get; private set; }
		[JsonProperty("vn")]
		public ReadOnlyCollection<VisualNovelMetadata> VisualNovels { get; private set; }
		public ReadOnlyCollection<ProducerRelease> Producers { get; private set; }
	}
}
