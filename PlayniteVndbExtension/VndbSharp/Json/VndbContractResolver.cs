﻿using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VndbSharp.Attributes;
using VndbSharp.Extensions;
using VndbSharp.Json.Converters;
 using VndbSharp.Models.Errors;
using VndbSharp.Models.VisualNovel;

namespace VndbSharp.Json
{
	internal class VndbContractResolver : DefaultContractResolver
	{
		private VndbContractResolver()
		{
			this.CustomConverters = new JsonConverter[]
			{
				new SimpleDateConverter(),
				new DurationToDateTimeOffsetConverter(),
				new ArrayOfArraysConverter<TagMetadata>(),
				new GenericEnumConverter<ErrorType>(),
				new GenericEnumConverter<ThrottledType>(), 
			};
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var prop = base.CreateProperty(member, memberSerialization);
			// Only work with PropertyInfo's.
			if (!(member is PropertyInfo property))
				return prop;

			if (!prop.Writable)
				prop.Writable = property?.SetMethod != null;

#if ContractResolverPropertyExceptionCheck
			// Unknown performance penalty.
			prop.ShouldSerialize = i => VndbContractResolver.ShouldSerialize(i, property);
			prop.ShouldDeserialize = i => VndbContractResolver.ShouldDeserialize(i, property);
#endif

			// Need to use JsonProperty.MemberConverter here, not JsonProperty.Converter. Not entirely sure why :s
			if (property.HasAttribute<IsUnixTimestampAttribute>() && prop.PropertyType == typeof(DateTime))
				prop.MemberConverter = VndbContractResolver.UnixConverter;

			if (property.HasAttribute<IsCsvAttribute>())
				prop.MemberConverter = VndbContractResolver.CsvConverter;

			return prop;
		}

		protected override JsonConverter ResolveContractConverter(Type objectType)
		{
			var converter = this.CustomConverters.FirstOrDefault(c => c.CanConvert(objectType));
			return converter ?? base.ResolveContractConverter(objectType);
		}

		internal static VndbContractResolver Instance
			=> VndbContractResolver._instance 
			?? (VndbContractResolver._instance = new VndbContractResolver());

		internal JsonConverter[] CustomConverters;

		internal static JsonConverter UnixConverter = new UnixTimestampConverter();
		internal static JsonConverter CsvConverter = new CommaSeparatedValueConverter<String>();

		private static VndbContractResolver _instance; // Singleton, Anti-Pattern yes, useful here? Yes.
	}
}
