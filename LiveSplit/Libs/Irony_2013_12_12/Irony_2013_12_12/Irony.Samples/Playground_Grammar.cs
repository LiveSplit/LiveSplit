using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;
using System.Globalization;
using Irony.Interpreter;
using Irony.Interpreter.Ast;

namespace Irony.Samples.TestBed {
  [Language("QueryLanguage", "1.0", "A Query Language based on JET where clauses")]
  public class QueryGrammar : Grammar {

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryGrammar"/> class.
    /// </summary>
    public QueryGrammar()
      : base(false) { // true means case sensitive
      GrammarComments = @"A Query Language based on JET where clauses. Case-insensitive.";

      // Terminals (Lexing)
      NumberLiteral number = new NumberLiteral("number");
      StringLiteral STRING = new StringLiteral("STRING", "\"");

      //Let's allow big integers (with unlimited number of digits):
      number.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
      IdentifierTerminal Name = new IdentifierTerminal("Name");
      //var Name = TerminalFactory.CreateSqlExtIdentifier(this, "id_simple");
      CommentTerminal comment = new CommentTerminal("comment", "//", "\n", "\r");
      //comment must be added to NonGrammarTerminals list; it is not used directly in grammar rules,
      // so we add it to this list to let Scanner know that it is also a valid terminal. 
      NonGrammarTerminals.Add(comment);
      comment = new CommentTerminal("multilineComment", "/*", "*/");
      NonGrammarTerminals.Add(comment);

      ConstantTerminal CONSTANT = new ConstantTerminal("CONSTANT");
      CONSTANT.Add("NULL", null);


      // Non-Terminals (Parsing)
      NonTerminal query = new NonTerminal("Query");
      NonTerminal tableExpression = new NonTerminal("TableExpression");
      NonTerminal tableExpressions = new NonTerminal("TableExpressions");
      NonTerminal table = new NonTerminal("Table");
      NonTerminal column = new NonTerminal("Column");
      NonTerminal tableOperator = new NonTerminal("TableOperator");
      NonTerminal value = new NonTerminal("Value");
      NonTerminal logicOp = new NonTerminal("LogicOp");
      NonTerminal parameter = new NonTerminal("Parameter");
      NonTerminal list = new NonTerminal("List");
      NonTerminal enclosure = new NonTerminal("Enclosure");
      NonTerminal closure = new NonTerminal("Closure");
      NonTerminal logicExpression = new NonTerminal("logicExpression");
      NonTerminal queryExpression = new NonTerminal("queryExpression");
      NonTerminal betweenStmt = new NonTerminal("BetweenStmt");
      NonTerminal expList = new NonTerminal("ExpList");

      //keywords
      KeyTerm AND = ToTerm("AND");
      KeyTerm OR = ToTerm("OR");
      KeyTerm IN = ToTerm("IN");
      KeyTerm BETWEEN = ToTerm("BETWEEN");
      KeyTerm LIKE = ToTerm("LIKE");
      KeyTerm NOT = ToTerm("NOT");
      KeyTerm dot = ToTerm(".", "dot");
      KeyTerm comma = ToTerm(",", "comma");
      KeyTerm LeftSquareBrace = ToTerm("[", "LeftSquareBrace");
      KeyTerm RightSquareBrace = ToTerm("]", "RightSquareBrace");
      KeyTerm LeftCurlyBrace = ToTerm("{", "LeftSCurlyBrace");
      KeyTerm RightCurlyBrace = ToTerm("}", "RightCurlyBrace");
      KeyTerm LeftQuote = ToTerm("\"", "LeftQuote");
      KeyTerm RightQuote = ToTerm("\"", "RightQuote");

      //set precedence of operators.
      RegisterOperators(90, AND);
      RegisterOperators(80, OR);
      RegisterOperators(70, "=", ">", "<", "<>", ">=", "<=", "IN", "LIKE", "NOT LIKE", "IS", "IS NOT", "BETWEEN");

      MarkPunctuation(",", "(", ")", "[", "]", ".", "\"", "{", "}");

      logicExpression.Rule = tableExpression + logicOp + tableExpression | "(" + logicExpression + ")";
      //queryExpression.Rule = MakePlusRule(queryExpression,logicOp,logicExpression);
      tableExpression.Rule = table + dot + column + tableOperator + value;
      tableExpression.ErrorRule = SyntaxError + ";";
      betweenStmt.Rule = BETWEEN + value + "AND";
      tableOperator.Rule = ToTerm("=") | ">" | "<" | "<>" | ">=" | "<=" | "LIKE" | "IN" | "NOT LIKE" | "IS" | "IS NOT" | betweenStmt;
      value.Rule = number | parameter | STRING | CONSTANT | expList;
      enclosure.Rule = ToTerm("(") | Empty;
      closure.Rule = ToTerm(")") | Empty;
      parameter.Rule = LeftQuote + "{" + Name + "}" + "\"" | "{" + Name + "}" | "#" + "{" + Name + "}" + "#";
      logicOp.Rule = AND | OR;
      expList.Rule = "(" + list + ")";
      list.Rule = MakePlusRule(list, comma, value);
      table.Rule = LeftSquareBrace + Name + RightSquareBrace;
      table.ErrorRule = SyntaxError + ";";
      column.Rule = LeftSquareBrace + Name + RightSquareBrace;
      column.ErrorRule = SyntaxError + ";";
      query.Rule = MakePlusRule(query, logicOp, logicExpression);

      Root = query;
    }
  }
}

