﻿using System;
using Newtonsoft.Json;

namespace VndbSharp.Interfaces
{
	/// <summary>
	///		The Request Options to send along to Vndb with the request
	/// </summary>
	public interface IRequestOptions
	{
	    /// <summary>
	    ///		The page to get the result from
	    /// </summary>
	    [JsonProperty("page")]
	    Int32? Page { get; }
	    /// <summary>
	    ///		How many Results to get
	    /// </summary>
	    [JsonProperty("results")]
	    Int32? Count { get; }
	    /// <summary>
	    ///		 The field to order the results by. The accepted field names differ per type, the default sort field is the ID of the database entry.
	    /// </summary> 
	    [JsonProperty("sort")]
	    String Sort { get; }
	    /// <summary>
	    ///		Sets if the order of the results be reversed or not.
	    /// </summary>
	    [JsonProperty("reverse")]
	    Boolean? Reverse { get; }
	}
}