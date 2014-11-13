using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
  
  public static class TestHelper {
    //A skeleton for a grammar with a single terminal, followed by optional terminator
    class TerminalTestGrammar : Grammar {
      public string Terminator;
      public TerminalTestGrammar(Terminal terminal, string terminator = null) : base(caseSensitive: true) {
        Terminator = terminator; 
        var rule = new BnfExpression(terminal);
        if (Terminator != null) {
          MarkReservedWords(Terminator);
          rule += Terminator; 
        }
        base.Root = new NonTerminal("Root");
        Root.Rule = rule;
      }

    }//class

    public static Parser CreateParser(Terminal terminal, string terminator = "end") {
      var grammar = new TerminalTestGrammar(terminal, terminator);
      var parser = new Parser(grammar);
      CheckGrammarErrors(parser);
      return parser; 
    }

    public static void CheckGrammarErrors(Parser parser) {
      var errors = parser.Language.Errors;
      if (errors.Count > 0)
        throw new Exception("Unexpected grammar contains error(s): " + string.Join("\n",  errors));
    }
    public static void CheckParseErrors(ParseTree parseTree) {
      if (parseTree.HasErrors())
        throw new Exception("Unexpected parse error(s): " + string.Join("\n", parseTree.ParserMessages));
    }

    public static Token ParseInput(this Parser parser, string input, bool useTerminator = true) {
      var g = (TerminalTestGrammar) parser.Language.Grammar;
      useTerminator &= g.Terminator != null; 
      if (useTerminator)
        input += " " + g.Terminator; 
      var tree = parser.Parse(input);
      //If error, then return this error token, this is probably what is expected.
      var first = tree.Tokens[0];
      if (first.IsError())
        return first; 
      //Verify that last or before-last token is a terminator
      if (useTerminator) {
        Assert.IsTrue(tree.Tokens.Count >= 2, "Wrong # of tokens - expected at least 2. Input: " + input);
        var count = tree.Tokens.Count; 
        //The last is EOF, the one before last should be a terminator
        Assert.AreEqual(g.Terminator, tree.Tokens[count - 2].Text, "Input terminator not found in the second token. Input: " + input); 
      }
      return tree.Tokens[0];
    }

  }//class
}
