using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Irony.Ast;
using Irony.Parsing;
using Irony.Interpreter;

namespace Irony.Interpreter.Ast {
  public class LiteralValueNode : AstNode {
    public object Value;

    public override void Init(AstContext context, ParseTreeNode treeNode) {
      base.Init(context, treeNode); 
      Value = treeNode.Token.Value;
      AsString = Value == null ? "null" : Value.ToString();
      if (Value is string)
        AsString = "\"" + AsString + "\"";
    }

    protected override object DoEvaluate(ScriptThread thread) {
      return Value; 
    }

    public override bool IsConstant() {
      return true; 
    }
  }//class
}
