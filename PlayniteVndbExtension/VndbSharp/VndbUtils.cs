﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VndbSharp.Attributes;
using VndbSharp.Json;
using VndbSharp.Json.Converters;
using VndbSharp.Models;
using VndbSharp.Models.Common;
using VndbSharp.Models.Dumps;

namespace VndbSharp
{
	/// <summary>
	///		Some misc utilities such as retrieving the Database Dumps
	/// </summary>
	public static class VndbUtils
	{
		internal static HttpClient _httpClientInstance;
		internal static Func<Task<HttpMessageHandler>> _httpClientHandlerCallback;

		internal static String ClientName { get; set; } = "VndbSharp";
		internal static String ClientVersion { get; set; } = "0.2";

		internal static HttpClient HttpClient
			=> VndbUtils._httpClientInstance ?? (VndbUtils._httpClientInstance = VndbUtils.CreateClient());

		/// <summary>
		///		The Size of the Buffer to use when Downloading the Dumps
		/// </summary>
		public static Int32 BufferSize { get; set; } = 4096;

		static VndbUtils()
		{
			VndbUtils._httpClientHandlerCallback = () => Task.FromResult((HttpMessageHandler) new HttpClientHandler());
		}

		/// <summary>
		/// Sets the callback to create a HttpMessageHandler instance for the HttpClient
		/// </summary>
		public static void WithHttpMessageHandler(Func<Task<HttpMessageHandler>> callback)
		{
			if (VndbUtils._httpClientInstance != null)
			{
				VndbUtils._httpClientInstance.Dispose();
				VndbUtils._httpClientInstance = null;
			}

			VndbUtils._httpClientHandlerCallback = callback;
		}

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

		/// <summary>
		///		Downloads and Parses the Tag Dump from Vndb
		/// </summary>
		/// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Tag"/></returns>
		/// <exception cref="HttpRequestException">Occurs when the tags.json.gz file returns a non-success status</exception>
		public static async Task<IEnumerable<Tag>> GetTagsDumpAsync()
			=> await VndbUtils.GetDumpAsync<IEnumerable<Tag>>(Constants.TagsDump).ConfigureAwait(false);

		/// <summary>
		///		Downloads and Parses the Traits Dump from Vndb
		/// </summary>
		/// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Trait"/></returns>
		/// <exception cref="HttpRequestException">Occurs when the traits.json.gz file returns a non-success status</exception>
		public static async Task<IEnumerable<Trait>> GetTraitsDumpAsync()
			=> await VndbUtils.GetDumpAsync<IEnumerable<Trait>>(Constants.TraitsDump).ConfigureAwait(false);

		/// <summary>
		///		Downloads and PArses the Votes Dump from Vndb
		/// </summary>
		/// <param name="version">The version of the Votes Dump to grab</param>
		/// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Vote"/></returns>
		/// <exception cref="HttpRequestException">Occurs when the votes.gz file returns a non-success status</exception>
		public static async Task<IEnumerable<Vote>> GetVotesDumpAsync(VoteDumpVersion version = VoteDumpVersion.Two)
			=> await VndbUtils.GetAndParseVotesAsync(version).ConfigureAwait(false);
		
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

		internal static async Task<T> GetDumpAsync<T>(String url)
			where T : class
		{
			// .Net Core removed WebClient and Http/WebRequests, so we need to use HttpClient.
			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
			// Manually add the headers every request rather then using the default headers,
			// incase the client was rebuilt with a new name / version mid-application session
			request.Headers.Add("User-Agent", $"{VndbUtils.ClientName} (v{VndbUtils.ClientVersion})");

			var response = await VndbUtils.HttpClient.SendAsync(request);
			response.EnsureSuccessStatusCode(); // Ensure we got data

			var gzipStream = await response.Content.ReadAsStreamAsync();
			var rawContents = await VndbUtils.UnGzip(gzipStream);

			return JsonConvert.DeserializeObject<T>(rawContents, new JsonSerializerSettings
			{
				ContractResolver = VndbContractResolver.Instance,
			});
		}

		internal static async Task<IEnumerable<Vote>> GetAndParseVotesAsync(VoteDumpVersion version)
		{
			var url = version == VoteDumpVersion.One ? Constants.VotesDump : Constants.VotesDump2;
			Debug.WriteLine($"Requesting Votes Dump via {url}");
			// .Net Core removed WebClient and Http/WebRequests, so we need to use HttpClient.
			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
			// Manually add the headers every request rather then using the default headers,
			// incase the client was rebuilt with a new name / version mid-application session
			request.Headers.Add("User-Agent", $"{VndbUtils.ClientName} (v{VndbUtils.ClientVersion})");

			var response = await VndbUtils.HttpClient.SendAsync(request);
			response.EnsureSuccessStatusCode(); // Ensure we got data

			var gzipStream = await response.Content.ReadAsStreamAsync();
			var rawContents = await VndbUtils.UnGzip(gzipStream);

			response.Dispose();
			request.Dispose();

			var results = new List<Vote>();

			var votes = rawContents.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
			var expectedValues = version == VoteDumpVersion.One ? 3 : 4;

			// Resharper "Loop can be converted to LINQ-expression won't work due to inline "out var" declaration
			foreach (var vote in votes)
			{
				var values = vote.Split(new [] {' '}, expectedValues, StringSplitOptions.RemoveEmptyEntries);

				if (values.Length != expectedValues)
					continue;

				SimpleDate date = null;

				if (!UInt32.TryParse(values[0], out var vnId) || 
					!UInt32.TryParse(values[1], out var uid) || 
					!Byte.TryParse(values[2], out var value))
					continue;

				if (version == VoteDumpVersion.Two && 
					(date = (SimpleDate) SimpleDateConverter.ParseString(values[3])) == null)
					continue;

				results.Add(new Vote(version, vnId, uid, value, date));
			}

			return results;
		}

		internal static async Task<String> UnGzip(Stream data, Boolean leaveOpen = false)
		{
			var buffer = new Byte[VndbUtils.BufferSize];
			using (var gzipStream = new GZipStream(data, CompressionMode.Decompress, leaveOpen))
			using (var rawStream = new MemoryStream())
			{
				var bytesRead = 0;
				while ((bytesRead = await gzipStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
					await rawStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
				return Encoding.UTF8.GetString(rawStream.ToArray());
			}
		}

		internal static HttpClient CreateClient()
		{
			var handler = VndbUtils._httpClientHandlerCallback.Invoke()
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();
			return new HttpClient(handler);
		}
	}
}
