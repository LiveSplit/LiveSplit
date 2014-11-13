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

  [TestClass]
  public class DataLiteralsTests {
  
    [TestMethod]
    public void TestDataLiterals() {
      Parser parser; Token token;
      Terminal term;

      // FixedLengthLiteral ---------------------------------------------------------
      term = new FixedLengthLiteral("fixedLengthInteger", 2, TypeCode.Int32);
      parser = TestHelper.CreateParser(term, null);
      
      token = parser.ParseInput("1200");
      Assert.IsTrue(token.Value != null, "Failed to parse fixed-length integer.");
      Assert.IsTrue((int)token.Value == 12, "Failed to parse fixed-length integer - result value does not match.");

      term = new FixedLengthLiteral("fixedLengthString", 2, TypeCode.String);
      parser = TestHelper.CreateParser(term);
      token = parser.ParseInput("abcd", useTerminator: false);
      Assert.IsTrue(token != null && token.Value != null, "Failed to parse fixed-length string.");
      Assert.IsTrue((string)token.Value == "ab", "Failed to parse fixed-length string - result value does not match");

      // DsvLiteral ----------------------------------------------------------------
      term = new DsvLiteral("DsvInteger", TypeCode.Int32, ",");
      parser = TestHelper.CreateParser(term);
      token = parser.ParseInput("12,");
      Assert.IsTrue(token != null && token.Value != null, "Failed to parse CSV integer.");
      Assert.IsTrue((int)token.Value == 12, "Failed to parse CSV integer - result value does not match.");

      term = new DsvLiteral("DsvInteger", TypeCode.String, ",");
      parser = TestHelper.CreateParser(term);
      token = parser.ParseInput("ab,");
      Assert.IsTrue(token != null && token.Value != null, "Failed to parse CSV string.");
      Assert.IsTrue((string)token.Value == "ab", "Failed to parse CSV string - result value does not match.");
    
      // QuotedValueLiteral ----------------------------------------------------------------
      term = new QuotedValueLiteral ("QVDate", "#", TypeCode.DateTime);
      parser = TestHelper.CreateParser(term);
      token = parser.ParseInput("#11/15/2009#");
      Assert.IsTrue(token != null && token.Value != null, "Failed to parse quoted date.");
      Assert.IsTrue((DateTime)token.Value == new DateTime(2009, 11, 15), "Failed to parse quoted date - result value does not match.");

    }//method


  }//class

}//namespace
