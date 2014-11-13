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
using System.Reflection;
using Irony.Interpreter.Ast;

namespace Irony.Interpreter {


  // Binding is a link between a variable in the script (for ex, IdentifierNode) and a value storage  - 
  // a slot in local or module-level Scope. Binding to internal variables is supported by SlotBinding class. 
  // Alternatively a symbol can be bound to external CLR entity in imported namespace - class, function, property, etc.
  // Binding is produced by Runtime.Bind method and allows read/write operations through GetValueRef and SetValueRef methods. 
  public class Binding {
    public readonly BindingTargetInfo TargetInfo; 
    public EvaluateMethod GetValueRef;     // ref to Getter method implementation
    public ValueSetterMethod SetValueRef;  // ref to Setter method implementation
    public bool IsConstant { get; protected set; }
    public Binding(BindingTargetInfo targetInfo) {
      TargetInfo = targetInfo;
    }
    public Binding(string symbol, BindingTargetType targetType) {
      TargetInfo = new BindingTargetInfo(symbol, targetType);
    }
    public override string ToString() {
      return "{Binding to + " + TargetInfo.ToString() + "}"; 
    }
  }//class

  //Binding to a "fixed", constant value
  public class ConstantBinding : Binding {
    public object Target; 
    public ConstantBinding(object target, BindingTargetInfo targetInfo) : base(targetInfo) {
      Target = target;
      base.GetValueRef = GetValue;
      IsConstant = true; 
    }

    public object GetValue(ScriptThread thread) {
      return Target; 
    }
    
  }

}
