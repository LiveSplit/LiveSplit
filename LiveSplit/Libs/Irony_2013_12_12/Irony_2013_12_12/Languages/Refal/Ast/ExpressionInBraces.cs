// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Refal.Runtime;

namespace Refal
{
	/// <summary>
	/// Expression or pattern in structure braces ().
	/// </summary>
	public class ExpressionInBraces : AstNode
	{
		public AstNode InnerExpression { get; private set; }

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			base.Init(context, parseNode);

			foreach (var node in parseNode.ChildNodes)
			{
				if (node.AstNode is AstNode)
				{
					var astNode = node.AstNode as AstNode;
					astNode.Parent = this;
					InnerExpression = astNode;
				}
			}

			AsString = "(structure braces)";
		}

		public override System.Collections.IEnumerable GetChildNodes()
		{
			return InnerExpression.GetChildNodes();
		}

		protected override object DoEvaluate(ScriptThread thread)
		{
			return EvaluateExpression(thread);
		}

		internal PassiveExpression EvaluateExpression(ScriptThread thread)
		{
			if (InnerExpression == null)
			{
				return PassiveExpression.Build(new OpeningBrace(), new ClosingBrace());
			}

			return PassiveExpression.Build(new OpeningBrace(), InnerExpression.Evaluate(thread), new ClosingBrace());
		}
	}
}
