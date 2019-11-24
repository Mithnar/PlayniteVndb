﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace VndbSharp.Json.Converters
{
	internal class CommaSeparatedValueConverter<T> : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
		{
			var values = value as IEnumerable<T>;
			serializer.Serialize(writer, values == null ? null : String.Join(",", values));
		}

		// Yay, one liners! Anyone reading this, don't do this.
		// Yay, brittle one liners! If the value cannot be casted to T, then it will throw an error :s
public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
	=> new ReadOnlyCollection<T>(reader.Value?.ToString().Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries).Cast<T>().ToList() ?? new List<T>());

		public override Boolean CanConvert(Type objectType)
			=> objectType == typeof(ReadOnlyCollection<T>);
	}
}
