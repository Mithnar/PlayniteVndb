﻿using System;

namespace VndbSharp.Attributes
{
	internal class FlagIdentityAttribute : Attribute
	{
		public FlagIdentityAttribute(String identity)
		{
			this.Identity = identity;
		}

		public String Identity { get; private set; }
	}
}
