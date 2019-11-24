﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Models.Errors
{
	/// <summary>
	///		The Throttled Error. Occurs when the Vndb API is throttling you
	/// </summary>
	public class ThrottledError : Error
	{
		/// <summary>
		///		The type of Throttling that occured
		/// </summary>
		[JsonProperty("type")]
		public ThrottledType ThrottledType { get; private set; }
		/// <summary>
		///		<para>How long until you can start sending commands again</para>
		///		<para>To check, do a comparsion of if (<see cref="DateTime.Now"/> > <see cref="MinimumWait"/>)</para>
		/// </summary>
		[JsonProperty("minwait")]
		public DateTimeOffset MinimumWait { get; private set; }
		/// <summary>
		///		<para>The recommended amount of time to wait before sending commands again</para>
		///		<para>To check, do a comparsion of 'if (<see cref="DateTime.Now"/> > <see cref="FullWait"/>)'</para>
		/// </summary>
		public DateTimeOffset FullWait { get; private set; }
	}
}
