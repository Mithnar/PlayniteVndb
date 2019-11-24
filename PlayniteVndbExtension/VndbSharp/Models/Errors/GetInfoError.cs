﻿using System;

namespace VndbSharp.Models.Errors
{
	/// <summary>
	///		Unknown info flag in a "get" command
	/// </summary>
	public class GetInfoError : Error
	{
		/// <summary>
		///		The unknown flag
		/// </summary>
		public String Flag { get; private set; }
	}
}