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

  public class PrecedenceBasedParserAction : ConditionalParserAction {
    ShiftParserAction _shiftAction;
    ReduceParserAction _reduceAction; 

    public PrecedenceBasedParserAction(BnfTerm shiftTerm, ParserState newShiftState, Production reduceProduction)  {
      _reduceAction = new ReduceParserAction(reduceProduction);
      var reduceEntry = new ConditionalEntry(CheckMustReduce, _reduceAction, "(Precedence comparison)");
      base.ConditionalEntries.Add(reduceEntry);
      base.DefaultAction = _shiftAction = new ShiftParserAction(shiftTerm, newShiftState);
    }

    private bool CheckMustReduce(ParsingContext context) {
      var input = context.CurrentParserInput;
      var stackCount = context.ParserStack.Count;
      var prodLength = _reduceAction.Production.RValues.Count;
      for (int i = 1; i <= prodLength; i++) {
        var prevNode = context.ParserStack[stackCount - i];
        if (prevNode == null) continue;
        if (prevNode.Precedence == BnfTerm.NoPrecedence) continue;
        //if previous operator has the same precedence then use associativity
        if (prevNode.Precedence == input.Precedence)
          return (input.Associativity == Associativity.Left); //if true then Reduce
        else
          return (prevNode.Precedence > input.Precedence); //if true then Reduce
      }
      //If no operators found on the stack, do shift
      return false;
    }

    public override string ToString() {
      return string.Format(Resources.LabelActionOp, _shiftAction.NewState.Name, _reduceAction.Production.ToStringQuoted());
    }

  }//class


}//namespace
