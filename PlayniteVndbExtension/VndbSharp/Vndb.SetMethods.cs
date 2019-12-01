﻿using System;
using System.Threading.Tasks;
using VndbSharp.Models.Common;

namespace VndbSharp
{
	public partial class Vndb
	{
		public async Task<Boolean> SetVoteListAsync(UInt32 id, Byte? vote)
			=> await this.SendSetRequestInternalAsync(Constants.SetVotelistCommand, id, vote.HasValue ? new { vote } : null)
				.ConfigureAwait(false);

		public async Task<Boolean> SetVisualNovelListAsync(UInt32 id, Status? status)
			=> await this.SendSetRequestInternalAsync(Constants.SetVisualNovelListCommand, id, status.HasValue ? new { status } : null)
				.ConfigureAwait(false);

		public async Task<Boolean> SetVisualNovelListAsync(UInt32 id, String notes)
			=> await this.SendSetRequestInternalAsync(Constants.SetVisualNovelListCommand, id, new { notes }, true)
				.ConfigureAwait(false);

		public async Task<Boolean> SetVisualNovelListAsync(UInt32 id, Status? status, String notes)
			=> await this.SendSetRequestInternalAsync(Constants.SetVisualNovelListCommand, id, status.HasValue ? new { status, notes } : null, true)
				.ConfigureAwait(false);

		public async Task<Boolean> SetWishlistAsync(UInt32 id, Priority? priority)
			=> await this.SendSetRequestInternalAsync(Constants.SetWishlistCommand, id, priority.HasValue ? new { priority } : null)
				.ConfigureAwait(false);
	}
}
