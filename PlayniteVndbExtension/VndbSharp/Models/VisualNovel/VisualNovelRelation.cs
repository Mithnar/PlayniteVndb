﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Models.VisualNovel
{
	public class VisualNovelRelation
	{
		public Int32 Id { get; private set; }
		[JsonProperty("relation")]
		public RelationType Type { get; private set; }
		public String Title { get; private set; }
		public String Original { get; private set; }
		public Boolean Official { get; private set; }
	}
}
