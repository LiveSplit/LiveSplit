// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System.Collections.Generic;
using System.Linq;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Irony.Ast;

namespace Refal
{
	/// <summary>
	/// Block is a sequence of sentences.
	/// </summary>
	public class Block : AstNode
	{
		public IList<Sentence> Sentences { get; private set; }

		public Runtime.Pattern BlockPattern { get; set; }

		public Runtime.PassiveExpression InputExpression { get; set; }

		public Block()
		{
			Sentences = new List<Sentence>();
		}

    public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			base.Init(context, parseNode);

			foreach (var node in parseNode.ChildNodes)
			{
				// copy sentences to block
				if (node.AstNode is AuxiliaryNode)
				{
					var auxNode = node.AstNode as AuxiliaryNode;

					foreach (var s in auxNode.ChildNodes.OfType<Sentence>())
					{
						s.Parent = this;
						Sentences.Add(s);
					}
				}
			}

			AsString = "block";
		}

		public override System.Collections.IEnumerable GetChildNodes()
		{
			foreach (Sentence s in Sentences)
				yield return s;
		}

		protected override object DoEvaluate(ScriptThread thread)
		{
			// standard prolog
			thread.CurrentNode = this;

			foreach (var sentence in Sentences)
			{
				sentence.InputExpression = InputExpression;
				sentence.BlockPattern = BlockPattern;

				var result = sentence.Evaluate(thread);
				if (result != null)
				{
					// standard epilog
					thread.CurrentNode = Parent;
					return result;
				}
			}

			// standard Refal exception: input expression doesn't match any pattern
			thread.ThrowScriptError("Recognition impossible");
			return null;
		}
	}
}
