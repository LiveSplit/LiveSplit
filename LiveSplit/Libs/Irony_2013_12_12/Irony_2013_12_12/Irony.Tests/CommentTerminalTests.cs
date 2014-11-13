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
  public class CommentTerminalTests {

    [TestMethod]
    public void TestCommentTerminal() {
      Parser parser; Token token; 

      parser = TestHelper.CreateParser(new CommentTerminal("Comment", "/*", "*/"));
      token = parser.ParseInput("/* abc  */");
      Assert.IsTrue(token.Category == TokenCategory.Comment, "Failed to read comment");

      parser = TestHelper.CreateParser(new CommentTerminal("Comment", "//", "\n"));
      token = parser.ParseInput("// abc  \n   ");
      Assert.IsTrue(token.Category == TokenCategory.Comment, "Failed to read line comment");

    }//method

  }//class
}//namespace
