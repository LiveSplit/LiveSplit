using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Irony.Parsing;

namespace Irony.Tests.TokenPreviewResolution {
#if USE_NUNIT
  using NUnit.Framework;
  using TestClass = NUnit.Framework.TestFixtureAttribute;
  using TestMethod = NUnit.Framework.TestAttribute;
  using TestInitialize = NUnit.Framework.SetUpAttribute;
#else
  using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

  [TestClass]
  public class ConflictResolutionTests {

    // samples to be parsed
    const string FieldSample = "private int SomeField;";
    const string PropertySample = "public string Name {}";
    const string FieldListSample = "private int Field1; public string Field2;";
  const string MixedListSample = @"
      public int Size {}
      private string TableName;
      override void Run()
      {
      }";

    // Full grammar, no hints - expect errors ---------------------------------------------------------------------
    [TestMethod]
    public void TestConflictGrammarNoHints_HasErrors() {
      var grammar = new ConflictGrammarNoHints();
      var parser = new Parser(grammar);
      Assert.IsTrue(parser.Language.Errors.Count > 0);
      //Cannot parse mixed list
      var sample = MixedListSample;
      var tree = parser.Parse(sample);
      Assert.IsNotNull(tree);
      Assert.IsTrue(tree.HasErrors());
    }

    // Hints in Rules --------------------------------------------------------------------------
    [TestMethod]
    public void TestConflictGrammarWithHintsOnRules() {
      var grammar = new ConflictGrammarWithHintsInRules();
      var parser = new Parser(grammar);
      Assert.IsTrue(parser.Language.Errors.Count == 0);
      // Field sample
      var sample = FieldSample;
      var tree = parser.Parse(sample);
      Assert.IsNotNull(tree);
      Assert.IsFalse(tree.HasErrors());

      Assert.IsNotNull(tree.Root);
      var term = tree.Root.Term as NonTerminal;
      Assert.IsNotNull(term);
      Assert.AreEqual("definition", term.Name);

      Assert.AreEqual(1, tree.Root.ChildNodes.Count);
      var modNode = tree.Root.ChildNodes[0].ChildNodes[0];
      Assert.AreEqual("fieldModifier", modNode.Term.Name);

      //Property 
      sample = PropertySample;
      tree = parser.Parse(sample);
      Assert.IsNotNull(tree);
      Assert.IsFalse(tree.HasErrors());

      Assert.IsNotNull(tree.Root);
      term = tree.Root.Term as NonTerminal;
      Assert.IsNotNull(term);
      Assert.AreEqual("definition", term.Name);

      Assert.AreEqual(1, tree.Root.ChildNodes.Count);
      modNode = tree.Root.ChildNodes[0].ChildNodes[0];
      Assert.AreEqual("propModifier", modNode.Term.Name);
    }

    //Hints on terms ---------------------------------------------------------------------
    [TestMethod]
    public void TestConflictGrammar_HintsOnTerms() {
      var grammar = new ConflictGrammarWithHintsOnTerms();
      var parser = new Parser(grammar);
      Assert.IsTrue(parser.Language.Errors.Count == 0);

      //Field list sample
      var sample = FieldListSample;
      var tree = parser.Parse(sample);
      Assert.IsNotNull(tree);
      Assert.IsFalse(tree.HasErrors());

      Assert.IsNotNull(tree.Root);
      var term = tree.Root.Term as NonTerminal;
      Assert.IsNotNull(term);
      Assert.AreEqual("StatementList", term.Name);

      Assert.AreEqual(2, tree.Root.ChildNodes.Count);
      var nodes = tree.Root.ChildNodes.Select(t => t.ChildNodes[0]).ToArray();
      Assert.AreEqual("fieldDef", nodes[0].Term.Name);
      Assert.AreEqual("fieldDef", nodes[1].Term.Name);

      //Mixed sample
      sample = MixedListSample;
      tree = parser.Parse(sample);
      Assert.IsNotNull(tree);
      Assert.IsFalse(tree.HasErrors());

      Assert.IsNotNull(tree.Root);
      term = tree.Root.Term as NonTerminal;
      Assert.IsNotNull(term);
      Assert.AreEqual("StatementList", term.Name);

      Assert.AreEqual(3, tree.Root.ChildNodes.Count);
      nodes = tree.Root.ChildNodes.Select(t => t.ChildNodes[0]).ToArray();
      Assert.AreEqual("propDef", nodes[0].Term.Name);
      Assert.AreEqual("fieldDef", nodes[1].Term.Name);
      Assert.AreEqual("methodDef", nodes[2].Term.Name);
    }

  }
}
