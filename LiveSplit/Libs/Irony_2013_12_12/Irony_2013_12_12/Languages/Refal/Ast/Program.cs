// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System.Collections.Generic;
using System.Linq;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Refal.Runtime;
using Irony.Ast;

namespace Refal
{
	/// <summary>
	/// Program is a list of functions.
	/// </summary>
	public class Program : AstNode
	{
		public IDictionary<string, Function> Functions { get; private set; }

		public IList<Function> FunctionList { get; private set; }

		public Function EntryPoint { get; private set; }

		public Program()
		{
			Functions = new Dictionary<string, Function>();
			FunctionList = new List<Function>();
			EntryPoint = null;
		}

    public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			base.Init(context, parseNode);

			foreach (var node in parseNode.ChildNodes)
			{
				if (node.AstNode is Function)
				{
					AddFunction(node.AstNode as Function);
				}
				else if (node.AstNode is AuxiliaryNode)
				{
					var ids = (node.AstNode as AuxiliaryNode).ChildNodes.OfType<IdentifierNode>();

					foreach (var id in ids)
					{
						ExternalFunction ef = new ExternalFunction();
						ef.SetSpan(id.Span);
						ef.Name = id.Symbol;
						AddFunction(ef);
					}
				}
			}

			AsString = "Refal-5 program";
		}

		public override System.Collections.IEnumerable GetChildNodes()
		{
			foreach (var fun in FunctionList)
				yield return fun;
		}

		public void AddFunction(Function function)
		{
			function.Parent = this;
			Functions[function.Name] = function;
			FunctionList.Add(function);
			
			if (function.Name == "Go")
			{
				EntryPoint = function;
			}
		}

		protected override object DoEvaluate(ScriptThread thread)
		{
			// standard prolog
			thread.CurrentNode = this;

			if (EntryPoint == null)
			{
				thread.ThrowScriptError("No entry point defined (entry point is a function named «Go»)");
				return null;
			}

			// define built-in runtime library functions
			var libraryFunctions = LibraryFunction.ExtractLibraryFunctions(thread, new RefalLibrary(thread));
			foreach (var libFun in libraryFunctions)
			{
				var binding = thread.Bind(libFun.Name, BindingRequestFlags.Write | BindingRequestFlags.ExistingOrNew);
				binding.SetValueRef(thread, libFun);
			}

			// define functions declared in the compiled program
			foreach (var fun in FunctionList)
			{
				fun.Evaluate(thread);
			}

			// call entry point with an empty expression
			EntryPoint.Call(thread, new object[] { PassiveExpression.Build() });

			// standard epilog
			thread.CurrentNode = Parent;
			return null;
		}
	}
}
