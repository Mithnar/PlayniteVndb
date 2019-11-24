﻿using System;
using VndbSharp.Models.Errors;

namespace VndbSharp.Interfaces
{
	/// <summary>
	///		Represents the Error Type and Message when Vndb Returns an Error
	/// </summary>
	public interface IVndbError
	{
		/// <summary>
		///		The type of the Error
		/// </summary>
		ErrorType Type { get; }
		/// <summary>
		///		A short Message about the error returned
		/// </summary>
		String Message { get; }
	}
}