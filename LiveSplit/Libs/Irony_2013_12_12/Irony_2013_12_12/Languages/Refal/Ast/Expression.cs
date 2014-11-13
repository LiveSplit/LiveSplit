// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System.Collections.Generic;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Refal.Runtime;
using Irony.Ast;

namespace Refal
{
	/// <summary>
	/// Expression is a sequence of symbols, macrodigits, bound variables and function calls.
	/// </summary>
	public class Expression : AstNode
	{
		public IList<AstNode> Terms { get; private set; }

		public Expression()
		{
			Terms = new List<AstNode>();
		}

    public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			base.Init(context, parseNode);

			foreach (var node in parseNode.ChildNodes)
			{
				if (node.AstNode is AstNode)
				{
					var astNode = node.AstNode as AstNode;
					astNode.Parent = this;
					Terms.Add(astNode);
				}
			}

			AsString = "expression";
		}

		public override System.Collections.IEnumerable GetChildNodes()
		{
			foreach (var term in Terms)
				yield return term;
		}

		public bool IsEmpty
		{
			get { return Terms.Count == 0; }
		}

		protected override object DoEvaluate(ScriptThread thread)
		{
			return EvaluateExpression(thread);
		}

		internal Runtime.PassiveExpression EvaluateExpression(ScriptThread thread)
		{
			// standard prolog
			thread.CurrentNode = this;

			var terms = new List<object>();

			foreach (var term in Terms)
			{
				var result = term.Evaluate(thread);
				terms.Add(result);
			}

			// standard epilog
			thread.CurrentNode = Parent;

			return PassiveExpression.Build(terms.ToArray());
		}
	}
}
