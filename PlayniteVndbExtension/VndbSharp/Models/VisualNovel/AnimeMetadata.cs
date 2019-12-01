﻿using System;
using Newtonsoft.Json;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.VisualNovel
{
	public class AnimeMetadata
	{
		[JsonProperty("id")]
		public Int32? AniDbId { get; private set; }
		[JsonProperty("ann_id")]
		public Int32? AnimeNewsNetworkId { get; private set; }
		[JsonProperty("nfo_id")]
		public String AnimeNfoId { get; private set; }
		[JsonProperty("title_romaji")]
		public String RomajiTitle { get; private set; }
		[JsonProperty("title_kanji")]
		public String KanjiTitle { get; private set; }
		[JsonProperty("year")]
		public SimpleDate AiringYear { get; private set; }
		[JsonProperty("type")]
		public String Type { get; private set; } // ??
	}
}
