using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Parsing {
  //TODO: Improve recovery by adding automatic injection of missing tokens.
  // Make sure we ALWAYS have output parse tree, even if it is messed up
  public class ErrorRecoveryParserAction : ParserAction {

    public override void Execute(ParsingContext context) {
      context.Status = ParserStatus.Error;
      var grammar = context.Language.Grammar;
      grammar.ReportParseError(context);
      // Do not recover if we're already at EOF, or if we're in command line mode
      if (context.CurrentParserInput.Term == grammar.Eof || context.Mode == ParseMode.CommandLine)
        return;
      //Try to recover from error
      context.Status = ParserStatus.Recovering;
      context.AddTrace(Resources.MsgTraceRecovering); // *** RECOVERING - searching for state with error shift *** 
      var recovered = TryRecoverFromError(context);
      if (recovered) {
        context.AddTrace(Resources.MsgTraceRecoverSuccess); //add new trace entry
        context.Status = ParserStatus.Parsing;
      } else {
        context.AddTrace(Resources.MsgTraceRecoverFailed);
        context.Status = ParserStatus.Error;
      }
    }

    protected bool TryRecoverFromError(ParsingContext context) {
      var grammar = context.Language.Grammar;
      var parser = context.Parser; 
      //1. We need to find a state in the stack that has a shift item based on error production (with error token), 
      // and error terminal is current. This state would have a shift action on error token. 
      ParserAction errorShiftAction = FindErrorShiftActionInStack(context);
      if (errorShiftAction == null) return false; //we failed to recover
      context.AddTrace(Resources.MsgTraceRecoverFoundState, context.CurrentParserState); 
      //2. Shift error token - execute shift action
      context.AddTrace(Resources.MsgTraceRecoverShiftError, errorShiftAction);
      errorShiftAction.Execute(context);
      //4. Now we need to go along error production until the end, shifting tokens that CAN be shifted and ignoring others.
      //   We shift until we can reduce
      context.AddTrace(Resources.MsgTraceRecoverShiftTillEnd);
      while (true) {
        if (context.CurrentParserInput == null) 
          parser.ReadInput(); 
        if (context.CurrentParserInput.Term == grammar.Eof)
          return false; 
        //Check if we can reduce
        var nextAction = parser.GetNextAction();
        if (nextAction == null) {
          parser.ReadInput();
          continue; 
        }
        if (nextAction is ReduceParserAction) {
          //We are reducing a fragment containing error - this is the end of recovery
          //Clear all input token queues and buffered input, reset location back to input position token queues; 
          context.SetSourceLocation(context.CurrentParserInput.Span.Location);
       
          //Reduce error production - it creates parent non-terminal that "hides" error inside
          context.AddTrace(Resources.MsgTraceRecoverReducing);
          context.AddTrace(Resources.MsgTraceRecoverAction, nextAction);
          nextAction.Execute(context); //execute reduce
          return true; //we recovered 
        }
        // If it is not reduce, simply execute it (it is most likely shift)
        context.AddTrace(Resources.MsgTraceRecoverAction, nextAction);
        nextAction.Execute(context); //shift input token
      }
    }//method

    private ParserAction FindErrorShiftActionInStack(ParsingContext context) {
      var grammar = context.Language.Grammar; 
      while (context.ParserStack.Count >= 1) {
        ParserAction errorShiftAction;
        if (context.CurrentParserState.Actions.TryGetValue(grammar.SyntaxError, out errorShiftAction) 
             && errorShiftAction is ShiftParserAction)
          return errorShiftAction;
        //pop next state from stack
        if (context.ParserStack.Count == 1)
          return null; //don't pop the initial state
        context.ParserStack.Pop();
        context.CurrentParserState = context.ParserStack.Top.State;
      }
      return null;
    }

  }//class
}//ns
