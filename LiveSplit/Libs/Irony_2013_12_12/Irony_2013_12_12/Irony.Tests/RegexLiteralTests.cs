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
  public class RegexLiteralTests  {

    //The following test method and a fix are contributed by ashmind codeplex user
    [TestMethod]
    public void TestRegExLiteral() {
      Parser parser; Token token;

      var term = new RegexLiteral("RegEx");
      parser = TestHelper.CreateParser(term);
      token = parser.ParseInput(@"/abc\\\/de/gm  ");
      Assert.IsNotNull(token, "Failed to produce a token on valid string.");
      Assert.AreEqual(term, token.Terminal, "Failed to scan a string - invalid Terminal in the returned token.");
      Assert.IsNotNull(token.Value, "Token Value field is null - should be Regex object.");
      var regex = token.Value as Regex;
      Assert.IsNotNull(regex, "Failed to create Regex object.");
      var match = regex.Match(@"00abc\/de00"); 
      Assert.AreEqual(match.Index, 2, "Failed to match a regular expression");
    }

  }//class
}//namespace
