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

// Original implementation is contributed by Alexey Yakovlev (yallie)

namespace Irony.Parsing {
  using ConditionalEntry = ConditionalParserAction.ConditionalEntry; 
 
  public class TokenPreviewHint : GrammarHint {
    public int MaxPreviewTokens = 1000;
    private PreferredActionType _actionType; 
    private string _firstString;
    private StringSet _beforeStrings = new StringSet();
    private Terminal _firstTerminal;
    private TerminalSet _beforeTerminals = new TerminalSet();
    private string _description; 
    
    public TokenPreviewHint(PreferredActionType actionType, string thisSymbol, params string[] comesBefore) {
      _actionType = actionType; 
      _firstString = thisSymbol;
      _beforeStrings.AddRange(comesBefore);
    }
    public TokenPreviewHint(PreferredActionType actionType, Terminal thisTerm, params Terminal[] comesBefore) {
      _actionType = actionType;
      _firstTerminal = thisTerm;
      _beforeTerminals.UnionWith(comesBefore);
    }


    public override void Init(GrammarData grammarData) {
      base.Init(grammarData);
      // convert strings to terminals, if needed
      _firstTerminal = _firstTerminal ?? Grammar.ToTerm(_firstString);
      if (_beforeStrings.Count > 0) {
        //SL pukes here, it does not support co/contravariance in full, we have to do it long way
        foreach (var s in _beforeStrings)
          _beforeTerminals.Add(Grammar.ToTerm(s));
      }
      //Build description
      var beforeTerms = string.Join(" ", _beforeTerminals.Select(t => t.Name));
      _description = string.Format("{0} if {1} comes before {2}.", _actionType, _firstTerminal.Name, beforeTerms); 
    }

    public override string ToString() {
      if (_description == null)
        _description = _actionType.ToString() + " if ..."; 
      return _description; 
    }

    public override void Apply(LanguageData language, Construction.LRItem owner) {
      var state = owner.State;
      if (!state.BuilderData.IsInadequate) return; //the state is adequate, we don't need to do anything
      var conflicts = state.BuilderData.Conflicts;
      // Note that we remove lookaheads from the state conflicts set at the end of this method - to let parser builder know
      // that this conflict is taken care of. 
      // On the other hand we may call this method multiple times for different LRItems if we have multiple hints in the same state. 
      // Since we remove lookahead from conflicts on the first call, on the consequitive calls it will not be a conflict -
      // but we still need to add a new conditional entry to a conditional parser action for this lookahead. 
      // Thus we process the lookahead anyway, even if it is not a conflict. 
      // if (conflicts.Count == 0) return; -- this is a wrong thing to do
      switch (_actionType) {
        case PreferredActionType.Reduce:
          if (!owner.Core.IsFinal) return; 
          //it is reduce action; find lookaheads in conflict
          var lkhs = owner.Lookaheads;
          if (lkhs.Count == 0) return; //if no conflicts then nothing to do
          var reduceAction = new ReduceParserAction(owner.Core.Production);
          var reduceCondEntry = new ConditionalEntry(CheckCondition, reduceAction, _description);
          foreach (var lkh in lkhs) {
            AddConditionalEntry(state, lkh, reduceCondEntry);
            if (conflicts.Contains(lkh))
              conflicts.Remove(lkh);
          }
          break; 
        case PreferredActionType.Shift:
          var curr = owner.Core.Current as Terminal;
          if (curr == null) return; //it is either reduce item, or curr is a NonTerminal - we cannot shift it
          var shiftAction = new ShiftParserAction(owner);
          var shiftCondEntry = new ConditionalEntry(CheckCondition, shiftAction, _description);
          AddConditionalEntry(state, curr, shiftCondEntry);
          if (conflicts.Contains(curr))
            conflicts.Remove(curr);
          break; 
      }

      
    }//method

    private bool CheckCondition(ParsingContext context) {
      var scanner = context.Parser.Scanner; 
      try {
        var eof = Grammar.Eof;
        var count = 0;
        scanner.BeginPreview();
        var token = scanner.GetToken();
        while (token != null && token.Terminal != eof) {
          if (token.Terminal == _firstTerminal)
            return true; //found!
          if (_beforeTerminals.Contains(token.Terminal))
            return false;
          if (++count > MaxPreviewTokens && MaxPreviewTokens > 0)
            return false;
          token = scanner.GetToken();
        }
        return false;
      } finally {
        scanner.EndPreview(true);
      }
    }

    //Check if there is an action already in state for this term; if yes, and it is Conditional action, 
    // then simply add an extra conditional entry to it. If an action does not exist, or it is not conditional, 
    // create new conditional action for this term.
    private void AddConditionalEntry(ParserState state, BnfTerm term, ConditionalEntry entry) {
      ParserAction oldAction;
      ConditionalParserAction condAction = null; 
      if (state.Actions.TryGetValue(term, out oldAction))
        condAction = oldAction as ConditionalParserAction;
      if (condAction == null) { //there's no old action, or it is not conditional; create new conditional action
        condAction = new ConditionalParserAction();
        condAction.DefaultAction = oldAction;
        state.Actions[term] = condAction;
      }
      condAction.ConditionalEntries.Add(entry);
      if (condAction.DefaultAction == null)
        condAction.DefaultAction = FindDefaultAction(state, term);
      if (condAction.DefaultAction == null) //if still no action, then use the cond. action as default.
        condAction.DefaultAction = entry.Action; 
    }

    //Find an LR item without hints compatible with term (either shift on term or reduce with term as lookahead); 
    // this item without hints would become our default. We assume that other items have hints, and when conditions
    // on all these hints fail, we chose this remaining item without hints.
    private ParserAction FindDefaultAction(ParserState state, BnfTerm term) {
      //First check reduce items
      var reduceItems = state.BuilderData.ReduceItems.SelectByLookahead(term as Terminal);
      foreach (var item in reduceItems)
        if (item.Core.Hints.Count == 0)
            return ReduceParserAction.Create(item.Core.Production);
      var shiftItem = state.BuilderData.ShiftItems.SelectByCurrent(term).FirstOrDefault();
      if (shiftItem != null)
        return new ShiftParserAction(shiftItem); 
      //if everything failed, returned first reduce item
      return null; 
    }

  }//class  
  
}
