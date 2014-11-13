#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Irony.Ast;
using Irony.Parsing;

namespace Irony.Interpreter.Ast {

  //A node representing function definition (named lambda)
  public class FunctionDefNode : AstNode {
    public AstNode NameNode;
    public LambdaNode Lambda; 

    public override void Init(AstContext context, ParseTreeNode treeNode) {
      base.Init(context, treeNode);
      //child #0 is usually a keyword like "def"
      var nodes = treeNode.GetMappedChildNodes();
      NameNode = AddChild("Name", nodes[1]);
      Lambda = new LambdaNode(context, treeNode, nodes[2], nodes[3]); //node, params, body
      Lambda.Parent = this; 
      AsString = "<Function " + NameNode.AsString + ">";
      //Lamda will set treeNode.AstNode to itself, we need to set it back to "this" here
      treeNode.AstNode = this; //
    }

    public override void Reset() {
      DependentScopeInfo = null;
      Lambda.Reset(); 
      base.Reset();
    }

    protected override object DoEvaluate(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      var closure = Lambda.Evaluate(thread); //returns closure
      NameNode.SetValue(thread, closure); 
      thread.CurrentNode = Parent; //standard epilog
      return closure;
    }

  }//class

}//namespace
