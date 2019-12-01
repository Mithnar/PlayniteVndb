﻿using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VndbSharp.Attributes;
using VndbSharp.Extensions;
using VndbSharp.Json.Converters;
using VndbSharp.Models.Character;
using VndbSharp.Models.Common;
using VndbSharp.Models.Dumps;
using VndbSharp.Models.Errors;
using VndbSharp.Models.Release;
using VndbSharp.Models.Staff;
using VndbSharp.Models.VisualNovel;
using CharacterVisualNovelMetadata = VndbSharp.Models.Character.VisualNovelMetadata;

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
#if UserAuth 
				new SecureStringConverter(),
#endif
				new ArrayOfArraysConverter<CharacterVisualNovelMetadata>(),
				new ArrayOfArraysConverter<TraitMetadata>(),
				new ArrayOfArraysConverter<TagMetadata>(),
				new ArrayOfArraysConverter<StaffAliases>(), 
				new GenericNullableEnumConverter<Gender, Gender?>(), // Ugly hack to return null when not present
				new GenericEnumConverter<RelationType>(), 
				new GenericEnumConverter<TagCategory>(),
				new GenericEnumConverter<ErrorType>(),
				new GenericEnumConverter<ThrottledType>(), 
				new GenericEnumConverter<Voiced>(), 
				new GenericEnumConverter<Animated>(), 
			};
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var prop = base.CreateProperty(member, memberSerialization);
			// Only work with PropertyInfo's.
			if (!(member is PropertyInfo property) || property == null)
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
			if (converter != default(JsonConverter))
				return converter;
			return base.ResolveContractConverter(objectType);
		}

#if ContractResolverPropertyExceptionCheck
		// Untested
		private static Boolean ShouldSerialize(Object instance, PropertyInfo property)
		{
			try
			{
				if (!property.CanRead)
					return false;
				property.GetValue(instance);
				return true;
			}
			catch
			{
				return false;
			}
		}

		// Untested
		private static Boolean ShouldDeserialize(Object instance, PropertyInfo property)
		{
			try
			{
				if (!property.CanWrite)
					return false;
				property.SetValue(instance, property.GetValue(instance));
				return true;
			}
			catch
			{
				return false;
			}
		}
#endif

		internal static VndbContractResolver Instance
			=> VndbContractResolver._instance 
			?? (VndbContractResolver._instance = new VndbContractResolver());

		internal JsonConverter[] CustomConverters;

		internal static JsonConverter UnixConverter = new UnixTimestampConverter();
		internal static JsonConverter CsvConverter = new CommaSeparatedValueConverter<String>();

		private static VndbContractResolver _instance; // Singleton, Anti-Pattern yes, useful here? Yes.
	}
}
