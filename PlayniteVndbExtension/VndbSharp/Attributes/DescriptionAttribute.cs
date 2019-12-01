﻿using System;

namespace VndbSharp.Attributes
{
	public class DescriptionAttribute : Attribute
	{
		public DescriptionAttribute(String description)
		{
			this.Description = description;
		}

		public String Description { get; }
	}
}