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
using System.Linq.Expressions; 
using System.Text;
using System.Reflection;

using Irony.Ast;
using Irony.Parsing;

namespace Irony.Interpreter.Ast {
  public class BinaryOperationNode : AstNode {
    public AstNode Left, Right;
    public string OpSymbol;
    public ExpressionType Op;
    private OperatorImplementation _lastUsed;
    private object _constValue;
    private int _failureCount;

    public BinaryOperationNode() { }

    public override void Init(AstContext context, ParseTreeNode treeNode) {
      base.Init(context, treeNode);
      var nodes = treeNode.GetMappedChildNodes();
      Left = AddChild("Arg", nodes[0]);
      Right = AddChild("Arg", nodes[2]);
      var opToken = nodes[1].FindToken();
      OpSymbol = opToken.Text;
      var ictxt = context as InterpreterAstContext;
      Op = ictxt.OperatorHandler.GetOperatorExpressionType(OpSymbol);
      // Set error anchor to operator, so on error (Division by zero) the explorer will point to 
      // operator node as location, not to the very beginning of the first operand.
      ErrorAnchor = opToken.Location;
      AsString = Op + "(operator)"; 
    }

    protected override object DoEvaluate(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      //assign implementation method
      switch (Op) {
        case ExpressionType.AndAlso:
          this.Evaluate = EvaluateAndAlso;
          break; 
        case ExpressionType.OrElse:
          this.Evaluate = EvaluateOrElse;
          break;
        default:
          this.Evaluate = DefaultEvaluateImplementation;
          break; 
      }
      // actually evaluate and get the result.
      var result = Evaluate(thread); 
      // Check if result is constant - if yes, save the value and switch to method that directly returns the result.
      if (IsConstant()) {
        _constValue = result;
        AsString = Op + "(operator) Const=" + _constValue;
        this.Evaluate = EvaluateConst;
      }
      thread.CurrentNode = Parent; //standard epilog
      return result;
    }

    private object EvaluateAndAlso(ScriptThread thread) {
      var leftValue = Left.Evaluate(thread); 
      if (!thread.Runtime.IsTrue(leftValue)) return leftValue; //if false return immediately
      return Right.Evaluate(thread); 
    }
    private object EvaluateOrElse(ScriptThread thread) {
      var leftValue = Left.Evaluate(thread);
      if (thread.Runtime.IsTrue(leftValue)) return leftValue;
      return Right.Evaluate(thread);
    }

    protected object EvaluateFast(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      var arg1 = Left.Evaluate(thread);
      var arg2 = Right.Evaluate(thread);
      //If we have _lastUsed, go straight for it; if types mismatch it will throw
      if (_lastUsed != null) {
        try {
          var res = _lastUsed.EvaluateBinary(arg1, arg2);
          thread.CurrentNode = Parent; //standard epilog
          return res;
        } catch {
          _lastUsed = null;
          _failureCount++;
          // if failed 3 times, change to method without direct try
          if (_failureCount > 3)
            Evaluate = DefaultEvaluateImplementation;
        } //catch
      }// if _lastUsed
      // go for normal evaluation
      var result = thread.Runtime.ExecuteBinaryOperator(this.Op, arg1, arg2, ref _lastUsed);
      thread.CurrentNode = Parent; //standard epilog
      return result;
    }//method

    protected object DefaultEvaluateImplementation(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      var arg1 = Left.Evaluate(thread);
      var arg2 = Right.Evaluate(thread);
      var result = thread.Runtime.ExecuteBinaryOperator(this.Op, arg1, arg2, ref _lastUsed);
      thread.CurrentNode = Parent; //standard epilog
      return result;
    }//method

    private object EvaluateConst(ScriptThread thread) {
      return _constValue; 
    }

    public override bool IsConstant() {
      if (_isConstant) return true; 
      _isConstant = Left.IsConstant() && Right.IsConstant();
      return _isConstant;
    } bool _isConstant; 
  }//class
}//namespace
