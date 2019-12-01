﻿using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace VndbSharp.Models.Dumps
{
	/// <summary>
	///		Represents a Tag Object from the Tags Dump
	/// </summary>
	public class Tag
	{
		/// <summary>
		///		The Id of the tag
		/// </summary>
		public UInt32 Id { get; private set; }
		/// <summary>
		///		The name of the tag
		/// </summary>
		public String Name { get; private set; }
		/// <summary>
		///		The description of the tag, which can include formatting codes described in http://vndb.org/d9.3
		/// </summary>
		public String Description { get; private set; }
		/// <summary>
		///		The number of Visual Novels with this tag
		/// </summary>
		[JsonProperty("vns")]
		public UInt32 VisualNovels { get; private set; }
		/// <summary>
		///		The category / Classification of the tag
		/// </summary>
		[JsonProperty("cat")]
		public TagCategory TagCategory { get; private set; }
		/// <summary>
		///		List of alternative names
		/// </summary>
		public ReadOnlyCollection<String> Aliases { get; private set; }
		/// <summary>
		///		List of parent Tags (Empty if Root Tag)
		/// </summary>
		public ReadOnlyCollection<UInt32> Parents { get; private set; }
		/// <summary>
		///		Undescribed Field
		/// </summary>
		[JsonProperty("meta")]
		public Boolean IsMeta { get; private set; }
	}
}
