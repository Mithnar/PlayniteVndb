﻿using System;
using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VndbSharp.Extensions;
using VndbSharp.Filters.Conditionals;
using VndbSharp.Interfaces;
using VndbSharp.Models;

namespace VndbSharp.Filters
{
	public abstract class AbstractFilter<T> : IFilter
	{
		protected AbstractFilter(T value, FilterOperator filterOperator)
		{
			this.Value = value;
			this.Operator = filterOperator;

			this.Type = typeof(T);

			this.IsArray = this.Type.GetTypeInfo().BaseType == typeof(Array);

			// This should be OrElse woops
			if (this.Type.GetTypeInfo().GenericTypeArguments.Length <= 0 && !this.IsArray)
				return;

			this.Type = this.IsArray 
				? this.Type.GetElementType() 
				: this.Type.GetTypeInfo().GenericTypeArguments[0];
			this.Count = (this.Value as IList)?.Count ?? (this.Value == null ? 0 : 1);
		}

		public override String ToString()
		{
			var res = $"{this.FilterName}{this.Operator}";

			if (this.CanBeNull && (this.Value == null))
				return $"{res}null";

			if (!this.IsArray)
				return this.Type == typeof(String) 
					? $"{res}\"{this.Value}\"" 
					: $"{res}{this.Value}";

			if (this.Count == 1 && this.IsArray)
			{
				var valueList = this.Value as IList;
				return this.Type == typeof(String)
					? $"{res}\"{valueList[0]}\""  // Allow the Null Reference to throw
					: $"{res}{valueList[0]}";  // Allow the Null Reference to throw
			}
			
			// Doesn't use the Contract Resolver, Some things *may* be funky,
			// but should be fine for value types like Int32 / String
			return $"{res}{new JArray(this.Value).ToString(Formatting.None)}";
		}

		/// <summary>
		///		Equivlant to IFilter.And(IFilter)
		/// </summary>
		public static FilterAnd operator &(AbstractFilter<T> leftFilter, IFilter rightFilter)
			=> leftFilter.And(rightFilter);

		/// <summary>
		///		Equivlant to IFilter.Or(IFilter)
		/// </summary>
		public static FilterOr operator |(AbstractFilter<T> leftFilter, IFilter rightFilter)
			=> leftFilter.Or(rightFilter);

		/// <summary>
		///		Called when constructing the filter of a request, to check that the Operator can be performed with the provided Value(s)
		/// </summary>
		/// <returns>True the current Operator can be used with the current Value(s)</returns>
		public abstract Boolean IsFilterValid();

		protected abstract FilterOperator[] ValidOperators { get; }
		protected abstract String FilterName { get; }
		protected Boolean CanBeNull = false;

		protected readonly FilterOperator Operator;
		protected readonly T Value;

		/// <summary>
		///		If an array is passed, the number of items.
		/// </summary>
		protected readonly Int32? Count;
		protected readonly Boolean IsArray;

		private Type Type;
	}
}
