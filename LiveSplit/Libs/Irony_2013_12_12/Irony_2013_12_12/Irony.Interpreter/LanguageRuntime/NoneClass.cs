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
  
  // A class for special reserved None value used in many scripting languages. 
  public class NoneClass {
    string _toString;
      
    private NoneClass() {
      _toString = Resources.LabelNone;
    }
    public NoneClass(string toString) {
      _toString = toString;
    }
    public override string ToString() {
      return _toString;
    }

    public static NoneClass Value = new NoneClass(); 
  }



}
