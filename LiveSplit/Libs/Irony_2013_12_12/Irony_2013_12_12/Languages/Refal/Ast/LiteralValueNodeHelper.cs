// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using Irony.Interpreter.Ast;
using Irony.Parsing;
using Irony.Ast;

namespace Refal
{
	/// <summary>
	/// Initializes Refal literal nodes.
	/// </summary>
	public static class LiteralValueNodeHelper
	{
		/// <summary>
		/// Converts identifiers to compound symbols (strings in double quotes),
		/// expands character strings (in single quotes) to arrays of characters
		/// </summary>
		public static void InitNode(AstContext context, ParseTreeNode parseNode)
		{
			foreach (var node in parseNode.ChildNodes)
			{
				if (node.AstNode is LiteralValueNode)
				{
					if (node.Term.Name == "Char")
					{
						var literal = node.AstNode as LiteralValueNode;
						literal.Value = literal.Value.ToString().ToCharArray();
					}

					parseNode.AstNode = node.AstNode;
				}
				else
				{
					// identifiers in expressions are treated as strings (True is same as "True")
					parseNode.AstNode = new LiteralValueNode()
					{
						Value = node.FindTokenAndGetText(),
						Span = node.Span
					};
				}
			}
		}
	}
}
