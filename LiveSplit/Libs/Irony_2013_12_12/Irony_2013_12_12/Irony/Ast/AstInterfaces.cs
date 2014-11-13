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
using System.Collections;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Irony.Ast {
  // Grammar Explorer uses this interface to discover and display the AST tree after parsing the input
  // (Grammar Explorer additionally uses ToString method of the node to get the text representation of the node)
  public interface IBrowsableAstNode {
    int Position { get; }
    IEnumerable GetChildNodes();
  }

  // Note that we expect more than one interpreter/AST implementation.
  // Irony.Interpreter namespace provides just one of them. That's why the following AST interfaces 
  // are here, in top Irony namespace and not in Irony.Interpreter.Ast.
  // In the future, I plan to introduce advanced interpreter, with its own set of AST classes - it will live
  // in a separate assembly Irony.Interpreter2.dll. 

  // Basic interface for AST nodes; Init method is the chance for AST node to get references to its child nodes, and all 
  // related information gathered during parsing
  // Implementing this interface is a minimum required from custom AST node class to enable its creation by Irony AST builder
  // Alternatively, if your custom AST node class does not implement this interface then you can create
  // and initialize node instances using AstNodeCreator delegate attached to corresponding non-terminal in your grammar.
  public interface IAstNodeInit {
    void Init(AstContext context, ParseTreeNode parseNode);
  }



}
