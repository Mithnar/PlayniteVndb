﻿using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
 using Playnite.SDK;
 using VndbSharp.Attributes;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.VisualNovel
{
	public class VisualNovel: GenericItemOption
	{
		private VisualNovel() { }

		public UInt32 Id { get; private set; }
		[JsonProperty("title")]
		public String Name { get; set; }
		[JsonProperty("original")]
		public String OriginalName { get; private set; }
		public SimpleDate Released { get; private set; } // Release or Released?
		[JsonProperty("orig_lang")]
		public ReadOnlyCollection<String> OriginalLanguages { get; private set; }
		public String Description { get; private set; }
		[JsonProperty("links")]
		public VisualNovelLinks VisualNovelLinks { get; private set; }
		public String Image { get; private set; }
		[JsonProperty("image_nsfw")]
		public Boolean IsImageNsfw { get; private set; }
		public ReadOnlyCollection<TagMetadata> Tags { get; private set; }
		public double Rating { get; private set; }
		[JsonProperty("screens")]
		public ReadOnlyCollection<ScreenshotMetadata> Screenshots { get; private set; }
	}
}
