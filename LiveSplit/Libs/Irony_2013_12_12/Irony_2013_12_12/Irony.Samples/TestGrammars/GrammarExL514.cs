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
using System.Text;
using Irony.Parsing;

namespace Irony.Samples {
  //Sample grammar for lookaheads calculation
  // The grammar is from example 5.14 in Kenneth Louden book "Compiler Construction", p 218
  // Grammar:
  //      A -> (A) | a
  // LALR(1) item list for this grammar is provided in example 5.17 on page 225.
  class GrammarExL514 : Grammar {
    public GrammarExL514() {
      NonTerminal A = new NonTerminal("A");
      Terminal a = new Terminal("a");

      A.Rule = "(" + A + ")" | a; 
      this.Root = A;
    }//method

  }//class

  //Expected state set:

    /*
    State I0
        [A' -> ·A   ,  <EOF> ]
        [A -> ·( A )   ,  <EOF> ]
        [A -> ·a   ,  <EOF> ]
    State I1
        [A' -> A ·  ,  <EOF> ]
    State I2
        [A -> ( ·A )   ,  <EOF>/) ]
        [A -> ·( A )   ,  ) ]
        [A -> ·a   ,  ) ]
    State I3
        [A -> a ·  ,  <EOF>/) ]
    State I4
        [A -> ( A ·)   ,  <EOF>/) ]
    State I5
        [A -> ( A ) ·  ,  <EOF>/) ]
 
   */
  
}//namespace
