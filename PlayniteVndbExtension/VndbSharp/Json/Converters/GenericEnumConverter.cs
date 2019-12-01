﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using VndbSharp.Attributes;

namespace VndbSharp.Json.Converters
{
	internal class GenericEnumConverter<TEnum> : JsonConverter
		where TEnum : struct, IConvertible
	{
		public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value.ToString());
		}

		public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Integer)
				return EnumConverter<TEnum>.Instance((Int64) reader.Value);
			if (reader.TokenType == JsonToken.Null)
				return default(TEnum);
			if (reader.TokenType != JsonToken.String)
			{
				Debug.WriteLine($"Unsupported Token Type passed to GenericEnumConverter for {typeof(TEnum)}. " +
					$"Token Type is {reader.TokenType}, expected {JsonToken.Integer}, {JsonToken.Null} or {JsonToken.String}.");
				return default(TEnum); // Unsupported
			}

			var strValue = (String) reader.Value;
			if (Enum.TryParse<TEnum>(strValue, true, out var validEnum))
				return validEnum;

			var realValues = GenericEnumConverter<TEnum>.GetAttributes<RealValueAttribute, TEnum>();
			return realValues.FirstOrDefault(kv => kv.Value?.RealValue == strValue).Key;
//			return realValues.FirstOrDefault(kv => kv.Key.RealValue == strValue).Value;
		}

		public override Boolean CanConvert(Type objectType)
			=> objectType == typeof(TEnum);

		internal static ReadOnlyDictionary<Enum, TAttribute> GetAttributes<TAttribute, TEnum>()
			where TAttribute : Attribute
			where TEnum : struct, IConvertible
		{
			var results = new Dictionary<Enum, TAttribute>();

			var type = typeof(TEnum);
			var typeInfo = type.GetTypeInfo();

			foreach (Enum value in Enum.GetValues(type))
			{
				var attribute = typeInfo.GetDeclaredField(value.ToString()).GetCustomAttribute<TAttribute>();
				results.Add(value, attribute);
			}

			return new ReadOnlyDictionary<Enum, TAttribute>(results);
		}

		// Credits for this amazing snippet to Raif Atef (http://stackoverflow.com/users/111830/raif-atef), from http://stackoverflow.com/a/26289874
		internal static class EnumConverter<TEnum>
			where TEnum : struct, IConvertible
		{
			public static readonly Func<Int64, TEnum> Instance = EnumConverter<TEnum>.GenerateConverter();

			private static Func<Int64, TEnum> GenerateConverter()
			{
				var param = Expression.Parameter(typeof(Int64));
				var dynamicMethod = Expression.Lambda<Func<Int64, TEnum>>
					(Expression.ConvertChecked(param, typeof(TEnum)), param);
				return dynamicMethod.Compile();
			}
		}
	}
}
