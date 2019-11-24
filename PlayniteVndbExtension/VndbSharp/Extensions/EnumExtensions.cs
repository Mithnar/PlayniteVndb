﻿using System;
using System.Linq;
using System.Reflection;
using VndbSharp.Attributes;
using VndbSharp.Models;
using VndbSharp.Models.VisualNovel;

namespace VndbSharp.Extensions
{
	/// <summary>
	///		Helper methods to handle common use cases of VndbSharp provided Enums
	/// </summary>
	public static class EnumExtensions
	{
		// Should likely be in a unique class...but meh
		internal static String AsString(this VndbFlags flags, String method)
			=> String.Join(",", VndbUtils.ConvertFlagsToString(method, flags).Distinct());
	}
}
