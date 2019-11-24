﻿using System;
using System.Linq;
using System.Reflection;

namespace VndbSharp.Extensions
{
	internal static class ReflectionExtensions
	{
		public static ConstructorInfo GetConstructor(this Type type, BindingFlags bindingFlags, params Type[] types)
		{
			return
				type.GetConstructors(bindingFlags)
					.FirstOrDefault(c => c.GetParameters()
						.Select(p => p.ParameterType)
						.SequenceEqual(types));
		}

		public static Boolean HasAttribute<T>(this PropertyInfo prop)
			where T : Attribute
			=> prop.GetCustomAttribute<T>() != null;
	}
}
