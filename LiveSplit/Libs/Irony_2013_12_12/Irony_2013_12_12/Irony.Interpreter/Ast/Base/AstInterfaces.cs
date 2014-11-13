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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Irony.Interpreter.Ast{ 

  //This interface is expected by Irony's Gramamr Explorer. 
  public interface ICallTarget {
    object Call(ScriptThread thread, object[] parameters); 
  }

  //Simple visitor interface
  public interface IAstVisitor {
    void BeginVisit(IVisitableNode node);
    void EndVisit(IVisitableNode node);
  }

  public interface IVisitableNode {
    void AcceptVisitor(IAstVisitor visitor);
  }

  public interface IOperatorHelper {
    ExpressionType GetOperatorExpressionType(string symbol);
    ExpressionType GetUnaryOperatorExpressionType(string symbol);

  }
}
