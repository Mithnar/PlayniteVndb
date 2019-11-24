﻿using System;
using VndbSharp.Attributes;

namespace VndbSharp.Models.Errors
{
	/// <summary>
	///		The known Error Types that VndbSharp can handle
	/// </summary>
	public enum ErrorType
	{
		/// <summary>
		///		An unexpected error occured, possibly due to a Vndb Api Update
		/// </summary>
		Unknown = 0,
		/// <summary>
		///		A Syntax Error occured, and Vndb API could not understand the request
		/// </summary>
		Parse = 1,
		/// <summary>
		///		A JSON object argument is missing a required member.
		/// </summary>
		Missing = 2,
		/// <summary>
		///		A JSON value is of the wrong type or in the wrong format.
		/// </summary>
		[RealValue("badarg")]
		BadArgument = 3,
		/// <summary>
		///		You need to be logged in to use this command
		/// </summary>
		[RealValue("needlogin")]
		LoginRequred = 4,
		/// <summary>
		///		You have used too many server resources within a short period of time, and need to wait a bit before sending the next command
		/// </summary>
		Throttled = 5,
		/// <summary>
		///		Incorrect username / password combination
		/// </summary>
		[RealValue("auth")]
		BadAuthentication = 6,
		/// <summary>
		///		Already Logged in, Only one successful login command can be issued on one connection
		/// </summary>
		LoggedIn = 7,
		/// <summary>
		///		Unknown type argument in a "get" command
		/// </summary>
		GetType = 8,
		/// <summary>
		///		Unknown info flag in a "get" command
		/// </summary>
		GetInfo = 9,
		/// <summary>
		///		Unknown filter field or invalid combination of field / operator / argument type
		/// </summary>
		[RealValue("filter")]
		InvalidFilter = 10,
		/// <summary>
		///		Unknown type argument to the "set" command
		/// </summary>
		SetType = 11,
		/// <summary>
		///		An error occured in the VndbSharp Library
		/// </summary>
		Library = Int32.MaxValue,
	}
}