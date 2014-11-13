// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System.Collections.Generic;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Irony.Ast;

namespace Refal
{
	/// <summary>
	/// Pattern is a passive expression that may contain free variables.
	/// </summary>
	public class Pattern : AstNode
	{
		public IList<AstNode> Terms { get; private set; }

		public bool IsEmpty
		{
			get { return Terms.Count == 0; }
		}

		public Pattern()
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
					astNode.UseType = NodeUseType.Name; // do not evaluate pattern variables
					Terms.Add(astNode);
				}
			}

			AsString = "pattern";
		}

		public override System.Collections.IEnumerable GetChildNodes()
		{
			foreach (var term in Terms)
				yield return term;
		}

		protected override object DoEvaluate(ScriptThread thread)
		{
			return Instantiate(thread);
		}

		private object[] EvaluateTerms(ScriptThread thread)
		{
			// standard prolog
			thread.CurrentNode = this;

			var terms = new List<object>();

			foreach (var term in Terms)
			{
				// in pattern, variables are never read
				var result = term.Evaluate(thread);
				terms.Add(result);
			}

			// standard epilog
			thread.CurrentNode = Parent;

			return terms.ToArray();
		}
		
		public Runtime.Pattern Instantiate(ScriptThread thread)
		{
			// evaluate pattern and instantiate Runtime.Pattern
			return new Runtime.Pattern(EvaluateTerms(thread));
		}
	}
}
