﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Models.VisualNovel
{
	public class ScreenshotMetadata
	{
		[JsonProperty("image")]
		public String Url { get; private set; }
		[JsonProperty("rid")]
		public String ReleaseId { get; private set; }
		[JsonProperty("nsfw")]
		public Boolean IsNsfw { get; private set; }
	}
}
