using System;

namespace VndbSharp
{
	public class UnexpectedResponseException : Exception
	{
		public UnexpectedResponseException(String command, String response)
		{
			this.Command = command;
			this.Response = response;
		}

		public String Command { get; private set; }
		public String Response { get; private set; }
	}
}