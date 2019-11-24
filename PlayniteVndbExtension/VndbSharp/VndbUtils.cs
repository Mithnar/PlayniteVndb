﻿﻿using System;
  using System.Collections.Generic;
  using System.Reflection;
  using VndbSharp.Attributes;
  using VndbSharp.Models;

  namespace VndbSharp
{
	/// <summary>
	///		Some misc utilities such as retrieving the Database Dumps
	/// </summary>
	public static class VndbUtils
	{
		internal static String ClientName { get; set; } = "VndbSharp";
		internal static String ClientVersion { get; set; } = "0.2";

		public static Boolean ValidateFlagsByMethod(String method, VndbFlags flags, out VndbFlags invalidFlags)
		{
			VndbFlags fullFlags;

			switch (method)
			{
				case Constants.GetVisualNovelCommand:
					fullFlags = VndbFlags.FullVisualNovel;
					break;
				case Constants.GetReleaseCommand:
					fullFlags = VndbFlags.FullRelease;
					break;
				case Constants.GetProducerCommand:
					fullFlags = VndbFlags.FullProducer;
					break;
				case Constants.GetCharacterCommand:
					fullFlags = VndbFlags.FullCharacter;
					break;
				case Constants.GetUserCommand:
					fullFlags = VndbFlags.FullUser;
					break;
				case Constants.GetVotelistCommand:
					fullFlags = VndbFlags.FullVotelist;
					break;
				case Constants.GetVisualNovelListCommand:
					fullFlags = VndbFlags.FullVisualNovelList;
					break;
				case Constants.GetWishlistCommand:
					fullFlags = VndbFlags.FullWishlist;
					break;
				case Constants.GetStaffCommand:
				    fullFlags = VndbFlags.FullStaff;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(method));
			}

			invalidFlags = flags & ~fullFlags;
			return invalidFlags == 0;
		}

		internal static IEnumerable<String> ConvertFlagsToString(String method, VndbFlags flags)
		{
			var type = typeof(VndbFlags);
			var typeInfo = type.GetTypeInfo();
			foreach (Enum value in Enum.GetValues(type))
			{
				if (!flags.HasFlag(value))
					continue;

				var fi = typeInfo.GetDeclaredField(value.ToString());
				var identity = fi.GetCustomAttribute<FlagIdentityAttribute>();

				if (identity == null)
					continue;

				if ((method == Constants.GetStaffCommand || method == Constants.GetCharacterCommand) && 
					(VndbFlags) value == VndbFlags.VisualNovels)
					yield return $"{identity.Identity}s"; // Ugly hack to work around *two* vn(s) flags
				else yield return identity.Identity;
			}
		}
	}
}
