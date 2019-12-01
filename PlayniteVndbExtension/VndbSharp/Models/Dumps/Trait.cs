﻿using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace VndbSharp.Models.Dumps
{
	/// <summary>
	/// Represents a Trait Object from the Traits Dump
	/// </summary>
	public class Trait
	{
		/// <summary>
		///		The Id of the trait
		/// </summary>
		public UInt32 Id { get; private set; }
		/// <summary>
		///		The name of the trait
		/// </summary>
		public String Name { get; private set; }
		/// <summary>
		///		The description of the trait, which can include formatting codes described in http://vndb.org/d9.3
		/// </summary>
		public String Description { get; private set; }
		/// <summary>
		///		The number of characters with this trait
		/// </summary>
		[JsonProperty("chars")]
		public UInt32 Characters { get; private set; }
		/// <summary>
		///		List of alternative names
		/// </summary>
		public ReadOnlyCollection<String> Aliases { get; private set; }
		/// <summary>
		///		List of parent traits (Empty if root)
		/// </summary>
		public ReadOnlyCollection<UInt32> Parents { get; private set; }
		/// <summary>
		///		Undescribed Field
		/// </summary>
		[JsonProperty("meta")]
		public Boolean IsMeta { get; private set; }
	}
}