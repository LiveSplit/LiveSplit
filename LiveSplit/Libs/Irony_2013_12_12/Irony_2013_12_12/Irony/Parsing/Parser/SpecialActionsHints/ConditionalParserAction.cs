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

  public enum PreferredActionType {
    Shift,
    Reduce,
  }

  public class ConditionalParserAction : ParserAction {

    #region embedded types
    public delegate bool ConditionChecker(ParsingContext context);

    public class ConditionalEntry {
      public ConditionChecker Condition;
      public ParserAction Action;
      public string Description; //for tracing
      public ConditionalEntry(ConditionChecker condition, ParserAction action, string description) {
        Condition = condition;
        Action = action;
        Description = description; 
      }
      public override string ToString() {
        return Description + "; action: " + Action.ToString(); 
      }
    }

    public class ConditionalEntryList : List<ConditionalEntry> { }
    #endregion

    public ConditionalEntryList ConditionalEntries = new ConditionalEntryList();
    public ParserAction DefaultAction;

    public override void Execute(ParsingContext context) {
      var traceEnabled = context.TracingEnabled;
      if (traceEnabled)  context.AddTrace("Conditional Parser Action.");
      for (int i = 0; i < ConditionalEntries.Count; i++) {
        var ce = ConditionalEntries[i];
        if (traceEnabled)  context.AddTrace("  Checking condition: " + ce.Description);
        if (ce.Condition(context)) {
          if (traceEnabled) context.AddTrace("  Condition is TRUE, executing action: " + ce.Action.ToString());
          ce.Action.Execute(context);
          return; 
        }
      }
      //if no conditions matched, execute default action
      if (DefaultAction == null) {
        context.AddParserError("Fatal parser error: no conditions matched in conditional parser action, and default action is null." +
            " State: {0}", context.CurrentParserState.Name);
        context.Parser.RecoverFromError();
        return; 
      }
      if (traceEnabled) context.AddTrace("  All conditions failed, executing default action: " + DefaultAction.ToString());
      DefaultAction.Execute(context);
    }//method

  }//class
}
