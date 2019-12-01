﻿using VndbSharp.Attributes;

namespace VndbSharp.Models.Common
{
	public enum Gender
	{
		[RealValue("m")]
		Male,
		[RealValue("f")]
		Female,
		[RealValue("b")]
		Both
	}
}