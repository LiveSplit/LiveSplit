// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Refal.Runtime;
using Irony.Ast;

namespace Refal
{
	/// <summary>
	/// DefinedFunction is a function defined in the current compulation unit.
	/// </summary>
	public class DefinedFunction : Function
	{
		public Block Block { get; private set; }

		public bool IsPublic { get; private set; }

		private ScopeInfo ScopeInfo { get; set; }

    public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			base.Init(context, parseNode);

			foreach (var node in parseNode.ChildNodes)
			{
				if (node.AstNode is IdentifierNode)
				{
					Name = (node.AstNode as IdentifierNode).Symbol;
				}
				else if (node.AstNode is Block)
				{
					Block = (node.AstNode as Block);
					Block.Parent = this;
				}
				else if (node.Term is KeyTerm && node.Term.Name == "$ENTRY")
				{
					IsPublic = true;
				}
			}

			ScopeInfo = new ScopeInfo(this, context.Language.Grammar.CaseSensitive);
			AsString = (IsPublic ? "public " : "private ") + Name;
		}

		public override System.Collections.IEnumerable GetChildNodes()
		{
			return Block.GetChildNodes();
		}

		public override object Call(ScriptThread thread, object[] parameters)
		{
			thread.PushScope(ScopeInfo, parameters);

			try
			{
				var expression =
					parameters != null && parameters.Length > 0 ?
						parameters[0] as PassiveExpression : null;

				Block.InputExpression = expression;
				Block.BlockPattern = null;

				return Block.Evaluate(thread);
			}
			finally
			{
				thread.PopScope();
			}
		}
	}
}
