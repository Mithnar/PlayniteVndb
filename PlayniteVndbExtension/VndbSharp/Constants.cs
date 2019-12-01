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

		#region .  Dump info  .
		public const String TagsDump = "https://vndb.org/api/tags.json.gz";
		public const String TraitsDump = "https://vndb.org/api/traits.json.gz";
		public const String VotesDump = "https://vndb.org/api/votes.gz";
		public const String VotesDump2 = "https://vndb.org/api/votes2.gz";
		#endregion

		#region .  Misc commands  .
		public const String LoginCommand = "login";
		public const String DbStatsCommand = "dbstats";
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

		#region .  Set commands  .
		public const String SetVotelistCommand = "set votelist";
		public const String SetVisualNovelListCommand = "set vnlist";
		public const String SetWishlistCommand = "set wishlist";
		#endregion

		#region .  Result values  .
		public const String Results = "results";
		public const String DbStats = "dbstats"; // Yes, this is identical to DbStatsCommand.
		public const String Error = "error";
		public const String Ok = "ok";
		#endregion

	}
}
