﻿using System;
using System.Threading.Tasks;
using VndbSharp.Extensions;
using VndbSharp.Interfaces;
using VndbSharp.Models;
using VndbSharp.Models.Errors;
using VndbSharp.Models.Release;
using VndbSharp.Models.VisualNovel;

namespace VndbSharp
{
	public partial class Vndb
	{
		public async Task<VndbResponse<VisualNovel>> GetVisualNovelAsync(IFilter filters, VndbFlags flags = VndbFlags.Basic,
			IRequestOptions options = null)
			=> await this.GetInternalAsync<VndbResponse<VisualNovel>>(Constants.GetVisualNovelCommand, filters, flags, options)
				.ConfigureAwait(false);

		public async Task<VndbResponse<Release>> GetReleaseAsync(IFilter filters, VndbFlags flags = VndbFlags.Basic,
			IRequestOptions options = null)
			=> await this.GetInternalAsync<VndbResponse<Release>>(Constants.GetReleaseCommand, filters, flags, options)
				.ConfigureAwait(false);

		// todo: Move this to Vndb.Helper.cs
		protected async Task<T> GetInternalAsync<T>(String method, IFilter filter, VndbFlags flags, IRequestOptions options = null)
			where T : class
		{
			// Need a way to communicate to the end user that these null values are not from the API?
			if (this.CheckFlags && !VndbUtils.ValidateFlagsByMethod(method, flags, out var invalidFlags))
			{
				this._invalidFlags?.Invoke(method, flags, invalidFlags);
				this.LastError = new LibraryError("CheckFlags is enabled and VndbSharp detected invalid flags");
				return null;
			}

			if (!filter.IsFilterValid())
			{
				this.LastError = new LibraryError($"A filter was not considered valid. The filter is of the type {filter.GetType().Name}");
				return null;
			}

			var requestData =
				this.FormatRequest($"{method} {flags.AsString(method)} ({filter})",
					options, false);
			return await this.SendGetRequestInternalAsync<T>(requestData).ConfigureAwait(false);
		}
	}
}
