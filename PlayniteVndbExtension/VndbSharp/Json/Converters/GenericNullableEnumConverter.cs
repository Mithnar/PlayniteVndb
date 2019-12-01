﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Json.Converters
{
	internal class GenericNullableEnumConverter<TEnum, TNullable> : GenericEnumConverter<TEnum>
		where TEnum : struct, IConvertible
	{
		public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return default(TNullable);
			return base.ReadJson(reader, objectType, existingValue, serializer);
		}
	}
}
