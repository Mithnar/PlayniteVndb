﻿﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VndbSharp.Extensions;
using VndbSharp.Interfaces;
using VndbSharp.Json;
using VndbSharp.Models;
using VndbSharp.Models.Errors;

namespace VndbSharp
{
	public partial class Vndb : IDisposable
	{
		#region .  Fluent Client Settings  .

		/// <summary>
		///		A helper method to set the Client Name and Client Version sent to the Vndb Api
		/// </summary>
		/// <param name="clientName">The name of your client</param>
		/// <param name="clientVersion">The version of your client. Valid values: a-z 0-9 _ . / -</param>
		/// <returns>The <see cref="Vndb"/> instance</returns>
		/// <exception cref="ArgumentOutOfRangeException">When <paramref name="clientVersion"/> is not a valid <see cref="Version"/></exception>
		public Vndb WithClientDetails(String clientName, String clientVersion)
		{
			VndbUtils.ClientName = clientName;
			VndbUtils.ClientVersion = clientVersion;
			return this;
		}

		/// <summary>
		///		Sets whether <see cref="VndbFlags"/> should be checked before being sent, and provides a callback to retrieve the invalid flags
		/// </summary>
		/// <param name="checkFlags">Should <see cref="VndbFlags"/> be checked before being sent</param>
		/// <param name="invalidCallback">A callback with which the Method, Provided Flags, and Invalid Flags will be passed to when the Flags are Invalid</param>
		/// <returns>The <see cref="Vndb"/> instance</returns>
		public Vndb WithFlagsCheck(Boolean checkFlags, Action<String, VndbFlags, VndbFlags> invalidCallback)
		{
			this.CheckFlags = checkFlags;
			this._invalidFlags = invalidCallback;
			return this;
		}

		public Vndb WithTimeout(UInt32 both)
			=> this.WithTimeout(TimeSpan.FromMilliseconds(both));

		public Vndb WithTimeout(TimeSpan both)
			=> this.WithTimeout(both, both);

		public Vndb WithTimeout(TimeSpan receive, TimeSpan send)
		{
			this.ReceiveTimeout = receive;
			this.SendTimeout = send;
			return this;
		}
		
		#endregion

		#region .  Error Methods  .

		/// <summary>
		///		Parses a Vndb Error json result to determine the error, and stores it at <see cref="Vndb.LastError"/>
		/// </summary>
		/// <param name="json">The Error json</param>
		protected void ParseError(String json)
		{
			Debug.WriteLine(json);

			var response = JObject.Parse(json);
			if (!response.TryGetValue("id", StringComparison.OrdinalIgnoreCase, out var typeToken))
				return;

			switch (typeToken.Value<String>())
			{
				case "parse":
					this.LastError = Error.Build<ParseError>(response);
					break;
				case "missing":
					this.LastError = Error.Build<MissingError>(response);
					break;
				case "badarg":
					this.LastError = Error.Build<BadArgumentError>(response);
					break;
				case "needlogin":
					this.LastError = Error.Build<LoginRequiredError>(response);
					break;
				case "throttled":
					this.LastError = Error.Build<ThrottledError>(response);
					break;
				case "auth":
					this.LastError = Error.Build<BadAuthenticationError>(response);
					break;
				case "loggedin":
					this.LastError = Error.Build<LoggedInError>(response);
					break;
				case "gettype":
					this.LastError = Error.Build<GetTypeError>(response);
					break;
				case "getinfo":
					this.LastError = Error.Build<GetInfoError>(response);
					break;
				case "filter":
					this.LastError = Error.Build<InvalidFilterError>(response);
					break;
				case "settype":
					this.LastError = Error.Build<SetTypeError>(response);
					break;
				default:
					this.LastError = null;
					break;
			}
		}
		#endregion

		#region .  Stream Read/Write  .

		/// <summary>
		///		<para>The most generic get request helper.</para>
		///		<para>It takes the request data, ensures the user is logged in (and logs them in if they are not), then sends the request and recieves the response</para>
		///		<para>Once the response is recieved, it checks to see if it is a known response, and returns an approiate Model or sets the Error and returns null</para>
		/// </summary>
		/// <typeparam name="T">The Model to return if the response is a result set</typeparam>
		/// <param name="requestData">The request to send to the Vndb Api</param>
		/// <returns><typeparamref name="T"/> or null</returns>
		protected async Task<T> SendGetRequestInternalAsync<T>(Byte[] requestData)
			where T : class
		{
			if (!await this.LoginAsync().ConfigureAwait(false))
				return null;

			Debug.WriteLine(this.GetString(requestData)); // Only performance issue in Debug build
			this.RenewCts();
			await this.SendDataAsync(requestData, this.CancellationTokenSource.Token)
				.TimeoutAfter(this.SendTimeout) // I wonder what happens if sending doesn't complete? :o
				.ConfigureAwait(false);

			this.RenewCts();
			var response = await this.GetResponseAsync(this.CancellationTokenSource.Token)
				.TimeoutAfter(this.ReceiveTimeout)
				.ConfigureAwait(false);
			Debug.WriteLine($"Get Response | {response}");

			var results = response.ToVndbResults();
			if (results.Length == 2 &&
				(results[0] == Constants.Results || results[0] == Constants.DbStats))
				return results[1].FromJson<T>();

			if (results.Length != 2 || results[0] != Constants.Error)
				throw new UnexpectedResponseException(this.GetString(requestData), response);

			this.ParseError(results[1]);
			return null;
		}

		/// <summary>
		///		Sends the request to the Vndb Api by writing to the <see cref="Vndb.Stream"/>
		/// </summary>
		/// <param name="data"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		protected async Task SendDataAsync(Byte[] data, CancellationToken cancellationToken)
			=> await this.Stream.WriteAsync(data, 0, data.Length, cancellationToken).ConfigureAwait(false);

		/// <summary>
		///		Gets the response from the Vndb Api by reading to the <see cref="Vndb.Stream"/>
		/// </summary>
		/// <param name="cancellationToken">The CancellationToken used while reading and writing</param>
		/// <returns>The raw response as a String</returns>
		protected async Task<String> GetResponseAsync(CancellationToken cancellationToken)
		{
			this.LastError = null;

			var ms = new MemoryStream();
			var buffer = new Byte[this.ReceiveBufferSize];

			Int32 bytesRead;
			while ((bytesRead = await this.Stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
			{
				await ms.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
				if (buffer[--bytesRead] == Constants.EotChar)
					break;
			}

			using (ms)
				return this.GetString(ms.ToArray()).TrimEnd(Constants.EotChar);
		}

		#endregion

		#region .  Request Formatting  .
		
		/// <summary>
		///		Formats a request with the Terminating Character
		/// </summary>
		/// <param name="data">The data to append the Terminating Character to</param>
		/// <returns>A string that ends with 0x04</returns>
		protected Byte[] FormatRequest(String data)
			=> this.GetBytes($"{data}{Constants.EotChar}");

		/// <summary>
		///		Converts an object to json and formats the request with the Terminating Character
		/// </summary>
		/// <param name="prefix">What to put before the <paramref name="data"/></param>
		/// <param name="data">The object to be Json Encoded for transmission</param>
		/// <param name="includeNull">Indicates if null values should be allowed</param>
		/// <returns>A formatted string that ends with 0x04</returns>
		protected Byte[] FormatRequest<T>(String prefix, T data, Boolean includeNull = true)
		{
			if (data == null && !includeNull)
				return this.FormatRequest(prefix);

			var json = JsonConvert.SerializeObject(data,
				new JsonSerializerSettings
				{
					ContractResolver = VndbContractResolver.Instance,
					NullValueHandling = includeNull
						? NullValueHandling.Include
						: NullValueHandling.Ignore,
				});

			return this.FormatRequest(json == "null" ? prefix : $"{prefix} {json}");
		}

		/// <inheritdoc cref="Encoding.GetBytes(String)"/>
		protected Byte[] GetBytes(String data)
			=> Encoding.UTF8.GetBytes(data);

		/// <inheritdoc cref="Encoding.GetString(Byte[])"/>
		protected String GetString(Byte[] data)
			=> Encoding.UTF8.GetString(data);

		#endregion

		#region .  IDisposable  .

		/// <summary>
		///		Disposes of the current instance
		/// </summary>
		public void Dispose()
			=> ((IDisposable) this).Dispose();

		/// <summary>
		///		Disposes of the current instance
		/// </summary>
		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///		Disposes of all IDisposable properties
		/// </summary>
		protected void Dispose(Boolean disposing)
		{
			if (!disposing)
				return;

#if netstandard1_3
			this.Client?.Dispose();
#endif
			this.Client = null;

			this.Stream?.Dispose();
			this.Stream = null;

			this.CancellationTokenSource?.Dispose();
			this.CancellationTokenSource = null;
		}

		#endregion

		/// <summary>
		///		Completely logout, disposing of any stored passwords as well of the connection
		/// </summary>
		public void Logout()
		{
#if UserAuth
			this.Password?.Dispose();
#endif
			this.Dispose();
		}

		/// <summary>
		///		Attempts to login to the Vndb API, including making the initial connection, optionally connecting via a SslStream and sending the Login Request itself
		/// </summary>
		/// <returns>True if already logged in, or successfully logged in. Otherwise false.</returns>
		protected async Task<Boolean> LoginAsync()
		{
			if (this.Client?.Connected == true && this.LoggedIn)
				return true;

			// Optionally dispose the old client, and create a new one
			this.InitializeClient();

			await this.Client.ConnectAsync(Constants.ApiDomain, this.UseTls ? Constants.ApiPortTls : Constants.ApiPort)
				.ConfigureAwait(false);

			if (this.UseTls)
			{
				// If UseTls was set to true, we want to use a SslStream to do our security magic
				var stream = new SslStream(this.Client.GetStream());
				await stream.AuthenticateAsClientAsync(Constants.ApiDomain).ConfigureAwait(false);
				this.Stream = stream;
			}
			else
			{
				// Otherwise, just use the NetworkStream directly.
				this.Stream = this.Client.GetStream();
			}

			this.RenewCts();


#if UserAuth
			// Create a login class that can have an optional Username / Password
			var login = new Login(this.Username, this.Password);
#else
			// Create a login that doesn't allow usernames / passwords
			var login = new Login();
#endif
			await this.SendDataAsync(this.FormatRequest(Constants.LoginCommand, login, false), this.CancellationTokenSource.Token)
				.TimeoutAfter(this.SendTimeout)
				.ConfigureAwait(false);

			this.RenewCts();
			var response = await this.GetResponseAsync(this.CancellationTokenSource.Token)
				.TimeoutAfter(this.ReceiveTimeout)
				.ConfigureAwait(false);

			// Success!
			if (response == Constants.Ok)
			{
				this.LoggedIn = true;
				return true;
			}

			// No response returned...?
			if (response.IsEmpty()) // We do not want to provide the full request/command here. It contains the password :o
				throw new UnexpectedResponseException("login", response);

			var results = response.ToVndbResults();
			// If we don't get Error, then we actually don't know what this response is.
			if (results.Length != 2 || results[0] != Constants.Error) // We do not want to provide the full request/command here. It contains the password :o
				throw new UnexpectedResponseException("login", response);

			this.ParseError(results[1]);
			return false;
		}

		/// <summary>
		///		Disposes the old state of the Vndb class, and re-creates the internal <see cref="TcpClient"/>
		/// </summary>
		protected void InitializeClient()
		{
			this.Dispose(true);

			this.Client = new TcpClient
			{
				SendBufferSize = this.SendBufferSize,
				ReceiveBufferSize = this.ReceiveBufferSize,
			};

			this.LoggedIn = false;
		}

		/// <summary>
		///		Disposes of the old CancellationTokenSource if it exists, and assigns a new one to <see cref="CancellationTokenSource"/>
		/// </summary>
		protected void RenewCts()
		{
			this.CancellationTokenSource?.Dispose();
			this.CancellationTokenSource = new CancellationTokenSource();
		}
	}
}
