using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Irony.Ast;
using Irony.Parsing;

namespace Irony.Interpreter.Ast {
  //Extension of AstContext
  public class InterpreterAstContext : AstContext {
    public readonly OperatorHandler OperatorHandler; 

    public InterpreterAstContext(LanguageData language, OperatorHandler operatorHandler = null) : base(language) {
      OperatorHandler = operatorHandler ?? new OperatorHandler(language.Grammar.CaseSensitive);
      base.DefaultIdentifierNodeType = typeof(IdentifierNode);
      base.DefaultLiteralNodeType = typeof(LiteralValueNode);
      base.DefaultNodeType = null; 
    }

  }//class
}//ns
