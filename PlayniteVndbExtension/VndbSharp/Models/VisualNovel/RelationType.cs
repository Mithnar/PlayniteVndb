﻿using VndbSharp.Attributes;

namespace VndbSharp.Models.VisualNovel
{
	public enum RelationType
	{
		[RealValue("seq")]
		Sequel,
		[RealValue("preq")]
		Prequel,
		[RealValue("set"), Description("Same Setting")]
		SameSetting,
		[RealValue("alt"), Description("Alternative Version")]
		AlternativeVersion,
		[RealValue("char"), Description("Shares Characters")]
		SharesCharacters,
		[RealValue("side"), Description("Side Story")]
		SideStory,
		[RealValue("par"), Description("Parent Story")]
		ParentStory,
		[RealValue("ser"), Description("Same Series")]
		SameSeries,
		[RealValue("fan")]
		Fandisc,
		[RealValue("orig"), Description("Original Game")]
		OriginalGame,
	}
}