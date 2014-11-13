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
using System.Text;
using System.Globalization;

namespace Irony.Parsing {
  #region notes
  //Identifier terminal. Matches alpha-numeric sequences that usually represent identifiers and keywords.
  // c#: @ prefix signals to not interpret as a keyword; allows \u escapes
  // 

  #endregion

  [Flags]
  public enum IdOptions : short {
    None = 0,
    AllowsEscapes = 0x01,
    CanStartWithEscape = 0x03, 
    
    IsNotKeyword = 0x10,
    NameIncludesPrefix = 0x20,
  }

  public enum CaseRestriction {
    None,
    FirstUpper,
    FirstLower,
    AllUpper,
    AllLower
  }

  public class UnicodeCategoryList : List<UnicodeCategory> { }

  public class IdentifierTerminal : CompoundTerminalBase {

    //Id flags for internal use
    internal enum IdFlagsInternal : short {
      HasEscapes = 0x100,     
    }


    #region constructors and initialization
    public IdentifierTerminal(string name) : this(name, IdOptions.None) {
    }
    public IdentifierTerminal(string name, IdOptions options) : this(name, "_", "_") {
      Options = options; 
    }
    public IdentifierTerminal(string name, string extraChars, string extraFirstChars = ""): base(name) {
      AllFirstChars = Strings.AllLatinLetters + extraFirstChars;
      AllChars = Strings.AllLatinLetters + Strings.DecimalDigits + extraChars;
    }

    public void AddPrefix(string prefix, IdOptions options) {
      base.AddPrefixFlag(prefix, (short)options);
    }
    #endregion

    #region properties: AllChars, AllFirstChars
    CharHashSet _allCharsSet;
    CharHashSet _allFirstCharsSet;

    public string AllFirstChars;
    public string AllChars; 
    public TokenEditorInfo KeywordEditorInfo = new TokenEditorInfo(TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
    public IdOptions Options; //flags for the case when there are no prefixes
    public CaseRestriction CaseRestriction;

    public readonly UnicodeCategoryList StartCharCategories = new UnicodeCategoryList(); //categories of first char
    public readonly UnicodeCategoryList CharCategories = new UnicodeCategoryList();      //categories of all other chars
    public readonly UnicodeCategoryList CharsToRemoveCategories = new UnicodeCategoryList(); //categories of chars to remove from final id, usually formatting category
    #endregion

    #region overrides
    public override void Init(GrammarData grammarData) {
      base.Init(grammarData);
      _allCharsSet = new CharHashSet(Grammar.CaseSensitive);
      _allCharsSet.UnionWith(AllChars.ToCharArray());

      //Adjust case restriction. We adjust only first chars; if first char is ok, we will scan the rest without restriction 
      // and then check casing for entire identifier
      switch(CaseRestriction) {
        case CaseRestriction.AllLower:
        case CaseRestriction.FirstLower:
          _allFirstCharsSet = new CharHashSet(true);
          _allFirstCharsSet.UnionWith(AllFirstChars.ToLowerInvariant().ToCharArray());
          break;
        case CaseRestriction.AllUpper:
        case CaseRestriction.FirstUpper:
          _allFirstCharsSet = new CharHashSet(true);
          _allFirstCharsSet.UnionWith(AllFirstChars.ToUpperInvariant().ToCharArray());
          break;
        default: //None
          _allFirstCharsSet = new CharHashSet(Grammar.CaseSensitive);
          _allFirstCharsSet.UnionWith(AllFirstChars.ToCharArray());
          break; 
      }
      //if there are "first" chars defined by categories, add the terminal to FallbackTerminals
      if (this.StartCharCategories.Count > 0)
        grammarData.NoPrefixTerminals.Add(this);
      if (this.EditorInfo == null) 
        this.EditorInfo = new TokenEditorInfo(TokenType.Identifier, TokenColor.Identifier, TokenTriggers.None);
    }

    public override IList<string> GetFirsts() {
      // new scanner: identifier has no prefixes
      return null; 
/*
      var list = new StringList();
      list.AddRange(Prefixes);
      foreach (char ch in _allFirstCharsSet)
        list.Add(ch.ToString());
      if ((Options & IdOptions.CanStartWithEscape) != 0)
        list.Add(this.EscapeChar.ToString());
      return list;
 */
    }

    protected override void InitDetails(ParsingContext context, CompoundTokenDetails details) {
      base.InitDetails(context, details);
      details.Flags = (short)Options;
    }

    //Override to assign IsKeyword flag to keyword tokens
    protected override Token CreateToken(ParsingContext context, ISourceStream source, CompoundTokenDetails details) {
      Token token = base.CreateToken(context, source, details);
      if (details.IsSet((short)IdOptions.IsNotKeyword))
        return token;
      //check if it is keyword
      CheckReservedWord(token);
      return token; 
    }
    private void CheckReservedWord(Token token) {
      KeyTerm keyTerm;
      if (Grammar.KeyTerms.TryGetValue(token.Text, out keyTerm)) {
        token.KeyTerm = keyTerm;
        //if it is reserved word, then overwrite terminal
        if (keyTerm.Flags.IsSet(TermFlags.IsReservedWord))
          token.SetTerminal(keyTerm); 
      }
    }

    protected override Token QuickParse(ParsingContext context, ISourceStream source) {
      if (!_allFirstCharsSet.Contains(source.PreviewChar)) 
        return null;
      source.PreviewPosition++;
      while (_allCharsSet.Contains(source.PreviewChar) && !source.EOF())
        source.PreviewPosition++;
      //if it is not a terminator then cancel; we need to go through full algorithm
      if (!this.Grammar.IsWhitespaceOrDelimiter(source.PreviewChar)) 
        return null;
      var token = source.CreateToken(this.OutputTerminal);
      if(CaseRestriction != CaseRestriction.None && !CheckCaseRestriction(token.ValueString))
        return null; 
      //!!! Do not convert to common case (all-lower) for case-insensitive grammar. Let identifiers remain as is, 
      //  it is responsibility of interpreter to provide case-insensitive read/write operations for identifiers
      // if (!this.GrammarData.Grammar.CaseSensitive)
      //    token.Value = token.Text.ToLower(CultureInfo.InvariantCulture);
      CheckReservedWord(token);
      return token; 
    }

    protected override bool ReadBody(ISourceStream source, CompoundTokenDetails details) {
      int start = source.PreviewPosition;
      bool allowEscapes = details.IsSet((short)IdOptions.AllowsEscapes);
      CharList outputChars = new CharList();
      while (!source.EOF()) {
        char current = source.PreviewChar;
        if (Grammar.IsWhitespaceOrDelimiter(current)) 
          break;
        if (allowEscapes && current == this.EscapeChar) {
          current = ReadUnicodeEscape(source, details);
          //We  need to back off the position. ReadUnicodeEscape sets the position to symbol right after escape digits.  
          //This is the char that we should process in next iteration, so we must backup one char, to pretend the escaped
          // char is at position of last digit of escape sequence. 
          source.PreviewPosition--; 
          if (details.Error != null) 
            return false;
        }
        //Check if current character is OK
        if (!CharOk(current, source.PreviewPosition == start)) 
          break; 
        //Check if we need to skip this char
        UnicodeCategory currCat = char.GetUnicodeCategory(current); //I know, it suxx, we do it twice, fix it later
        if (!this.CharsToRemoveCategories.Contains(currCat))
          outputChars.Add(current); //add it to output (identifier)
        source.PreviewPosition++;
      }//while
      if (outputChars.Count == 0)
        return false;
      //Convert collected chars to string
      details.Body =  new string(outputChars.ToArray());
      if (!CheckCaseRestriction(details.Body))
        return false; 
      return !string.IsNullOrEmpty(details.Body); 
    }

    private bool CharOk(char ch, bool first) {
      //first check char lists, then categories
      var charSet = first? _allFirstCharsSet : _allCharsSet;
      if(charSet.Contains(ch)) return true;
      //check categories
      if (CharCategories.Count > 0) {
        UnicodeCategory chCat = char.GetUnicodeCategory(ch);
        UnicodeCategoryList catList = first ? StartCharCategories : CharCategories;
        if (catList.Contains(chCat)) return true;
      }
      return false; 
    }

    private bool CheckCaseRestriction(string body) {
      switch(CaseRestriction) {
        case CaseRestriction.FirstLower: return Char.IsLower(body, 0);  
        case CaseRestriction.FirstUpper: return Char.IsUpper(body, 0);  
        case CaseRestriction.AllLower: return body.ToLower() == body; 
        case CaseRestriction.AllUpper: return body.ToUpper() == body;  
        default : return true; 
      }
    }//method
    

    private char ReadUnicodeEscape(ISourceStream source, CompoundTokenDetails details) {
      //Position is currently at "\" symbol
      source.PreviewPosition++; //move to U/u char
      int len;
      switch (source.PreviewChar) {
        case 'u': len = 4; break;
        case 'U': len = 8; break; 
        default:
          details.Error = Resources.ErrInvEscSymbol; // "Invalid escape symbol, expected 'u' or 'U' only."
          return '\0'; 
      }
      if (source.PreviewPosition + len > source.Text.Length) {
        details.Error = Resources.ErrInvEscSeq; // "Invalid escape sequence";
        return '\0';
      }
      source.PreviewPosition++; //move to the first digit
      string digits = source.Text.Substring(source.PreviewPosition, len);
      char result = (char)Convert.ToUInt32(digits, 16);
      source.PreviewPosition += len;
      details.Flags |= (int) IdFlagsInternal.HasEscapes;
      return result;
    }

    protected override bool ConvertValue(CompoundTokenDetails details) {
      if (details.IsSet((short)IdOptions.NameIncludesPrefix))
        details.Value = details.Prefix + details.Body;
      else
        details.Value = details.Body;
      return true; 
    }

    #endregion 

  }//class


} //namespace
