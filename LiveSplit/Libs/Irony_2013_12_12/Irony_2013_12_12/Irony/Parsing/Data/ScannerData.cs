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

  public class TerminalLookupTable : Dictionary<char, TerminalList> { }

  // ScannerData is a container for all detailed info needed by scanner to read input. 
  public class ScannerData {
    public readonly LanguageData Language;
    public readonly TerminalLookupTable TerminalsLookup = new TerminalLookupTable(); //hash table for fast terminal lookup by input char
    public readonly TerminalList MultilineTerminals = new TerminalList();
    public TerminalList NoPrefixTerminals = new TerminalList(); //Terminals with no limited set of prefixes, copied from GrammarData 
    //hash table for fast lookup of non-grammar terminals by input char
    public readonly TerminalLookupTable NonGrammarTerminalsLookup = new TerminalLookupTable(); 

    public ScannerData(LanguageData language) {
      Language  = language;
    }
  }//class

}//namespace
