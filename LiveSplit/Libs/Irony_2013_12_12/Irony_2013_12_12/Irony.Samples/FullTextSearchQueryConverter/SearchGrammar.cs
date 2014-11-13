using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using System.Globalization;
//using System.Data.SqlClient;
//using System.Data;

namespace Irony.Samples.FullTextSearch
{
  [Language("SearchGrammar", "1.0", "Google-to-SQL query converter")]
  public class SearchGrammar : Grammar, ICanRunSample  {
        public SearchGrammar() : base(false)   {
          this.GrammarComments = 
            "Google-to-SQL full-text query format converter. Based on original project by Michael Coles.\r\n" +
            "http://www.sqlservercentral.com/articles/Full-Text+Search+(2008)/64248/ \r\n" +
            "Slightly revised to work with latest version of Irony. "; 
          
          // Terminals
          var Term = CreateTerm("Term");
          var Phrase = new StringLiteral("Phrase", "\"");
          var ImpliedAnd = new ImpliedSymbolTerminal("ImpliedAnd"); 

          // NonTerminals
          var BinaryExpression = new NonTerminal("BinaryExpression"); 
          var BinaryOp = new NonTerminal("BinaryOp"); 
          var Expression = new NonTerminal("Expression"); 
          var PrimaryExpression = new NonTerminal("PrimaryExpression");
          var ThesaurusExpression = new NonTerminal("ThesaurusExpression");
          var ThesaurusOperator = new NonTerminal("ThesaurusOperator");
          var ExactExpression = new NonTerminal("ExactExpression");
          var ParenthesizedExpression = new NonTerminal("ParenthesizedExpression");
          var ProximityExpression = new NonTerminal("ProximityExpression");
          var ProximityList = new NonTerminal("ProximityList");

          this.Root = Expression; 
          Expression.Rule = PrimaryExpression | BinaryExpression;
          BinaryExpression.Rule = Expression + BinaryOp + Expression; 
          BinaryOp.Rule = ImpliedAnd | "and" | "&" | "-" | "or" | "|";
          PrimaryExpression.Rule = Term
                                 | ThesaurusExpression
                                 | ExactExpression
                                 | ParenthesizedExpression
                                 | Phrase
                                 | ProximityExpression;
          ThesaurusExpression.Rule = "~" + Term;
          ExactExpression.Rule = "+" + Term | "+" + Phrase;
          ParenthesizedExpression.Rule = "(" + Expression + ")";
          ProximityExpression.Rule = "<" + ProximityList + ">";
          MakePlusRule(ProximityList, Term);

          MarkTransient(PrimaryExpression, Expression, ProximityExpression, ParenthesizedExpression, BinaryOp);
          MarkPunctuation("<", ">", "(", ")");
          RegisterOperators(10, "or", "|"); 
          RegisterOperators(20, "and", "&", "-"); 
          RegisterOperators(20, ImpliedAnd);
          //Register brace pairs to improve error reporting
          RegisterBracePair("(", ")");
          RegisterBracePair("<", ">");
          //Do not report ImpliedAnd as expected symbol - it is not really a symbol
          this.AddToNoReportGroup(ImpliedAnd);
          //also do not report braces as expected
          this.AddToNoReportGroup("(", ")", "<", ">"); 

        }

        //Creates extended identifier terminal that allows international characters
        // Following the pattern used for c# identifier terminal in TerminalFactory.CreateCSharpIdentifier method;
        private IdentifierTerminal CreateTerm(string name) {
          IdentifierTerminal term = new IdentifierTerminal(name,   "!@#$%^*_'.?-", "!@#$%^*_'.?0123456789");
          term.CharCategories.AddRange(new UnicodeCategory[] {
             UnicodeCategory.UppercaseLetter, //Ul
             UnicodeCategory.LowercaseLetter, //Ll
             UnicodeCategory.TitlecaseLetter, //Lt
             UnicodeCategory.ModifierLetter,  //Lm
             UnicodeCategory.OtherLetter,     //Lo
             UnicodeCategory.LetterNumber,     //Nl
             UnicodeCategory.DecimalDigitNumber, //Nd
             UnicodeCategory.ConnectorPunctuation, //Pc
             UnicodeCategory.SpacingCombiningMark, //Mc
             UnicodeCategory.NonSpacingMark,       //Mn
             UnicodeCategory.Format                //Cf
          });
          //StartCharCategories are the same
          term.StartCharCategories.AddRange(term.CharCategories); 
          return term;
        }

        public string RunSample(RunSampleArgs args) {
          var sql = ConvertQuery(args.ParsedSample.Root);
          return sql;
        }


        public enum TermType
        {
            Inflectional = 1,
            Thesaurus = 2,
            Exact = 3
        }
        public static string ConvertQuery(ParseTreeNode node) {
          return ConvertQuery(node, TermType.Inflectional); 
        }

        private static string ConvertQuery(ParseTreeNode node, TermType type) {
          string result = "";
          // Note that some NonTerminals don't actually get into the AST tree, 
          // because of some Irony's optimizations - punctuation stripping and 
          // transient nodes elimination. For example, ParenthesizedExpression - parentheses 
          // symbols get stripped off as punctuation, and child expression node 
          // (parenthesized content) replaces the parent ParenthesizedExpression node
          switch (node.Term.Name) {
            case "BinaryExpression":
              string opSym = string.Empty;
              string op = node.ChildNodes[1].FindTokenAndGetText().ToLower(); 
              string sqlOp = "";
              switch(op) {
                case "":  case "&":  case "and":
                  sqlOp = " AND ";
                  type = TermType.Inflectional;
                  break;
                case "-":
                  sqlOp = " AND NOT ";
                  break;
                case "|":   case "or":
                  sqlOp = " OR ";
                  break;
              }//switch

              result = "(" + ConvertQuery(node.ChildNodes[0], type) + sqlOp +  ConvertQuery(node.ChildNodes[2], type) + ")";
              break;

            case "PrimaryExpression":
              result = "(" + ConvertQuery(node.ChildNodes[0], type) + ")";
              break;

            case "ProximityList":
              string[] tmp = new string[node.ChildNodes.Count];
              type = TermType.Exact;
              for (int i = 0; i < node.ChildNodes.Count; i++) {
                tmp[i] = ConvertQuery(node.ChildNodes[i], type);
              }
              result = "(" + string.Join(" NEAR ", tmp) + ")";
              type = TermType.Inflectional;
              break;

            case "Phrase":
              result = '"' + node.Token.ValueString + '"';
              break;

            case "ThesaurusExpression":
              result = " FORMSOF (THESAURUS, " +
                  node.ChildNodes[1].Token.ValueString + ") ";
              break;

            case "ExactExpression":
              result = " \"" + node.ChildNodes[1].Token.ValueString + "\" ";
              break;

            case "Term":
              switch (type) {
                case TermType.Inflectional:
                  result = node.Token.ValueString;
                  if (result.EndsWith("*"))
                    result = "\"" + result + "\"";
                  else
                    result = " FORMSOF (INFLECTIONAL, " + result + ") ";
                  break;
                case TermType.Exact:
                  result = node.Token.ValueString;

                  break;
              }
              break;

            // This should never happen, even if input string is garbage
            default:
              throw new ApplicationException("Converter failed: unexpected term: " +
                  node.Term.Name + ". Please investigate.");

          }
          return result;
        }
    /*      
            private static string connectionString = "DATA SOURCE=(local);INITIAL CATALOG=AdventureWorks;INTEGRATED SECURITY=SSPI;";

            //commented out, to avoid referencing extra Data/SQL namespaces 
            public static DataTable ExecuteQuery(string ftsQuery) {
              SqlDataAdapter da = null;
              DataTable dt = null;
              try {
                dt = new DataTable();
                da = new SqlDataAdapter
                    (
                        "SELECT ROW_NUMBER() OVER (ORDER BY DocumentId) AS Number, " +
                        "   Title, " +
                        "   DocumentSummary " +
                        "FROM Production.Document " +
                        "WHERE CONTAINS(*, @ftsQuery);",
                        connectionString
                    );
                da.SelectCommand.Parameters.Add("@ftsQuery", SqlDbType.NVarChar, 4000).Value = ftsQuery;
                da.Fill(dt);
                da.Dispose();
              } catch (Exception ex) {
                if (da != null)
                  da.Dispose();
                if (dt != null)
                  dt.Dispose();
                throw (ex);
              }
              return dt;
            }

     */
  }

}
