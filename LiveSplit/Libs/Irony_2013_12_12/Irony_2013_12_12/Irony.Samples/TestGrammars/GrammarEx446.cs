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
  // The grammar is from example 4.46 in Dragon book, p 241
  // LALR(1) items set for this grammar is provided in fig. 4.45 on page 245.
  // Note that the table in the book contains kernel-only items for each state,
  //  while we print out all items
  class GrammarEx446  : Grammar {
    public GrammarEx446() {
      // A' is augmented root
      NonTerminal S = new NonTerminal("S");
      NonTerminal L = new NonTerminal("L");
      NonTerminal R = new NonTerminal("R");
      Terminal id = new IdentifierTerminal("id");

      S.Rule = L + "=" + R | R;
      L.Rule = "*" + R | id;
      R.Rule = L;
      this.Root = S;
    }//method
//Expected state set:
    /*
State I0
    [S' -> ·S   ,  <EOF> ]
    [S -> ·L = R   ,  <EOF> ]
    [S -> ·R   ,  <EOF> ]
    [L -> ·* R   ,  = ]
    [L -> ·id   ,  = ]
    [R -> ·L   ,  <EOF> ]
State I1
    [S' -> S ·  ,  <EOF> ]
State I2
    [S -> L ·= R   ,  <EOF> ]
    [R -> L ·  ,  <EOF> ]
State I3
    [S -> R ·  ,  <EOF> ]
State I4
    [L -> * ·R   ,  =/<EOF> ]
    [R -> ·L   ,  =/<EOF> ]
    [L -> ·* R   ,  =/<EOF> ]
    [L -> ·id   ,  =/<EOF> ]
State I5
    [L -> id ·  ,  =/<EOF> ]
State I6
    [S -> L = ·R   ,  <EOF> ]
    [R -> ·L   ,  <EOF> ]
    [L -> ·* R   ,  <EOF> ]
    [L -> ·id   ,  <EOF> ]
State I7
    [L -> * R ·  ,  =/<EOF> ]
State I8
    [R -> L ·  ,  =/<EOF> ]
State I9
    [S -> L = R ·  ,  <EOF> ]

     
 */
  }
}//namespace
