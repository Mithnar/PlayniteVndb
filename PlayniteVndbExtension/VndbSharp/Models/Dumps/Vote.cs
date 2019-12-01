﻿using System;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.Dumps
{
	public class Vote
	{
		internal Vote(VoteDumpVersion version, UInt32 visualNovelId, UInt32 userId, Byte value, SimpleDate addedOn)
		{
			this.Version = version;
			this.VisualNovelId = visualNovelId;
			this.UserID = userId;
			this.Value = value;
			this.AddedOn = addedOn;
		}

		public VoteDumpVersion Version { get; }
		public UInt32 VisualNovelId { get; }
		public UInt32 UserID { get; }
		public Byte Value { get; }
		public SimpleDate AddedOn { get; }
	}
}
