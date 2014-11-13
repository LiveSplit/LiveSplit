using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;

namespace Irony.Tests.TokenPreviewResolution {

  [Language("Grammar with conflicts, no hints", "1.1", "Grammar with conflicts, no hints.")]
  public class ConflictGrammarNoHints : Grammar {
    public ConflictGrammarNoHints()
      : base(true) {
      var name = new IdentifierTerminal("id");

      var stmt = new NonTerminal("Statement");
      var stmtList = new NonTerminal("StatementList");
      var fieldModifier = new NonTerminal("fieldModifier");
      var propModifier = new NonTerminal("propModifier");
      var methodModifier = new NonTerminal("methodModifier");
      var fieldModifierList = new NonTerminal("fieldModifierList");
      var propModifierList = new NonTerminal("propModifierList");
      var methodModifierList = new NonTerminal("methodModifierList");
      var fieldDef = new NonTerminal("fieldDef");
      var propDef = new NonTerminal("propDef");
      var methodDef = new NonTerminal("methodDef");

      //Rules
      this.Root = stmtList;
      stmtList.Rule = MakePlusRule(stmtList, stmt);
      stmt.Rule = fieldDef | propDef | methodDef;
      fieldDef.Rule = fieldModifierList + name + name + ";";
      propDef.Rule = propModifierList + name + name + "{" + "}";
      methodDef.Rule = methodModifierList + name + name + "(" + ")" + "{" + "}";
      fieldModifierList.Rule = MakeStarRule(fieldModifierList, fieldModifier);
      propModifierList.Rule = MakeStarRule(propModifierList, propModifier);
      methodModifierList.Rule = MakeStarRule(methodModifierList, methodModifier);

      // That's the key of the problem: 3 modifiers have common members
      //   so parser automaton has hard time deciding which modifiers list to produce - 
      //   is it a field, prop or method we are beginning to parse?
      fieldModifier.Rule = ToTerm("public") | "private" | "readonly" | "volatile";
      propModifier.Rule = ToTerm("public") | "private" | "readonly" | "override";
      methodModifier.Rule = ToTerm("public") | "private" | "override";

      MarkReservedWords("public", "private", "readonly", "volatile", "override");
    }
  }

  [Language("Grammar with conflicts #2", "1.1", "Conflict grammar with hints added to productions.")]
  public class ConflictGrammarWithHintsInRules : Grammar {
    public ConflictGrammarWithHintsInRules()  : base(true) {
      var name = new IdentifierTerminal("id");

      var definition = new NonTerminal("definition");
      var fieldDef = new NonTerminal("fieldDef");
      var propDef = new NonTerminal("propDef");
      var fieldModifier = new NonTerminal("fieldModifier");
      var propModifier = new NonTerminal("propModifier");

      definition.Rule = fieldDef | propDef;
      fieldDef.Rule = fieldModifier + name + name + ";";
      propDef.Rule = propModifier + name + name + "{" + "}";
      var fieldHint = ReduceIf(";", comesBefore: "{");
      fieldModifier.Rule = "public" + fieldHint | "private" + fieldHint | "readonly";
      propModifier.Rule = ToTerm("public") | "private" | "override";

      Root = definition;
    }
  }//class

  [Language("Grammar with conflicts #4", "1.1", "Test conflict grammar with conflicts and hints: hints are added to non-terminals.")]
  public class ConflictGrammarWithHintsOnTerms : Grammar {
    public ConflictGrammarWithHintsOnTerms()
      : base(true) {
      var name = new IdentifierTerminal("id");

      var stmt = new NonTerminal("Statement");
      var stmtList = new NonTerminal("StatementList");
      var fieldModifier = new NonTerminal("fieldModifier");
      var propModifier = new NonTerminal("propModifier");
      var methodModifier = new NonTerminal("methodModifier");
      var fieldModifierList = new NonTerminal("fieldModifierList");
      var propModifierList = new NonTerminal("propModifierList");
      var methodModifierList = new NonTerminal("methodModifierList");
      var fieldDef = new NonTerminal("fieldDef");
      var propDef = new NonTerminal("propDef");
      var methodDef = new NonTerminal("methodDef");

      //Rules
      this.Root = stmtList;
      stmtList.Rule = MakePlusRule(stmtList, stmt);
      stmt.Rule = fieldDef | propDef | methodDef;
      fieldDef.Rule = fieldModifierList + name + name + ";";
      propDef.Rule = propModifierList + name + name + "{" + "}";
      methodDef.Rule = methodModifierList + name + name + "(" + ")" + "{" + "}";
      fieldModifierList.Rule = MakeStarRule(fieldModifierList, fieldModifier);
      propModifierList.Rule = MakeStarRule(propModifierList, propModifier);
      methodModifierList.Rule = MakeStarRule(methodModifierList, methodModifier);

      fieldModifier.Rule = ToTerm("public") | "private" | "readonly" | "volatile";
      propModifier.Rule = ToTerm("public") | "private" | "readonly" | "override";
      methodModifier.Rule = ToTerm("public") | "private" | "override";

      // conflict resolution
      var fieldHint = new TokenPreviewHint(PreferredActionType.Reduce, thisSymbol: ";", comesBefore: new string[] { "(", "{" });
      fieldModifier.AddHintToAll(fieldHint);
      fieldModifierList.AddHintToAll(fieldHint);
      var propHint = new TokenPreviewHint(PreferredActionType.Reduce, thisSymbol: "{", comesBefore: new string[] { ";", "(" });
      propModifier.AddHintToAll(propHint);
      propModifierList.AddHintToAll(propHint);

      MarkReservedWords("public", "private", "readonly", "volatile", "override");
    }
  }


}
