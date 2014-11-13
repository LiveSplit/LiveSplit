// Refal5.NET runtime
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System;
using System.IO;
using System.Text;
using Irony.Interpreter.Ast;
using Irony.Interpreter;
using System.Reflection;
using System.Collections.Generic;

namespace Refal.Runtime
{
	/// <summary>
	/// RecognitionImpossibleException means that the current function cannot be aplied to input expression.
	/// </summary>
	public class RecognitionImpossibleException : Exception
	{
		public RecognitionImpossibleException() : base()
		{
		}

		public RecognitionImpossibleException(string msg) : base(msg)
		{
		}
	}
}
