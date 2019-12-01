﻿using VndbSharp.Attributes;

namespace VndbSharp.Models.VisualNovel
{
	public enum VisualNovelLength
	{
		Unknown = 0,
		[Description("< 2 hours")]
		VeryShort = 1,
		[Description("2 - 10 hours")]
		Short = 2,
		[Description("10 - 20 hours")]
		Medium = 3,
		[Description("30 - 50 hours")]
		Long = 4,
		[Description("> 50 hours")]
		VeryLong = 5,
	}
}