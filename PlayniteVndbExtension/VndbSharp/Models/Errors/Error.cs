﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VndbSharp.Interfaces;
using VndbSharp.Json;

namespace VndbSharp.Models.Errors
{
	/// <summary>
	///		Represents an Error, with the most basic Id and Message
	/// </summary>
	public abstract class Error : IVndbError
	{
		internal static IVndbError Build<T>(JObject jObj)
			where T : Error
			=> new JsonSerializer { ContractResolver = VndbContractResolver.Instance }
				.Deserialize<T>(new JTokenReader(jObj));

		/// <inheritdoc cref="IVndbError.Type"/>
		[JsonProperty("id")]
		public ErrorType Type { get; internal set; }
		/// <inheritdoc cref="IVndbError.Message"/>
		[JsonProperty("msg")]
		public String Message { get; internal set; }
	}
}
