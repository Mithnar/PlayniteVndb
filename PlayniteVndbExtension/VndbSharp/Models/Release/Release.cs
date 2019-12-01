﻿using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.Release
{
	public class Release
	{
		public UInt32 Id { get; private set; }
		[JsonProperty("title")]
		public String Name { get; private set; }
		[JsonProperty("original")]
		public String OriginalName { get; private set; }
		public SimpleDate Released { get; private set; }
		public ReleaseType Type { get; private set; }
		[JsonProperty("patch")]
		public Boolean IsPatch { get; private set; }
		[JsonProperty("freeware")]
		public Boolean IsFreeware { get; private set; }
		[JsonProperty("doujin")]
		public Boolean IsDoujin { get; private set; }
		public ReadOnlyCollection<String> Languages { get; private set; }
		public String Website { get; private set; }
		public String Notes { get; private set; } // Possibly rename to description
		[JsonProperty("minage")]
		public UInt32? MinimumAge { get; private set; }
		/// <summary>
		///		JAN/UPC/EAN code.
		/// </summary>
		public String Gtin { get; private set; }
		public String Catalog { get; private set; }
		public String Resolution { get; private set; }
		public Voiced Voiced { get; private set; }
		/// <summary>
		///		The array has two integer members, the first one indicating the story animations, the second the ero scene animations. Both members can be null if unknown or not applicable.
		/// </summary>
		public ReadOnlyCollection<Animated> Animation { get; set; }
		public ReadOnlyCollection<String> Platforms { get; private set; }
		public ReadOnlyCollection<Media> Media { get; private set; }
		[JsonProperty("vn")]
		public ReadOnlyCollection<VisualNovelMetadata> VisualNovels { get; private set; }
		public ReadOnlyCollection<ProducerRelease> Producers { get; private set; }
	}
}
