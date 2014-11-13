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
using System.Reflection;
using System.Text;

namespace Irony.Interpreter {

  // Module export, container for public, exported symbols from module
  // Just a skeleton, to be completed
  public class ModuleExport: IBindingSource {
    public ModuleInfo Module;
    public ModuleExport(ModuleInfo module)  {
      Module = module; 
    }

    public Binding Bind(BindingRequest request) {
      return null;
    }
  }



}
