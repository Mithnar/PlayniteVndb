﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Json.Converters
{
	internal class UnixTimestampConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, ((DateTime) value - UnixTimestampConverter._epoch).TotalMilliseconds);
		}

		public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.Integer)
				return null;
			return UnixTimestampConverter._epoch.AddSeconds((Int64) reader.Value);
		}

		public override Boolean CanConvert(Type objectType)
			=> objectType == typeof(VndbUtils);

		private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}
