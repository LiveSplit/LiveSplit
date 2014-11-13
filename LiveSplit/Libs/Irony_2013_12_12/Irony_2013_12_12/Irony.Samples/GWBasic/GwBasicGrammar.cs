using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Irony.Samples {
  /// <summary>
  /// This class defines the Grammar for the GwBASIC language.
  /// (Not complete.)
  /// </summary>
  /// <remarks>
  /// http://www.xs4all.nl/~hwiegman/gw-man/index.html
  /// or
  /// http://www.geocities.com/KindlyRat/GWBASIC_Help.zip
  /// </remarks>
  [Language("GwBasic", "1.01", "Sample GW Basic grammar")]
  public class GWBasicGrammar : Grammar  { 

        public GWBasicGrammar() : base(false)  // BASIC is not case sensitive...
        {
          this.GrammarComments = "This grammar uses one new Irony feature - Scanner-Parser link. Parser helps Scanner to disambiguate " +
                             "similar but different token types when more than one terminal matches the current input char.\r\n" +
                             "See comments in GwBasicGrammar.cs file.";
          /*
           Scanner-Parser link. 
           The grammar defines 3 terminals for numbers: number, lineNumber and fileNumber. All three return decimal 
           digits in GetFirsts() method. So when current input is a digit, the scanner has 3 potential candidate terminals
           to match the input. However, because each of the 3 terminals can appear in specific "places" in grammar, 
           the parser is able to assist scanner to pick the correct terminal, depending on the current parser state. 
           The disambiguation happens in Scanner.SelectTerminals method. When the terminal list for current input char
           has more than 1 terminal, the scanner code gets the current parser state from core parser (through compilerContext), 
           and then checks the terminals agains the ExpectedTerms set in the parser state. 
           As you might see in Grammar Explorer, the conflict is resolved successfully. 
           */

            //Terminals
            var lineNumber = new NumberLiteral("LINE_NUMBER", NumberOptions.IntOnly);
            var fileNumber = new NumberLiteral("FILE_NUMBER", NumberOptions.IntOnly);
            
            var number = new NumberLiteral("NUMBER", NumberOptions.AllowStartEndDot);
            //ints that are too long for int32 are converted to int64
            number.DefaultIntTypes = new TypeCode[] {TypeCode.Int32, TypeCode.Int64};
            number.AddExponentSymbols("eE", TypeCode.Single); 
            number.AddExponentSymbols("dD", TypeCode.Double);
            number.AddSuffix("!", TypeCode.Single);
            number.AddSuffix("#", TypeCode.Double); 

            var variable = new IdentifierTerminal("Identifier");
            variable.AddSuffix("$", TypeCode.String);
            variable.AddSuffix("%", TypeCode.Int32); 
            variable.AddSuffix("!", TypeCode.Single);
            variable.AddSuffix("#", TypeCode.Double); 
          
            var stringLiteral = new StringLiteral("STRING", "\"", StringOptions.None);
            //Important: do not add comment term to base.NonGrammarTerminals list - we do use this terminal in grammar rules
            var userFunctionName = variable;
            var comment = new CommentTerminal("Comment", "REM", "\n");
            var short_comment = new CommentTerminal("ShortComment", "'", "\n");
            var line_cont = new LineContinuationTerminal("LineContinuation", "_");
            var comma = ToTerm(",", "comma");
            var colon = ToTerm(":", "colon");

            var comma_opt = new NonTerminal("comma_opt");
            comma_opt.Rule = Empty | ",";
            var semi_opt = new NonTerminal("semi_opt");
            semi_opt.Rule = Empty | ";";
            var pound_opt = new NonTerminal("pound_opt");
            pound_opt.Rule = Empty | "#";

            // Non-terminals
            var PROGRAM = new NonTerminal("PROGRAM");
            var LINE = new NonTerminal("LINE");
            var LINE_CONTENT_OPT = new NonTerminal("LINE_CONTENT_OPT");
            var COMMENT_OPT = new NonTerminal("COMMENT_OPT");
            var STATEMENT_LIST = new NonTerminal("STATEMENT_LIST");
            var STATEMENT = new NonTerminal("STATEMENT");
            var PRINT_STMT = new NonTerminal("PRINT_STMT");
            var PRINT_LIST = new NonTerminal("PRINT_LIST");
            var PRINT_ARG = new NonTerminal("PRINT_ARG");
            var OPEN_STMT = new NonTerminal("OPEN_STMT");
            var OPEN_STMT_MODE = new NonTerminal("OPEN_STMT_MODE");
            var OPEN_STMT_ACCESS = new NonTerminal("OPEN_STMT_ACCESS");
            var CLOSE_STMT = new NonTerminal("CLOSE_STMT");
            var INPUT_STMT = new NonTerminal("INPUT_STMT");
            var VARIABLES = new NonTerminal("VARIABLES");
            var IF_STMT = new NonTerminal("IF_STMT");
            var THEN_CLAUSE = new NonTerminal("THEN_CLAUSE");
            var ELSE_CLAUSE_OPT = new NonTerminal("ELSE_CLAUSE_OPT"); //, typeof(AstNode));
            var EXPR = new NonTerminal("EXPRESSION");
            var EXPR_LIST = new NonTerminal("EXPRESSION_LIST");
            var BINARY_OP = new NonTerminal("BINARY_OP", "operator");
            var BINARY_EXPR = new NonTerminal("BINARY_EXPR");
            var UNARY_EXPR = new NonTerminal("UNARY_EXPR");
            var SIGN = new NonTerminal("SIGN");
            var ASSIGN_STMT = new NonTerminal("ASSIGN_STMT");
            var FOR_STMT = new NonTerminal("FOR_STMT");
            var STEP_OPT = new NonTerminal("STEP_OPT");
            var NEXT_STMT = new NonTerminal("NEXT_STMT");
            var LOCATE_STMT = new NonTerminal("LOCATE_STMT");
            var WHILE_STMT = new NonTerminal("WHILE_STMT");
            var WEND_STMT = new NonTerminal("WEND_STMT");
            var SWAP_STMT = new NonTerminal("SWAP_STMT");
            var FUN_CALL = new NonTerminal("FUN_CALL");
            var VARIABLE_OR_FUNCTION_EXPR = new NonTerminal("VARIABLE_OR_FUNCTION_EXPR");
            var ARG_LIST = new NonTerminal("ARG_LIST");
            var LINE_INPUT_STMT = new NonTerminal("LINE_INPUT_STMT");
            var LINE_INPUT_POUND_STMT = new NonTerminal("LINE_INPUT_POUND_STMT");
            var END_STMT = new NonTerminal("END_STMT");
            var CLS_STMT = new NonTerminal("CLS_STMT");
            var CLEAR_STMT = new NonTerminal("CLEAR_STMT");
            var DIM_STMT = new NonTerminal("DIM_STMT");
            var DEF_FN_STMT = new NonTerminal("DEF_FN_STMT");
            var GOTO_STMT = new NonTerminal("GOTO_STMT");
            var GOSUB_STMT = new NonTerminal("GOSUB_STMT");
            var RETURN_STMT = new NonTerminal("RETURN_STMT");
            var ON_STMT = new NonTerminal("ON_STMT");
            var LINE_NUMBERS = new NonTerminal("LINE_NUMBERS");
            var RANDOMIZE_STMT = new NonTerminal("RANDOMIZE_STMT");

            // set the PROGRAM to be the root node of BASIC programs.
            this.Root = PROGRAM;

            // BNF Rules
            PROGRAM.Rule = MakePlusRule(PROGRAM, LINE);

            // A line can be an empty line, or it's a number followed by a statement list ended by a new-line.
            LINE.Rule = NewLine | lineNumber + LINE_CONTENT_OPT + COMMENT_OPT + NewLine | SyntaxError + NewLine;
            LINE.NodeCaptionTemplate = "Line #{0}";

            // A statement list is 1 or more statements separated by the ':' character
            LINE_CONTENT_OPT.Rule = Empty | IF_STMT | STATEMENT_LIST;
            STATEMENT_LIST.Rule = MakePlusRule(STATEMENT_LIST, colon, STATEMENT);
            COMMENT_OPT.Rule = short_comment | comment | Empty; 

            // A statement can be one of a number of types
            STATEMENT.Rule = ASSIGN_STMT | PRINT_STMT | INPUT_STMT | OPEN_STMT | CLOSE_STMT
                | LINE_INPUT_POUND_STMT | LINE_INPUT_STMT
                | LOCATE_STMT | CLS_STMT
                | END_STMT | CLEAR_STMT | DIM_STMT | DEF_FN_STMT
                | SWAP_STMT | RANDOMIZE_STMT
                | GOSUB_STMT | RETURN_STMT | GOTO_STMT | ON_STMT
                | FOR_STMT | NEXT_STMT | WHILE_STMT | WEND_STMT;

            // The different statements are defined here
            PRINT_STMT.Rule = "print" + PRINT_LIST;
            PRINT_LIST.Rule = MakeStarRule(PRINT_LIST, null, PRINT_ARG);
            PRINT_ARG.Rule = EXPR + semi_opt;
            INPUT_STMT.Rule = "input" + semi_opt + stringLiteral + ";" + VARIABLES;
            OPEN_STMT.Rule = "open" + EXPR + (Empty | "for" + OPEN_STMT_MODE) +
                (Empty | "access" + OPEN_STMT_ACCESS) + "as" + pound_opt + fileNumber;
            OPEN_STMT_ACCESS.Rule = "read" + (Empty | "write") | "write";
            OPEN_STMT_MODE.Rule = ToTerm("o") | "i" | "a" | "output" | "input" | "append";
            CLOSE_STMT.Rule = "close" + pound_opt + fileNumber;
            LINE_INPUT_STMT.Rule = ToTerm("line") + "input" + semi_opt + stringLiteral + ";" + VARIABLE_OR_FUNCTION_EXPR;
            LINE_INPUT_POUND_STMT.Rule = ToTerm("line") + "input" + ToTerm("#") + fileNumber + comma + VARIABLE_OR_FUNCTION_EXPR;
            DIM_STMT.Rule = "dim" + VARIABLES;
            DEF_FN_STMT.Rule = "def" + userFunctionName + (Empty | "(" + ARG_LIST + ")") + "=" + EXPR;
            VARIABLES.Rule = VARIABLE_OR_FUNCTION_EXPR | VARIABLE_OR_FUNCTION_EXPR + "," + VARIABLES;

            IF_STMT.Rule = "if" + EXPR + THEN_CLAUSE + ELSE_CLAUSE_OPT;
            THEN_CLAUSE.Rule = "then" + STATEMENT_LIST | GOTO_STMT;

            //Inject PreferShift hint here to explicitly set shift as preferred action. Suppresses warning message about conflict. 
            ELSE_CLAUSE_OPT.Rule = Empty | PreferShiftHere()  + "else" + STATEMENT_LIST;

            GOTO_STMT.Rule = "goto" + lineNumber;
            GOSUB_STMT.Rule = "gosub" + lineNumber;
            RETURN_STMT.Rule = "return";
            ON_STMT.Rule = "on" + EXPR + (ToTerm("goto") | "gosub") + LINE_NUMBERS;
            LINE_NUMBERS.Rule = MakePlusRule(LINE_NUMBERS, comma, lineNumber);
            ASSIGN_STMT.Rule = VARIABLE_OR_FUNCTION_EXPR + "=" + EXPR;
            LOCATE_STMT.Rule = "locate" + EXPR + comma + EXPR;
            SWAP_STMT.Rule = "swap" + EXPR + comma + EXPR;
            END_STMT.Rule = "end";
            CLS_STMT.Rule = "cls";
            CLEAR_STMT.Rule = ToTerm("clear") + comma + (Empty | number) + (Empty | comma + number) | "clear" + number | "clear";
            RANDOMIZE_STMT.Rule = "randomize" + EXPR;

            // An expression is a number, or a variable, a string, or the result of a binary comparison.
            EXPR.Rule = number | variable | FUN_CALL | stringLiteral | BINARY_EXPR 
                      | "(" + EXPR + ")" | UNARY_EXPR;
            BINARY_EXPR.Rule = EXPR + BINARY_OP + EXPR;
            UNARY_EXPR.Rule = SIGN + EXPR;
            SIGN.Rule = ToTerm("-") | "+";

            //Inject PreferShift hint here to explicitly set shift as preferred action. Suppresses warning message about conflict. 
            //The conflict arises from PRINT statement, when there may be ambigous interpretations for expression like
            //  PRINT F (X) -- either function call or identifier followed by expression
            FUN_CALL.Rule = variable + PreferShiftHere() + "(" + ARG_LIST + ")";
            VARIABLE_OR_FUNCTION_EXPR.Rule = variable | FUN_CALL;

            BINARY_OP.Rule = ToTerm("+") | "^" | "-" | "*" | "/" | "=" | "<=" | ">=" | "<" | ">" | "<>" | "and" | "or";
            //let's do operator precedence right here
            RegisterOperators(60, "^");
            RegisterOperators(50, "*", "/");
            RegisterOperators(40, "+", "-");
            RegisterOperators(30, "=", "<=", ">=", "<", ">", "<>");
            RegisterOperators(20, "and", "or");

            EXPR_LIST.Rule = MakeStarRule(EXPR_LIST, EXPR);

            FOR_STMT.Rule = "for" + ASSIGN_STMT + "to" + EXPR + STEP_OPT;
            STEP_OPT.Rule = Empty | "step" + EXPR;
            NEXT_STMT.Rule = "next" + VARIABLES | "next";
            WHILE_STMT.Rule = "while" + EXPR;
            WEND_STMT.Rule = "wend";

            //TODO: check number of arguments for particular function in node constructor
            ARG_LIST.Rule = MakePlusRule(ARG_LIST, comma, EXPR);


            //Punctuation and Transient elements
            MarkPunctuation("(", ")", ",");
            MarkTransient(EXPR, STATEMENT, LINE_CONTENT_OPT, VARIABLE_OR_FUNCTION_EXPR, COMMENT_OPT);
            NonGrammarTerminals.Add(line_cont);

            this.LanguageFlags = LanguageFlags.NewLineBeforeEOF;

            lineNumber.ValidateToken += lineNumber_ValidateToken;
      }

        void lineNumber_ValidateToken(object sender, ParsingEventArgs e) {
          if (e.Context.CurrentToken.ValueString.Length > 4)
            e.Context.CurrentToken = e.Context.CreateErrorToken("Line number cannot be longer than 4 characters"); 
        }//constructor

    }//class
}//namespace
