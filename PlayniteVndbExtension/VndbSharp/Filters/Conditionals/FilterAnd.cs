﻿using System;
using VndbSharp.Extensions;
using VndbSharp.Interfaces;

namespace VndbSharp.Filters.Conditionals
{
	public class FilterAnd : IFilter
	{
		public FilterAnd(IFilter leftFilter, IFilter rightFilter)
		{
			leftFilter.ThrowIfNull();
			rightFilter.ThrowIfNull();
			this.LeftFilter = leftFilter;
			this.RightFilter = rightFilter;
		}

		public override String ToString()
		{
			if (!this.IsFilterValid())
				throw new ArgumentOutOfRangeException("filters", "One of the provided filters are not valid.");
			return $"({this.LeftFilter} and {this.RightFilter})";
		}

		/// <summary>
		///		Called when constructing the filter of a request, to check that the Operator can be performed with the provided Value(s)
		/// </summary>
		/// <returns>True the current Operator can be used with the current Value(s)</returns>
		public Boolean IsFilterValid()
		{
			return this.LeftFilter.IsFilterValid() && this.RightFilter.IsFilterValid();
		}

		/// <summary>
		///		Equivlant to IFilter.And(IFilter)
		/// </summary>
		public static FilterAnd operator &(FilterAnd leftFilter, IFilter rightFilter)
			=> leftFilter.And(rightFilter);

		/// <summary>
		///		Equivlant to IFilter.Or(IFilter)
		/// </summary>
		public static FilterOr operator |(FilterAnd leftFilter, IFilter rightFilter)
			=> leftFilter.Or(rightFilter);

		/// <summary>
		///		The Filter on the Left, or First Condition
		/// </summary>
		public IFilter LeftFilter { get; }
		/// <summary>
		///		The Filter on the Right, or Last Condition
		/// </summary>
		public IFilter RightFilter { get; }
	}
}
