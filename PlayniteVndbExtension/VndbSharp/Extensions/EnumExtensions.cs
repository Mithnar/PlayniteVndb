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
		/// <summary>
		///		Retrieves the Description metadata from the VisualNovelLength
		/// </summary>
		/// <returns>The Vndb-like length string for the provided length</returns>
		public static String Description(this VisualNovelLength length)
			=> length.GetAttributeValue<DescriptionAttribute>(a => a.Description);

		/// <summary>
		///		Retrieves the Description metadata from the VisualNovelLength
		/// </summary>
		/// <returns>The Vndb-like length string for the provided length</returns>
		public static String Description(this VisualNovelLength? length)
			=> length.HasValue 
				? length.Value.GetAttributeValue<DescriptionAttribute>(a => a.Description) 
				: String.Empty;

		// Should likely be in a unique class...but meh
		internal static String AsString(this VndbFlags flags, String method)
			=> String.Join(",", VndbUtils.ConvertFlagsToString(method, flags).Distinct());

		internal static String GetAttributeValue<T>(this Enum from, Func<T, String> resolver)
			where T : Attribute
		{
			var fi = from.GetType().GetTypeInfo().GetDeclaredField(from.ToString());
			if (fi == null)
				return String.Empty;
			var attribute = fi.GetCustomAttribute<T>();
			return resolver(attribute);
		}
	}
}
