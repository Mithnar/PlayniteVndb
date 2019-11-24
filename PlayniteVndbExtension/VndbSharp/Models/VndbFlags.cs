﻿using System;
using VndbSharp.Attributes;

namespace VndbSharp.Models
{
	/// <summary>
	///		The Flag Values for Commands
	/// </summary>
	[Flags]
	public enum VndbFlags
	{
		/// <summary>
		///		<para>The most basic of information for the command</para>
		///		<para>Valid on: Vn, Release, Producer, Character, User, Votelist, Vnlist, Wishlist</para>
		/// </summary>
		[FlagIdentity("basic")]
		Basic = 1 << 0,

		/// <summary>
		///		<para>Provides more detailed information</para>
		///		<para>Valid on: Vn, Release, Producer, Character</para>
		/// </summary>
		[FlagIdentity("details")]
		Details = 1 << 1,

		/// <summary>
		///		<para>Provides metadata of anime adaptions for the visual novel</para>
		///		<para>Valid on: Vn</para>
		/// </summary>
		[FlagIdentity("anime")]
		Anime = 1 << 2,

		/// <summary>
		///		<para>Provides metadata on related visual novel(s)</para>
		///		<para>Valid on: Vn, Producer</para>
		/// </summary>
		[FlagIdentity("relations")]
		Relations = 1 << 3,

		/// <summary>
		///		<para>Provides a list of tag metadata linked to the visual novel</para>
		///		<para>Valid on: Vn</para>
		/// </summary>
		[FlagIdentity("tags")]
		Tags = 1 << 4,

		/// <summary>
		///		<para>Provides user voting stats</para>
		///		<para>Valid on: Vn</para>
		/// </summary>
		[FlagIdentity("stats")]
		Stats = 1 << 5,

		/// <summary>
		///		<para>Provides screenshot metadata</para>
		///		<para>Valid on: Vn</para>
		/// </summary>
		[FlagIdentity("screens")]
		Screenshots = 1 << 6,

		/// <summary>
		///		<para>Provides visual novel metadata</para>
		///		<para>Valid on: Release, Character</para>
		/// </summary>
		[FlagIdentity("vn")]
		VisualNovels = 1 << 7,

		/// <summary>
		///		<para>Provides producers metadata</para>
		///		<para>Valid on: Release</para>
		/// </summary>
		[FlagIdentity("producers")]
		Producers = 1 << 8,

		/// <summary>
		///		<para>Provides the characters 3 sizes, height and weight</para>
		///		<para>Valid on: Release, Producer, Character</para>
		/// </summary>
		[FlagIdentity("meas")]
		Measurements = 1 << 9,

		/// <summary>
		///		<para>Provides the metadata for the traits linked to the character</para>
		///		<para>Valid on: Character</para>
		/// </summary>
		[FlagIdentity("traits")]
		Traits = 1 << 10,

		/// <summary>
		///		<para>Provides the staff who worked on the VN</para>
		///		<para>Valid on: Vn</para>
		/// </summary>
		[FlagIdentity("staff")]
		Staff = 1 << 11,

	    /// <summary>
	    ///		<para>Provides the list of voice actresses who worked on the VN</para>
	    ///		<para>Valid on: Character, Staff</para>
	    /// </summary>
		[FlagIdentity("voiced")]
		Voiced = 1 << 12,

		/// <summary>
		///		<para>Provides a list of instances, of the character (excluding the current instance)</para>
		///		<para>Valid on: Character</para>
		/// </summary>
		[FlagIdentity("instances")]
		Instances = 1 << 13,


		/// <summary>
		///		<para>Provides the aliases of the staff who worked on the VN</para>
		///		<para>Valid on: Staff</para>
		/// </summary>
		[FlagIdentity("aliases")]
		Aliases = 1 << 14,

		/// <summary>
		///		Equivlant to <see cref="Basic"/> | <see cref="Details"/> | <see cref="Anime"/> | <see cref="Relations"/> | <see cref="Tags"/> | <see cref="Stats"/> | <see cref="Screenshots"/> | <see cref="Staff"/>
		/// </summary>
		FullVisualNovel = VndbFlags.Basic | VndbFlags.Details | VndbFlags.Anime | VndbFlags.Relations | VndbFlags.Tags | VndbFlags.Stats | VndbFlags.Screenshots | VndbFlags.Staff,

		/// <summary>
		///		Equivlant to <see cref="Basic"/> | <see cref="Details"/> | <see cref="VisualNovel"/> | <see cref="Producers"/>
		/// </summary>
		FullRelease = VndbFlags.Basic | VndbFlags.Details | VndbFlags.VisualNovels | VndbFlags.Producers,

		/// <summary>
		///		Equivlant to <see cref="Basic"/> | <see cref="Details"/> | <see cref="Relations"/>
		/// </summary>
		FullProducer = VndbFlags.Basic | VndbFlags.Details | VndbFlags.Relations,
		
		/// <summary>
		///		Equivlant to <see cref="Basic"/> | <see cref="Details"/> | <see cref="Measurements"/> | <see cref="Traits"/> | <see cref="VisualNovels" /> | <see cref="Voiced"/> | <see cref="Instances"/>
		/// </summary>
		FullCharacter = VndbFlags.Basic | VndbFlags.Details | VndbFlags.Measurements | VndbFlags.Traits | VndbFlags.VisualNovels | VndbFlags.Voiced | VndbFlags.Instances,

	    /// <summary>
	    ///		Equivlant to <see cref="Basic"/> | <see cref="Details"/> | <see cref="VisualNovels"/> | <see cref="Voiced" />
	    /// </summary>
		FullStaff = VndbFlags.Basic | VndbFlags.Details | VndbFlags.VisualNovels | VndbFlags.Voiced | VndbFlags.Aliases,

		/// <summary>
		///		Equivlant to <see cref="Basic"/>
		/// </summary>
		[FlagIdentity("basic")]
		FullUser = VndbFlags.Basic,

		/// <summary>
		///		Equivlant to <see cref="Basic"/>
		/// </summary>
		[FlagIdentity("basic")]
		FullVotelist = VndbFlags.Basic,

		/// <summary>
		///		Equivlant to <see cref="Basic"/>
		/// </summary>
		[FlagIdentity("basic")]
		FullVisualNovelList = VndbFlags.Basic,

		/// <summary>
		///		Equivlant to <see cref="Basic"/>
		/// </summary>
		[FlagIdentity("basic")]
		FullWishlist = VndbFlags.Basic,
	}
}
