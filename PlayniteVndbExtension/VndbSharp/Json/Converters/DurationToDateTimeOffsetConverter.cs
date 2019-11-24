﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Json.Converters
{
	internal class DurationToDateTimeOffsetConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
		{
			// Is this the correct order? Or should it be 'value - Now'?
			serializer.Serialize(writer, (DateTime.Now - (DateTimeOffset) value).TotalSeconds);
		}

		public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.Float && reader.TokenType != JsonToken.Integer) // Is float the type we want here :?
				return default(DateTimeOffset);
			
			var seconds = Double.Parse(reader.Value.ToString());
			return new DateTimeOffset(DateTime.Now).AddSeconds(seconds);
		}

		public override Boolean CanConvert(Type objectType)
			=> objectType == typeof(DateTimeOffset);
	}
}
