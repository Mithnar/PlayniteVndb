﻿using System;

namespace VndbSharp.Attributes
{
	internal class RealValueAttribute : Attribute
	{
		public RealValueAttribute(String realValue)
		{
			this.RealValue = realValue;
		}

		public String RealValue { get; }
	}
}
