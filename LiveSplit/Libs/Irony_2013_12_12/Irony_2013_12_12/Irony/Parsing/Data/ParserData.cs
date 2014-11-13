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
  // ParserData is a container for all information used by CoreParser in input processing.
  // ParserData is a field in LanguageData structure and is used by CoreParser when parsing intput. 
  // The state graph entry is InitialState state; the state graph encodes information usually contained 
  // in what is known in literature as transiton/goto tables.
  // The graph is built from the language grammar by ParserDataBuilder. 
  using Irony.Parsing.Construction;
  public class ParserData {
    public readonly LanguageData Language;
    public ParserState InitialState; //main initial state
    public ParserStateTable InitialStates = new ParserStateTable(); // Lookup table: AugmRoot => InitialState
    public readonly ParserStateList States = new ParserStateList();
    public ParserAction ErrorAction; 
    public ParserData(LanguageData language) {
      Language = language;
    }
  }

  public partial class ParserState {
    public readonly string Name;
    public readonly ParserActionTable Actions = new ParserActionTable();
    //Defined for states with a single reduce item; Parser.GetAction returns this action if it is not null.
    public ParserAction DefaultAction;
    //Expected terms contains terminals is to be used in 
    //Parser-advise-to-Scanner facility would use it to filter current terminals when Scanner has more than one terminal for current char,
    //   it can ask Parser to filter the list using the ExpectedTerminals in current Parser state. 
    public readonly TerminalSet ExpectedTerminals = new TerminalSet();
    //Used for error reporting, we would use it to include list of expected terms in error message 
    // It is reduced compared to ExpectedTerms - some terms are "merged" into other non-terminals (with non-empty DisplayName)
    //   to make message shorter and cleaner. It is computed on-demand in CoreParser
    public StringSet ReportedExpectedSet;
    internal ParserStateData BuilderData; //transient, used only during automaton construction and may be cleared after that

    //Custom flags available for use by language/parser authors, to "mark" states in some way
    // Irony reserves the highest order byte for internal use
    public int CustomFlags;

    public ParserState(string name) {
      Name = name;
    }
    public void ClearData() {
      BuilderData = null;
    }
    public override string ToString() {
      return Name;
    }
    public override int GetHashCode() {
      return Name.GetHashCode();
    }

    public bool CustomFlagIsSet(int flag) {
      return (CustomFlags & flag) != 0;
    }
  }//class

  public class ParserStateList : List<ParserState> { }
  public class ParserStateSet : HashSet<ParserState> { }
  public class ParserStateHash : Dictionary<string, ParserState> { }
  public class ParserStateTable : Dictionary<NonTerminal, ParserState> { }

  [Flags]
  public enum ProductionFlags {
    None = 0,
    HasTerminals = 0x02, //contains terminal
    IsError = 0x04,      //contains Error terminal
    IsEmpty = 0x08,
  }

  public partial class Production {
    public ProductionFlags Flags;
    public readonly NonTerminal LValue;                              // left-side element
    public readonly BnfTermList RValues = new BnfTermList();         //the right-side elements sequence
    internal readonly Construction.LR0ItemList LR0Items = new Construction.LR0ItemList();        //LR0 items based on this production 

    public Production(NonTerminal lvalue) {
      LValue = lvalue;
    }//constructor

    public string ToStringQuoted() {
      return "'" + ToString() + "'";
    }
    public override string ToString() {
      return ProductionToString(this, -1); //no dot
    }
    public static string ProductionToString(Production production, int dotPosition) {
      char dotChar = '\u00B7'; //dot in the middle of the line
      StringBuilder bld = new StringBuilder();
      bld.Append(production.LValue.Name);
      bld.Append(" -> ");
      for (int i = 0; i < production.RValues.Count; i++) {
        if (i == dotPosition)
          bld.Append(dotChar);
        bld.Append(production.RValues[i].Name);
        bld.Append(" ");
      }//for i
      if (dotPosition == production.RValues.Count)
        bld.Append(dotChar);
      return bld.ToString();
    }

  }//Production class

  public class ProductionList : List<Production> { }


}//namespace
