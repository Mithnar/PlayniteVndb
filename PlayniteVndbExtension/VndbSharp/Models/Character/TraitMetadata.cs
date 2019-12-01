﻿using System;
using Newtonsoft.Json.Linq;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.Character
{
	public class TraitMetadata
	{
		internal TraitMetadata(JArray array)
		{
			this.Id = array[0].Value<UInt32>();
			this.SpoilerLevel = (SpoilerLevel) array[1].Value<Int32>();
		}

		public UInt32 Id { get; private set; }
		public SpoilerLevel SpoilerLevel { get; private set; }
	}
}
