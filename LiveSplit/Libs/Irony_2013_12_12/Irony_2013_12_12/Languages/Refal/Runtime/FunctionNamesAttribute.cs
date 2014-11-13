// Refal5.NET runtime
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Refal.Runtime
{
	/// <summary>
	/// FunctionName attribute is used to specify Refal name(s) for run-time library method written in C#.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	class FunctionNamesAttribute : Attribute
	{
		public string[] Names { get; private set; }

		public FunctionNamesAttribute(params string[] names)
		{
			Names = names;
		}
	}
}
