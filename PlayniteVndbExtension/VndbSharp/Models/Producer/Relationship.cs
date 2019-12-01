﻿using System;

namespace VndbSharp.Models.Producer
{
	public class Relationship
	{
		public UInt32 Id { get; private set; }
		public String Relation { get; private set; } // TODO: Enum?
		public String Name { get; private set; }
		public String OriginalName { get; private set; }
	}
}