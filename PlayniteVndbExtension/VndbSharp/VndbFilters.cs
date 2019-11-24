﻿﻿using System;
using System.Linq;
using VndbSharp.Extensions;
using VndbSharp.Filters;
using VndbSharp.Models;
using VndbSharp.Models.Common;

namespace VndbSharp
{
	// Not public yet
	public class VndbFilters
	{
		public class Id : AbstractFilter<UInt32[]>
		{
			internal Id(UInt32[] value, FilterOperator filterOperator)
				: base(value, filterOperator)
			{ }

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual, FilterOperator.LessOrEqual, FilterOperator.LessThan,
				FilterOperator.GreaterOrEqual, FilterOperator.GreaterThan
			};

			protected override String FilterName { get; } = "id";

			public static Id Equals(params UInt32[] value) => new Id(value, FilterOperator.Equal);
			public static Id NotEquals(params UInt32[] value) => new Id(value, FilterOperator.NotEqual);

			public static Id GreaterThan(UInt32 value) => new Id(new[] { value }, FilterOperator.GreaterThan);
			public static Id GreaterOrEqual(UInt32 value) => new Id(new[] { value }, FilterOperator.GreaterOrEqual);
			public static Id LessThan(UInt32 value) => new Id(new[] { value }, FilterOperator.LessThan);
			public static Id LessOrEqual(UInt32 value) => new Id(new[] { value }, FilterOperator.LessOrEqual);

			public override Boolean IsFilterValid()
			{
				if (this.Count > 1) // Only = and != are allowed when multiple values are passed
					return this.Operator == FilterOperator.Equal || this.Operator == FilterOperator.NotEqual;
				return this.ValidOperators.Contains(this.Operator);
			}
		}

		public class AliasId : AbstractFilter<UInt32[]>
		{
			internal AliasId(UInt32[] value, FilterOperator filterOperator)
				: base(value, filterOperator)
			{ }

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal
			};

			protected override String FilterName { get; } = "id";

			public static AliasId Equals(params UInt32[] value) => new AliasId(value, FilterOperator.Equal);

			public override Boolean IsFilterValid()
			{;
				return this.ValidOperators.Contains(this.Operator);
			}
		}

		public class UserId : AbstractFilter<UInt32>
		{
			private UserId(UInt32 value, FilterOperator filterOperator) : base(value, filterOperator)
			{ }

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal
			};

			protected override String FilterName { get; } = "uid";

			public static UserId Equals(UInt32 value) => new UserId(value, FilterOperator.Equal);

			public override Boolean IsFilterValid()
				=> this.Operator == FilterOperator.Equal;
		}

		public class FirstChar : AbstractFilter<Char?>
		{
			private FirstChar(Char? value, FilterOperator filterOperator)
				: base(value ?? Char.ToLower(value.Value), filterOperator) // May cause problems?
			{
				this.CanBeNull = true;
			}

			protected override FilterOperator[] ValidOperators { get; } = { FilterOperator.Equal, FilterOperator.NotEqual };

			protected override String FilterName { get; } = "firstchar";

			public static FirstChar Equals(Char? value) => new FirstChar(value, FilterOperator.Equal);
			public static FirstChar NotEquals(Char? value) => new FirstChar(value, FilterOperator.NotEqual);

			public static FirstChar Equals(Char value) => new FirstChar(value, FilterOperator.Equal);
			public static FirstChar NotEquals(Char value) => new FirstChar(value, FilterOperator.NotEqual);

			public override Boolean IsFilterValid()
				=> this.ValidOperators.Contains(this.Operator);
		}

		public class Released : AbstractFilter<String>
		{
			private Released(SimpleDate value, FilterOperator filterOperator)
				: base(value.ToString(), filterOperator)
			{
				this.CanBeNull = true;
			}

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual, FilterOperator.LessOrEqual, FilterOperator.LessThan,
				FilterOperator.GreaterOrEqual, FilterOperator.GreaterThan
			};

			protected override String FilterName { get; } = "released";

			public static Released Equals(SimpleDate value) => new Released(value, FilterOperator.Equal);
			public static Released NotEquals(SimpleDate value) => new Released(value, FilterOperator.NotEqual);
			public static Released GreaterThan(SimpleDate value) => new Released(value, FilterOperator.LessOrEqual);
			public static Released GreaterOrEqual(SimpleDate value) => new Released(value, FilterOperator.LessOrEqual);
			public static Released LessThan(SimpleDate value) => new Released(value, FilterOperator.LessOrEqual);
			public static Released LessOrEqual(SimpleDate value) => new Released(value, FilterOperator.LessOrEqual);

			public override Boolean IsFilterValid()
			{
				if (this.Value == null)
					return this.Operator == FilterOperator.Equal || this.Operator == FilterOperator.NotEqual;
				return this.Operator != FilterOperator.Fuzzy;
			}
		}

		public class Languages : AbstractFilter<String[]>
		{
			private Languages(String[] value, FilterOperator filterOperator) : base(value, filterOperator)
			{
				this.CanBeNull = true;
			}

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual
			};

			protected override String FilterName { get; } = "languages";

			public static Languages Equals(params String[] value) => new Languages(value, FilterOperator.Equal);
			public static Languages NotEquals(params String[] value) => new Languages(value, FilterOperator.NotEqual);

			public override Boolean IsFilterValid()
				=> this.ValidOperators.Contains(this.Operator);
		}

		public class OriginalName : AbstractFilter<String>
		{
			public OriginalName(String value, FilterOperator filterOperator) : base(value, filterOperator)
			{
				this.CanBeNull = true;
			}

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual, FilterOperator.Fuzzy
			};

			protected override String FilterName { get; } = "original";

			public static OriginalName Equals(String value) => new OriginalName(value, FilterOperator.Equal);
			public static OriginalName NotEquals(String value) => new OriginalName(value, FilterOperator.NotEqual);
			public static OriginalName Fuzzy(String value) => new OriginalName(value, FilterOperator.Fuzzy);

			public override Boolean IsFilterValid()
			{
				if (this.Value == null)
					return this.Operator == FilterOperator.Equal || this.Operator == FilterOperator.NotEqual;
				return this.Operator == FilterOperator.Equal || this.Operator == FilterOperator.NotEqual ||
					   this.Operator == FilterOperator.Fuzzy;
			}
		}

		public class OriginalLanguage : AbstractFilter<String[]>
		{
			private OriginalLanguage(String[] value, FilterOperator filterOperator) : base(value, filterOperator)
			{ }

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual
			};

			protected override String FilterName { get; } = "orig_lang";

			public static OriginalLanguage Equals(params String[] value) => new OriginalLanguage(value, FilterOperator.Equal);
			public static OriginalLanguage NotEquals(params String[] value) => new OriginalLanguage(value, FilterOperator.NotEqual);
			
			public override Boolean IsFilterValid()
				=> this.ValidOperators.Contains(this.Operator);
		}

		public class Platforms : AbstractFilter<String[]>
		{
			private Platforms(String[] value, FilterOperator filterOperator) : base(value, filterOperator)
			{
				this.CanBeNull = true;
			}

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual
			};

			protected override String FilterName { get; } = "platforms";
			
			public static Platforms Equals(params String[] value) => new Platforms(value, FilterOperator.Equal);
			public static Platforms NotEquals(params String[] value) => new Platforms(value, FilterOperator.NotEqual);

			public override Boolean IsFilterValid()
				=> this.ValidOperators.Contains(this.Operator);
		}

		public class Search : AbstractFilter<String>
		{
			private Search(String value, FilterOperator filterOperator) : base(value, filterOperator)
			{ }

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Fuzzy
			};

			protected override String FilterName { get; } = "search";

			public static Search Fuzzy(String value) => new Search(value, FilterOperator.Fuzzy);

			public override Boolean IsFilterValid()
				=> this.Operator == FilterOperator.Fuzzy;
		}

		public class Tags : AbstractFilter<Int32[]>
		{
			private Tags(Int32[] value, FilterOperator filterOperator) : base(value, filterOperator)
			{ }

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual
			};

			protected override String FilterName { get; } = "tags";

			public static Tags Equals(params Int32[] value) => new Tags(value, FilterOperator.Equal);
			public static Tags NotEquals(params Int32[] value) => new Tags(value, FilterOperator.NotEqual);

			public override Boolean IsFilterValid()
				=> this.ValidOperators.Contains(this.Operator);
		}

		// So kindly stolen from http://vndb.org/d11
		/// <summary>
		///		<para>Find chars by traits. When providing an array of ints, the '=' filter will return chars that are linked to any (not all) of the given traits, the '!=' filter will return chars that are not linked to any of the given traits. You can combine multiple trait filters with 'and' and 'or' to get the exact behavior you need.</para>
		///		<para>This filter may used cached data, it may take up to 24 hours before a char entry will have its traits updated with respect to this filter.</para>
		///		<para>Chars that are linked to childs of the given trait are also included.</para>
		///		<para>Be warned that this filter ignores spoiler settings, fetch the traits associated with the returned char to verify the spoiler level.</para>
		/// </summary>
		public class Traits : AbstractFilter<UInt32[]>
		{
			private Traits(UInt32[] value, FilterOperator filterOperator) : base(value, filterOperator)
			{ }

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual
			};

			protected override String FilterName { get; } = "traits";

			public static Traits Equals(params UInt32[] value) => new Traits(value, FilterOperator.Equal);
			public static Traits NotEquals(params UInt32[] value) => new Traits(value, FilterOperator.NotEqual);

			public override Boolean IsFilterValid()
				=> this.ValidOperators.Contains(this.Operator);
		}

		public class Title : AbstractFilter<String>
		{
			private Title(String value, FilterOperator filterOperator) : base(value, filterOperator)
			{ }

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual, FilterOperator.Fuzzy
			};

			protected override String FilterName { get; } = "title";

			public static Title Equals(String value) => new Title(value, FilterOperator.Equal);
			public static Title NotEquals(String value) => new Title(value, FilterOperator.NotEqual);
			public static Title Fuzzy(String value) => new Title(value, FilterOperator.Fuzzy);

			public override Boolean IsFilterValid()
				=> this.ValidOperators.Contains(this.Operator);
		}

		public class Name : AbstractFilter<String>
		{
			private Name(String value, FilterOperator filterOperator) : base(value, filterOperator)
			{ }

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual, FilterOperator.Fuzzy
			};

			protected override String FilterName { get; } = "name";

			public static Name Equals(String value) => new Name(value, FilterOperator.Equal);
			public static Name NotEquals(String value) => new Name(value, FilterOperator.NotEqual);
			public static Name Fuzzy(String value) => new Name(value, FilterOperator.Fuzzy);

			public override Boolean IsFilterValid()
				=> this.ValidOperators.Contains(this.Operator);
		}

		public class Username : AbstractFilter<String[]>
		{
			private Username(String[] value, FilterOperator filterOperator) : base(value, filterOperator)
			{ }

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual, FilterOperator.Fuzzy
			};

			protected override String FilterName { get; } = "username";
			
			public static Username Equals(params String[] value) => new Username(value, FilterOperator.Equal);
			public static Username NotEquals(params String[] value) => new Username(value, FilterOperator.NotEqual);
			public static Username Fuzzy(params String[] value) => new Username(value, FilterOperator.Fuzzy);

			public override Boolean IsFilterValid()
			{
				if (this.Value.Length > 1)
					return this.Operator == FilterOperator.Equal;
				return this.ValidOperators.Contains(this.Operator);
			}
		}

		public class VisualNovel : AbstractFilter<UInt32[]>
		{
			private VisualNovel(UInt32[] value, FilterOperator filterOperator) : base(value, filterOperator)
			{ }

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual, FilterOperator.LessOrEqual, FilterOperator.LessThan,
				FilterOperator.GreaterOrEqual, FilterOperator.GreaterThan
			};

			protected override String FilterName { get; } = "vn";

			public static VisualNovel Equals(params UInt32[] value) => new VisualNovel(value, FilterOperator.Equal);
			public static VisualNovel NotEquals(params UInt32[] value) => new VisualNovel(value, FilterOperator.NotEqual);

			public static VisualNovel GreaterThan(UInt32 value) => new VisualNovel(new[] { value }, FilterOperator.GreaterThan);
			public static VisualNovel GreaterOrEqual(UInt32 value) => new VisualNovel(new[] { value }, FilterOperator.GreaterOrEqual);
			public static VisualNovel LessThan(UInt32 value) => new VisualNovel(new[] { value }, FilterOperator.LessThan);
			public static VisualNovel LessOrEqual(UInt32 value) => new VisualNovel(new[] { value }, FilterOperator.LessOrEqual);

			// This may fail on filters where vn is only =...
			public override Boolean IsFilterValid()
				=> this.Count > 1
					? this.Operator == FilterOperator.Equal || this.Operator == FilterOperator.NotEqual
					: this.ValidOperators.Contains(this.Operator);
		}

		public class Platform : AbstractFilter<String[]>
		{
			private Platform(String[] value, FilterOperator filterOperator) : base(value, filterOperator)
			{ }

			protected override FilterOperator[] ValidOperators { get; } = {
				FilterOperator.Equal, FilterOperator.NotEqual
			};

			protected override String FilterName { get; } = "platforms";

			public static Platform Equals(params String[] value) => new Platform(value, FilterOperator.Equal);
			public static Platform NotEquals(params String[] value) => new Platform(value, FilterOperator.NotEqual);

			public override Boolean IsFilterValid()
				=> this.ValidOperators.Contains(this.Operator);
		}

		public class Raw : AbstractFilter<String>
		{
			private Raw(String name, String value, FilterOperator filterOperator)
				: base(value, filterOperator)
			{
				this.FilterName = name;
				this.CanBeNull = true;
			}

			// This being blank should be fine, since we won't be using it.
			protected override FilterOperator[] ValidOperators { get; } = { };

			protected override String FilterName { get; }

			public static Raw Create(String name, String value, FilterOperator filterOperator) 
				=> new Raw(name, value, filterOperator);

			// This is a filter we will rely on vndb yelling at us about.
			public override Boolean IsFilterValid() 
				=> true;
		}
	}
}
