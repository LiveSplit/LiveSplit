using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Irony.Parsing;
using Irony.Parsing.New;
using Irony.Interpreter.Evaluator;
using System.Diagnostics;

namespace Irony.Tests {

  [TestClass]
  public class _newArchTests {
    
    [TestMethod]
    public void TestNewScanner() {
      var grammar = new ExpressionEvaluatorGrammar();
      var language = new LanguageData(grammar); 
      var parser = new Parser(language); 
      var ctx = new ParsingContext(parser);
      PrefixBasedScanner scanner = new PrefixBasedScanner(language); 
      var src = @"
# this is comment
""0123"".Substring(1) + ""abcd"".Length + ""456""[1]    # expected '12345'
";
      var srcSegm = new TextSegment(src); 
      var segments = scanner.Scan(ctx, srcSegm).ToList();
      foreach (var segm in segments) {
        Debug.WriteLine(" Segment: " + segm.GetType().Name + ", " + segm.ToString());
      }
    }
  }
}
