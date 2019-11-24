﻿using VndbSharp.Attributes;

namespace VndbSharp.Models.Errors
{
	/// <summary>
	///		Represents the type of Throttling that occured
	/// </summary>
	public enum ThrottledType
	{
		/// <summary>
		///		VndbSharp was unable to determine the Throttle Type
		/// </summary>
		Unknown = 0,
		/// <summary>
		///		The throttle is due to exceeding the commands / 10 minute limit
		/// </summary>
		[RealValue("cmd")]
		Command = 1,
		/// <summary>
		///		The throttle is due to exceeding the sql time / minute limit
		/// </summary>
		Sql = 2,
	}
}