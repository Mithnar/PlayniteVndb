﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.Staff
{
    public class StaffLinks: CommonLinks
    {
		public String Homepage { get; private set; }
        public String Twitter { get; private set; }
        public String AniDb { get; private set; }
	}
}
