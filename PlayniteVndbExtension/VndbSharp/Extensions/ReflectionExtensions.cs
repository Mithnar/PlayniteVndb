﻿using System;
using System.Linq;
using System.Reflection;

namespace VndbSharp.Extensions
{
	internal static class ReflectionExtensions
	{
		public static ConstructorInfo GetConstructor(this Type type, BindingFlags bindingFlags)
		{
			return type.GetConstructor(bindingFlags, new Type[0]);
		}

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

		public static Boolean HasAttribute<T>(this FieldInfo prop)
			where T : Attribute
			=> prop.GetCustomAttribute<T>() != null;

		public static Boolean HasAttribute<T>(this MemberInfo prop)
			where T : Attribute
			=> prop.GetCustomAttribute<T>() != null;
	}
}
