﻿using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VndbSharp.Attributes;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.Character
{
    public class Character
    {
	    public UInt32 Id { get; private set; }
	    public String Name { get; private set; }
		[JsonProperty("original")]
		public String OriginalName { get; private set; }
	    public Gender? Gender { get; private set; }
		[JsonProperty("bloodt")]
		public BloodType? BloodType { get; private set; }
	    public SimpleDate Birthday { get; private set; }
//				[JsonConverter(typeof(CommaSeparatedValueConverter<String>))]
				[IsCsv]
		public ReadOnlyCollection<String> Aliases { get; private set; }
		public String Description { get; private set; }
	    public String Image { get; private set; }
		/// <summary>
		///		Size in Centimeters
		/// </summary>
	    public Int64? Bust { get; private set; }
		/// <summary>
		///		Size in Centimeters
		/// </summary>
		public Int64? Waist { get; private set; }
		/// <summary>
		///		Size in Centimeters
		/// </summary>
	    public Int64? Hip { get; private set; }
		/// <summary>
		///		Height in Centimeters
		/// </summary>
		public Int64? Height { get; private set; }
		/// <summary>
		///		Weight in Kilograms
		/// </summary>
	    public Int64? Weight { get; private set; }
	    public ReadOnlyCollection<TraitMetadata> Traits { get; private set; }
	    [JsonProperty("vns")]
	    public ReadOnlyCollection<VisualNovelMetadata> VisualNovels { get; private set; }
	    [JsonProperty("voiced")]
	    public ReadOnlyCollection<VoiceActorMetadata> VoiceActorMetadata { get; private set; }
	}
}
