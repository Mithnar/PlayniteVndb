﻿using VndbSharp.Attributes;

namespace VndbSharp.Models.Dumps
{
	/// <summary>
	///		Indicates what type the tag is
	/// </summary>
	public enum TagCategory
	{
		/// <summary>
		///		The tag describes the contents of the game
		/// </summary>
		[RealValue("cont")]
		Content,
		/// <summary>
		///		The tag describes the sexual contents of the game
		/// </summary>
		[RealValue("ero")]
		Sexual,
		/// <summary>
		///		The tag describes the features of the game
		/// </summary>
		[RealValue("tech")]
		Technical,
	}
}