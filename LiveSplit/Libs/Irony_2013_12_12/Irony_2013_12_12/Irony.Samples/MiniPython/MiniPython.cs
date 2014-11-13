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
using Irony.Parsing;
using Irony.Interpreter;
using Irony.Interpreter.Ast;

namespace Irony.Samples.MiniPython {
  // The grammar for a very small subset of Python. This is work in progress, 
  // I will be adding more features as we go along, bringing it closer to real python.
  // Current version: expressions, assignments, indented code blocks, function defs, function calls
  // Full support for Python line joining rules: line continuation symbol "\", automatic line joining when 
  //  line ends in the middle of expression, with unbalanced parenthesis
  // Python is important test case for Irony as an indentation-sensitive language.

  [Language("MiniPython", "0.2", "Micro-subset of Python, work in progress")]
  public class MiniPythonGrammar : InterpretedLanguageGrammar {
    public MiniPythonGrammar() : base(caseSensitive: true) {

      // 1. Terminals
      var number = TerminalFactory.CreatePythonNumber("number");
      var identifier = TerminalFactory.CreatePythonIdentifier("identifier");
      var comment = new CommentTerminal("comment", "#", "\n", "\r");
      //comment must to be added to NonGrammarTerminals list; it is not used directly in grammar rules,
      // so we add it to this list to let Scanner know that it is also a valid terminal. 
      base.NonGrammarTerminals.Add(comment);
      var comma = ToTerm(",");
      var colon = ToTerm(":");

      // 2. Non-terminals
      var Expr = new NonTerminal("Expr");
      var Term = new NonTerminal("Term");
      var BinExpr = new NonTerminal("BinExpr", typeof(BinaryOperationNode));
      var ParExpr = new NonTerminal("ParExpr");
      var UnExpr = new NonTerminal("UnExpr", typeof(UnaryOperationNode));
      var UnOp = new NonTerminal("UnOp", "operator");
      var BinOp = new NonTerminal("BinOp", "operator");
      var AssignmentStmt = new NonTerminal("AssignmentStmt", typeof(AssignmentNode));
      var Stmt = new NonTerminal("Stmt");
      var ExtStmt = new NonTerminal("ExtStmt");
      //Just as a test for NotSupportedNode
      var ReturnStmt = new NonTerminal("return", typeof(NotSupportedNode));
      var Block = new NonTerminal("Block");
      var StmtList = new NonTerminal("StmtList", typeof(StatementListNode));

      var ParamList = new NonTerminal("ParamList", typeof(ParamListNode));
      var ArgList = new NonTerminal("ArgList", typeof(ExpressionListNode));
      var FunctionDef = new NonTerminal("FunctionDef", typeof(FunctionDefNode));
      var FunctionCall = new NonTerminal("FunctionCall", typeof(FunctionCallNode));


      // 3. BNF rules
      Expr.Rule = Term | UnExpr | BinExpr;
      Term.Rule = number | ParExpr | identifier | FunctionCall;
      ParExpr.Rule = "(" + Expr + ")";
      UnExpr.Rule = UnOp + Term;
      UnOp.Rule = ToTerm("+") | "-";
      BinExpr.Rule = Expr + BinOp + Expr;
      BinOp.Rule = ToTerm("+") | "-" | "*" | "/" | "**";
      AssignmentStmt.Rule = identifier + "=" + Expr;
      Stmt.Rule = AssignmentStmt | Expr | ReturnStmt | Empty;
      ReturnStmt.Rule = "return" + Expr; //Not supported for execution! - we associate NotSupportedNode with ReturnStmt
      //Eos is End-Of-Statement token produced by CodeOutlineFilter
      ExtStmt.Rule = Stmt + Eos | FunctionDef;
      Block.Rule = Indent + StmtList + Dedent;
      StmtList.Rule = MakePlusRule(StmtList, ExtStmt);

      ParamList.Rule = MakeStarRule(ParamList, comma, identifier);
      ArgList.Rule = MakeStarRule(ArgList, comma, Expr);
      FunctionDef.Rule = "def" + identifier + "(" + ParamList + ")" + colon + Eos + Block;
      FunctionDef.NodeCaptionTemplate = "def #{1}(...)"; 
      FunctionCall.Rule = identifier + "(" + ArgList + ")";
      FunctionCall.NodeCaptionTemplate = "call #{0}(...)";

      this.Root = StmtList;       // Set grammar root

      // 4. Token filters - created in a separate method CreateTokenFilters
      //    we need to add continuation symbol to NonGrammarTerminals because it is not used anywhere in grammar
      NonGrammarTerminals.Add(ToTerm(@"\"));

      // 5. Operators precedence
      RegisterOperators(1, "+", "-");
      RegisterOperators(2, "*", "/");
      RegisterOperators(3, Associativity.Right, "**");

      // 6. Miscellaneous: punctuation, braces, transient nodes
      MarkPunctuation("(", ")", ":");
      RegisterBracePair("(", ")");
      MarkTransient(Term, Expr, Stmt, ExtStmt, UnOp, BinOp, ExtStmt, ParExpr, Block);

      // 7. Error recovery rule
      ExtStmt.ErrorRule = SyntaxError + Eos;
      FunctionDef.ErrorRule = SyntaxError + Dedent; 

      // 8. Syntax error reporting
      AddToNoReportGroup("(");
      AddToNoReportGroup(Eos); 
      AddOperatorReportGroup("operator"); 

      // 9. Initialize console attributes
      ConsoleTitle = "Mini-Python Console";
      ConsoleGreeting =
@"Irony Sample Console for mini-Python.
 
   Supports a small sub-set of Python: assignments, arithmetic operators, 
   function declarations with 'def'. Supports big integer arithmetics.
   Supports Python indentation and line-joining rules, including '\' as 
   a line joining symbol. 

Press Ctrl-C to exit the program at any time.
";
      ConsolePrompt = ">>>"; 
      ConsolePromptMoreInput = "..."; 
      
      // 10. Language flags
      this.LanguageFlags = LanguageFlags.NewLineBeforeEOF | LanguageFlags.CreateAst | LanguageFlags.SupportsBigInt;

    }//constructor

    public override void CreateTokenFilters(LanguageData language, TokenFilterList filters) {
      var outlineFilter = new CodeOutlineFilter(language.GrammarData, 
        OutlineOptions.ProduceIndents | OutlineOptions.CheckBraces, ToTerm(@"\")); // "\" is continuation symbol
      filters.Add(outlineFilter);
    }

  }//class
}//namespace


