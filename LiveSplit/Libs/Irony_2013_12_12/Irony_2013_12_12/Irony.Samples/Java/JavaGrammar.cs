using Irony.Parsing;

namespace Irony.Samples.Java
{
  [Language("Java", "1.6", "An (almost) complete java parser")]
  public partial class JavaGrammar : Grammar
  {
    private readonly TerminalSet mSkipTokensInPreview = new TerminalSet(); //used in token preview for conflict resolution

    public JavaGrammar()
    {
      GrammarComments = "NOTE: This grammar does not parse hex floating point literals.";

      var singleLineComment = new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
      var delimitedComment = new CommentTerminal("DelimitedComment", "/*", "*/");
      NonGrammarTerminals.Add(singleLineComment);
      NonGrammarTerminals.Add(delimitedComment);
      
      MarkPunctuation(";", ",", "(", ")", "{", "}", "[", "]", ":", "@");

      InitializeSyntax();
    }

    public override void OnScannerSelectTerminal(ParsingContext context)
    {
      if (context.Source.PreviewChar == '>' && context.Status == ParserStatus.Previewing)
      {
        context.CurrentTerminals.Clear();
        context.CurrentTerminals.Add(ToTerm(">", "gt")); //select the ">" terminal
      }
      base.OnScannerSelectTerminal(context);
    }

    private void ResolveConflicts(ParsingContext context, CustomParserAction action) { 
    }
    /* BROKEN
    public override void OnResolvingConflict(ConflictResolutionArgs args)
    {
      switch (args.Context.CurrentParserInput.Term.Name)
      {
        case "[":
          {
            args.Scanner.BeginPreview();
            var preview = args.Scanner.GetToken();
            string previewSym = preview.Terminal.Name;
            args.Result = previewSym == "]" ? PreferredActionType.Reduce : PreferredActionType.Shift;
            args.Scanner.EndPreview(true);
            return;
          }
        case "dot":
          {
            args.Scanner.BeginPreview();
            var preview = args.Scanner.GetToken();
            string previewSym = preview.Text;
            if (previewSym == "<")
            {
              // skip over any type arguments
              int depth = 0;
              do
              {
                if (previewSym == "<")
                {
                  ++depth;
                }
                else if (previewSym == ">")
                {
                  --depth;
                }
                preview = args.Scanner.GetToken();
                previewSym = preview.Text;
              } while (depth > 0 && preview.Terminal != Eof);
            }

            switch (previewSym)
            {
              case "new":
              case "super":
              case "this":
                args.Result = PreferredActionType.Reduce;
                break;
              default:
                args.Result = PreferredActionType.Shift;
                break;
            }
            args.Scanner.EndPreview(true);
            return;
          }
        case "lt":
          {
            args.Scanner.BeginPreview();
            int ltCount = 0;
            string previewSym;
            while (true)
            {
              //Find first token ahead (using preview mode) that is either end of generic parameter (">") or something else
              Token preview;
              do
              {
                preview = args.Scanner.GetToken();
              } while (mSkipTokensInPreview.Contains(preview.Terminal) && preview.Terminal != Eof);
              //See what did we find
              previewSym = preview.Terminal.Name;
              if ((previewSym == "<") || (previewSym == "lt"))
              {
                ltCount++;
              }
              else if (((previewSym == ">") || (previewSym == "gt")) && ltCount > 0)
              {
                ltCount--;
                continue;
              }
              else
                break;
            }
            //if we see ">", then it is type argument, not operator
            if ((previewSym == ">") || (previewSym == "gt"))
            {
              args.Result = PreferredActionType.Shift;
            }
            else
            {
              args.Result = PreferredActionType.Reduce;
            }
            args.Scanner.EndPreview(true);
            //keep previewed tokens; important to keep ">>" matched to two ">" symbols, not one combined symbol (see method below)
            return;
          }
      }
    } */
  }
}
