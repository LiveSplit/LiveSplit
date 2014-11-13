// Refal5.NET runtime
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System;

namespace Refal.Runtime
{
	/// <summary>
	/// Pattern variables is a special kind of pattern items.
	/// They can be bound to expressions.
	/// </summary>
	public abstract class Variable : PatternItem
	{
		public string Name { get; private set; }

		public int FirstOccurance { get; set; }

		public object Value { get; set; }

		public Variable(string name)
		{
			Name = name;
			FirstOccurance = -1;
			Value = null;
		}

		public override MatchResult Match(PassiveExpression expression, ref int exIndex, int patIndex)
		{
			// if it's the first occurance of the variable, it can match any value
			if (patIndex == FirstOccurance)
				return MatchAny(expression, ref exIndex);

			// if it's not the first, it can match only the same value as before
			return MatchSame(expression, ref exIndex);
		}

		protected abstract MatchResult MatchAny(PassiveExpression expression, ref int exIndex);

		protected abstract MatchResult MatchSame(PassiveExpression expression, ref int exIndex);

		public override string ToString()
		{
			return Value == null ? Name : string.Format("{0}={1}", Name, Value);
		}
	}

	/// <summary>
	/// SymbolVariable matches single symbol
	/// </summary>
	public class SymbolVariable : Variable
	{
		public SymbolVariable(string name) : base(name)
		{
		}

		public object Symbol
		{
			get { return base.Value; }
			set { base.Value = value; }
		}

		protected override MatchResult MatchAny(PassiveExpression expression, ref int exIndex)
		{
			if (expression == null || exIndex >= expression.Count)
				return MatchResult.Failure;

			// match anything except braces
			if (!(expression[exIndex] is StructureBrace))
			{
				this.Symbol = expression[exIndex++];
				return MatchResult.Success;
			}

			return MatchResult.Failure;
		}

		protected override MatchResult MatchSame(PassiveExpression expression, ref int exIndex)
		{
			if (expression == null || exIndex >= expression.Count)
				return MatchResult.Failure;

			// match the bound value
			if (Value.Equals(expression[exIndex++]))
			{
				return MatchResult.Success;
			}

			return MatchResult.Failure;
		}

		public override string ToString()
		{
			return Value == null ? Name : string.Format("{0}={1}", Name,
				Value is char ? "'" + Value.ToString() + "'" :
				Value is string ? "\"" + Value.ToString() + "\"" :
				Value.ToString());
		}
	}

	/// <summary>
	/// TermVariable matches either single symbol of expression in a structure braces
	/// </summary>
	public class TermVariable : Variable
	{
		// term is either a symbol or an expression in structure braces
		public TermVariable(string name) : base(name)
		{
		}

		// return value if it's not an expression
		public object Symbol
		{
			get { return (base.Value is PassiveExpression ? null : base.Value); }
			set { base.Value = value; }
		}

		// return value if it's passive expression
		public PassiveExpression Expression
		{
			get { return base.Value as PassiveExpression; }
			set { base.Value = value; }
		}

		protected override MatchResult MatchAny(PassiveExpression expression, ref int exIndex)
		{
			if (expression == null || exIndex >= expression.Count)
				return MatchResult.Failure;

			// match single symbol (symbol == not a brace)
			if (!(expression[exIndex] is StructureBrace))
			{
				this.Symbol = expression[exIndex++];
				return MatchResult.Success;
			}

			// match subexpression
			else if (expression[exIndex] is OpeningBrace)
			{
				this.Expression = PassiveExpression.Build();
				this.Expression.Add(expression[exIndex++]);

				// extract subexpression within the structure braces
				int rank = 1;
				while (exIndex < expression.Count && rank > 0)
				{
					object ex = expression[exIndex++];
					this.Expression.Add(ex);

					if (ex is OpeningBrace)
						rank++;
					else if (ex is ClosingBrace)
						rank--;
				}

				// subexpression with surrounding braces is extracted
				if (rank == 0)
					return MatchResult.Success;
			}

			// unmatched braces
			return MatchResult.Failure;
		}

		protected override MatchResult MatchSame(PassiveExpression expression, ref int exIndex)
		{
			if (expression == null || exIndex >= expression.Count)
				return MatchResult.Failure;

			// match same symbol
			if (this.Symbol != null)
			{
				// match the bound value
				if (Symbol.Equals(expression[exIndex++]))
					return MatchResult.Success;
			}

			// match same subexpression
			else if (this.Expression != null)
			{
				if (expression.CompareToExpression(exIndex, this.Expression))
				{
					exIndex += this.Expression.Count;
					return MatchResult.Success;
				}
			}

			return MatchResult.Failure;
		}
	}

	/// <summary>
	/// ExpressionVariable matches any expression
	/// </summary>
	public class ExpressionVariable : Variable
	{
		public ExpressionVariable(string name) : base(name)
		{
		}

		public PassiveExpression Expression
		{
			get { return base.Value as PassiveExpression; }
			set { base.Value = value; }
		}

		protected override MatchResult MatchAny(PassiveExpression expression, ref int exIndex)
		{
			if (this.Expression == null)
			{
				// start with empty expression, don't advance exIndex
				this.Expression = PassiveExpression.Build();
				return MatchResult.PartialSuccess;
			}
			else
			{
				if (expression == null || exIndex >= expression.Count)
					return MatchResult.Failure;

				// continue adding terms to expression
				object ex = expression[exIndex++];

				// add single symbol
				if (!(ex is StructureBrace))
				{
					this.Expression.Add(ex);
					return MatchResult.PartialSuccess;
				}
				else if (ex is OpeningBrace)
				{
					// add subexpression
					this.Expression.Add(ex);

					// extract subexpression within the structure braces
					int rank = 1;
					while (exIndex < expression.Count && rank > 0)
					{
						ex = expression[exIndex++];
						this.Expression.Add(ex);

						if (ex is OpeningBrace)
							rank++;
						else if (ex is ClosingBrace)
							rank--;
					}

					// subexpression with surrounding braces is extracted
					if (rank == 0)
						return MatchResult.PartialSuccess;
				}

				return MatchResult.Failure;
			}
		}

		protected override MatchResult MatchSame(PassiveExpression expression, ref int exIndex)
		{
			if (expression != null && expression.CompareToExpression(exIndex, this.Expression))
			{
				exIndex += this.Expression.Count;
				return MatchResult.Success;
			}

			return MatchResult.Failure;
		}

		public override string ToString()
		{
			return Value == null ? Name : string.Format("{0}={1}", Name,
				Value is PassiveExpression && (Value as PassiveExpression).Count > 0 ? Value.ToString() : "[]");
		}
	}
}
