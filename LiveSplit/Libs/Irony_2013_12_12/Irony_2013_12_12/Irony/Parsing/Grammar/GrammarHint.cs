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

using Irony.Parsing.Construction;

namespace Irony.Parsing {

  public class GrammarHintList : List<GrammarHint> { }

  //Hints are additional instructions for parser added inside BNF expressions.
  // Hint refers to specific position inside the expression (production), so hints are associated with LR0Item object 
  // One example is a PreferredActionHint produced by the Grammar.PreferShiftHere() method. It tells parser to perform
  // shift in case of a shift/reduce conflict. It is in fact the default action of LALR parser, so the hint simply suppresses the error 
  // message about the shift/reduce conflict in the grammar.
  public abstract class GrammarHint : BnfTerm {
    public GrammarHint() : base("hint") { }

    /// <summary> Gives a chance to a custom code in hint to interfere in parser automaton construction.</summary>
    /// <param name="language">The LanguageData instance.</param>
    /// <param name="owner">The LRItem that "owns" the hint. </param>
    /// <remarks>
    /// The most common purpose of this method (it's overrides) is to resolve the conflicts
    /// by adding specific actions into State.Actions dictionary. 
    /// The owner parameter represents the position in the grammar expression where the hint
    /// is found. The parser state is available through owner.State property. 
    /// </remarks>
    public virtual void Apply(LanguageData language, LRItem owner) {
      // owner.State  -- the parser state
      // owner.State.BuilderData.Conflicts -- as set of conflict terminals
      // owner.State.Actions -- a dictionary of actions in the current state.
    }
  } //class


}
