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

  //GrammarData is a container for all basic info about the grammar
  // GrammarData is a field in LanguageData object. 
  public class GrammarData {
    public readonly LanguageData Language; 
    public readonly Grammar Grammar;
    public NonTerminal AugmentedRoot;
    public NonTerminalSet AugmentedSnippetRoots = new NonTerminalSet(); 
    public readonly BnfTermSet AllTerms = new BnfTermSet();
    public readonly TerminalSet Terminals = new TerminalSet();
    public readonly NonTerminalSet NonTerminals = new NonTerminalSet();
    public TerminalSet NoPrefixTerminals = new TerminalSet(); //Terminals that have no limited set of prefixes

    public GrammarData(LanguageData language) {
      Language = language;
      Grammar = language.Grammar;
    }

  }//class



}//namespace
