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

  //These two delegates define custom methods that Grammar can implement to execute custom action
  public delegate void PreviewActionMethod(CustomParserAction action);
  public delegate void ExecuteActionMethod(ParsingContext context, CustomParserAction action);

  public class CustomActionHint: GrammarHint {
    private ExecuteActionMethod _executeMethod;
    private PreviewActionMethod _previewMethod;

    public CustomActionHint(ExecuteActionMethod executeMethod, PreviewActionMethod previewMethod = null) {
      _executeMethod = executeMethod;
      _previewMethod = previewMethod;
    }

    public override void Apply(LanguageData language, Construction.LRItem owner) {
      //Create custom action and put it into state.Actions table
      var state = owner.State;
      var action = new CustomParserAction(language, state, _executeMethod);
      if (_previewMethod != null)
        _previewMethod(action);
      if (!state.BuilderData.IsInadequate) // adequate state, with a single possible action which is DefaultAction
        state.DefaultAction = action;
      else if (owner.Core.Current != null) //shift action
        state.Actions[owner.Core.Current] = action;
      else foreach (var lkh in owner.Lookaheads)
          state.Actions[lkh] = action;
      //We consider all conflicts handled by the action
      state.BuilderData.Conflicts.Clear(); 
    }//method

  }//Hint class


  // CustomParserAction is in fact action selector: it allows custom Grammar code to select the action to execute from a set of 
  // shift/reduce actions available in this state.
  public class CustomParserAction : ParserAction {
    public LanguageData Language;
    public ParserState State;
    public ExecuteActionMethod ExecuteRef;
    public TerminalSet Conflicts = new TerminalSet();
    public IList<ShiftParserAction> ShiftActions = new List<ShiftParserAction>();
    public IList<ReduceParserAction> ReduceActions = new List<ReduceParserAction>();
    public object CustomData;

    public CustomParserAction(LanguageData language, ParserState state,
                                            ExecuteActionMethod executeRef) {
      Language = language;
      State = state;
      ExecuteRef = executeRef;
      Conflicts.UnionWith(state.BuilderData.Conflicts);
      // Create default shift and reduce actions
      foreach (var shiftItem in state.BuilderData.ShiftItems)
        ShiftActions.Add(new ShiftParserAction(shiftItem));
      foreach (var item in state.BuilderData.ReduceItems)
        ReduceActions.Add(ReduceParserAction.Create(item.Core.Production));
    }

    public override void Execute(ParsingContext context) {
      if (context.TracingEnabled)
        context.AddTrace(Resources.MsgTraceExecCustomAction);
      //States with DefaultAction do NOT read input, so we read it here
      if (context.CurrentParserInput == null)
        context.Parser.ReadInput();
      // Remember old state and input; if they don't change after custom action - it is error, we may fall into an endless loop
      var oldState = context.CurrentParserState;
      var oldInput = context.CurrentParserInput;
      ExecuteRef(context, this);
      //Prevent from falling into an infinite loop 
      if (context.CurrentParserState == oldState && context.CurrentParserInput == oldInput) {
        context.AddParserError(Resources.MsgErrorCustomActionDidNotAdvance);
        context.Parser.RecoverFromError(); 
      }
    }//method

    public override string ToString() {
      return "CustomParserAction";
    }
  }//class

}//ns
