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

namespace Irony.Parsing {

  //Note: This in incomplete implementation. 
  // this implementation sets precedence only on operator symbols that are already "shifted" into the parser stack, 
  // ie those on the "left" of precedence comparison. It does not set precedence when operator symbol first appears in parser
  // input. This works OK for unary operator but might break some advanced scenarios.

  public class ImpliedPrecedenceHint : GrammarHint {
    public const int ImpliedPrecedenceCustomFlag = 0x01000000; // a flag to mark a state for setting implied precedence

    //GrammarHint inherits Precedence and Associativity members from BnfTerm; we'll use them to store implied values for this hint

    public ImpliedPrecedenceHint(int precedence, Associativity associativity) {
      Precedence = precedence; 
      Associativity = associativity; 
    }

    public override void Apply(LanguageData language, Construction.LRItem owner) {
      //Check that owner is not final - we can imply precedence only in shift context
      var curr = owner.Core.Current;
      if (curr == null) 
        return;
      //mark the state, to make sure we do stuff in Term_Shifting event handler only in appropriate states
      owner.State.CustomFlags |= ImpliedPrecedenceCustomFlag; 
      curr.Shifting += Term_Shifting;
    }

    void Term_Shifting(object sender, ParsingEventArgs e) {
      //Set the values only if we are in the marked state
      if (!e.Context.CurrentParserState.CustomFlagIsSet(ImpliedPrecedenceCustomFlag))
        return;
      e.Context.CurrentParserInput.Associativity = Associativity;
      e.Context.CurrentParserInput.Precedence = Precedence;
    }
  
  }//class
}
