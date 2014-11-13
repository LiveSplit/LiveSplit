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
using Irony.Interpreter.Ast; 

namespace Irony.Interpreter {
  // A general delegate representing a built-in method implementation. 
  public delegate object BuiltInMethod(ScriptThread thread, object[] args);
  
  //A wrapper to convert BuiltInMethod delegate (referencing some custom method in LanguageRuntime) into an ICallTarget instance (expected by FunctionCallNode)
  public class BuiltInCallTarget : ICallTarget {
    public string Name;
    public readonly BuiltInMethod Method;
    public readonly int MinParamCount, MaxParamCount;
    public string[] ParameterNames; //Just for information purpose
    public BuiltInCallTarget(BuiltInMethod method, string name, int minParamCount = 0, int maxParamCount = 0, string parameterNames = null) {
      Method = method;
      Name = name;
      MinParamCount = minParamCount;
      MaxParamCount = Math.Max(MinParamCount, maxParamCount); 
      if (!string.IsNullOrEmpty(parameterNames))
        ParameterNames = parameterNames.Split(','); 
    }

    #region ICallTarget Members
    public object Call(ScriptThread thread, object[] parameters) {
      return Method(thread, parameters);  
    }
    #endregion
  }

  // The class contains information about built-in function. It has double purpose. 
  // First, it is used as a BindingTargetInfo instance (meta-data) for a binding to a built-in function. 
  // Second, we use it as a reference to a custom built-in method that we store in LanguageRuntime.BuiltIns table. 
  // For this, we make it implement IBindingSource - we can add it to BuiltIns table of LanguageRuntime, which is a table of IBindingSource instances.
  // Being IBindingSource, it can produce a binding object to the target method - singleton in fact; 
  // the same binding object is used for all calls to the method from all function-call AST nodes. 
  public class BuiltInCallableTargetInfo : BindingTargetInfo,  IBindingSource  {
    public Binding BindingInstance; //A singleton binding instance; we share it for all AST nodes (function call nodes) that call the method. 

    public BuiltInCallableTargetInfo(BuiltInMethod method, string methodName, int minParamCount = 0, int maxParamCount = 0, string parameterNames = null) : 
        this(new BuiltInCallTarget(method, methodName, minParamCount, maxParamCount, parameterNames)) {
    }
    public BuiltInCallableTargetInfo(BuiltInCallTarget target)  : base(target.Name, BindingTargetType.BuiltInObject) {
      BindingInstance = new ConstantBinding(target, this); 
    }

    //Implement IBindingSource.Bind
    public Binding Bind(BindingRequest request) {
      return BindingInstance;
    }

  }//class

  // Method for adding methods to BuiltIns table in Runtime
  public static partial class BindingSourceTableExtensions {
    public static BindingTargetInfo AddMethod(this BindingSourceTable targets, BuiltInMethod method, string methodName,
          int minParamCount = 0, int maxParamCount = 0, string parameterNames = null) {
      var callTarget = new BuiltInCallTarget(method, methodName, minParamCount, maxParamCount, parameterNames); 
      var targetInfo = new BuiltInCallableTargetInfo(callTarget);
      targets.Add(methodName, targetInfo);
      return targetInfo; 
    }
  }

}//namespace 
