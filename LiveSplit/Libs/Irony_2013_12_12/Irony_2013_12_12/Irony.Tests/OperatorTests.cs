using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Irony.Parsing;

namespace Irony.Tests {
#if USE_NUNIT
    using NUnit.Framework;
    using TestClass = NUnit.Framework.TestFixtureAttribute;
    using TestMethod = NUnit.Framework.TestAttribute;
    using TestInitialize = NUnit.Framework.SetUpAttribute;
#else
  using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

  [TestClass]
  public class OperatorTests {

    #region Grammars
    public class OperatorGrammar : Grammar {
      public OperatorGrammar()  {
        var id = new IdentifierTerminal("id");
        var binOp = new NonTerminal("binOp");
        var unOp = new NonTerminal("unOp");
        var expr = new NonTerminal("expr");
        var binExpr = new NonTerminal("binExpr");
        var unExpr = new NonTerminal("unExpr");

        base.Root = expr;
        expr.Rule = id | binExpr | unExpr;
        binExpr.Rule = expr + binOp + expr; 
        binOp.Rule = ToTerm("+") | "-" | "*" | "/";
        unExpr.Rule = unOp + expr;
        unOp.Rule = ToTerm("+") | "-";

        RegisterOperators(10, "+", "-");
        RegisterOperators(20, "*", "/");
        MarkTransient(expr, binOp, unOp);
      }
    }//operator grammar class

    public class OperatorGrammarHintsOnTerms : Grammar {
      public OperatorGrammarHintsOnTerms() {
        var id = new IdentifierTerminal("id");
        var binOp = new NonTerminal("binOp");
        var unOp = new NonTerminal("unOp");
        var expr = new NonTerminal("expr");
        var binExpr = new NonTerminal("binExpr");
        var unExpr = new NonTerminal("unExpr");

        base.Root = expr;
        expr.Rule = id | binExpr | unExpr;
        binExpr.Rule = expr + binOp + expr;
        binOp.Rule = ToTerm("+") | "-" | "*" | "/";
        unExpr.Rule = unOp + expr;
        var unOpHint = ImplyPrecedenceHere(30); // Force higher precedence than multiplication precedence
        unOp.Rule = unOpHint + "+" | unOpHint + "-";
        RegisterOperators(10, "+", "-");
        RegisterOperators(20, "*", "/");
        MarkTransient(expr, binOp, unOp);
      }
    }//operator grammar class

    public class OperatorGrammarHintsOnNonTerms : Grammar {
      public OperatorGrammarHintsOnNonTerms() {
        var id = new IdentifierTerminal("id");
        var binOp = new NonTerminal("binOp");
        var unOp = new NonTerminal("unOp");
        var expr = new NonTerminal("expr");
        var binExpr = new NonTerminal("binExpr");
        var unExpr = new NonTerminal("unExpr");

        base.Root = expr;
        expr.Rule = id | binExpr | unExpr;
        binExpr.Rule = expr + binOp + expr;
        binOp.Rule = ToTerm("+") | "-" | "*" | "/";
        var unOpHint = ImplyPrecedenceHere(30); // Force higher precedence than multiplication precedence
        unExpr.Rule = unOpHint + unOp + expr;
        unOp.Rule = ToTerm("+") | "-";
        RegisterOperators(10, "+", "-");
        RegisterOperators(20, "*", "/");
        MarkTransient(expr, binOp, unOp);
      }
    }//operator grammar class

    #endregion

    [TestMethod]
    public void TestOperatorPrecedence() {

      var grammar = new OperatorGrammar();
      var parser = new Parser(grammar);
      TestHelper.CheckGrammarErrors(parser);

      var parseTree = parser.Parse("x + y * z");
      TestHelper.CheckParseErrors(parseTree);
      Assert.IsTrue(parseTree.Root != null, "Root not found.");
      Assert.IsTrue(parseTree.Root.Term.Name == "binExpr", "Expected binExpr.");
      Assert.IsTrue(parseTree.Root.ChildNodes[1].Term.Name == "+", "Expected + operator."); //check that top operator is "+", not "*"

      parseTree = parser.Parse("x * y + z");
      TestHelper.CheckParseErrors(parseTree);
      Assert.IsTrue(parseTree.Root != null, "Root not found.");
      Assert.IsTrue(parseTree.Root.Term.Name == "binExpr", "Expected binExpr.");
      Assert.IsTrue(parseTree.Root.ChildNodes[1].Term.Name == "+", "Expected + operator."); //check that top operator is "+", not "*"

      parseTree = parser.Parse("-x * y"); //should be interpreted as -(x*y), so top operator should be -
      TestHelper.CheckParseErrors(parseTree);
      Assert.IsTrue(parseTree.Root != null, "Root not found.");
      Assert.IsTrue(parseTree.Root.Term.Name == "unExpr", "Expected unExpr.");
      Assert.IsTrue(parseTree.Root.ChildNodes[0].Term.Name == "-", "Expected - operator."); //check that top operator is "+", not "*"

    }

    //These tests check how implied precedence work. We use ImpliedPrecedenceHint to set precedence on unary +,- operators and make it 
    // higher than binary +,-. We make it even higher than * precedence, so that -x*y is interpreted as '(-x)*y', not like '-(x*y)' 
    // the second interpretation is chosen when there are no hints.
    [TestMethod]
    public void TestOperatorPrecedenceWithHints() {

      var grammar = new OperatorGrammarHintsOnTerms();
      var parser = new Parser(grammar);
      TestHelper.CheckGrammarErrors(parser);

      var parseTree = parser.Parse("x + y * z");
      TestHelper.CheckParseErrors(parseTree);
      Assert.IsTrue(parseTree.Root != null, "Root not found.");
      Assert.IsTrue(parseTree.Root.Term.Name == "binExpr", "Expected binExpr.");
      Assert.IsTrue(parseTree.Root.ChildNodes[1].Term.Name == "+", "Expected + operator."); //check that top operator is "+", not "*"

      parseTree = parser.Parse("-x * y"); //should be interpreted as (-x)*y, so top operator should be *
      TestHelper.CheckParseErrors(parseTree);
      Assert.IsTrue(parseTree.Root != null, "Root not found.");
      Assert.IsTrue(parseTree.Root.Term.Name == "binExpr", "Expected binExpr.");
      Assert.IsTrue(parseTree.Root.ChildNodes[1].Term.Name == "*", "Expected * operator."); //check that top operator is "+", not "*"

      var grammar2 = new OperatorGrammarHintsOnNonTerms();
      parser = new Parser(grammar2);
      TestHelper.CheckGrammarErrors(parser);

      parseTree = parser.Parse("x + y * z");
      TestHelper.CheckParseErrors(parseTree);
      Assert.IsTrue(parseTree.Root != null, "Root not found.");
      Assert.IsTrue(parseTree.Root.Term.Name == "binExpr", "Expected binExpr.");
      Assert.IsTrue(parseTree.Root.ChildNodes[1].Term.Name == "+", "Expected + operator."); //check that top operator is "+", not "*"

      parseTree = parser.Parse("-x*y"); //should be interpreted as (-x)*y, so top operator should be *
      TestHelper.CheckParseErrors(parseTree);
      Assert.IsTrue(parseTree.Root != null, "Root not found.");
      Assert.IsTrue(parseTree.Root.Term.Name == "binExpr", "Expected binExpr.");
      Assert.IsTrue(parseTree.Root.ChildNodes[1].Term.Name == "*", "Expected * operator."); //check that top operator is "+", not "*"
    
    }

  }//class
}//namespace
