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
using System.Globalization;
using System.Text;

namespace Irony.Parsing {

  //Scanner class. The Scanner's function is to transform a stream of characters into aggregates/words or lexemes, 
  // like identifier, number, literal, etc. 

  public class Scanner  {
    #region Properties and Fields: Data, _source
    public readonly ScannerData Data;
    public readonly Parser Parser;
    Grammar _grammar;
    //buffered tokens can come from expanding a multi-token, when Terminal.TryMatch() returns several tokens packed into one token

    private ParsingContext Context {
      get { return Parser.Context; }
    }
    #endregion

    public Scanner(Parser parser) {
      Parser = parser; 
      Data = parser.Language.ScannerData;
      _grammar = parser.Language.Grammar;
      //create token streams
      var tokenStream = GetUnfilteredTokens();
      //chain all token filters
      Context.TokenFilters.Clear();
      _grammar.CreateTokenFilters(Data.Language, Context.TokenFilters);
      foreach (TokenFilter filter in Context.TokenFilters) {
        tokenStream = filter.BeginFiltering(Context, tokenStream);
      }
      Context.FilteredTokens = tokenStream.GetEnumerator();
    }

    internal void Reset() {
    }

    public Token GetToken() {
      //get new token from pipeline
      if (!Context.FilteredTokens.MoveNext()) return null;
      var token = Context.FilteredTokens.Current;
      if (Context.Status == ParserStatus.Previewing)
        Context.PreviewTokens.Push(token);
      else 
        Context.CurrentParseTree.Tokens.Add(token);
      return token;
    }

    //This is iterator method, so it returns immediately when called directly
    // returns unfiltered, "raw" token stream
    private IEnumerable<Token> GetUnfilteredTokens() {
      //We don't do "while(!_source.EOF())... because on EOF() we need to continue and produce EOF token 
      while (true) {  
        Context.PreviousToken = Context.CurrentToken;
        Context.CurrentToken = null; 
        NextToken();
        Context.OnTokenCreated();
        yield return Context.CurrentToken;
        //Don't yield break, continue returning EOF
      }//while
    }// method

    #region Scanning tokens
    private void NextToken() {
      //1. Check if there are buffered tokens
      if(Context.BufferedTokens.Count > 0) {
        Context.CurrentToken = Context.BufferedTokens.Pop();
        return; 
      }
      //2. Skip whitespace.
      _grammar.SkipWhitespace(Context.Source);
      //3. That's the token start, calc location (line and column)
      Context.Source.Position = Context.Source.PreviewPosition;
      //4. Check for EOF
      if (Context.Source.EOF()) {
        Context.CurrentToken = new Token(_grammar.Eof, Context.Source.Location, string.Empty, _grammar.Eof.Name);;
        return; 
      }
      //5. Actually scan the source text and construct a new token
      ScanToken(); 
    }//method

    //Scans the source text and constructs a new token
    private void ScanToken() {
      if (!MatchNonGrammarTerminals() && !MatchRegularTerminals()) {
        //we are in error already; try to match ANY terminal and let the parser report an error
        MatchAllTerminals(); //try to match any terminal out there
      }
      var token = Context.CurrentToken;
      //If we have normal token then return it
      if (token != null && !token.IsError()) {
        var src = Context.Source;
        //set position to point after the result token
        src.PreviewPosition = src.Position + token.Length;
        src.Position = src.PreviewPosition;
        return;
      }
      //we have an error: either error token or no token at all
      if (token == null)   //if no token then create error token
        Context.CurrentToken = Context.CreateErrorToken(Resources.ErrInvalidChar, Context.Source.PreviewChar);
      Recover();
    }

    private bool MatchNonGrammarTerminals() {
      TerminalList terms;
      if(!Data.NonGrammarTerminalsLookup.TryGetValue(Context.Source.PreviewChar, out terms)) 
        return false;
      foreach(var term in terms) {
        Context.Source.PreviewPosition = Context.Source.Location.Position; 
        Context.CurrentToken = term.TryMatch(Context, Context.Source);
        if (Context.CurrentToken != null) 
          term.OnValidateToken(Context);
        if (Context.CurrentToken != null) {
          //check if we need to fire LineStart token before this token; 
          // we do it only if the token is not a comment; comments should be ignored by the outline logic
          var token = Context.CurrentToken;
          if (token.Category == TokenCategory.Content && NeedLineStartToken(token.Location)) {
            Context.BufferedTokens.Push(token); //buffer current token; we'll eject LineStart instead
            Context.Source.Location = token.Location; //set it back to the start of the token
            Context.CurrentToken = Context.Source.CreateToken(_grammar.LineStartTerminal); //generate LineStart
            Context.PreviousLineStart = Context.Source.Location; //update LineStart
          }
          return true;
        }//if
      }//foreach term
      Context.Source.PreviewPosition = Context.Source.Location.Position;
      return false; 
    }

    private bool NeedLineStartToken(SourceLocation forLocation) {
      return _grammar.LanguageFlags.IsSet(LanguageFlags.EmitLineStartToken) && 
          forLocation.Line > Context.PreviousLineStart.Line;
    }

    private bool MatchRegularTerminals() {
      //We need to eject LineStart BEFORE we try to produce a real token; this LineStart token should reach 
      // the parser, make it change the state and with it to change the set of expected tokens. So when we 
      // finally move to scan the real token, the expected terminal set is correct.
        if (NeedLineStartToken(Context.Source.Location)) {
        Context.CurrentToken = Context.Source.CreateToken(_grammar.LineStartTerminal);
        Context.PreviousLineStart = Context.Source.Location;
        return true;
      }
      //Find matching terminal
      // First, try terminals with explicit "first-char" prefixes, selected by current char in source
      ComputeCurrentTerminals();
      //If we have more than one candidate; let grammar method select
      if (Context.CurrentTerminals.Count > 1)
        _grammar.OnScannerSelectTerminal(Context);
 
      MatchTerminals();
      //If we don't have a token from terminals, try Grammar's method
      if (Context.CurrentToken == null)
        Context.CurrentToken = _grammar.TryMatch(Context, Context.Source);
      if (Context.CurrentToken is MultiToken)
        UnpackMultiToken();
      return Context.CurrentToken != null;
    }//method

    // This method is a last attempt by scanner to match ANY terminal, after regular matching (by input char) had failed.
    // Likely this will produce some token which is invalid for current parser state (for ex, identifier where a number 
    // is expected); in this case the parser will report an error as "Error: expected number".
    // if this matching fails, the scanner will produce an error as "unexpected character."
    private bool MatchAllTerminals() {
      Context.CurrentTerminals.Clear(); 
      Context.CurrentTerminals.AddRange(Data.Language.GrammarData.Terminals); 
      MatchTerminals();
      if (Context.CurrentToken is MultiToken)
        UnpackMultiToken();
      return Context.CurrentToken != null;
    }

    //If token is MultiToken then push all its child tokens into _bufferdTokens and return the first token in buffer
    private void UnpackMultiToken() {
      var mtoken = Context.CurrentToken as MultiToken;
      if (mtoken == null) return; 
      for (int i = mtoken.ChildTokens.Count-1; i >= 0; i--)
        Context.BufferedTokens.Push(mtoken.ChildTokens[i]);
      Context.CurrentToken = Context.BufferedTokens.Pop();
    }
    
    private void ComputeCurrentTerminals() {
      Context.CurrentTerminals.Clear(); 
      TerminalList termsForCurrentChar;
      if(!Data.TerminalsLookup.TryGetValue(Context.Source.PreviewChar, out termsForCurrentChar))
        termsForCurrentChar = Data.NoPrefixTerminals; 
      //if we are recovering, previewing or there's no parser state, then return list as is
      if(Context.Status == ParserStatus.Recovering || Context.Status == ParserStatus.Previewing
          || Context.CurrentParserState == null || _grammar.LanguageFlags.IsSet(LanguageFlags.DisableScannerParserLink)
          || Context.Mode == ParseMode.VsLineScan) {
        Context.CurrentTerminals.AddRange(termsForCurrentChar);
        return; 
      }
      // Try filtering terms by checking with parser which terms it expects; 
      var parserState = Context.CurrentParserState;
      foreach(var term in termsForCurrentChar) {
        //Note that we check the OutputTerminal with parser, not the term itself;
        //in most cases it is the same as term, but not always
        if (parserState.ExpectedTerminals.Contains(term.OutputTerminal) || _grammar.NonGrammarTerminals.Contains(term))
          Context.CurrentTerminals.Add(term);
      }

    }//method

    private void MatchTerminals() {
      Token priorToken = null;
      for (int i=0; i<Context.CurrentTerminals.Count; i++) {
        var term = Context.CurrentTerminals[i];
        // If we have priorToken from prior term in the list, check if prior term has higher priority than this term; 
        //  if term.Priority is lower then we don't need to check anymore, higher priority (in prior token) wins
        // Note that terminals in the list are sorted in descending priority order
        if (priorToken  != null && priorToken.Terminal.Priority > term.Priority)
          return;
        //Reset source position and try to match
        Context.Source.PreviewPosition = Context.Source.Location.Position;
        var token = term.TryMatch(Context, Context.Source);
        if (token == null) continue; 
        //skip it if it is shorter than previous token
        if (priorToken != null && !priorToken.IsError() && (token.Length < priorToken.Length))
          continue; 
        Context.CurrentToken = token; //now it becomes current token
        term.OnValidateToken(Context); //validate it
        if (Context.CurrentToken != null) 
          priorToken = Context.CurrentToken;
      }
    }//method

    #endregion

    #region VS Integration methods
    //Use this method for VS integration; VS language package requires scanner that returns tokens one-by-one. 
    // Start and End positions required by this scanner may be derived from Token : 
    //   start=token.Location.Position; end=start + token.Length;
    public Token VsReadToken(ref int state) {
      Context.VsLineScanState.Value = state;
      if (Context.Source.EOF()) return null;
      if (state == 0)
        NextToken();
      else {
        Terminal term = Data.MultilineTerminals[Context.VsLineScanState.TerminalIndex - 1];
        Context.CurrentToken = term.TryMatch(Context, Context.Source); 
      }
      //set state value from context
      state = Context.VsLineScanState.Value;
      if (Context.CurrentToken != null && Context.CurrentToken.Terminal == _grammar.Eof)
        return null; 
      return Context.CurrentToken;
    }
    public void VsSetSource(string text, int offset) {
      var line = Context.Source==null ? 0 : Context.Source.Location.Line;
      var newLoc = new SourceLocation(offset, line + 1, 0);
      Context.Source = new SourceStream(text, Context.Language.Grammar.CaseSensitive, Context.TabWidth, newLoc); 
    }
    #endregion

    #region Error recovery
    //Simply skip until whitespace or delimiter character
    private bool Recover() {
      var src = Context.Source; 
      src.PreviewPosition++;
      while (!Context.Source.EOF()) {
        if(_grammar.IsWhitespaceOrDelimiter(src.PreviewChar)) {
          src.Position = src.PreviewPosition;
          return true;
        }
        src.PreviewPosition++;
      }
      return false; 
    }
    #endregion 

    #region TokenPreview
    //Preview mode allows custom code in grammar to help parser decide on appropriate action in case of conflict
    // Preview process is simply searching for particular tokens in "preview set", and finding out which of the 
    // tokens will come first.
    // In preview mode, tokens returned by FetchToken are collected in _previewTokens list; after finishing preview
    //  the scanner "rolls back" to original position - either by directly restoring the position, or moving the preview
    //  tokens into _bufferedTokens list, so that they will read again by parser in normal mode.
    // See c# grammar sample for an example of using preview methods
    SourceLocation _previewStartLocation;

    //Switches Scanner into preview mode
    public void BeginPreview() {
      Context.Status = ParserStatus.Previewing;
      _previewStartLocation = Context.Source.Location;
      Context.PreviewTokens.Clear();
    }

    //Ends preview mode
    public void EndPreview(bool keepPreviewTokens) {
      if (keepPreviewTokens) {
        //insert previewed tokens into buffered list, so we don't recreate them again
        while (Context.PreviewTokens.Count > 0)
          Context.BufferedTokens.Push(Context.PreviewTokens.Pop()); 
      }  else
        Context.SetSourceLocation(_previewStartLocation);
      Context.PreviewTokens.Clear();
      Context.Status = ParserStatus.Parsing;
    }
    #endregion




  }//class

}//namespace
