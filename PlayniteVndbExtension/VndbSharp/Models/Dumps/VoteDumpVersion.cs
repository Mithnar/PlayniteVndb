﻿namespace VndbSharp.Models.Dumps
{
	/// <summary>
	///		The version of the Vote Dump retrieved
	/// </summary>
	public enum VoteDumpVersion
	{
		/// <summary>
		///		Represents the most basic vote dump
		/// </summary>
		One = 1,
		/// <summary>
		///		Represents the v2 of the vote dump, which adds Dates
		/// </summary>
		Two = 2,
	}
}