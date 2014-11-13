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
using System.Linq.Expressions;
using System.Text;

using Irony.Ast;
using Irony.Parsing;

namespace Irony.Interpreter.Ast {
  public class AssignmentNode : AstNode {
    public AstNode Target;
    public string AssignmentOp;
    public bool IsAugmented; // true if it is augmented operation like "+="
    public ExpressionType BinaryExpressionType; 
    public AstNode Expression;
    private OperatorImplementation _lastUsed;
    private int _failureCount;

    public override void Init(AstContext context, ParseTreeNode treeNode) {
      base.Init(context, treeNode);
      var nodes = treeNode.GetMappedChildNodes();
      Target = AddChild(NodeUseType.ValueWrite, "To", nodes[0]);
      //Get Op and baseOp if it is combined assignment
      AssignmentOp = nodes[1].FindTokenAndGetText();
      if (string.IsNullOrEmpty(AssignmentOp))
        AssignmentOp = "=";
      BinaryExpressionType = CustomExpressionTypes.NotAnExpression;
      //There maybe an "=" sign in the middle, or not - if it is marked as punctuation; so we just take the last node in child list
      Expression = AddChild(NodeUseType.ValueRead, "Expr", nodes[nodes.Count - 1]);
      AsString = AssignmentOp + " (assignment)";
      // TODO: this is not always correct: in Pascal the assignment operator is :=.
      IsAugmented = AssignmentOp.Length > 1;
      if (IsAugmented) {

        var ictxt = context as InterpreterAstContext;
        base.ExpressionType = ictxt.OperatorHandler.GetOperatorExpressionType(AssignmentOp);
        BinaryExpressionType = ictxt.OperatorHandler.GetBinaryOperatorForAugmented(this.ExpressionType);
        Target.UseType = NodeUseType.ValueReadWrite;
      }
    }

    protected override object DoEvaluate(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      if (IsAugmented)
        Evaluate = EvaluateAugmentedFast;
      else
        Evaluate = EvaluateSimple; //non-augmented
      //call self-evaluate again, now to call real methods
      var result = this.Evaluate(thread);
      thread.CurrentNode = Parent; //standard epilog
      return result; 
    }


    private object EvaluateSimple(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      var value = Expression.Evaluate(thread);
      Target.SetValue(thread, value);
      thread.CurrentNode = Parent; //standard epilog
      return value;
    }

    private object EvaluateAugmentedFast(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      var value = Target.Evaluate(thread);
      var exprValue = Expression.Evaluate(thread);
      object result = null; 
      if (_lastUsed != null) {
        try {
          result = _lastUsed.EvaluateBinary(value, exprValue);
        } catch {
          _failureCount++;
          // if failed 3 times, change to method without direct try
          if (_failureCount > 3)
            Evaluate = EvaluateAugmented;
        } //catch
      }// if _lastUsed
      if (result == null)
        result = thread.Runtime.ExecuteBinaryOperator(BinaryExpressionType, value, exprValue, ref _lastUsed);
      Target.SetValue(thread, result);
      thread.CurrentNode = Parent; //standard epilog
      return result;
    }

    private object EvaluateAugmented(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      var value = Target.Evaluate(thread);
      var exprValue = Expression.Evaluate(thread);
      var result = thread.Runtime.ExecuteBinaryOperator(BinaryExpressionType, value, exprValue, ref _lastUsed);
      Target.SetValue(thread, result);
      thread.CurrentNode = Parent; //standard epilog
      return result;
    }

  
  }
}
