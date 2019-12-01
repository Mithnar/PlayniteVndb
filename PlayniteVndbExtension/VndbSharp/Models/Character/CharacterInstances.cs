﻿using System;
using Newtonsoft.Json;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.Character
{
	public class CharacterInstances
	{
		public Int32 Id { get; private set; }
		public SpoilerLevel Spoiler { get; private set; }
		public String Name { get; private set; }
		[JsonProperty("original")]
		public String Kanji { get; private set; }
	}
}
