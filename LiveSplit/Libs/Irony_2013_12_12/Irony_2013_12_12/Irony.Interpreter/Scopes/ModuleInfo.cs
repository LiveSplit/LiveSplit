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

  public class ModuleInfoList : List<ModuleInfo> { }

  public class ModuleInfo {
    public readonly string Name;
    public readonly string FileName;
    public readonly ScopeInfo ScopeInfo; //scope for module variables
    public readonly BindingSourceList Imports = new BindingSourceList();

    public ModuleInfo(string name, string fileName, ScopeInfo scopeInfo) {
      Name = name;
      FileName = fileName;
      ScopeInfo = scopeInfo;
    }

    //Used for imported modules
    public Binding BindToExport(BindingRequest request) {
      return null; 
    }

  }
}
