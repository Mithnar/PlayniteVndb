﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Models.Common
{
    public class CommonLinks
	{
		[JsonProperty("wikipedia")]
		public String Wikipedia { get; private set; }
	}
}
