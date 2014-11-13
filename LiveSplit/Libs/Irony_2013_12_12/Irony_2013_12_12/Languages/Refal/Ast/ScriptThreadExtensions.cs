// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System;
using System.Collections.Generic;
using Irony.Interpreter;

namespace Refal
{
	/// <summary>
	/// Refal script thread should be able to store the last recognized pattern.
	/// </summary>
	public static class ScriptThreadExtensions
	{
		/// Unique names for implicit local variables
		static string LastPatternSymbolName = Guid.NewGuid().ToString();

		/// <summary>
		/// Retrieve last evaluated pattern.
		/// </summary>
		public static Refal.Runtime.Pattern GetLastPattern(this ScriptThread thread)
		{
			var binding = thread.Bind(LastPatternSymbolName, BindingRequestFlags.Read);
			return binding.GetValueRef(thread) as Refal.Runtime.Pattern;
		}

		/// <summary>
		/// Set last evaluated pattern.
		/// </summary>
		public static void SetLastPattern(this ScriptThread thread, Refal.Runtime.Pattern pattern)
		{
			var binding = thread.Bind(LastPatternSymbolName, BindingRequestFlags.Write | BindingRequestFlags.ExistingOrNew);
			binding.SetValueRef(thread, pattern);
		}
	}
}
