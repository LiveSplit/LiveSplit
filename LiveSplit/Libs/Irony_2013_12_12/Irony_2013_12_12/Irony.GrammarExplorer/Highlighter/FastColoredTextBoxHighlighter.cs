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

// Aknowledgments
// This module borrows code and ideas from TinyPG framework by Herre Kuijpers,
// specifically TextMarker.cs and TextHighlighter.cs classes.
// http://www.codeproject.com/KB/recipes/TinyPG.aspx
// Written by Alexey Yakovlev <yallie@yandex.ru>, based on RichTextBoxHighlighter
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using Irony.Parsing;
using System.Diagnostics;
using FastColoredTextBoxNS;

namespace Irony.GrammarExplorer.Highlighter {

  /// <summary>
  /// Highlights text inside FastColoredTextBox control.
  /// </summary>
  public class FastColoredTextBoxHighlighter : NativeWindow, IDisposable, IUIThreadInvoker {
    public FastColoredTextBox TextBox;
    private readonly Dictionary<TokenColor, Style> TokenStyles = new Dictionary<TokenColor, Style>();
    private readonly Style DefaultTokenStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);
    private readonly Style ErrorTokenStyle = new WavyLineStyle(240, Color.Red);
    public readonly EditorAdapter Adapter;
    public readonly EditorViewAdapter ViewAdapter;
    public readonly LanguageData Language;

    private IntPtr _savedEventMask = IntPtr.Zero;
    bool _colorizing;
    bool _disposed;

    #region Constructor, initialization and disposing
    public FastColoredTextBoxHighlighter(FastColoredTextBox textBox, LanguageData language) {
      TextBox = textBox;
      Adapter = new EditorAdapter(language);
      ViewAdapter = new EditorViewAdapter(Adapter, this);
      Language = language;
      InitStyles();
      InitBraces();
      Connect();
      UpdateViewRange();
      ViewAdapter.SetNewText(TextBox.Text);
    }

    private void Connect() {
      TextBox.MouseMove += TextBox_MouseMove;
      TextBox.TextChanged += TextBox_TextChanged;
      TextBox.KeyDown += TextBox_KeyDown;
      TextBox.VisibleRangeChanged += TextBox_ScrollResize;
      TextBox.SizeChanged += TextBox_ScrollResize;
      TextBox.Disposed += TextBox_Disposed;
      ViewAdapter.ColorizeTokens += Adapter_ColorizeTokens;
      this.AssignHandle(TextBox.Handle);
    }

    private void Disconnect() {
      if (TextBox != null) {
        TextBox.MouseMove -= TextBox_MouseMove;
        TextBox.TextChanged -= TextBox_TextChanged;
        TextBox.KeyDown -= TextBox_KeyDown;
        TextBox.Disposed -= TextBox_Disposed;
        TextBox.VisibleRangeChanged -= TextBox_ScrollResize;
        TextBox.SizeChanged -= TextBox_ScrollResize;
      }
      TextBox = null;
    }

    public void Dispose() {
      Adapter.Stop();
      _disposed = true;
      Disconnect();
      this.ReleaseHandle();
      GC.SuppressFinalize(this);
    }

    private void InitStyles() {
      var commentStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
      var keywordStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
      var literalStyle = new TextStyle(Brushes.DarkRed, null, FontStyle.Regular);

      TokenStyles[TokenColor.Comment] = commentStyle;
      TokenStyles[TokenColor.Identifier] = DefaultTokenStyle;
      TokenStyles[TokenColor.Keyword] = keywordStyle;
      TokenStyles[TokenColor.Number] = literalStyle;
      TokenStyles[TokenColor.String] = literalStyle;
      TokenStyles[TokenColor.Text] = DefaultTokenStyle;

      TextBox.ClearStylesBuffer();
      TextBox.AddStyle(DefaultTokenStyle);
      TextBox.AddStyle(ErrorTokenStyle);
      TextBox.AddStyle(commentStyle);
      TextBox.AddStyle(keywordStyle);
      TextBox.AddStyle(literalStyle);
      TextBox.BracketsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Blue)));
      TextBox.BracketsStyle2 = new MarkerStyle(new SolidBrush(Color.FromArgb(70, Color.Green)));
    }

    private void InitBraces() {
      // select the first two pair of braces with the length of exactly one char (FCTB restrictions)
      var braces = Language.Grammar.KeyTerms
        .Select(pair => pair.Value)
        .Where(term => term.Flags.IsSet(TermFlags.IsOpenBrace))
        .Where(term => term.IsPairFor != null && term.IsPairFor is KeyTerm)
        .Where(term => term.Text.Length == 1)
        .Where(term => ((KeyTerm)term.IsPairFor).Text.Length == 1)
        .Take(2);
      if (braces.Any()) {
        // first pair
        var brace = braces.First();
        TextBox.LeftBracket = brace.Text.First();
        TextBox.RightBracket = ((KeyTerm)brace.IsPairFor).Text.First();
        // second pair
        if (braces.Count() > 1) {
          brace = braces.Last();
          TextBox.LeftBracket2 = brace.Text.First();
          TextBox.RightBracket2 = ((KeyTerm)brace.IsPairFor).Text.First();
        }
      }
    }

    #endregion

    #region TextBox event handlers

    void TextBox_MouseMove(object sender, MouseEventArgs e) {
      //TODO: implement showing tip
    }

    void TextBox_KeyDown(object sender, KeyEventArgs e) {
      //TODO: implement showing intellisense hints or drop-downs
    }

    void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
      //if we are here while colorizing, it means the "change" event is a result of our coloring action
      if (_colorizing) return;
      ViewAdapter.SetNewText(TextBox.Text);
    }

    void TextBox_ScrollResize(object sender, EventArgs e) {
      UpdateViewRange();
    }

    void TextBox_Disposed(object sender, EventArgs e) {
      Dispose();
    }

    private void UpdateViewRange() {
      //int minpos = TextBox.GetCharIndexFromPosition(new Point(0, 0));
      //int maxpos = TextBox.GetCharIndexFromPosition(new Point(TextBox.ClientSize.Width, TextBox.ClientSize.Height));
      ViewAdapter.SetViewRange(0, TextBox.Text.Length);
    }
    #endregion

    #region WinAPI
    // some winapís required
    [DllImport("user32", CharSet =  CharSet.Auto)]
    private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool PostMessageA(IntPtr hWnd, int nBar, int wParam, int lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int GetScrollPos(int hWnd, int nBar);

    [DllImport("user32.dll")]
    private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

    private const int WM_SETREDRAW = 0x000B;
    private const int WM_USER = 0x400;
    private const int EM_GETEVENTMASK = (WM_USER + 59);
    private const int EM_SETEVENTMASK = (WM_USER + 69);
    private const int SB_HORZ = 0x0;
    private const int SB_VERT = 0x1;
    private const int WM_HSCROLL = 0x114;
    private const int WM_VSCROLL = 0x115;
    private const int SB_THUMBPOSITION = 4;
    const int WM_PAINT = 0x000F;

    private int HScrollPos {
      get {
        //sometimes explodes with null reference exception
        return GetScrollPos((int)TextBox.Handle, SB_HORZ);
      }
      set {
        SetScrollPos((IntPtr)TextBox.Handle, SB_HORZ, value, true);
        PostMessageA((IntPtr)TextBox.Handle, WM_HSCROLL, SB_THUMBPOSITION + 0x10000 * value, 0);
      }
    }

    private int VScrollPos {
      get {
        return GetScrollPos((int)TextBox.Handle, SB_VERT);
      }
      set {
        SetScrollPos((IntPtr)TextBox.Handle, SB_VERT, value, true);
        PostMessageA((IntPtr)TextBox.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * value, 0);
      }
    }
    #endregion

    #region Colorizing tokens
    public void LockTextBox() {
      // Stop redrawing:
      TextBox.BeginUpdate();
      SendMessage(TextBox.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
      // Stop sending of events:
      _savedEventMask = SendMessage(TextBox.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);
      SendMessage(TextBox.Handle, EM_SETEVENTMASK, 0, IntPtr.Zero);
    }

    public void UnlockTextBox() {
      // turn on events
      SendMessage(TextBox.Handle, EM_SETEVENTMASK, 0, _savedEventMask);
      // turn on redrawing
      SendMessage(TextBox.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
      TextBox.EndUpdate();
    }

    void Adapter_ColorizeTokens(object sender, ColorizeEventArgs args) {
      if (_disposed) return;
      _colorizing = true;
      TextBox.BeginUpdate();
      try {
        foreach (Token tkn in args.Tokens) {
          var tokenRange = TextBox.GetRange(tkn.Location.Position, tkn.Location.Position + tkn.Length);
          var tokenStyle = GetTokenStyle(tkn);
          tokenRange.ClearStyle(StyleIndex.All);
          tokenRange.SetStyle(tokenStyle);
        }
      } finally {
        TextBox.EndUpdate();
        _colorizing = false;
      }
    }

    private Style GetTokenStyle(Token token) {
      if (token.IsError()) return ErrorTokenStyle;
      if (token.EditorInfo == null) return DefaultTokenStyle;
      //Right now we scan source, not parse; initially all keywords are recognized as Identifiers; then they are "backpatched"
      // by parser when it detects that it is in fact keyword from Grammar. So now this backpatching does not happen,
      // so we have to detect keywords here
      var styleIndex = token.EditorInfo.Color;
      if (token.KeyTerm != null && token.KeyTerm.EditorInfo != null && token.KeyTerm.Flags.IsSet(TermFlags.IsKeyword)) {
        styleIndex = token.KeyTerm.EditorInfo.Color;
      }//if
      Style result;
      if (TokenStyles.TryGetValue(styleIndex, out result)) return result;
      return DefaultTokenStyle;
    }

    #endregion

    #region IUIThreadInvoker Members

    public void InvokeOnUIThread(ColorizeMethod colorize) {
      TextBox.BeginInvoke(new MethodInvoker(colorize));
    }

    #endregion

  }//class

}//namespace
