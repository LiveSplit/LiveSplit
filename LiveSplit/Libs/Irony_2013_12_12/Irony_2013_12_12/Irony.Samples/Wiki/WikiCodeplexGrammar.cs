using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Irony.Parsing;

namespace Irony.Samples {

  [Language("Wiki-Codeplex", "1.0", "Wiki/Codeplex markup grammar.")]
  public class WikiCodeplexGrammar : Grammar, ICanRunSample {
    public WikiCodeplexGrammar() {
      this.GrammarComments = 
@"A grammar for reading codeplex wiki files and transforming them into HTML 
http://codeplex.codeplex.com/wikipage?title=Wiki%20Formatting
http://wikiplex.codeplex.com
";
      //Terminals
      var text = new WikiTextTerminal("text");
      //Quote-like terminals
      var bold = new WikiTagTerminal("bold", WikiTermType.Format, "*", "b"); 
      var italic = new WikiTagTerminal("italic",WikiTermType.Format, "_", "i"); 
      var underline = new WikiTagTerminal("underline", WikiTermType.Format, "+", "u"); 
      var strike = new WikiTagTerminal("strike", WikiTermType.Format, "~~", "del"); 
      var super = new WikiTagTerminal("super", WikiTermType.Format, "^^", "sup"); 
      var sub = new WikiTagTerminal("sub", WikiTermType.Format, ",,", "sub"); 

      //Headings
      var h1 = new WikiTagTerminal("h1", WikiTermType.Heading, "!", "h1"); 
      var h2 = new WikiTagTerminal("h2", WikiTermType.Heading, "!!", "h2"); 
      var h3 = new WikiTagTerminal("h3", WikiTermType.Heading, "!!!", "h3"); 
      var h4 = new WikiTagTerminal("h4", WikiTermType.Heading, "!!!!", "h4"); 
      var h5 = new WikiTagTerminal("h5", WikiTermType.Heading, "!!!!!", "h5"); 
      var h6 = new WikiTagTerminal("h6", WikiTermType.Heading, "!!!!!!", "h6"); 

      //Ruler
      var ruler = new WikiTagTerminal("ruler", WikiTermType.Heading, "----", "hr");

      //Bulletted lists
      var bl1 = new WikiTagTerminal("bl1", WikiTermType.List, "*", "li") { ContainerHtmlElementName = "ul" }; 
      var bl2 = new WikiTagTerminal("bl2", WikiTermType.List, "**", "li") { ContainerHtmlElementName = "ul" }; 
      var bl3 = new WikiTagTerminal("bl3", WikiTermType.List, "***", "li"){ ContainerHtmlElementName = "ul" }; 

      //Numbered lists
      var nl1 = new WikiTagTerminal("nl1", WikiTermType.List, "#", "li"){ ContainerHtmlElementName = "ol" }; 
      var nl2 = new WikiTagTerminal("nl2", WikiTermType.List, "##", "li"){ ContainerHtmlElementName = "ol" }; 
      var nl3 = new WikiTagTerminal("nl3", WikiTermType.List, "###", "li"){ ContainerHtmlElementName = "ol" }; 

      //Block tags
      var codeBlock = new WikiBlockTerminal("codeBlock", WikiBlockType.CodeBlock, "{{", "}}", "pre"); 
      var escapedBlock = new WikiBlockTerminal("escapedBlock", WikiBlockType.EscapedText, "{\"", "\"}", "pre"); 
      var anchor = new WikiBlockTerminal("anchor", WikiBlockType.Anchor, "{anchor:", "}", string.Empty);

      //Links
      var linkToAnchor = new WikiBlockTerminal("linkToAnchor", WikiBlockType.LinkToAnchor, "[#", "]", string.Empty);
      var url = new WikiBlockTerminal("url", WikiBlockType.Url, "[url:", "]", string.Empty);
      var fileLink = new WikiBlockTerminal("fileLink", WikiBlockType.FileLink, "[file:", "]", string.Empty);
      var image = new WikiBlockTerminal("image", WikiBlockType.Image, "[image:", "]", string.Empty);
      
      //Tables
      var tableHeading = new WikiTagTerminal("tableHeading", WikiTermType.Table, "||", "th");
      var tableRow = new WikiTagTerminal("tableRow", WikiTermType.Table, "|", "td");

      //Alignment, indents
      var leftAlignStart = new WikiTagTerminal("leftAlignStart", WikiTermType.Format, "<{", string.Empty)
                             { OpenHtmlTag = "<div style=\"text-align:left;float:left;\">"};
      var leftAlignEnd = new WikiTagTerminal("leftAlignEnd", WikiTermType.Format, "}<", string.Empty)
                             { OpenHtmlTag = "</div>"};
      var rightAlignStart = new WikiTagTerminal("rightAlignStart", WikiTermType.Format, ">{", string.Empty)
                             { OpenHtmlTag = "<div style=\"text-align:right;float:right;\">"};
      var rightAlignEnd = new WikiTagTerminal("rightAlignEnd", WikiTermType.Format, "}>", string.Empty)
                             { OpenHtmlTag = "</div>"};
      var indentOne = new WikiTagTerminal("indentOne", WikiTermType.Heading, ":", string.Empty)
                             { OpenHtmlTag = "<blockquote>", CloseHtmlTag = "</blockquote>" }; 
      var indentTwo = new WikiTagTerminal("indentTwo", WikiTermType.Heading, "::", string.Empty)
                             { OpenHtmlTag = "<blockquote><blockquote>", CloseHtmlTag = "</blockquote></blockquote>" }; 

      //Non-terminals
      var wikiElement = new NonTerminal("wikiElement");
      var wikiText = new NonTerminal("wikiText"); 

      //Rules
      wikiElement.Rule = text | bold | italic | strike | underline | super | sub 
        | h1 | h2 | h3 | h4 | h5 | h6 | ruler 
        | bl1 | bl2 | bl3 
        | nl1 | nl2 | nl3 
        | codeBlock | escapedBlock | anchor
        | linkToAnchor | url | fileLink | image
        | tableHeading | tableRow 
        | leftAlignStart | leftAlignEnd | rightAlignStart | rightAlignEnd | indentOne | indentTwo
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
      var html = converter.Convert(this, args.ParsedSample.Tokens);
      var path = Path.Combine(Path.GetTempPath(), "@wikiSample.html");
      File.WriteAllText(path, html);
      System.Diagnostics.Process.Start(path);
      return html; 
    }

  }//class


}//namespace
