﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace VndbSharp.Models
{
	[JsonObject]
	public class VndbResponse<T> : IEnumerable<T>
	{
		// Disable publicly constructing the Response Object
		private VndbResponse() { }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		public IEnumerator<T> GetEnumerator()
		{
			using (var iterator = this.Items.GetEnumerator())
				while (iterator.MoveNext())
					yield return iterator.Current;
		}

		[JsonProperty("more")]
		public Boolean HasMore { get; private set; }
		[JsonProperty("num")]
		public Int32 Count { get; private set; }
		public ReadOnlyCollection<T> Items { get; private set; }
	}
}
