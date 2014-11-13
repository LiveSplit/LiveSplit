using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Irony.Samples {

  public class WikiHtmlConverter {
    private enum TableStatus {
      None, 
      Table, 
      Cell
    }
    internal class FlagTable : Dictionary<Terminal, bool>{}
    internal class WikiTermStack : Stack<WikiTerminalBase> { }

    StringBuilder _output;
    FlagTable _flags = new FlagTable();  
    WikiTermStack _openLists = new WikiTermStack(); 
    bool _atLineStart = true; 
    WikiTerminalBase _currentHeader = null; 
    //Table flags
    bool _insideTable = false; 
    bool _insideCell = false; 
    WikiTagTerminal _lastTableTag; 

    private int CurrentListLevel {
      get { return _openLists.Count == 0 ? 0 : _openLists.Peek().OpenTag.Length; }
    }


    //HtmlEncode method - we don't use System.Web.HttpUtility.HtmlEncode method, because System.Web assembly is not part of 
    // .NET Client profile; so we just embed implementation here
    // This is reformatted version of Rick Strahl's original code: http://www.west-wind.com/Weblog/posts/617930.aspx
    public static string HtmlEncode(string text)  {
        if (text == null)  return null;
        StringBuilder sb = new StringBuilder(text.Length);
        int len = text.Length;
        for (int i = 0; i < len; i++)
        {
            switch (text[i]) {
                case '<':   sb.Append("&lt;"); break;
                case '>':   sb.Append("&gt;"); break;
                case '"':   sb.Append("&quot;");    break;
                case '&':   sb.Append("&amp;");  break;
                default:
                    if (text[i] > 159)    {
                        // decimal numeric entity
                        sb.Append("&#");
                        sb.Append(((int)text[i]).ToString());
                        sb.Append(";");
                    } else
                        sb.Append(text[i]);
                    break;
            }
        }
        return sb.ToString();
    }

    public string Convert(Grammar grammar, TokenList tokens) {
      _output = new StringBuilder(8192); //8k
      _output.AppendLine("<html>"); 
      
      foreach(var token in tokens) {
        var term = token.Terminal;
        if(_atLineStart || term == grammar.Eof) {
          CheckOpeningClosingLists(token);
          CheckTableStatus(token); 
          if (term == grammar.Eof) break; 
        }
        if (term is WikiTerminalBase) 
          ProcessWikiToken(token); 
        else if(term == grammar.NewLine) {
          ProcessNewLine(token);
        } else //non-wike element and not new line 
          _output.Append(HtmlEncode(token.ValueString)); 
        _atLineStart = term == grammar.NewLine; //set for the next token
      }//foreach token

      _output.AppendLine(); 
      _output.AppendLine("</html>"); 
      return _output.ToString(); 
    }//method

    private void ProcessNewLine(Token token) {
      if (_insideTable & !_insideCell) return; //ignore it in one special case - to make output look nicer
      if (_currentHeader != null)
        _output.AppendLine(_currentHeader.CloseHtmlTag);
      else 
        _output.AppendLine("<br/>"); 
      _currentHeader = null; 
    }//method

    private void ProcessWikiToken(Token token) {
      //we check that token actually contains some chars - to allow "invisible spaces" after last table tag
      if(_lastTableTag != null && !_insideCell && token.ValueString.Trim().Length > 0) {
        _output.Append(_lastTableTag.OpenHtmlTag);
        _insideCell = true; 
      }
      var wikiTerm = token.Terminal as WikiTerminalBase; 
      switch(wikiTerm.TermType) {
        case WikiTermType.Element: 
          _output.Append(wikiTerm.OpenHtmlTag); 
          _output.Append(wikiTerm.CloseHtmlTag);
          break; 
        case WikiTermType.Format: 
          ProcessFormatTag(token); 
          break; 
        case WikiTermType.Heading: 
        case WikiTermType.List:
          _output.Append(wikiTerm.OpenHtmlTag);
          _currentHeader = wikiTerm;    
          break; 
        case WikiTermType.Block:
          ProcessWikiBlockTag(token); 
          break; 
        case WikiTermType.Text:
          _output.Append(HtmlEncode(token.ValueString));     
          break; 
        case WikiTermType.Table:
          if (_insideCell)
            _output.Append(_lastTableTag.CloseHtmlTag); //write out </td> or </th>
          //We do not write opening tag immediately: we need to know if it is the last table tag on the line.
          // if yes, we don't write it at all; _lastTableTag will be cleared when we start new line
          _lastTableTag = wikiTerm as WikiTagTerminal;
          _insideCell = false; 
          break; 
      }
    }

    private void ProcessFormatTag(Token token) {
      var term = token.Terminal as WikiTerminalBase; 
      bool value;
      bool isOn = _flags.TryGetValue(term, out value) && value;
      if (isOn)
        _output.Append(term.CloseHtmlTag);
      else 
        _output.Append(term.OpenHtmlTag); 
      _flags[term] = !isOn;    
    }

    private void ProcessWikiBlockTag(Token token) {
      var term = token.Terminal as WikiBlockTerminal;
      string template;
      string[] segments; 

      switch(term.BlockType) {
        case WikiBlockType.EscapedText:
        case WikiBlockType.CodeBlock:
          _output.Append(term.OpenHtmlTag);
          _output.Append(HtmlEncode(token.ValueString));
          _output.AppendLine(term.CloseHtmlTag);
          break;
        case WikiBlockType.Anchor:
          _output.Append("<a name=\"" + token.ValueString +"\"/>");
          break;
        case WikiBlockType.LinkToAnchor:
          _output.Append("<a href=\"#" + token.ValueString +"\">" + HtmlEncode(token.ValueString) + "</a>");
          break; 
        case WikiBlockType.Url:
        case WikiBlockType.FileLink:
          template = "<a href=\"{0}\">{1}</a>";
          segments = token.ValueString.Split('|');
          if(segments.Length > 1)
            _output.Append(string.Format(template, segments[1], segments[0]));
          else
            _output.Append(string.Format(template, segments[0], segments[0]));
          break; 
        case WikiBlockType.Image:
          segments = token.ValueString.Split('|');
          switch(segments.Length){
            case 1:
              template = "<img src=\"{0}\"/>";
              _output.Append(string.Format(template, segments[0]));
              break; 
            case 2:
              template = "<img src=\"{1}\" alt=\"{0}\" title=\"{0}\" />";
              _output.Append(string.Format(template, segments[0], segments[1]));
              break; 
            case 3:
              template = "<a href=\"{2}\"><img src=\"{1}\" alt=\"{0}\" title=\"{0}\" /></a>";
              _output.Append(string.Format(template, segments[0], segments[1], segments[2]));
              break;
          }//switch segments.Length
          break; 
      }//switch    
    }//method

    #region comments
    /* Note: we allow mix of bulleted/numbered lists, so we can have bulleted list inside numbered item:
     
      # item 1
      ** bullet 1
      ** bullet 2
      # item 2
     
     This is a bit different from codeplex rules - the bulletted list resets the numeration of items, so that "item 2" would 
     appear with number 1, not 2. While our handling seems more flexible, you can easily change the following method to 
     follow codeplex rules.  */
    #endregion 
    //Called at the start of each line (after NewLine)
    private void CheckOpeningClosingLists(Token token) {
      var nextLevel = 0;
      var wikiTerm = token.Terminal as WikiTerminalBase;
      if(wikiTerm != null && wikiTerm.TermType == WikiTermType.List)
        nextLevel = wikiTerm.OpenTag.Length;
      //for codeplex-style additionally check that the control char is the same (# vs *). If not, unwind the levels
      if (CurrentListLevel == nextLevel) return; //it is at the same level; 
      //New list begins
      if(nextLevel > CurrentListLevel) {
        _output.Append(wikiTerm.ContainerOpenHtmlTag);
        _openLists.Push(wikiTerm);
        return; 
      } 
      //One or more lists end
      while(nextLevel < CurrentListLevel) {
        var oldTerm =_openLists.Pop();
        _output.Append( oldTerm.ContainerCloseHtmlTag);
      }//while
    }//method

    //Called at the start of each line
    private void CheckTableStatus(Token token) {
      var wikiTerm = token.Terminal as WikiTerminalBase;
      bool isTableTag = wikiTerm != null && wikiTerm.TermType == WikiTermType.Table;
      if ( !_insideTable && !isTableTag) return;
      _insideCell = false; //if we are at line start, drop this flag
      _lastTableTag = null; 
      //New table begins
      if(!_insideTable && isTableTag) {
        _output.AppendLine("<table border=1>"); 
        _output.Append("<tr>");
        _insideTable = true; 
        return; 
      }
      //existing table continues
      if(_insideTable && isTableTag) {
        _output.AppendLine("</tr>");
        _output.Append("<tr>"); 
        return;
      }
      //existing table ends
      if(_insideTable && !isTableTag) {
        _output.AppendLine("</tr>"); 
        _output.AppendLine("</table>");
        _insideTable = false; 
        return;
      }
    }//method

  }//class

}//namespace
