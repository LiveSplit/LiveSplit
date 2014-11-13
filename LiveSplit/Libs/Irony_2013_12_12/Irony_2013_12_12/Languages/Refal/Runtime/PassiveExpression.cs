// Refal5.NET runtime
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System;
using System.Text;

namespace Refal.Runtime
{
	/// <summary>
	/// Passive expression is expression that don't contain 
	/// execution braces: &lt;&gt;
	/// 
	/// It is basically a collection of symbols (single characters,
	/// strings - called 'compound characters' and treated as symbols,
	/// macrodigits and identifiers, which can be thought of as a
	/// special case of compound characters)
	/// </summary>
	public class PassiveExpression : System.Collections.CollectionBase
	{
		protected PassiveExpression()
		{
		}

		public static PassiveExpression Build(params object[] objects)
		{
			PassiveExpression result = new PassiveExpression();

			// flatten expressions, if needed
			foreach (object obj in objects)
			{
				if (obj is PassiveExpression)
				{
					foreach (object symbol in (PassiveExpression)obj)
						result.Add(symbol);
				}
				else
					result.Add(obj);
			}

			return result;
		}

		public object this[int index]
		{
			get { return List[index]; }
		}

		public virtual int Add(object symbol)
		{
			if (symbol is char[])
			{
				int index = -1;

				foreach (char c in (char[])symbol)
					index = List.Add(c);

				return index;
			}
			else if (symbol != null)
				return List.Add(symbol);

			return -1;
		}

		public void Remove(object o)
		{
			List.Remove(o);
		}

		public bool IsEmpty
		{
			get { return List.Count == 0; }
		}

		public override int GetHashCode()
		{
			int hashCode = this.List.Count ^ (int)0xBAD1DEA;

			if (this.List.Count >= 1)
			{
				hashCode ^= List[0].GetHashCode();
			}

			return hashCode;
		}

		public override bool Equals(object o)
		{
			bool equals = true;

			if (o is PassiveExpression)
			{
				PassiveExpression ex = (PassiveExpression)o;

				if (ex.Count != Count)
					return false;

				for (int i = 0; i < Count; i++)
				{
					object my = this[i];
					object his = ex[i];

					if (my == null)
					{
						if (his != null)
							return false;
					}
					else // my != null
					{
						if (his == null)
							return false;

						equals = my.Equals(his);
					}

					if (!equals)
						return false;
				}

				return equals;
			}

			// ex
			return false;
		}

		public override string ToString()
		{
			return ToStringBuilder(0).ToString();
		}

		public StringBuilder ToStringBuilder(int startIndex)
		{
			StringBuilder sb = new StringBuilder();
			
			for (int i = startIndex; i < Count; i++)
			{
				object value = this[i];

				if (value is PassiveExpression)
					sb.AppendFormat("({0}) ", (value as PassiveExpression).ToStringBuilder(0).ToString());
				else if (value is char || value is StructureBrace)
					sb.AppendFormat("{0}", value);
				else
					sb.AppendFormat("{0} ", value);
			}

			return sb;
		}

		public bool CompareToExpression(int startIndex, PassiveExpression expression)
		{
			if (expression == null || expression.IsEmpty)
				return true;

			for (int i = 0; i < expression.Count; i++)
			{
				if (startIndex + i >= this.Count)
					return false;

				object ex1 = this[startIndex + i];
				object ex2 = expression[i];

				if (ex1 is OpeningBrace)
				{
					if (!(ex2 is OpeningBrace))
						return false;
				}
				else if (ex1 is ClosingBrace)
				{
					if (!(ex2 is ClosingBrace))
						return false;
				}
				else if (!ex1.Equals(ex2))
					return false;
			}

			return true;
		}
	}
}
