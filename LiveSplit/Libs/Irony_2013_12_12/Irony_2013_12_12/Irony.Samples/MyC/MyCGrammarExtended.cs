using System;
using System.Collections.Generic;
using System.Linq;
using Irony.Parsing;

namespace Demo {
  // Extended MyC Grammar. 
  // For use in testing macros, conditional compilation, includes

  [Language("MyC-Extended", "1.0", "MyC Language, extended")]
  public class MyCGrammarExtended : Irony.Parsing.Grammar {
    public MyCGrammarExtended() {
      #region Declare Terminals Here
      CommentTerminal blockComment = new CommentTerminal("block-comment", "/*", "*/");
      CommentTerminal lineComment = new CommentTerminal("line-comment", "//",
          "\r", "\n", "\u2085", "\u2028", "\u2029");
      NonGrammarTerminals.Add(blockComment);
      NonGrammarTerminals.Add(lineComment);

      NumberLiteral number = new NumberLiteral("number");
      IdentifierTerminal identifier = new IdentifierTerminal("identifier");
      #endregion

      #region Declare NonTerminals Here
      NonTerminal program = new NonTerminal("program");
      NonTerminal declarations = new NonTerminal("declaration");
      NonTerminal declaration = new NonTerminal("declaration");
      NonTerminal simpleDeclarations = new NonTerminal("simple-declarations");
      NonTerminal simpleDeclaration = new NonTerminal("simple-declaration");
      NonTerminal semiDeclaration = new NonTerminal("semi-declaration");
      NonTerminal parenParameters = new NonTerminal("paren-parameters");
      NonTerminal parameters = new NonTerminal("parameters");
      NonTerminal classOption = new NonTerminal("class-option");
      NonTerminal variableType = new NonTerminal("variable-type");
      NonTerminal block = new NonTerminal("block");
      NonTerminal blockContent = new NonTerminal("block-content");
      NonTerminal statements = new NonTerminal("statements");
      NonTerminal statement = new NonTerminal("statement");
      NonTerminal parenExpressionAlways = new NonTerminal("paren-expression-always");
      NonTerminal parenExpression = new NonTerminal("paren-expression");
      NonTerminal forHeader = new NonTerminal("for-header");
      NonTerminal forBlock = new NonTerminal("for-block");
      NonTerminal semiStatement = new NonTerminal("semi-statement");
      NonTerminal arguments = new NonTerminal("arguments");
      NonTerminal parenArguments = new NonTerminal("paren-arguments");
      NonTerminal assignExpression = new NonTerminal("assign-expression");
      NonTerminal expression = new NonTerminal("expression");
      NonTerminal booleanOperator = new NonTerminal("boolean-operator");
      NonTerminal relationalExpression = new NonTerminal("relational-expression");
      NonTerminal relationalOperator = new NonTerminal("relational-operator");
      NonTerminal bitExpression = new NonTerminal("bit-expression");
      NonTerminal bitOperator = new NonTerminal("bit-operator");
      NonTerminal addExpression = new NonTerminal("add-expression");
      NonTerminal addOperator = new NonTerminal("add-operator");
      NonTerminal multiplyExpression = new NonTerminal("multiply");
      NonTerminal multiplyOperator = new NonTerminal("multiply-operator");
      NonTerminal prefixExpression = new NonTerminal("prefix-expression");
      NonTerminal prefixOperator = new NonTerminal("prefix-operator");
      NonTerminal factor = new NonTerminal("factor");
      NonTerminal identifierExpression = new NonTerminal("identifier-expression");
      #endregion

      #region Place Rules Here
      this.Root = program;

      program.Rule = declarations;

      declarations.Rule = MakeStarRule(declarations, declaration);

      declaration.Rule
          = classOption + variableType + identifier + parameters + block
          | classOption + identifier + parenParameters + block
          | variableType + identifier + parenParameters + block
          | identifier + parenParameters + block
          | simpleDeclaration;

      simpleDeclarations.Rule = MakePlusRule(simpleDeclarations, simpleDeclaration);

      simpleDeclaration.Rule = semiDeclaration + ";";

      semiDeclaration.Rule
          = semiDeclaration + "," + identifier
          | classOption + variableType + identifier
          | variableType + identifier;

      parameters.Rule
          = parameters + "," + variableType + identifier
          | variableType + identifier;

      parenParameters.Rule
          = ToTerm("(") + ")"
          | "(" + parameters + ")";

      classOption.Rule
          = ToTerm("static")
          | "auto"
          | "extern";

      variableType.Rule
          = ToTerm("int")
          | "void";

      block.Rule
          = ToTerm("{") + "}"
          | "{" + blockContent + "}";

      blockContent.Rule
          = simpleDeclarations + statements
          | simpleDeclarations
          | statements;

      statements.Rule = MakePlusRule(statements, statement);

      statement.Rule
          = semiStatement + ";"
          | block
          | "while" + parenExpressionAlways + statement
          | "for" + forHeader + statement
          | "if" + parenExpressionAlways + statement
          | "if" + parenExpressionAlways + statement + PreferShiftHere() + "else" + statement;

      parenExpressionAlways.Rule = parenExpression;

      parenExpression.Rule = ToTerm("(") + expression + ")";

      forHeader.Rule = "(" + forBlock + ")";

      forBlock.Rule = assignExpression + ";" + expression + ";" + assignExpression;

      semiStatement.Rule
          = assignExpression
          | "return" + expression
          | "break"
          | "continue";

      arguments.Rule
          = expression + "," + arguments
          | expression;

      parenArguments.Rule
          = ToTerm("(") + ")"
          | "(" + arguments + ")";

      assignExpression.Rule
          = identifier + "=" + expression
          | expression;

      expression.Rule
          = relationalExpression + booleanOperator + expression
          | relationalExpression;

      booleanOperator.Rule
          = ToTerm("&&")
          | "||";

      relationalExpression.Rule
          = bitExpression + relationalOperator + bitExpression
          | bitExpression;

      relationalOperator.Rule
          = ToTerm(">")
          | ">="
          | "<"
          | "<="
          | "=="
          | "!=";

      bitExpression.Rule
          = addExpression + bitOperator + bitExpression
          | addExpression;

      bitOperator.Rule
          = ToTerm("|")
          | "&"
          | "^";

      addExpression.Rule
          = multiplyExpression + addOperator + addExpression
          | prefixExpression;

      addOperator.Rule
          = ToTerm("+") | "-";

      multiplyExpression.Rule
          = prefixExpression + multiplyOperator + multiplyExpression
          | prefixExpression;

      multiplyOperator.Rule
          = ToTerm("*")
          | "/";

      prefixExpression.Rule
          = prefixOperator + factor
          | factor;

      prefixOperator.Rule = ToTerm("!");

      factor.Rule
          = identifierExpression + parenArguments
          | identifierExpression
          | number
          | parenExpression;

      identifierExpression.Rule
          = identifier
          | identifierExpression + "." + identifier;
      #endregion

      #region Define Keywords
      this.MarkReservedWords("break", "continue", "else", "extern", "for",
          "if", "int", "return", "static", "void", "while");
      #endregion
    }
  }
}
