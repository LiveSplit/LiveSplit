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

  public interface IBindingSource {
    Binding Bind(BindingRequest request);
  }

  public class BindingSourceList : List<IBindingSource> {
  }

  public class BindingSourceTable : Dictionary<string, IBindingSource>, IBindingSource {
    public BindingSourceTable(bool caseSensitive)
      : base(caseSensitive ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase) {
    }
    //IBindingSource Members
    public Binding Bind(BindingRequest request) {
      IBindingSource target;
      if (TryGetValue(request.Symbol, out target))
        return target.Bind(request);
      return null;
    }
  }//class

  // This class will be used to define extensions for BindingSourceTable
  public static partial class BindingSourceTableExtensions {
  }

}
