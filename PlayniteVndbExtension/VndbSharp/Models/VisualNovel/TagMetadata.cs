﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.VisualNovel
{
	public class TagMetadata
	{
		internal TagMetadata(JArray array)
		{
			this.Id = array[0].Value<UInt32>();
			this.Score = array[1].Value<Single>();
			this.SpoilerLevel = (SpoilerLevel) array[2].Value<Int32>();
		}

		public UInt32 Id { get; private set; }
		public Single Score { get; private set; }
		[JsonProperty("spoiler")]
		public SpoilerLevel SpoilerLevel { get; private set; }
	}
}
