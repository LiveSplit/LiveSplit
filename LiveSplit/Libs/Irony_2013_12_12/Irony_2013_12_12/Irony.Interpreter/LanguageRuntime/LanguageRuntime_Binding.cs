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

  public partial class LanguageRuntime : IBindingSource {

    //Binds to local variables, enclosing scopes, module scopes/globals and built-ins
    public virtual Binding Bind(BindingRequest request) {
      var symbol = request.Symbol;
      var mode = request.Flags;
      if (mode.IsSet(BindingRequestFlags.Write))
        return BindSymbolForWrite(request);
      else if (mode.IsSet(BindingRequestFlags.Read))
        return BindSymbolForRead(request);
      else {
        //TODO: need to throw fatal error here
        request.Thread.ThrowScriptError("Invalid binding request, access type (Read or Write) is not set in request Options.");
        return null; // never happens
      }
    }//method

    public virtual Binding BindSymbolForWrite(BindingRequest request) {
      var scope = request.Thread.CurrentScope;
      var existingSlot = scope.Info.GetSlot(request.Symbol);
      //1. If new only, check it does not exist yet, create and return it
      if (request.Flags.IsSet(BindingRequestFlags.NewOnly)) {
        if (existingSlot != null) 
            request.Thread.ThrowScriptError("Variable {0} already exists.", request.Symbol);     
        var newSlot = scope.AddSlot(request.Symbol);
        return new SlotBinding(newSlot, request.FromNode, request.FromScopeInfo);
      }
      //2. If exists, then return it
      if (existingSlot != null && request.Flags.IsSet(BindingRequestFlags.ExistingOrNew)) {
        //TODO: For external client, check that slot is actually public or exported
        return new SlotBinding(existingSlot, request.FromNode, request.FromScopeInfo);
      }

      //3. Check external module imports
      foreach (var imp in request.FromModule.Imports) {
        var result = imp.Bind(request);
        if (result != null)
          return result;
      }

      //4. If nothing found, create new slot in current scope
      if (request.Flags.IsSet(BindingRequestFlags.ExistingOrNew)) {
        var newSlot = scope.AddSlot(request.Symbol);
        return new SlotBinding(newSlot, request.FromNode, request.FromScopeInfo);
      }

      //5. Check built-in methods
      var builtIn = BuiltIns.Bind(request);
      if (builtIn != null) return builtIn; 

      //6. If still not found, return null.
      return null; 
    }//method

    public virtual Binding BindSymbolForRead(BindingRequest request) {
      var symbol = request.Symbol;
      // First check current and enclosing scopes
      var currScope = request.Thread.CurrentScope;
      do {
        var existingSlot = currScope.Info.GetSlot(symbol);
        if (existingSlot != null)
          return new SlotBinding(existingSlot, request.FromNode, request.FromScopeInfo);
        currScope = currScope.Parent;
      } while (currScope != null);

      // If not found, check imports
      foreach (var imp in request.FromModule.Imports) {
        var result = imp.Bind(request);
        if (result != null)
          return result; 
      }

      // Check built-in modules
      var builtIn = BuiltIns.Bind(request);
      if (builtIn != null) return builtIn;
      
      // if not found, return null
      return null;
    }

    //Binds symbol to a public member exported by a module.
    public virtual Binding BindSymbol(BindingRequest request, ModuleInfo module) {
      return module.BindToExport(request); 
    }


  }//class
}
