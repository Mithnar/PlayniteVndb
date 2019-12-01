﻿using System;
using Newtonsoft.Json.Linq;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.Character
{
	public class VisualNovelMetadata
	{
		internal VisualNovelMetadata(UInt32 Id, UInt32 ReleaseId, SpoilerLevel spoilerLevel, CharacterRole characterRole)
		{
			this.Id = Id;
			this.ReleaseId = ReleaseId;
			this.SpoilerLevel = spoilerLevel;
			this.Role = characterRole;
		}

		internal VisualNovelMetadata(JArray array)
		{
			this.Id = array[0].Value<UInt32>();
			this.ReleaseId = array[1].Value<UInt32>();
			this.SpoilerLevel = (SpoilerLevel) array[2].Value<Int32>();
			this.Role = (CharacterRole) Enum.Parse(typeof(CharacterRole), array[3].Value<String>(), true);
		}

		public UInt32 Id { get; private set; }
		public UInt32 ReleaseId { get; private set; }
		public SpoilerLevel SpoilerLevel { get; private set; }
		public CharacterRole Role { get; private set; }
	}
}
