﻿using System;

namespace VndbSharp.Models.Errors
{
	/// <summary>
	///		A JSON value is of the wrong type or in the wrong format.
	/// </summary>
	public class BadArgumentError : Error
	{
		/// <summary>
		///		The field that is incorrect
		/// </summary>
		public String Field { get; private set; }
	}
}
