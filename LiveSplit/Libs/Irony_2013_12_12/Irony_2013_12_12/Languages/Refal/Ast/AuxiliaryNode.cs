// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System;
using System.Collections.Generic;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Irony.Ast;

namespace Refal
{
	/// <summary>
	/// Temporary AST nodes used internally while building AST.
	/// </summary>
	public class AuxiliaryNode : AstNode
	{
		public IList<ParseTreeNode> ChildParseNodes { get; private set; }

		public AuxiliaryNode()
		{
			ChildParseNodes = new List<ParseTreeNode>();
		}

    public override void Init(AstContext context, ParseTreeNode treeNode)
		{
			base.Init(context, treeNode);
			
			foreach (var node in treeNode.ChildNodes)
			{
				// linearize AuxiliaryNode children
				if (node.AstNode is AuxiliaryNode)
				{
					var auxNode = node.AstNode as AuxiliaryNode;

					foreach (var n in auxNode.ChildNodes)
						ChildNodes.Add(n);

					foreach (var n in auxNode.ChildParseNodes)
						ChildParseNodes.Add(n);

					continue;
				}

				// copy AstNode nodes
				if (node.AstNode is AstNode)
				{
					ChildNodes.Add(node.AstNode as AstNode);
					continue;
				}

				// otherwise, save parse nodes
				ChildParseNodes.Add(node);
			}
		}

		public override System.Collections.IEnumerable GetChildNodes()
		{
			throw new NotImplementedException("Auxiliary nodes should not appear in the final AST");
		}

		protected override object DoEvaluate(ScriptThread thread)
		{
			throw new NotImplementedException("Auxiliary node cannot be interpreted");
		}
	}
}
