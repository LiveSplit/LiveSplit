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

namespace Irony.Interpreter {

  public enum BindingTargetType {
    Slot,
    BuiltInObject,
    SpecialForm,
    ClrInterop,
    Custom, // any special non-standard type for specific language
  }


  public class BindingTargetInfo {
    public readonly string Symbol;
    public readonly BindingTargetType Type;
    public BindingTargetInfo(string symbol, BindingTargetType type) {
      Symbol = symbol; 
      Type = type;
    }

    public override string ToString() {
      return Symbol + "/" + Type.ToString();
    }

  }//class


}
