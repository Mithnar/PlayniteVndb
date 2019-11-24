﻿using System;

namespace VndbSharp.Models.Errors
{
	/// <summary>
	/// 	An error occured in the VndbSharp Library
	/// </summary>
	public class LibraryError : Error
	{
		internal LibraryError(String message)
		{
			this.Type = ErrorType.Library;
			this.Message = message;
		}
	}
}
