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

  // Implements fast access to a variable (local/global var or parameter) in local scope or in any enclosing scope
  // Important: the following code is very sensitive to even tiny changes - do not know exactly particular reasons. 
  public sealed class SlotBinding : Binding {
    public SlotInfo Slot;
    public ScopeInfo FromScope;
    public int SlotIndex;
    public int StaticScopeIndex;
    public AstNode FromNode;

    public SlotBinding(SlotInfo slot, AstNode fromNode, ScopeInfo fromScope) : base(slot.Name, BindingTargetType.Slot) {
      Slot = slot;
      FromNode = fromNode;
      FromScope = fromScope;
      SlotIndex = slot.Index;
      StaticScopeIndex = Slot.ScopeInfo.StaticIndex;
      SetupAccessorMethods();
    }

    private void SetupAccessorMethods() {
      // Check module scope
      if (Slot.ScopeInfo.StaticIndex >= 0) {
        GetValueRef = FastGetStaticValue;
        SetValueRef = SetStatic; 
        return;
      }
      var levelDiff = Slot.ScopeInfo.Level - FromScope.Level;
      switch (levelDiff) {
        case 0: // local scope 
          if (Slot.Type == SlotType.Value) {
            base.GetValueRef = FastGetCurrentScopeValue;
            base.SetValueRef = SetCurrentScopeValue;
          } else {
            base.GetValueRef = FastGetCurrentScopeParameter;
            base.SetValueRef = SetCurrentScopeParameter;
          }
          return; 
        case 1: //direct parent
          if (Slot.Type == SlotType.Value) {
            base.GetValueRef = GetImmediateParentScopeValue;
            base.SetValueRef = SetImmediateParentScopeValue;
          } else {
            base.GetValueRef = GetImmediateParentScopeParameter;
            base.SetValueRef = SetImmediateParentScopeParameter;
          }
          return;
        default: // some enclosing scope
          if (Slot.Type == SlotType.Value) {
            base.GetValueRef = GetParentScopeValue;
            base.SetValueRef = SetParentScopeValue;
          } else {
            base.GetValueRef = GetParentScopeParameter;
            base.SetValueRef = SetParentScopeParameter;
          }
          return;
      }
    }

    // Specific method implementations =======================================================================================================
    // Optimization: in most cases we go directly for Values array; if we fail, then we fallback to full method 
    // with proper exception handling. This fallback is expected to be extremely rare, so overall we have considerable perf gain
    // Note that in we expect the methods to be used directly by identifier node (like: IdentifierNode.EvaluateRef = Binding.GetValueRef; } - 
    // to save a few processor cycles. Therefore, we need to provide a proper context (thread.CurrentNode) in case of exception. 
    // In all "full-method" implementations we set current node to FromNode, so exception correctly points 
    // to the owner Identifier node as a location of error. 

    // Current scope
    private object FastGetCurrentScopeValue(ScriptThread thread) {
      try {
        //optimization: we go directly for values array; if we fail, then we fallback to regular "proper" method.
        return thread.CurrentScope.Values[SlotIndex];
      } catch {
        return GetCurrentScopeValue(thread);
      }
    }

    private object GetCurrentScopeValue(ScriptThread thread) {
      try {
        return thread.CurrentScope.GetValue(SlotIndex);
      } catch { thread.CurrentNode = FromNode; throw; }
    }


    private object FastGetCurrentScopeParameter(ScriptThread thread) {
      //optimization: we go directly for parameters array; if we fail, then we fallback to regular "proper" method.
      try {
        return thread.CurrentScope.Parameters[SlotIndex];
      } catch {
        return GetCurrentScopeParameter(thread);
      }
    }
    private object GetCurrentScopeParameter(ScriptThread thread) {
      try {
        return thread.CurrentScope.GetParameter(SlotIndex);
      } catch { thread.CurrentNode = FromNode; throw; }
    }

    private void SetCurrentScopeValue(ScriptThread thread, object value) {
      thread.CurrentScope.SetValue(SlotIndex, value);
    }

    private void SetCurrentScopeParameter(ScriptThread thread, object value) {
      thread.CurrentScope.SetParameter(SlotIndex, value);
    }

    // Static scope (module-level variables)
    private object FastGetStaticValue(ScriptThread thread) {
      try {
        return thread.App.StaticScopes[StaticScopeIndex].Values[SlotIndex];
      } catch {
        return GetStaticValue(thread);
      }
    }
    private object GetStaticValue(ScriptThread thread) {
      try {
        return thread.App.StaticScopes[StaticScopeIndex].GetValue(SlotIndex);
      } catch { thread.CurrentNode = FromNode; throw; }
    }


    private void SetStatic(ScriptThread thread, object value) {
      thread.App.StaticScopes[StaticScopeIndex].SetValue(SlotIndex, value);
    }

    // Direct parent
    private object GetImmediateParentScopeValue(ScriptThread thread) {
      try {
        return thread.CurrentScope.Parent.Values[SlotIndex];
      } catch { }
      //full method
      try {
        return thread.CurrentScope.Parent.GetValue(SlotIndex);
      } catch { thread.CurrentNode = FromNode; throw; }
    }

    private object GetImmediateParentScopeParameter(ScriptThread thread) {
      try {
        return thread.CurrentScope.Parent.Parameters[SlotIndex];
      } catch { }
      //full method
      try {
        return thread.CurrentScope.Parent.GetParameter(SlotIndex);
      } catch { thread.CurrentNode = FromNode; throw; }
    }

    private void SetImmediateParentScopeValue(ScriptThread thread, object value) {
      thread.CurrentScope.Parent.SetValue(SlotIndex, value);
    }

    private void SetImmediateParentScopeParameter(ScriptThread thread, object value) {
      thread.CurrentScope.Parent.SetParameter(SlotIndex, value);
    }

    // Generic case
    private object GetParentScopeValue(ScriptThread thread) {
      var targetScope = GetTargetScope(thread);
      return targetScope.GetValue(SlotIndex);
    }
    private object GetParentScopeParameter(ScriptThread thread) {
      var targetScope = GetTargetScope(thread);
      return targetScope.GetParameter(SlotIndex);
    }
    private void SetParentScopeValue(ScriptThread thread, object value) {
      var targetScope = GetTargetScope(thread);
      targetScope.SetValue(SlotIndex, value);
    }
    private void SetParentScopeParameter(ScriptThread thread, object value) {
      var targetScope = GetTargetScope(thread);
      targetScope.SetParameter(SlotIndex, value);
    }
    private Scope GetTargetScope(ScriptThread thread) {
      var targetLevel = Slot.ScopeInfo.Level;
      var scope = thread.CurrentScope.Parent;
      while (scope.Info.Level > targetLevel)
        scope = scope.Parent;
      return scope;
    }


  }//class SlotReader

}

