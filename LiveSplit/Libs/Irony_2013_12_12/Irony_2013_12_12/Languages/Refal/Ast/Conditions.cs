// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System;
using System.Linq;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Irony.Ast;

namespace Refal
{
	/// <summary>
	/// Where- and When-clauses.
	/// </summary>
	public class Conditions : AstNode
	{
		public Expression Expression { get; private set; }

		public Pattern Pattern { get; private set; }

		public Conditions MoreConditions { get; private set; }

		public Expression ResultExpression { get; private set; }

		public Block Block { get; private set; }

    public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			base.Init(context, parseNode);

			foreach (var node in parseNode.ChildNodes)
			{
				if (node.AstNode is Expression)
				{
					Expression = (node.AstNode as Expression);
				}
				else if (node.AstNode is AuxiliaryNode)
				{
					var nodes = (node.AstNode as AuxiliaryNode).ChildNodes;
					Pattern = nodes.OfType<Pattern>().FirstOrDefault();
					Block = nodes.OfType<Block>().FirstOrDefault();
					MoreConditions = nodes.OfType<Conditions>().FirstOrDefault();
					ResultExpression = nodes.OfType<Expression>().FirstOrDefault();
				}

				foreach (var astNode in new AstNode[] { Expression, Pattern, Block, MoreConditions, ResultExpression })
				{
					if (astNode != null)
						astNode.Parent = this;
				}
			}

			AsString = Block != null ? "with-clause" : "where-clause";
		}

		public override System.Collections.IEnumerable GetChildNodes()
		{
			yield return Expression;

			if (Block != null)
				yield return Block;

			if (Pattern != null)
				yield return Pattern;

			if (ResultExpression != null)
				yield return ResultExpression;

			if (MoreConditions != null)
				yield return MoreConditions;
		}

		protected override object DoEvaluate(ScriptThread thread)
		{
			// standard prolog
			thread.CurrentNode = this;

			// evaluate expression
			var expression = Expression.EvaluateExpression(thread);
			object result = null;

			// extract last recognized pattern (it contains bound variables)
			var lastPattern = thread.GetLastPattern();
			if (lastPattern == null)
			{
				thread.ThrowScriptError("Internal error: last recognized pattern is lost.");
				return null; // never gets here
			}

			// with-clause
			if (Block != null)
			{
				Block.InputExpression = expression;
				Block.BlockPattern = lastPattern;
				result = Block.Evaluate(thread);
			}

			// where-clause
			else if (Pattern != null)
			{
				result = EvaluateWhereClause(expression, lastPattern, thread);
			}

			// internal compiler error
			else
			{
				thread.ThrowScriptError("Internal error: AST node doen't represent neither where- nor when-clause.");
				return null; // never get here
			}

			// standard epilog
			thread.CurrentNode = Parent;
			return result;
		}

		object EvaluateWhereClause(Runtime.PassiveExpression expr, Runtime.Pattern lastPattern, ScriptThread thread)
		{
			// instantiate where-clause pattern
			var patt = Pattern.Instantiate(thread);
			patt.CopyBoundVariables(lastPattern);

			// perform matching
			var result = patt.Match(expr);
			if (result)
			{
				// store last recognized pattern as a local variable
				thread.SetLastPattern(patt);

				// match succeeded, return result expression
				if (ResultExpression != null)
				{
					return ResultExpression.Evaluate(thread);
				}

				// match succeeded, evaluate more conditions
				if (MoreConditions != null)
				{
					return MoreConditions.Evaluate(thread);
				}
			}

			// matching failed, return nothing
			return null;
		}
	}
}
