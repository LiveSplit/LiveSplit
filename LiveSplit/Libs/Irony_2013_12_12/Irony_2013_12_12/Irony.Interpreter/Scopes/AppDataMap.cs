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
using System.Text;
using System.Threading;
using Irony.Parsing;
using Irony.Interpreter.Ast;

namespace Irony.Interpreter {

  /// <summary> Represents a set of all of static scopes/modules in the application.  </summary>
  public class AppDataMap  {
    public AstNode ProgramRoot; //artificial root associated with MainModule
    public ScopeInfoList StaticScopeInfos = new ScopeInfoList();
    public ModuleInfoList Modules = new ModuleInfoList(); 
    public ModuleInfo MainModule;
    public readonly bool LanguageCaseSensitive;

    public AppDataMap(bool languageCaseSensitive, AstNode programRoot = null) {
      LanguageCaseSensitive = languageCaseSensitive;
      ProgramRoot = programRoot?? new AstNode(); 
      var mainScopeInfo = new ScopeInfo(ProgramRoot, LanguageCaseSensitive); 
      StaticScopeInfos.Add(mainScopeInfo);
      mainScopeInfo.StaticIndex = 0;
      MainModule = new ModuleInfo("main", "main", mainScopeInfo);
      Modules.Add(MainModule);
    }

    public ModuleInfo GetModule(AstNode moduleNode) {
      foreach (var m in Modules)
        if (m.ScopeInfo == moduleNode.DependentScopeInfo)
          return m;
      return null;
    }


  }//class

}
