﻿#if UserAuth
using System;
using System.Runtime.InteropServices;
using System.Security;
using Newtonsoft.Json;

namespace VndbSharp.Json.Converters
{
	// Not safe, converts *secure* strings
	internal class SecureStringConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
		{
			var unmanagedString = IntPtr.Zero;
			try
			{
				// Class changed compared to .Net 4.x versions, not sure if i should be using SecureStringTo GlobalAlloc or CoTaskMem
				// Opted for GlobalAlloc because that was what was being used in .Net 4.x versions?
				// Still not secure for usage on Unix, but better then nothing.
				unmanagedString = SecureStringMarshal.SecureStringToGlobalAllocUnicode((SecureString) value);
				serializer.Serialize(writer, Marshal.PtrToStringUni(unmanagedString));
			}
			finally
			{
				Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
			}
		}

		public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override Boolean CanConvert(Type objectType)
			=> objectType == typeof(SecureString);

		public override Boolean CanRead { get; } = false;
	}
}
#endif
