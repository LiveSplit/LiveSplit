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
  //Expression grammar (4.19), from example 4.34 in Dragon book, p.222
  class GrammarEx434 : Grammar {
    public GrammarEx434() {
      NonTerminal E = new NonTerminal("E");
      NonTerminal T = new NonTerminal("T");
      NonTerminal F = new NonTerminal("F");
      Terminal id = new Terminal("id");

      E.Rule = E + "+" + T | T;
      T.Rule = T + "*" + F | F;
      F.Rule = "(" + E + ")" | id;
      this.Root = E;
    }
/* LR(0) item list for above grammar (identical to fig. 4.35 in dragon book, p.225) 
State I0
    E' -> ·E 
    E -> ·E + T 
    E -> ·T 
    T -> ·T * F 
    T -> ·F 
    F -> ·( E ) 
    F -> ·id 
State I1
    E' -> E ·
    E -> E ·+ T 
State I2
    E -> T ·
    T -> T ·* F 
State I3
    T -> F ·
State I4
    F -> ( ·E ) 
    E -> ·E + T 
    E -> ·T 
    T -> ·T * F 
    T -> ·F 
    F -> ·( E ) 
    F -> ·id 
State I5
    F -> id ·
State I6
    E -> E + ·T 
    T -> ·T * F 
    T -> ·F 
    F -> ·( E ) 
    F -> ·id 
State I7
    T -> T * ·F 
    F -> ·( E ) 
    F -> ·id 
State I8
    E -> E ·+ T 
    F -> ( E ·) 
State I9
    E -> E + T ·
    T -> T ·* F 
State I10
    T -> T * F ·
State I11
    F -> ( E ) ·
      
     */

  }
}//namespace
