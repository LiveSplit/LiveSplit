// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using Irony.Interpreter;
using Irony.Interpreter.Ast;

namespace Refal
{
	/// <summary>
	/// Base node for all functions.
	/// </summary>
	public abstract class Function : AstNode, ICallTarget
	{
		public string Name { get; set; } // TODO: value.Replace("-", "__")

		protected override object DoEvaluate(ScriptThread thread)
		{
			// standard prolog
			thread.CurrentNode = this;

			// define function: bind function name to the current instance
			var binding = thread.Bind(Name, BindingRequestFlags.Write | BindingRequestFlags.NewOnly);
			binding.SetValueRef(thread, this);

			// set Evaluate method and return the current node
			Evaluate = t => this;

			// standard epilog
			thread.CurrentNode = Parent;
			return this;
		}

		public abstract object Call(ScriptThread thread, object[] parameters);
	}
}
