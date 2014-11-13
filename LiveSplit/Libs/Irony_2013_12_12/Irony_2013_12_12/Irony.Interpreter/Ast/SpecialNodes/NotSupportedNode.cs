using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Irony.Ast; 
using Irony.Parsing;

namespace Irony.Interpreter.Ast {
  //A substitute node to use on constructs that are not yet supported by language implementation.
  // The script would compile Ok but on attempt to evaluate the node would throw a runtime exception
  public class NotSupportedNode : AstNode {
    string Name;
    public override void Init(AstContext context, ParseTreeNode treeNode) {
      base.Init(context, treeNode);
      Name = treeNode.Term.ToString();
      AsString = Name + " (not supported)";
    }

    protected override object DoEvaluate(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      thread.ThrowScriptError(Resources.ErrConstructNotSupported, Name);
      return null; //never happens
    }

  }//class
}
