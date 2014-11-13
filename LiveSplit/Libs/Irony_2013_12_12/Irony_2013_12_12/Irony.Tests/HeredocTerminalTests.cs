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

  //Currently not used, HereDocTerminal needs to be finished

    /// <summary>
    /// Summary description for HeredocTerminalTests
    /// </summary>
    [TestClass]
    public class HeredocTerminalTests {
        private TestContext testContextInstance;
        private Parser _p;

        private class HereDocTestGrammar : Grammar {
            public HereDocTestGrammar()
                : base(true) {
                var heredoc = new HereDocTerminal("HereDoc", "<<", HereDocOptions.None);
                heredoc.AddSubType("<<-", HereDocOptions.AllowIndentedEndToken);
                var @string = new StringLiteral("string", "\"");
                var program = new NonTerminal("program");
                program.Rule = heredoc + @"+" + @string + this.NewLine + @"+" + @string
                    | heredoc + @"+" + heredoc + @"+" + @string + this.NewLine
                    | heredoc + @"+" + @string + this.NewLine
                    | heredoc + @"+" + @string + @"+" + heredoc
                    | heredoc + @"+" + heredoc
                    | heredoc;
                this.Root = program;
                this.MarkPunctuation("+");
            }
        }

        private string NormalizeParseTree(ParseTree tree) {
            StringBuilder fullString = new StringBuilder();
            foreach (ParseTreeNode node in tree.Root.ChildNodes) {
                fullString.Append(node.Token.Value);
            }
            fullString = fullString.Replace("\r\n", "\\n");
            fullString = fullString.Replace("\n", "\\n");
            return fullString.ToString();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestInitialize]
        public void HereDocSetup() {
            HereDocTestGrammar grammar = new HereDocTestGrammar();
            _p = new Parser(grammar);
            _p.Context.SetOption(ParseOptions.TraceParser, true);
        }

        [TestMethod]
        public void TestHereDocLiteral() {
            var term = new HereDocTerminal("Heredoc","<<",HereDocOptions.None);
            parser = TestHelper.CreateParser(term);
            token = parser.ParseToken(@"<<BEGIN
test
BEGIN");
            Assert.IsNotNull(_token, "Failed to produce a token on valid string.");
            Assert.IsNotNull(token.Value, "Token Value field is null - should be string.");
            Assert.IsTrue((string)token.Value == "test", "Token Value is wrong, got {0} of type {1}", token.Value, token.Value.GetType().ToString());
        }

        [TestMethod]
        public void TestHereDocIndentedLiteral() {
            var term = new HereDocTerminal("Heredoc", "<<-", HereDocOptions.AllowIndentedEndToken);
            parser = TestHelper.CreateParser(term);
            token = parser.ParseToken(@"<<-BEGIN
test
                        BEGIN");
            Assert.IsNotNull(_token, "Failed to produce a token on valid string.");
            Assert.IsNotNull(token.Value, "Token Value field is null - should be string.");
            Assert.IsTrue((string)token.Value == "test", "Token Value is wrong, got {0} of type {1}", token.Value, token.Value.GetType().ToString());
        }

        [TestMethod]
        public void TestHereDocLiteralError() {
            var term = new HereDocTerminal("Heredoc","<<",HereDocOptions.None);
            parser = TestHelper.CreateParser(term);
            token = parser.ParseToken(@"<<BEGIN
test");
            Assert.IsNotNull(_token, "Failed to produce a token on valid string.");
            Assert.IsTrue(token.IsError(), "Failed to detect error on invalid heredoc.");
        }

        [TestMethod]
        public void TestHereDocIndentedLiteralError() {
            var term = new HereDocTerminal("Heredoc", "<<-", HereDocOptions.AllowIndentedEndToken);
            parser = TestHelper.CreateParser(term);
            token = parser.ParseToken(@"<<-BEGIN
test");
            Assert.IsNotNull(_token, "Failed to produce a token on valid string.");
            Assert.IsTrue(token.IsError(), "Failed to detect error on invalid heredoc.");
        }

        [TestMethod]
        public void TestHereDocLiteralErrorIndented() {
            var term = new HereDocTerminal("Heredoc", "<<", HereDocOptions.None);
            parser = TestHelper.CreateParser(term);
            token = parser.ParseToken(@"<<BEGIN
test
     BEGIN");
            Assert.IsNotNull(_token, "Failed to produce a token on valid string.");
            Assert.IsTrue(token.IsError(), "Failed to detect error on invalid heredoc.");
        }

        [TestMethod]
        public void TestHereDocParseHereDocStringString() {
            ParseTree tree = _p.Parse(@"<<HELLO + ""<--- this is the middle --->\n""
This is the beginning.
It is two lines long.
HELLO 
+ ""And now it's over!""");
            Assert.AreEqual(@"This is the beginning.\nIt is two lines long.\n<--- this is the middle --->\nAnd now it's over!", NormalizeParseTree(tree), "Incorrectly parsed heredoc.");
        }

        [TestMethod]
        public void TestHereDocParseHereDocStringHereDoc() {
            ParseTree tree = _p.Parse(@"<<HELLO + ""<--- this is the middle --->\n"" + <<END
This is the beginning.
It is more than two lines long.
It is three lines long.
HELLO 
And now it's over!
But we have three lines left.
Now two more lines.
Oops, last line! :(
END");
            Assert.AreEqual(@"This is the beginning.\nIt is more than two lines long.\nIt is three lines long.\n<--- this is the middle --->\nAnd now it's over!\nBut we have three lines left.\nNow two more lines.\nOops, last line! :(", NormalizeParseTree(tree), "Incorrectly parsed heredoc.");
        }

        [TestMethod]
        public void TestHereDocParseHereDocHereDoc() {
            ParseTree tree = _p.Parse(@"<<HELLO + <<END
This is the beginning.
How are you doing?
HELLO 
I'm fine.
And now it's over!
END");
            Assert.AreEqual(@"This is the beginning.\nHow are you doing?\nI'm fine.\nAnd now it's over!", NormalizeParseTree(tree), "Incorrectly parsed heredoc.");
        }

        [TestMethod]
        public void TestHereDocParseHereDoc() {
            ParseTree tree = _p.Parse(@"<<HELLO
This is the beginning.
I hope you enjoyed these tests.
HELLO");
            Assert.AreEqual(@"This is the beginning.\nI hope you enjoyed these tests.", NormalizeParseTree(tree), "Incorrectly parsed heredoc.");
        }

        [TestMethod]
        public void TestHereDocParseHereDocHereDocString() {
            ParseTree tree = _p.Parse(@"<<HELLO + <<MIDDLE + ""<--- And now it's over --->""
This is the beginning.
HELLO
And this is the middle.
MIDDLE");
            Assert.AreEqual(@"This is the beginning.\nAnd this is the middle.\n<--- And now it's over --->", NormalizeParseTree(tree), "Incorrectly parsed heredoc.");
        }

        [TestMethod]
        public void TestHereDocParseHereDocString() {
            ParseTree tree = _p.Parse(@"<<HELLO + ""<--- this is the end --->""
This is the beginning.
HELLO");
            Assert.AreEqual(@"This is the beginning.\n<--- this is the end --->", NormalizeParseTree(tree), "Incorrectly parsed heredoc.");
        }

        [TestMethod]
        public void TestHereDocParseIndentHereDocStringHereDoc() {
            ParseTree tree = _p.Parse(@"<<-BEGIN + ""<--- middle --->\n"" + <<-END
This is the beginning:
		BEGIN
And now it is over!
		END");
            Assert.AreEqual(@"This is the beginning:\n<--- middle --->\nAnd now it is over!", NormalizeParseTree(tree), "Incorrectly parsed heredoc.");
        }
    }
}
