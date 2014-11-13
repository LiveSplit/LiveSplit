using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Interpreter.Ast; 

namespace Irony.Interpreter {
  public delegate object SpecialForm(ScriptThread thread, AstNode[] childNodes);

  public static class SpecialFormsLibrary {
    public static object Iif(ScriptThread thread, AstNode[] childNodes) {
      var testValue = childNodes[0].Evaluate(thread);
      object result = thread.Runtime.IsTrue(testValue) ? childNodes[1].Evaluate(thread) : childNodes[2].Evaluate(thread);
      return result; 

    }
  }//class
}
