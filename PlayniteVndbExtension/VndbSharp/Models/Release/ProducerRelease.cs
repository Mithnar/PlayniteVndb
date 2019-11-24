﻿using System;
using Newtonsoft.Json;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.Release
{
	public class ProducerRelease : ProducerCommon
	{
		[JsonProperty("developer")]
		public Boolean IsDeveloper { get; private set; }
		[JsonProperty("publisher")]
		public Boolean IsPublisher { get; private set; }
	}
}
