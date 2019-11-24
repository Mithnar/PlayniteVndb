﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VndbSharp.Models.Common;

namespace VndbSharp.Json.Converters
{
	internal class SimpleDateConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}

		public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
		{
			switch (reader.TokenType)
			{
				case JsonToken.Integer:
				case JsonToken.String:
					return SimpleDateConverter.ParseString(reader.Value.ToString());
				case JsonToken.StartArray:
					return SimpleDateConverter.ParseArray(JArray.Load(reader));
				default:
					return null;
			}
		}

		public override Boolean CanConvert(Type objectType)
			=> objectType == typeof(SimpleDate);

		internal static Object ParseString(String value)
		{
			if (value.Equals("tba", StringComparison.OrdinalIgnoreCase))
				return new SimpleDate();
			var splits = value.Split(new[] { '/', '-' }, 3, StringSplitOptions.RemoveEmptyEntries);
			var times = new UInt32[splits.Length];

			for (var i = 0; i < splits.Length; i++)
			{
				if (!UInt32.TryParse(splits[i], out var time))
					break;
				times[i] = time;
			}

			if (times.Length != splits.Length) // Should only ever happen when a value isn't a number
				return null; // Not worth the risk of parsing

			switch (times.Length)
			{
				case 3:
					return new SimpleDate(times[0], (Byte) times[1], (Byte) times[2]);
				case 2:
					return new SimpleDate(times[0], (Byte) times[1]);
				default:
					return new SimpleDate(times[0]);
			}
		}

		internal static Object ParseArray(JArray array)
		{
			if (array[0].Type == JTokenType.Null)
				return null;

			var day = array[0].Value<Byte>();
			var month = array[1].Value<Byte>();

			return new SimpleDate(month, day);
		}
	}
}
