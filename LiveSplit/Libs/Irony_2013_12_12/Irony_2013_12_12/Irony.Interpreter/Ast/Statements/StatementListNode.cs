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

  public class StatementListNode : AstNode {
    AstNode _singleChild; //stores a single child when child count == 1, for fast access

    public override void Init(AstContext context, ParseTreeNode treeNode) {
      base.Init(context, treeNode);
      var nodes = treeNode.GetMappedChildNodes();
      foreach (var child in nodes) {
        //don't add if it is null; it can happen that "statement" is a comment line and statement's node is null.
        // So to make life easier for language creator, we just skip if it is null
        if (child.AstNode != null) 
          AddChild(string.Empty, child); 
      }
      AsString = "Statement List";
      if (ChildNodes.Count == 0) {
        AsString += " (Empty)";
      } else 
         ChildNodes[ChildNodes.Count - 1].Flags |= AstNodeFlags.IsTail;
    }

    protected override object DoEvaluate(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      lock (LockObject) {
        switch (ChildNodes.Count) {
          case 0:
            Evaluate = EvaluateEmpty;
            break;
          case 1:
            _singleChild = ChildNodes[0];
            Evaluate = EvaluateOne;
            break; 
          default:
            Evaluate = EvaluateMultiple;
            break; 
        }//switch
      }//lock
      var result = Evaluate(thread);
      thread.CurrentNode = Parent; //standard epilog
      return result;
    }

    private object EvaluateEmpty(ScriptThread thread) {
      return null; 
    }

    private object EvaluateOne(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      object result = _singleChild.Evaluate(thread);
      thread.CurrentNode = Parent; //standard epilog
      return result;
    }

    private object EvaluateMultiple(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      object result = null;
      for (int i=0; i< ChildNodes.Count; i++) {
        result = ChildNodes[i].Evaluate(thread);
      }
      thread.CurrentNode = Parent; //standard epilog
      return result; //return result of last statement
    }

    public override void SetIsTail() {
      base.SetIsTail();
      if (ChildNodes.Count > 0)
        ChildNodes[ChildNodes.Count - 1].SetIsTail(); 
    }

    
  }//class

}//namespace
