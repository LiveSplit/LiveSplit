using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Irony.Parsing; 

namespace Irony.Samples {
  [Language("Clarion", "0.1", "Clarion grammar.")]
  public class ClarionGrammar : Grammar {
    //Special words - those that may not be used as proc names
    public readonly HashSet<string> SpecialWords;
    CommentTerminal _comment; 

    public ClarionGrammar() : base(false) { //case insensitive
      base.GrammarComments = "Clarion grammar for parsing Clarion sources.";

      //Terminals
      var comment = new CommentTerminal("comment", "!", "\n");
      this.NonGrammarTerminals.Add(comment);
      _comment = comment; 
      var identifier = new IdentifierTerminal("identifier", ":", string.Empty);
      var label = new IdentifierTerminal("label", ":", string.Empty);
      label.ValidateToken += label_ValidateToken;
      label.Priority = TerminalPriority.High;
      //Clarion uses suffixes for identifying number base (bin, octal, hex) - unlike Irony's NumberLiteral which supports suffixes.
      // We treat all numbers as hexadecimal with optional prefixes that don't mean anything. We "fix" the value in ValidateToken handler
      var number = new NumberLiteral("number");
      number.Options = NumberOptions.Hex | NumberOptions.DisableQuickParse;
      number.AddSuffix("b", TypeCode.Int64);
      number.AddSuffix("o", TypeCode.Int64);
      number.AddSuffix("h", TypeCode.Int64);
      number.ValidateToken += number_ValidateToken;
     
      var string_lit = new StringLiteral("string_lit", "'", StringOptions.AllowsDoubledQuote | StringOptions.NoEscapes);

      var comma = ToTerm(",");
      var not = ToTerm("NOT");
      
      //Non-terminals
      var source_file = new NonTerminal("source_file");
      var main_file = new NonTerminal("main_file");
      //whitespace - NewLine, Empty line, line with compiler directive, or line with end-tag of Compile(..) directive
      var wspace = new NonTerminal("wspace");
      var ws_lines = new NonTerminal("ws_lines");
      var ws_line = new NonTerminal("ws_line");
      var compiler_dir = new NonTerminal("compiler_dir");
      var compiler_dir_line = new NonTerminal("compiler_dir_line");
      var cdir_compile_end_tag = new CustomTerminal("compile_end_tag", Match_compile_end_tag);

      var cdir_include = new NonTerminal("cdir_include");
      var cdir_equate = new NonTerminal("cdir_equate");
      var cdir_compile_omit = new NonTerminal("cdir_compile_omit");
      var compile_or_omit = new NonTerminal("compile_or_omit");
      var cdir_section = new NonTerminal("cdir_section");
      var once_opt = new NonTerminal("once_opt");
      var const_value = new NonTerminal("const_value");
      var comma_expr_opt = new NonTerminal("comma_expr_opt");
      var comma_string_lit_opt = new NonTerminal("comma_string_lit_opt");


      var member_file = new NonTerminal("member_file");
      var file_hearder = new NonTerminal("file_header");

      var map = new NonTerminal("map");
      var map_opt = new NonTerminal("map_opt");
      var map_elem = new NonTerminal("map_elem");
      var map_elem_list = new NonTerminal("map_elem_list");
      
      var proc_decl_list = new NonTerminal("proc_decl_list");
      var proc_decl = new NonTerminal("proc_decl");
      var proc_impl_list = new NonTerminal("proc_impl_list");
      var proc_impl = new NonTerminal("proc_impl");
      var data_decl_list = new NonTerminal("data_decl_list");
      var data_decl_list_opt = new NonTerminal("data_decl_list_opt");
      var data_decl = new NonTerminal("data_decl");
      var stmt_list = new NonTerminal("stmt_list");
      var stmt_line = new NonTerminal("stmt_line");
      var stmt = new NonTerminal("stmt");
      var assign_stmt = new NonTerminal("assign_stmt");
      var expr = new NonTerminal("expr");
      var expr_opt = new NonTerminal("expr_opt");
      var bin_expr = new NonTerminal("bin_expr");
      var bin_op = new NonTerminal("bin_op");
      var fun_call = new NonTerminal("fun_call");
      var par_expr = new NonTerminal("par_expr");
      var expr_list = new NonTerminal("expr_list");
      var term = new NonTerminal("term");

      var module_decl = new NonTerminal("module_decl");
      var module_header = new NonTerminal("module_ref");
      var data_type = new NonTerminal("data_type");
      var param_init_opt = new NonTerminal("param_init_opt");
      var type_args_opt = new NonTerminal("type_args_opt");
      var type_arg_list = new NonTerminal("type_arg_list");
      var type_arg = new NonTerminal("type_arg"); 

      var label_opt = new NonTerminal("label_opt");
      var param_list = new NonTerminal("param_list"); 
      var param = new NonTerminal("param");
      var param_list_par_opt = new NonTerminal("param_list_par_opt");
      var attr_list_tail_opt = new NonTerminal("attr_list_tail_opt");
      var attr_list = new NonTerminal("attr_list");
      var attr_def = new NonTerminal("attr_def");
      var return_line_opt = new NonTerminal("return_line_opt");
      var end_line = new NonTerminal("end_line");
      
      //Rules 
      base.Root = source_file;
      source_file.Rule = main_file | member_file;

      //whitespace - includes compiler directives
      wspace.Rule = NewLine + ws_lines;// +ReduceHere();
      ws_lines.Rule =  MakeStarRule(ws_lines, ws_line);
      ws_line.Rule = compiler_dir | PreferShiftHere() + NewLine + ReduceHere();
      //compiler_dir_line.Rule = compiler_dir;
      compiler_dir.Rule = cdir_include | cdir_equate | cdir_compile_omit | cdir_section | cdir_compile_end_tag;
      cdir_include.Rule = ToTerm("INCLUDE") + "(" + string_lit + comma_string_lit_opt + ")" + once_opt;
      comma_string_lit_opt.Rule = Empty | comma + string_lit;
      once_opt.Rule = comma + "ONCE" | Empty;
      cdir_equate.Rule = //ShiftIf("EQUATE").ComesBefore(NewLine)  + 
        label + "EQUATE" + "(" + const_value + ")";
      const_value.Rule = number | string_lit;
      cdir_compile_omit.Rule = compile_or_omit + "(" + string_lit + comma_expr_opt + ")";
      comma_expr_opt.Rule = Empty | comma + expr;
      cdir_section.Rule = ToTerm("SECTION") + "(" + string_lit + ")";
      compile_or_omit.Rule = ToTerm("COMPILE") | "OMIT";
      
      //File structure
      main_file.Rule = "PROGRAM" + wspace + map + data_decl_list_opt + wspace +
                     "CODE" + wspace + stmt_list + return_line_opt + proc_impl_list;
      member_file.Rule = "MEMBER" + wspace + map_opt + data_decl_list + proc_impl_list;

      //map
      map_opt.Rule = map | Empty;
      map.Rule = "MAP" + wspace + map_elem_list + end_line;
      map_elem_list.Rule = MakeStarRule(map_elem_list, wspace, map_elem);
      map_elem.Rule = proc_decl | module_header;
      module_decl.Rule = module_header + proc_decl_list + end_line;
      module_header.Rule = ToTerm("MODULE") + "(" + string_lit + ")" + wspace; 

      proc_decl_list.Rule = MakePlusRule(proc_decl_list, proc_decl);
      proc_decl.Rule = label + "PROCEDURE" + param_list_par_opt + attr_list_tail_opt + wspace;
      param_list_par_opt.Rule = Empty | "(" + param_list + ")";
      param_list.Rule = MakePlusRule(param_list, comma, param);
      param.Rule = data_type + identifier + param_init_opt;
      param_init_opt.Rule = Empty | "=" + number;
      data_type.Rule = identifier + type_args_opt; 
      type_args_opt.Rule = Empty | "(" + type_arg_list + ")";
      type_arg_list.Rule = MakePlusRule(type_arg_list, comma, type_arg);
      type_arg.Rule = number | identifier; 
      
      attr_list_tail_opt.Rule = Empty | comma + attr_list;
      attr_list.Rule = MakePlusRule(attr_list, comma, attr_def); 
      attr_def.Rule = identifier + param_list_par_opt;

      data_decl_list.Rule = MakePlusRule(data_decl_list, data_decl);
      data_decl_list_opt.Rule = data_decl_list | Empty; 
      data_decl.Rule = identifier + data_type + wspace;

      proc_impl_list.Rule = MakeStarRule(proc_impl_list, proc_impl);
      proc_impl.Rule = proc_decl + data_decl_list + "CODE" + wspace + stmt_list + return_line_opt + wspace;
      stmt_list.Rule = MakeStarRule(stmt_list, stmt_line);
      stmt_line.Rule = stmt + wspace;
      stmt.Rule = assign_stmt | fun_call;
      assign_stmt.Rule = identifier + "=" + expr;
      expr.Rule = fun_call | par_expr | bin_expr | term;
      par_expr.Rule = "(" + expr + ")";
      bin_expr.Rule = expr + bin_op + expr;
      bin_op.Rule = ToTerm("+") | "-" | "*" | "/" | "^" | "%"
                  | "&"
                  | "=" | "<" | ">" | "~=" | "~<" | "~>" | not + "=" | not + "<" | not + ">"
                  | "<>" | "<=" | "=<" | ">=" | "=>"
                  | "~" | not | "AND" | "OR" | "XOR";
      fun_call.Rule = identifier + "(" + expr_list + ")";
      expr_list.Rule = MakeStarRule(expr_list, comma, expr);
      expr_opt.Rule = expr | Empty;
      term.Rule = identifier | number | string_lit;

      return_line_opt.Rule = "RETURN" + expr_opt | Empty;
      end_line.Rule = "END" + NewLine | "." + NewLine;

      //operator precedence 
      RegisterOperators(10,  "OR", "XOR");
      RegisterOperators(20, "AND");

      RegisterOperators(50, "=", "<", ">", "~=", "~<", "~>", "<>", "<=", "=<", ">=", "=>"); 
      RegisterOperators(100, "+", "-");
      RegisterOperators(110, "*", "/", "%", "&");
      RegisterOperators(120, Associativity.Right, "^");
      RegisterOperators(130, not);
      RegisterOperators(130, "~");

      //punctuation, brace pairs, transient nodes
      MarkPunctuation("(", ")", ",");
      RegisterBracePair("(", ")");
      
      //Reserved words and special words
      var resWordList = "ACCEPT AND BEGIN BREAK BY CASE CHOOSE COMPILE CYCLE DO ELSE ELSIF END EXECUTE EXIT FUNCTION GOTO IF INCLUDE LOOP" +
                        " MEMBER NEW NOT NULL OF OMIT OR OROF PARENT PROCEDURE PROGRAM RETURN ROUTINE SECTION SELF THEN TIMES TO UNTIL WHILE XOR";
      this.MarkReservedWords(resWordList.Split(' '));
      var specialWordsList = "APPLICATION CLASS CODE DATA DETAIL FILE FOOTER FORM GROUP HEADER ITEM ITEMIZE JOIN MAP MENU MENUBAR" + 
                         " MODULE OLECONTROL OPTION QUEUE RECORD REPORT ROW SHEET TAB TABLE TOOLBAR VIEW WINDOW";
      //Initialize special words list (words that cannot be used as proc names); we'll later use them for verifying proc names
      SpecialWords = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
      SpecialWords.UnionWith(specialWordsList.Split(' ')); 
    }


    #region Parsing event handlers

    Token Match_compile_end_tag(Terminal terminal, ParsingContext context, ISourceStream source) {
      if (source.MatchSymbol("__End")) {
        var p = source.Location.Position; 
        var lineEnd = source.Text.IndexOf("\n", p); 
        var text = source.Text.Substring(p, lineEnd - p + 1);
        return new Token(_comment, source.Location, text, text);
      }
      //otherwise return null
      return null; 
    }

    //All numbers are treated as HEX initially; here we need to analyze suffix (if any) and convert the value
    void number_ValidateToken(object sender, ValidateTokenEventArgs e) {
      
    }

    void label_ValidateToken(object sender, ValidateTokenEventArgs e) {
      //check that token starts at position 0
      if (e.Token.Location.Column > 0) //Column is 0-based
        e.RejectToken(); 
    }//constructor
    #endregion

  }//class
}
