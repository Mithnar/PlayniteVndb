﻿using System;
using System.IO;
using System.Net.Sockets;
#if UserAuth
using System.Security;
#endif
using System.Threading;
using System.Threading.Tasks;
using VndbSharp.Extensions;
using VndbSharp.Interfaces;
using VndbSharp.Models;

namespace VndbSharp
{
	/// <summary>
	///		The main class to issue get and set commands to the Vndb API with
	/// </summary>
	public partial class Vndb
	{
		/// <summary>
		///		Creates a new instance of the Vndb class, to issue commands to the API
		/// </summary>
		/// <param name="useTls">Should the connection to the API be secure</param>
		public Vndb(Boolean useTls = false)
		{
			this.UseTls = useTls;
		}

		#region .  Public Properties  .

		/// <inheritdoc cref="TcpClient.SendBufferSize"/>
		public Int32 SendBufferSize
		{
			get { return this.Client?.SendBufferSize ?? this._sendBufferSize; }
			set
			{
				if (this.Client != null)
					this.Client.SendBufferSize = value;
				this._sendBufferSize = value;
			}
		}

		/// <inheritdoc cref="TcpClient.ReceiveBufferSize"/>
		public Int32 ReceiveBufferSize
		{
			get { return this.Client?.ReceiveBufferSize ?? this._receiveBufferSize; }
			set
			{
				if (this.Client != null)
					this.Client.ReceiveBufferSize = value;
				this._sendBufferSize = value;
			}
		}

		/// <inheritdoc cref="TcpClient.SendTimeout"/>
		public TimeSpan SendTimeout
		{
			get { return this._sendTimeout; }
			set
			{
				if (this.Client != null)
					this.Client.SendTimeout = (Int32) value.TotalMilliseconds;
				this._sendTimeout = value;
			}
		}

		/// <inheritdoc cref="TcpClient.ReceiveTimeout"/>
		public TimeSpan ReceiveTimeout
		{
			get { return this._receiveTimeout; }
			set
			{
				if (this.Client != null)
					this.Client.ReceiveTimeout = (Int32) value.TotalMilliseconds;
				this._receiveTimeout = value;
			}
		}

		/// <summary>
		///		Should the Connection to the Vndb API be done over a secure stream
		/// </summary>
		/// <exception cref="InvalidOperationException">When trying to change UseTls while logged in.</exception>
		public Boolean UseTls
		{
			get { return this._useTls; }
			set
			{
				//				if (!this.Username.IsEmpty() || this.Password != null)
				//					throw new InvalidOperationException($"Cannont change {nameof(this.UseTls)} state while using a username / password.");

				this._useTls = value;
				this.LoggedIn = false;
			}
		}

		/// <summary>
		///		Sets whether <see cref="VndbFlags"/> should be checked before being sent
		/// </summary>
		public Boolean CheckFlags { get; set; } = true;

		#endregion

		#region .  Protected Fields  .

		/// <summary>
		///		Indicates if the instance has logged in yet or not
		/// </summary>
		protected Boolean LoggedIn;

		/// <summary>
		///		The <see cref="CancellationToken"/> Source for all Async Tasks.
		/// </summary>
		protected CancellationTokenSource CancellationTokenSource;

		/// <summary>
		///		The last error that occured will be stored here until another command is sent
		/// </summary>
		protected IVndbError LastError;

		//		/// <summary>
		//		///		The users password, if provided
		//		/// </summary>
		//		protected SecureString Password;

		/// <summary>
		///		The Connections Stream, for Reading and Writing
		/// </summary>
		protected Stream Stream;

		/// <summary>
		///		The Connections Client
		/// </summary>
		protected TcpClient Client;

		#endregion

		#region .  Backing Fields  .

		private Boolean _useTls = false;

		private Int32 _sendBufferSize = 4096;
		private Int32 _receiveBufferSize = 4096;

		private TimeSpan _sendTimeout = TimeSpan.FromSeconds(30);
		private TimeSpan _receiveTimeout = TimeSpan.FromSeconds(30);

		private Action<String, VndbFlags, VndbFlags> _invalidFlags;

		#endregion
	}
}
