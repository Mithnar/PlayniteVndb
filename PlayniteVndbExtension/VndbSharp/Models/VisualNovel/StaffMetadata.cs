﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace VndbSharp.Models.VisualNovel
{
	public class StaffMetadata
	{
		private StaffMetadata() { }

		[JsonProperty("sid")]
		public UInt32 StaffId { get; private set; }
		[JsonProperty("aid")]
		public UInt32 AliasId { get; private set; }
		public String Name { get; private set; }
		public String Kanji { get; private set; }
		/// <summary>
		///		The role they served as staff
		/// </summary>
		public String Role { get; private set; } // TODO: Convert to enum
		/// <summary>
		///		Contains more info on their role as staff
		/// </summary>
		public String Note { get; private set; }
	}
}
