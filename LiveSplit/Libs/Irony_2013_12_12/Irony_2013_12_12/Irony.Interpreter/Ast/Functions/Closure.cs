using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Interpreter.Ast {
  public class Closure : ICallTarget {
    //The scope that created closure; is used to find Parents (enclosing scopes) 
    public Scope ParentScope; 
    public LambdaNode Lamda;
    public Closure(Scope parentScope, LambdaNode targetNode) {
      ParentScope = parentScope;
      Lamda = targetNode;
    }

    public object Call(ScriptThread thread, object[] parameters) {
      return Lamda.Call(ParentScope, thread, parameters);
    }

    public override string ToString() {
      return Lamda.ToString(); //returns nice string like "<function add>"
    }

  } //class
}
