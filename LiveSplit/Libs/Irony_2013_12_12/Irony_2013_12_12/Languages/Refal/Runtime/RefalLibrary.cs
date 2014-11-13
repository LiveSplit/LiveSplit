// Refal5.NET runtime
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Irony.Interpreter.Ast;
using Irony.Interpreter;
using System.Numerics;

namespace Refal.Runtime
{
	/// <summary>
	/// Refal run-time library.
	/// </summary>
	public class RefalLibrary
	{
		/// <summary>
		/// Script execution thread.
		/// </summary>
		public ScriptThread ScriptThread { get; private set; }

		/// <summary>
		/// File I/O support: handle (expression) -> StreamReader/StreamWriter.
		/// </summary>
		IDictionary<string, object> OpenFiles { get; set; }

		/// <summary>
		/// Bury/Dig functions expression storage.
		/// </summary>
		IDictionary<string, PassiveExpression> BuriedKeys { get; set; }

		IDictionary<string, PassiveExpression> BuriedValues { get; set; }

		/// <summary>
		/// Command line arguments.
		/// </summary>
		protected string[] CommandLineArguments { get; set; }

		public RefalLibrary(ScriptThread thread)
		{
			ScriptThread = thread;
			OpenFiles = new Dictionary<string, object>();
			BuriedKeys = new Dictionary<string, PassiveExpression>();
			BuriedValues = new Dictionary<string, PassiveExpression>();
			CommandLineArguments = null;
		}

		// Standard RTL routines

		public PassiveExpression Print(PassiveExpression expression)
		{
			if (expression == null)
				return null;

			//Console.WriteLine("{0}", expression.ToStringBuilder(0));
			ScriptThread.App.WriteLine(expression.ToStringBuilder(0).ToString());

			return expression;
		}

		public PassiveExpression Prout(PassiveExpression expression)
		{
			if (expression == null)
				return null;

			//Console.WriteLine("{0}", expression.ToStringBuilder(0));
			ScriptThread.App.WriteLine(expression.ToStringBuilder(0).ToString());

			return null;
		}

		public PassiveExpression Card(PassiveExpression expression)
		{
			throw new NotSupportedException();

			/*string s = Console.ReadLine();

			if (s != null)
				return PassiveExpression.Build(s.ToCharArray());
			else
				return PassiveExpression.Build(0);*/
		}

		public PassiveExpression Open(PassiveExpression expression)
		{
			// <Open s.Mode s.D e.File-name>
			if (expression == null || expression.Count < 1)
				throw new ArgumentNullException("s.Mode");
			else if (expression.Count < 2)
				throw new ArgumentNullException("s.D");

			string mode = expression[0].ToString().ToUpper();
			string handle = expression[1].ToString();
			string fileName = string.Format("refal{0}.dat", handle);

			// fileName can be omitted
			if (expression.Count > 2)
			{
				fileName = expression.ToStringBuilder(2).ToString();
			}

			// R - read, W - write, A - append
			if (mode.StartsWith("R"))
			{
				OpenFiles[handle] = new StreamReader(File.OpenRead(fileName));
			}
			else if (mode.StartsWith("W"))
			{
				OpenFiles[handle] = new StreamWriter(File.Create(fileName));
			}
			else if (mode.StartsWith("A"))
			{
				OpenFiles[handle] = File.AppendText(fileName);
			}
			else
			{
				throw new NotSupportedException("Bad file open mode: " + mode + " (R, W, or A expected)");
			}

			// AFAIK, Open don't return anything
			return null;
		}

		public PassiveExpression Get(PassiveExpression expression)
		{
			if (expression == null || expression.IsEmpty)
				return Card(expression);

			string handle = expression[0].ToString();
			StreamReader sr = OpenFiles[handle] as StreamReader;

			if (sr == null)
				return Card(expression);

			string s = sr.ReadLine();
			if (s != null)
				return PassiveExpression.Build(s.ToCharArray());
			else
				return PassiveExpression.Build(0);
		}

		public PassiveExpression Put(PassiveExpression expression)
		{
			if (expression == null || expression.IsEmpty)
				return Prout(expression);

			string handle = expression[0].ToString();
			StreamWriter sw = OpenFiles[handle] as StreamWriter;

			if (sw == null)
				return Prout(expression);

			sw.WriteLine("{0}", expression.ToStringBuilder(1));

			PassiveExpression result = PassiveExpression.Build(expression);
			result.Remove(result[0]);
			return result;
		}

		protected void CloseFiles()
		{
			foreach (object o in OpenFiles.Values)
			{
				if (o is StreamWriter)
					(o as StreamWriter).Close();
				else if (o is StreamReader)
					(o as StreamReader).Close();
			}
		}

		public PassiveExpression Arg(PassiveExpression expression)
		{
			if (expression == null || expression.IsEmpty || CommandLineArguments == null)
				return PassiveExpression.Build();

			int index = Convert.ToInt32(expression[0]) - 1; // in Refal, index is 1-based

			if (index >= CommandLineArguments.Length)
			{
				return PassiveExpression.Build();
			}

			return PassiveExpression.Build(CommandLineArguments[index].ToCharArray());
		}

		public PassiveExpression Br(PassiveExpression expression)
		{
			// <Br e.N '=' e.Expr>, where e.N is expression which does not
			// include '=' on the upper level of the bracket's structure
			Pattern pattern = new Pattern(new ExpressionVariable("Key"), '=', new ExpressionVariable("Value"));
			if (pattern.Match(expression))
			{
				var key = (PassiveExpression)pattern.GetVariable("Key");
				var value = (PassiveExpression)pattern.GetVariable("Value");
				var strKey = key.ToString();

				BuriedKeys[strKey] = key;
				BuriedValues[strKey] = value;

				return PassiveExpression.Build();
			}

			throw new RecognitionImpossibleException("<Br e.N '=' e.Expr>: unexpected arguments");
		}

		public PassiveExpression Dg(PassiveExpression expression)
		{
			// <Dg e.N>
			string strKey = expression.ToString();

			if (BuriedValues.ContainsKey(strKey))
			{
				PassiveExpression result = PassiveExpression.Build(BuriedValues[strKey] as PassiveExpression);
				BuriedValues[strKey] = null;
				BuriedKeys[strKey] = null;
				return result;
			}

			return PassiveExpression.Build();
		}

		public PassiveExpression Dgall(PassiveExpression expression)
		{
			List<object> result = new List<object>();
			foreach (string strKey in BuriedKeys.Keys)
			{
				result.AddRange(new object[] {new OpeningBrace(), BuriedKeys[strKey], '=', BuriedValues[strKey], new ClosingBrace()});
				BuriedKeys[strKey] = null;
				BuriedValues[strKey] = null;
			}

			return PassiveExpression.Build(result.ToArray());
		}

		// extract arguments specified as <Function t.1 e.2>
		void GetArguments(PassiveExpression expression, out object arg1, out object arg2)
		{
			var p = new Pattern(new TermVariable("t.1"), new ExpressionVariable("e.2"));
			if (p.Match(expression))
			{
				arg1 = p.GetVariable("t.1");
				arg2 = p.GetVariable("e.2");
				return;
			}

			// can't find match
			throw new RecognitionImpossibleException();
		}

		// find the first numeric symbol in an expression and convert to BigInteger
		BigInteger ToBigInteger(object value)
		{
			// try convert expression
			var expr = value as PassiveExpression;
			if (expr != null)
			{
				int sign = 1;

				foreach (object o in expr)
				{
					if (o is StructureBrace)
						continue;

					if (o is char)
					{
						var c = (char)o;
						if (c == '-')
							sign *= -1;
					}

					if (o is int)
						return new BigInteger(sign * (int)o);

					if (o is long)
						return new BigInteger(sign * (long)o);

					if (o is BigInteger)
						return sign * (BigInteger)o;

					// warning: BigInteger doesn't support parsing strings
					if (o is string)
						return new BigInteger(sign * Convert.ToInt64(o));
				}

				return BigInteger.Zero;
			}

			// try convert single symbol
			if (value is int)
				return new BigInteger((int)value);
			if (value is long)
				return new BigInteger((long)value);
			if (value is BigInteger)
				return (BigInteger)value;
			if (value is string)
				return new BigInteger(Convert.ToInt64(value)); // TODO
			return BigInteger.Zero;
		}

		// extract arguments and convert to BigIntegers
		void GetBigIntegerArguments(PassiveExpression expr, out BigInteger arg1, out BigInteger arg2)
		{
			object op1, op2;
			GetArguments(expr, out op1, out op2);

			arg1 = ToBigInteger(op1);
			arg2 = ToBigInteger(op2);;
		}

		/// <summary>
		/// Converts BigInteger to the minimal possible number type
		/// Negative numbers get converted to positive numbers prefixed with '-'
		/// For example, -520582(BigInteger) -> '-'(char) 52082(int)
		/// </summary>
		object[] ConvertBigIntegerToRefalNumber(BigInteger bigIntValue)
		{
			var negative = bigIntValue < 0;
			var result = new object[negative ? 2 : 1];
			var valueIndex = negative ? 1 : 0;
			if (negative)
			{
				result[0] = '-';
				bigIntValue = -bigIntValue;
			}

			if (bigIntValue >= int.MinValue && bigIntValue <= int.MaxValue)
				result[valueIndex] = (int)bigIntValue;
			else if (bigIntValue >= long.MinValue && bigIntValue <= long.MaxValue)
				result[valueIndex] = (long)bigIntValue;
			else
				result[valueIndex] = bigIntValue;

			return result;
		}

		int ToInt32(object expression)
		{
			string s = expression.ToString().Trim(" \t\r\n()".ToCharArray());
			return Convert.ToInt32(s);
		}

		[FunctionNames("+", "Add")]
		public PassiveExpression Add(PassiveExpression expression)
		{
			BigInteger op1, op2;
			GetBigIntegerArguments(expression, out op1, out op2);

			return PassiveExpression.Build(ConvertBigIntegerToRefalNumber(op1 + op2));
		}

		[FunctionNames("-", "Sub")]
		public PassiveExpression Sub(PassiveExpression expression)
		{
			BigInteger op1, op2;
			GetBigIntegerArguments(expression, out op1, out op2);

			return PassiveExpression.Build(ConvertBigIntegerToRefalNumber(op1 - op2));
		}

		[FunctionNames("*", "Mul")]
		public PassiveExpression Mul(PassiveExpression expression)
		{
			BigInteger op1, op2;
			GetBigIntegerArguments(expression, out op1, out op2);

			return PassiveExpression.Build(ConvertBigIntegerToRefalNumber(op1 * op2));
		}

		[FunctionNames("/", "Div")]
		public PassiveExpression Div(PassiveExpression expression)
		{
			BigInteger op1, op2;
			GetBigIntegerArguments(expression, out op1, out op2);

			return PassiveExpression.Build(ConvertBigIntegerToRefalNumber(op1 / op2));
		}

		public PassiveExpression Cp(PassiveExpression expression)
		{
			throw new NotImplementedException();
		}

		public PassiveExpression Rp(PassiveExpression expression)
		{
			throw new NotImplementedException();
		}

		public PassiveExpression Type(PassiveExpression expression)
		{
			if (expression == null || expression.IsEmpty)
				return PassiveExpression.Build('*', 0, expression);

			object o = expression[0];

			if (o is OpeningBrace)
				return PassiveExpression.Build('B', 0, expression);

			if (o is char)
			{
				char c = (char)o;
				char subtype = Char.IsUpper(c) ? 'u' : 'l';

				if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
					return PassiveExpression.Build('L', subtype, expression);
				
				if (c >= '0' && c <= '9')
					return PassiveExpression.Build('D', 0, expression);

				if (Char.IsControl(c))
					return PassiveExpression.Build('O', subtype, expression);

				// printable
				return PassiveExpression.Build('P', subtype, expression);
			}

			if (o is int || o is long)
				return PassiveExpression.Build('N', 0, expression);

			if (o is string)
			{
				string s = o as string;
				char subtype = 'i';

				if (s.Length == 0 || !(Char.IsLetter(s[0])) || s.IndexOf(" ") >= 0)
					subtype = 'q';

				return PassiveExpression.Build('W', subtype, expression);
			}

			return PassiveExpression.Build('P', 'l', expression);
		}

		public PassiveExpression Mu(PassiveExpression expression)
		{
			throw new NotImplementedException();
		}

		[FunctionNames("Implode-Ext")]
		public PassiveExpression Implode_Ext(PassiveExpression expression)
		{
			throw new NotImplementedException();
		}

		public PassiveExpression Implode(PassiveExpression expression)
		{
			string s = expression.ToString().Trim();

			int index = 0;
			while (index < s.Length && (char.IsLetterOrDigit(s[index]) || "-_".IndexOf(s[index]) >= 0))
			{
				index++;
			}

			return PassiveExpression.Build(s.Substring(0, index));
		}

		[FunctionNames("Explode", "Symb")]
		public PassiveExpression Explode(PassiveExpression expression)
		{
			// convert expression to string and remove trailing space, if any
			var sb = expression.ToStringBuilder(0);
			if (sb.Length > 0 && sb[sb.Length - 1] == ' ')
				sb.Length -= 1;

			return PassiveExpression.Build(sb.ToString().ToCharArray());
		}

		public PassiveExpression Numb(PassiveExpression expression)
		{
			if (expression == null || expression.IsEmpty)
				return PassiveExpression.Build();

			return PassiveExpression.Build(ConvertBigIntegerToRefalNumber(ToBigInteger(expression.ToString())));
		}

		public PassiveExpression Chr(PassiveExpression expression)
		{
			var args = new List<object>();

			foreach (object o in expression)
			{
				var v = (o is int) ? (char)Convert.ToByte(o) : o;

				// <Chr NewLine> returns Environment.NewLine
				if (o != null && o.ToString().ToLower() == "newline")
				{
					v = Environment.NewLine.ToCharArray();
				}

				args.Add(v);
			}

			return PassiveExpression.Build(args.ToArray());
		}

		public PassiveExpression Ord(PassiveExpression expression)
		{
			var args = new List<object>();

			foreach (object o in expression)
			{
				var v = (o is char) ? Convert.ToInt32(o) : o;
				args.Add(v);
			}

			return PassiveExpression.Build(args.ToArray());
		}

		public PassiveExpression Divmod(PassiveExpression expression)
		{
			throw new NotImplementedException();
		}

		public PassiveExpression First(PassiveExpression expression)
		{
			throw new NotImplementedException();
		}

		public PassiveExpression Putout(PassiveExpression expression)
		{
			throw new NotImplementedException();
		}
	}
}
