/*
// Exepack.NET helper class
// http://www.codeplex.com/exepack
// Copyright (c) 2008-2009 Alexey Yakovlev
*/

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace Refal.Runtime
{
	/// <summary>
	/// Extension methods for System.Reflection classes
	/// </summary>
	public static class ReflectionExtensions
	{
		public static T GetCustomAttribute<T>(this MemberInfo mi, bool inherit) where T : Attribute
		{
			object[] attributes = mi.GetCustomAttributes(typeof(T), inherit);

			if (attributes.Length == 0)
				return null;

			return (T)attributes[0];
		}

		public static T[] GetCustomAttributes<T>(this MemberInfo mi, bool inherit) where T : Attribute
		{
			object[] attributes = mi.GetCustomAttributes(typeof(T), inherit);
			T[] result = new T[attributes.Length];
			Array.Copy(attributes, result, result.Length);
			return result;
		}

		public static T[] GetCustomAttributes<T>(this MemberInfo mi) where T : Attribute
		{
			return mi.GetCustomAttributes<T>(false);
		}

		public static T GetCustomAttribute<T>(this MemberInfo mi) where T : Attribute
		{
			return mi.GetCustomAttribute<T>(false);
		}

		// where T : Delegate — not supported by C#

		public static T CreateDelegate<T>(this MethodInfo mi, object instance) where T : class // Delegate
		{
			return (T)(object)Delegate.CreateDelegate(typeof(T), instance, mi);
		}

		public static T CreateDelegate<T>(this MethodInfo mi) where T : class // Delegate
		{
			return (T)(object)Delegate.CreateDelegate(typeof(T), null, mi);
		}

		public static T CreateDelegate<T>(this MethodInfo mi, object instance, bool throwOnBindFailure) where T : class // Delegate
		{
			return (T)(object)Delegate.CreateDelegate(typeof(T), instance, mi, throwOnBindFailure);
		}

		public static T CreateDelegate<T>(this MethodInfo mi, bool throwOnBindFailure) where T : class // Delegate
		{
			return (T)(object)Delegate.CreateDelegate(typeof(T), null, mi, throwOnBindFailure);
		}
	}
}
