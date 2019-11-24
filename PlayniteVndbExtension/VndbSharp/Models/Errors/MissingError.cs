﻿using System;

namespace VndbSharp.Models.Errors
{
	/// <summary>
	///		A JSON object argument is missing a required member.
	/// </summary>
	public class MissingError : Error
	{
		/// <summary>
		///		The field that is incorrect
		/// </summary>
		public String Field { get; private set; }
	}
}