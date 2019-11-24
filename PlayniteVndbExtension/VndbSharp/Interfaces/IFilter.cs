﻿using System;

namespace VndbSharp.Interfaces
{
	/// <summary>
	///		A common interface to allow the <see cref="Vndb"/> class to determine if the filter is valid
	/// </summary>
	public interface IFilter
	{
		/// <summary>
		///		Called when constructing the filter of a request, to check that the Operator can be performed with the provided Value(s)
		/// </summary>
		/// <returns>True the current Operator can be used with the current Value(s)</returns>
		Boolean IsFilterValid();
	}
}