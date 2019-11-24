﻿using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VndbSharp.Extensions;

namespace VndbSharp.Json.Converters
{
	internal class ArrayOfArraysConverter<T> : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, new JArray(value));
		}

		public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
		{
			var ctor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, typeof(JArray));
			return ctor.Invoke(new Object[] { JArray.Load(reader) });
		}

		public override Boolean CanConvert(Type objectType)
			=> objectType == typeof(T);
	}
}
