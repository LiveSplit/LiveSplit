using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Irony.Parsing;

namespace Irony.Samples {
  //What does not work yet:
  // * Raw links, CamelCase words should be interpreted as links 
  // * Automatic paragraphs
  // * Linked images
  [Language("Wiki-Creole", "1.0", "Wiki/Creole markup grammar.")]
  public class WikiCreoleGrammar : Grammar, ICanRunSample {

    public WikiCreoleGrammar() {
      this.GrammarComments = 
@"A grammar for parsing Creole wiki files and transforming them into HTML 
http://www.wikicreole.org/";
      //Terminals
      var text = new WikiTextTerminal("text") { EscapeChar = '~' };
      var lineBreak = new WikiTagTerminal("lineBreak", WikiTermType.Element, @"\\", string.Empty) { OpenHtmlTag = "<br/>\n" }; 
      //Quote-like terminals
      var bold = new WikiTagTerminal("bold", WikiTermType.Format, "**", "strong"); 
      var italic = new WikiTagTerminal("italic",WikiTermType.Format, "//", "em"); 

      //Headings
      var h1 = new WikiBlockTerminal("h1", WikiBlockType.EscapedText, "=", "\n", "h1"); 
      var h2 = new WikiBlockTerminal("h2", WikiBlockType.EscapedText, "==", "\n", "h2"); 
      var h3 = new WikiBlockTerminal("h3", WikiBlockType.EscapedText, "===", "\n", "h3"); 
      var h4 = new WikiBlockTerminal("h4", WikiBlockType.EscapedText, "====", "\n", "h4"); 
      var h5 = new WikiBlockTerminal("h5", WikiBlockType.EscapedText, "=====", "\n", "h5"); 
      var h6 = new WikiBlockTerminal("h6", WikiBlockType.EscapedText, "======", "\n", "h6"); 

      //Bulletted lists
      var bl1 = new WikiTagTerminal("bl1", WikiTermType.List, "*", "li") { ContainerHtmlElementName = "ul" }; 
      var bl2 = new WikiTagTerminal("bl2", WikiTermType.List, "**", "li") { ContainerHtmlElementName = "ul" }; 
      var bl3 = new WikiTagTerminal("bl3", WikiTermType.List, "***", "li"){ ContainerHtmlElementName = "ul" }; 

      //Numbered lists
      var nl1 = new WikiTagTerminal("nl1", WikiTermType.List, "#", "li"){ ContainerHtmlElementName = "ol" }; 
      var nl2 = new WikiTagTerminal("nl2", WikiTermType.List, "##", "li"){ ContainerHtmlElementName = "ol" }; 
      var nl3 = new WikiTagTerminal("nl3", WikiTermType.List, "###", "li"){ ContainerHtmlElementName = "ol" }; 

      //Ruler
      var ruler = new WikiTagTerminal("ruler", WikiTermType.Heading, "----", "hr");

      //Image
      var image = new WikiBlockTerminal("image", WikiBlockType.Image, "{{", "}}", string.Empty);

      //Link 
      var link = new WikiBlockTerminal("link", WikiBlockType.Url, "[[", "]]", string.Empty);

      //Tables
      var tableHeading = new WikiTagTerminal("tableHeading", WikiTermType.Table, "|=", "th");
      var tableRow = new WikiTagTerminal("tableRow", WikiTermType.Table, "|", "td");
      
      //Block tags
      //TODO: implement full rules for one-line and multi-line nowiki element
      var nowiki = new WikiBlockTerminal("nowiki", WikiBlockType.EscapedText, "{{{", "}}}", "pre"); 

      //Paragraph - not used in rules but added by postprocessing
      //_paragraph = new WikiTagTerminal("para", WikiTermType.
      //Non-terminals
      var wikiElement = new NonTerminal("wikiElement");
      var wikiText = new NonTerminal("wikiText"); 

      //Rules
      wikiElement.Rule = text | lineBreak | bold | italic 
        | h1 | h2 | h3 | h4 | h5 | h6  
        | bl1 | bl2 | bl3 
        | nl1 | nl2 | nl3 
        | ruler | image | nowiki | link  
        | tableHeading | tableRow 
        | NewLine;
      wikiText.Rule = MakeStarRule(wikiText, wikiElement); 

      this.Root =  wikiText; 
      MarkTransient(wikiElement); 
      //We need to clear punctuation flag on NewLine, so it is not removed from parse tree
      NewLine.SetFlag(TermFlags.IsPunctuation, false); 
      this.LanguageFlags |= LanguageFlags.DisableScannerParserLink | LanguageFlags.NewLineBeforeEOF;
 
    }

    public override void SkipWhitespace(ISourceStream source) {
      return; //no whitespaces at all
    }


    public string RunSample(RunSampleArgs args) {
      var converter = new WikiHtmlConverter();
      PreprocessTokens(args.ParsedSample.Tokens); 
      var html = converter.Convert(this, args.ParsedSample.Tokens);
      var path = Path.Combine(Path.GetTempPath(), "@wikiSample.html");
      File.WriteAllText(path, html);
      System.Diagnostics.Process.Start(path); 
      return html; 
    }

    private void PreprocessTokens(TokenList tokens) {
      var result = new TokenList(); 
      Token prevToken = null;  
      bool prevIsParaBreak = true; 
      bool insidePara = true; 
      foreach(var token in tokens) {
        //fix heading ends: ending '=' chars in Headings should be ignored
        var wikiTerm = token.Terminal as WikiTerminalBase;
        bool isHeading = (wikiTerm != null && wikiTerm.TermType == WikiTermType.Block && wikiTerm.OpenTag.StartsWith("="));
        if (isHeading) {
          token.Value = token.ValueString.TrimEnd(' ');  //first trim trailing spaces
          token.Value = token.ValueString.TrimEnd('=');  //now trim ='s
        }//if
        var termName = token.Terminal.Name; 
        var paraBreak = termName.StartsWith("h") || termName.StartsWith("bl") || termName.StartsWith("nl") 
                      || termName.StartsWith("table"); 
        //token.Terminal.Options = TermOptions.IsDelimiter;
        
        //check for paragraph start
        if (!insidePara && !paraBreak && prevIsParaBreak) 

        prevToken = token; 
        prevIsParaBreak = paraBreak;
      }//for each
      foreach(var token in tokens) {
      
      }

    }//method

  }//class

}//namespace
