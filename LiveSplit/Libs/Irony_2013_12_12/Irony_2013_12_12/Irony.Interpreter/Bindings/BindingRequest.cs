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

  [Flags]
  public enum BindingRequestFlags {
    Read = 0x01,
    Write = 0x02,
    Invoke = 0x04,
    ExistingOrNew = 0x10,
    NewOnly = 0x20,  // for new variable, for ex, in JavaScript "var x..." - introduces x as new variable
  }

  //Binding request is a container for information about requested binding. Binding request goes from an Ast node to language runtime. 
  // For example, identifier node would request a binding for an identifier. 
  public class BindingRequest {
    public ScriptThread Thread;
    public AstNode FromNode;
    public ModuleInfo FromModule;
    public BindingRequestFlags Flags;
    public string Symbol;
    public ScopeInfo FromScopeInfo;
    public bool IgnoreCase;
    public BindingRequest(ScriptThread thread, AstNode fromNode, string symbol, BindingRequestFlags flags) {
      Thread = thread;
      FromNode = fromNode;
      FromModule = thread.App.DataMap.GetModule(fromNode.ModuleNode);
      Symbol = symbol;
      Flags = flags;
      FromScopeInfo = thread.CurrentScope.Info;
      IgnoreCase = !thread.Runtime.Language.Grammar.CaseSensitive;
    }
  }

}
