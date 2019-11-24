﻿using System;
using Newtonsoft.Json;
using VndbSharp.Json;

namespace VndbSharp.Extensions
{
	internal static class StringExtensions
	{
		public static String[] ToVndbResults(this String response)
			=> response.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries); // Ensure we only split the first space

		public static Boolean IsEmpty(this String contents)
			=> String.IsNullOrWhiteSpace(contents);

		public static T FromJson<T>(this String json)
			=> JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings {ContractResolver = VndbContractResolver.Instance});
	}
}
