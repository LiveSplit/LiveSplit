using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Interpreter.Ast;

namespace Irony.Interpreter {
  public static class InterpreterEnumExtensions {

    public static bool IsSet(this BindingRequestFlags enumValue, BindingRequestFlags flag) {
      return (enumValue & flag) != 0;
    }
    public static bool IsSet(this AstNodeFlags enumValue, AstNodeFlags flag) {
      return (enumValue & flag) != 0;
    }

  }


}
