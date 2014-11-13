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

  //A node representing an anonymous function
  public class LambdaNode : AstNode {
    public AstNode Parameters;
    public AstNode Body;

    public LambdaNode() { }

    //Used by FunctionDefNode
    public LambdaNode(AstContext context, ParseTreeNode node, ParseTreeNode parameters, ParseTreeNode body) {
      InitImpl(context, node, parameters, body);
    }

    public override void Init(AstContext context, ParseTreeNode parseNode) {
      var mappedNodes = parseNode.GetMappedChildNodes();
      InitImpl(context, parseNode, mappedNodes[0], mappedNodes[1]);
    }

    private void InitImpl(AstContext context, ParseTreeNode parseNode, ParseTreeNode parametersNode, ParseTreeNode bodyNode) {
      base.Init(context, parseNode);
      Parameters = AddChild("Parameters", parametersNode);
      Body = AddChild("Body", bodyNode);
      AsString = "Lambda[" + Parameters.ChildNodes.Count + "]";
      Body.SetIsTail(); //this will be propagated to the last statement
    }

    public override void Reset() {
      DependentScopeInfo = null; 
      base.Reset();
    }

    protected override object DoEvaluate(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      lock (LockObject) {
        if (DependentScopeInfo == null) {
          var langCaseSensitive = thread.App.Language.Grammar.CaseSensitive;
          DependentScopeInfo = new ScopeInfo(this, langCaseSensitive);
        }
        // In the first evaluation the parameter list will add parameter's SlotInfo objects to Scope.ScopeInfo
        thread.PushScope(DependentScopeInfo, null);
        Parameters.Evaluate(thread);
        thread.PopScope();
        //Set Evaluate method and invoke it later
        this.Evaluate = EvaluateAfter;
      }
      var result = Evaluate(thread);
      thread.CurrentNode = Parent; //standard epilog
      return result;
    }

    private object EvaluateAfter(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      var closure = new Closure(thread.CurrentScope, this);
      thread.CurrentNode = Parent; //standard epilog
      return closure;
    }

    public object Call(Scope creatorScope, ScriptThread thread, object[] parameters) {
      var save = thread.CurrentNode; //prolog, not standard - the caller is NOT target node's parent
      thread.CurrentNode = this;
      thread.PushClosureScope(DependentScopeInfo, creatorScope, parameters);
      Parameters.Evaluate(thread); // pre-process parameters
      var result = Body.Evaluate(thread);
      thread.PopScope();
      thread.CurrentNode = save; //epilog, restoring caller 
      return result;
    }


    public override void SetIsTail() {
      //ignore this call, do not mark this node as tail, it is meaningless
    }
  }//class

}//namespace
