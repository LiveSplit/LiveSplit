using System;
using System.Collections.Generic;
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

  [TestClass]
  public class LineContinuationTests {

    [TestMethod]
    public void TestContinuationTerminal_Simple() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(new LineContinuationTerminal("LineContinuation", "\\"));
      token = parser.ParseInput("\\\r\t");
      Assert.IsTrue(token.Category == TokenCategory.Outline, "Failed to read simple line continuation terminal");
    }

    [TestMethod]
    public void TestContinuationTerminal_Default() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(new LineContinuationTerminal("LineContinuation"));
      token = parser.ParseInput("_\r\n\t");
      Assert.IsTrue(token.Category == TokenCategory.Outline, "Failed to read default line continuation terminal");

      token = parser.ParseInput("\\\v    ");
      Assert.IsTrue(token.Category == TokenCategory.Outline, "Failed to read default line continuation terminal");
    }

    [TestMethod]
    public void TestContinuationTerminal_Complex() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(new LineContinuationTerminal("LineContinuation", @"\continue", @"\cont", "++CONTINUE++"));
      token = parser.ParseInput("\\cont   \r\n    ");
      Assert.IsTrue(token.Category == TokenCategory.Outline, "Failed to read complex line continuation terminal");

      token = parser.ParseInput("++CONTINUE++\t\v");
      Assert.IsTrue(token.Category == TokenCategory.Outline, "Failed to read complex line continuation terminal");
    }

    [TestMethod]
    public void TestContinuationTerminal_Incomplete() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(new LineContinuationTerminal("LineContinuation"));
      token = parser.ParseInput("\\   garbage");
      Assert.IsTrue(token.Category == TokenCategory.Error, "Failed to read incomplete line continuation terminal");

      parser = TestHelper.CreateParser(new LineContinuationTerminal("LineContinuation"));
      token = parser.ParseInput("_");
      Assert.IsTrue(token.Category == TokenCategory.Error, "Failed to read incomplete line continuation terminal");
    }
  }
}
