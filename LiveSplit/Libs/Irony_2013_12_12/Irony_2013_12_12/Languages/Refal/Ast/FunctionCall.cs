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
	/// Function call.
	/// </summary>
	public class FunctionCall : AstNode
	{
		public string FunctionName { get; private set; }

		public Expression Expression { get; private set; }

		private SourceSpan? NameSpan { get; set; }

    public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			base.Init(context, parseNode);

			foreach (var node in parseNode.ChildNodes)
			{
				if (node.AstNode is AuxiliaryNode)
				{
					var auxNode = node.AstNode as AuxiliaryNode;
					NameSpan = auxNode.Span;

					// function call can be either Identifier: <Prout e.1>
					var identifier = auxNode.ChildNodes.OfType<IdentifierNode>().FirstOrDefault();
					if (identifier != null)
					{
						FunctionName = identifier.Symbol;
						continue;
					}

					// or symbol of arithmetic operation: <- s.1 1>
					var pnode = auxNode.ChildParseNodes.Where(n => n.Term != null).FirstOrDefault();
					if (pnode != null)
					{
						FunctionName = pnode.Term.Name;
						continue;
					}

					// and nothing else
					throw new InvalidOperationException("Invalid function call");
				}
				else if (node.AstNode is Expression)
				{
					var astNode = node.AstNode as Expression;
					astNode.Parent = this;
					Expression = astNode;
				}
			}

			// error anchor points to the exact error location in the source code
			ErrorAnchor = (NameSpan != null ? NameSpan.Value : Span).Location;
			AsString = "call " + FunctionName;
		}

		public override System.Collections.IEnumerable GetChildNodes()
		{
			return Expression.GetChildNodes();
		}

		private ICallTarget CallTarget { get; set; }

		protected override object DoEvaluate(ScriptThread thread)
		{
			// standard prolog
			thread.CurrentNode = this;

			var binding = thread.Bind(FunctionName, BindingRequestFlags.Read);
			var result = binding.GetValueRef(thread);
			if (result == null)
			{
				thread.ThrowScriptError("Unknown identifier: {0}", FunctionName);
				return null;
			}

			CallTarget = result as ICallTarget;
			if (CallTarget == null)
			{
				thread.ThrowScriptError("This identifier cannot be called: {0}", FunctionName);
				return null;
			}

			// set Evaluate pointer
			Evaluate = DoCall;

			// standard epilog is done by DoCall
			return DoCall(thread);
		}

		private object DoCall(ScriptThread thread)
		{
			// standard prolog
			thread.CurrentNode = this;

			var parameter = Expression.Evaluate(thread);
			var result = CallTarget.Call(thread, new object[] { parameter });

			// standard epilog
			thread.CurrentNode = Parent;
			return result;
		}
	}
}
