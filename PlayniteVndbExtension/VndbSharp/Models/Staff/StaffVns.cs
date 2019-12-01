﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace VndbSharp.Models.Staff
{
    public class StaffVns
    {
        [JsonProperty("id")]
        public UInt32 StaffId { get; private set; }
        [JsonProperty("aid")]
        public UInt32 AliasId { get; private set; }
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
