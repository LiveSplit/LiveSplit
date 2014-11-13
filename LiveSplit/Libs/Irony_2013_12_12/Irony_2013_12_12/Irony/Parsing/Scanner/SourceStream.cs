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

namespace Irony.Parsing {

  public class SourceStream : ISourceStream {
    StringComparison _stringComparison;
    int _tabWidth;
    char[] _chars;
    int _textLength;

    public SourceStream(string text, bool caseSensitive, int tabWidth) : this(text, caseSensitive, tabWidth, new SourceLocation()) {
    }
    
    public SourceStream(string text, bool caseSensitive, int tabWidth, SourceLocation initialLocation) {
      _text = text;
      _textLength = _text.Length; 
      _chars = Text.ToCharArray(); 
      _stringComparison = caseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
      _tabWidth = tabWidth; 
      _location = initialLocation;
      _previewPosition = _location.Position;
      if (_tabWidth <= 1) 
        _tabWidth = 8;
    }

    #region ISourceStream Members
    public string Text {
      get { return _text; } 
    } string _text;

    public int Position {
      get { return _location.Position; }
      set {
        if (_location.Position != value) 
          SetNewPosition(value); 
      }
    }

    public SourceLocation Location {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _location; }
      set { _location = value; }
    } SourceLocation _location;

    public int PreviewPosition {
      get { return _previewPosition; }
      set { _previewPosition = value; } 
    } int _previewPosition;

    public char PreviewChar {
      [System.Diagnostics.DebuggerStepThrough]
      get {
        if (_previewPosition >= _textLength) 
          return '\0';
        return _chars[_previewPosition];
      }
    }

    public char NextPreviewChar {
      [System.Diagnostics.DebuggerStepThrough]
      get {
        if (_previewPosition + 1 >= _textLength) return '\0';
        return _chars[_previewPosition + 1];
      }
    }

    public bool MatchSymbol(string symbol) {
      try {
        int cmp = string.Compare(_text, PreviewPosition, symbol, 0, symbol.Length, _stringComparison);
        return cmp == 0;
      } catch { 
        //exception may be thrown if Position + symbol.length > text.Length; 
        // this happens not often, only at the very end of the file, so we don't check this explicitly
        //but simply catch the exception and return false. Again, try/catch block has no overhead
        // if exception is not thrown. 
        return false;
      }
    }

    public Token CreateToken(Terminal terminal) {
      var tokenText = GetPreviewText();
      return new Token(terminal, this.Location, tokenText, tokenText);
    }
    public Token CreateToken(Terminal terminal, object value) {
      var tokenText = GetPreviewText();
      return new Token(terminal, this.Location, tokenText, value); 
    }

    [System.Diagnostics.DebuggerStepThrough]
    public bool EOF() {
      return _previewPosition >= _textLength;
    }
    #endregion

    //returns substring from Location.Position till (PreviewPosition - 1)
    private string GetPreviewText() {
      var until = _previewPosition;
      if (until > _textLength) until = _textLength;
      var p = _location.Position;
      string text = Text.Substring(p, until - p);
      return text;
    }

    // To make debugging easier: show 20 chars from current position
    public override string ToString() {
      string result;
      try {
        var p = Location.Position;
        if (p + 20 < _textLength)
          result = _text.Substring(p, 20) + Resources.LabelSrcHaveMore;// " ..."
        else
          result = _text.Substring(p) + Resources.LabelEofMark; //"(EOF)"
      } catch (Exception) {
        result = PreviewChar + Resources.LabelSrcHaveMore;
      }
      return string.Format(Resources.MsgSrcPosToString , result, Location); //"[{0}], at {1}"
    }

    //Computes the Location info (line, col) for a new source position.
    private void SetNewPosition(int newPosition) {
      if (newPosition < Position)
        throw new Exception(Resources.ErrCannotMoveBackInSource); 
      int p = Position; 
      int col = Location.Column;
      int line = Location.Line; 
      while(p <  newPosition) {
        if (p >= _textLength)
          break;
        var curr = _chars[p];
        switch (curr) {
          case '\n': line++; col = 0; break;
          case '\r': break; 
          case '\t': col = (col / _tabWidth + 1) * _tabWidth;     break;
          default: col++; break; 
        } //switch
        p++;
      }
      Location = new SourceLocation(p, line, col); 
    }


  }//class

}//namespace
