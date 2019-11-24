﻿using System;
using VndbSharp.Filters.Conditionals;
using VndbSharp.Interfaces;

namespace VndbSharp.Extensions
{
	public static class FilterExtensions
	{
		public static FilterAnd And(this IFilter filterA, IFilter filterB) => new FilterAnd(filterA, filterB);
		public static FilterOr Or(this IFilter filterA, IFilter filterB) => new FilterOr(filterA, filterB);


		internal static void ThrowIfNull(this IFilter filter)
		{
			if (filter == null)
				throw new ArgumentNullException("The provided filter is null");
		}
	}
}
