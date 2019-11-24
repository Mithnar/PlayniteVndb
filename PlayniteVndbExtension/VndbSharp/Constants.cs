﻿using System;

namespace VndbSharp
{
	internal static class Constants
	{
		public const Char EotChar = '\u0004';

		#region .  Connection Info  .
		public const String ApiDomain = "api.vndb.org";
		public const UInt16 ApiPort = 19534;
		public const UInt16 ApiPortTls = 19535;
		#endregion

		#region .  Misc commands  .
		public const String LoginCommand = "login";
		#endregion

		#region .  Get commands  .
		public const String GetVisualNovelCommand = "get vn";
		public const String GetReleaseCommand = "get release";
		public const String GetProducerCommand = "get producer";
		public const String GetCharacterCommand = "get character";
		public const String GetUserCommand = "get user";
		public const String GetVotelistCommand = "get votelist";
		public const String GetVisualNovelListCommand = "get vnlist";
		public const String GetWishlistCommand = "get wishlist";
	    public const String GetStaffCommand = "get staff";
		#endregion

		#region .  Result values  .
		public const String Results = "results";
		public const String DbStats = "dbstats"; // Yes, this is identical to DbStatsCommand.
		public const String Error = "error";
		public const String Ok = "ok";
		#endregion

	}
}
