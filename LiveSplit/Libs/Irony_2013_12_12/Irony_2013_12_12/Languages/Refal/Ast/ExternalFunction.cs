// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using Irony.Ast; 
using Irony.Interpreter;
using Irony.Parsing;

namespace Refal
{
	/// <summary>
	/// External function is a library function referenced from the current compilation unit.
	/// External functions are not supported yet.
	/// </summary>
	public class ExternalFunction : Function
	{
		public void SetSpan(SourceSpan sourceSpan)
		{
			Span = sourceSpan;
		}

    public override void Init(AstContext context, ParseTreeNode treeNode)
		{
			base.Init(context, treeNode);
			AsString = "extern " + Name;
		}

		public override System.Collections.IEnumerable GetChildNodes()
		{
			yield break;
		}

		public override object Call(ScriptThread thread, object[] parameters)
		{
			thread.ThrowScriptError("Calling external function is not supported");
			return null;
		}
	}
}
