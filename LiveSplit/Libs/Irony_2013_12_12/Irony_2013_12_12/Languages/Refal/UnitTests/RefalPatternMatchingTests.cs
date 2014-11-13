using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Refal.Runtime;

namespace Refal.Runtime.UnitTests
{
	#region Unit testing platform abstraction layer
#if NUNIT
	using NUnit.Framework;
	using TestClass = NUnit.Framework.TestFixtureAttribute;
	using TestMethod = NUnit.Framework.TestAttribute;
	using TestContext = System.Object;
#else
	using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
	#endregion

	/// <summary>
	/// Refal pattern matching tests.
	/// Written by Alexey Yakovlev, yallie@yandex.ru.
	/// http://refal.codeplex.com
	/// </summary>
	[TestClass]
	public class RefalPatternMatchingTests
	{
		// elementary tests

		[TestMethod]
		public void Symbol_MatchesSameSymbol()
		{
			var pattern = new Pattern(123); // 123
			var expr = PassiveExpression.Build(123); // 123

			var result = pattern.Match(expr);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Symbol_DontMatchAnotherSymbol()
		{
			var pattern = new Pattern(123); // 123
			var expr = PassiveExpression.Build('X'); // 'X'

			var result = pattern.Match(expr);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void SymbolChain_MatchesSameSymbolChain()
		{
			var pattern = new Pattern(123, 'T', 'e', 's', 't'); // 123 'Test'
			var expr = PassiveExpression.Build(123, 'T', 'e', 's', 't'); // 123 'Test'

			var result = pattern.Match(expr);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void SymbolChain_DontMatchDifferentSymbolChain()
		{
			var pattern = new Pattern(123, 'T', 'e', 's', 't'); // 123 'Test'
			var expr = PassiveExpression.Build(123, 'T', 'E', 'S', 'T'); // 123 'TEST'

			var result = pattern.Match(expr);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void SymbolVariable_MatchesAnySymbol()
		{
			var pattern = new Pattern(new SymbolVariable("s.1")); // s.1
			var expr = PassiveExpression.Build(123); // 123

			var result = pattern.Match(expr);
			Assert.IsTrue(result);
			Assert.IsTrue(pattern.Variables.ContainsKey("s.1"));
			Assert.AreEqual(123, pattern.Variables["s.1"].Value);

			expr = PassiveExpression.Build('A'); // 'A'

			result = pattern.Match(expr);
			Assert.IsTrue(result);
			Assert.IsTrue(pattern.Variables.ContainsKey("s.1"));
			Assert.AreEqual('A', pattern.Variables["s.1"].Value);
		}

		[TestMethod]
		public void SymbolVariable_DontMatchSymbolChain()
		{
			var pattern = new Pattern(new SymbolVariable("s.1")); // s.1
			var expr = PassiveExpression.Build(123, 321); // 123 321

			var result = pattern.Match(expr);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void SymbolVariable_DontMatchBraces()
		{
			var pattern = new Pattern(new SymbolVariable("s.1")); // s.1
			var expr = PassiveExpression.Build(new OpeningBrace()); // (

			var result = pattern.Match(expr);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void TermVariable_MatchesAnySymbol()
		{
			var pattern = new Pattern(new TermVariable("t.1")); // t.1
			var expr = PassiveExpression.Build(123); // 123

			var result = pattern.Match(expr);
			Assert.IsTrue(result);
			Assert.IsTrue(pattern.Variables.ContainsKey("t.1"));
			Assert.AreEqual(123, pattern.Variables["t.1"].Value);

			expr = PassiveExpression.Build('A'); // 'A'

			result = pattern.Match(expr);
			Assert.IsTrue(result);
			Assert.IsTrue(pattern.Variables.ContainsKey("t.1"));
			Assert.AreEqual('A', pattern.Variables["t.1"].Value);
		}

		[TestMethod]
		public void TermVariable_DontMatchSymbolChain()
		{
			var pattern = new Pattern(new TermVariable("t.1")); // t.1
			var expr = PassiveExpression.Build(123, 321); // 123 321

			var result = pattern.Match(expr);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void TermVariable_MatchesEmptyExpressionInBraces()
		{
			var pattern = new Pattern(new TermVariable("t.1")); // t.1
			var expr = PassiveExpression.Build(new OpeningBrace(), new ClosingBrace()); // ()

			var result = pattern.Match(expr);
			Assert.IsTrue(result);
			Assert.IsTrue(pattern.Variables.ContainsKey("t.1"));
			Assert.AreEqual(PassiveExpression.Build(new OpeningBrace(), new ClosingBrace()), pattern.Variables["t.1"].Value);
		}

		[TestMethod]
		public void TermVariable_MatchesNonEmptyExpressionInBraces()
		{
			var pattern = new Pattern(new TermVariable("t.1")); // t.1
			var expr = PassiveExpression.Build(new OpeningBrace(), "Hello", new ClosingBrace()); // ()

			var result = pattern.Match(expr);
			Assert.IsTrue(result);
			Assert.IsTrue(pattern.Variables.ContainsKey("t.1"));
			Assert.AreEqual(PassiveExpression.Build(new OpeningBrace(), "Hello", new ClosingBrace()), pattern.Variables["t.1"].Value);
		}

		[TestMethod]
		public void ExpressionVariable_MatchesEmptyExpression()
		{
			var pattern = new Pattern(new ExpressionVariable("e.1")); // e.1
			var expr = PassiveExpression.Build(); // 

			var result = pattern.Match(expr);
			Assert.IsTrue(result);
			Assert.IsTrue(pattern.Variables.ContainsKey("e.1"));
		}

		[TestMethod]
		public void ExpressionVariableInBraces_DontMatchEmptyExpression()
		{
			var pattern = new Pattern(new OpeningBrace(), new ExpressionVariable("e.1"), new ClosingBrace()); // (e.1)
			var expr = PassiveExpression.Build(); //

			var result = pattern.Match(expr);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void ExpressionVariableInBraces_MatchesEmptyBraces()
		{
			var pattern = new Pattern(new OpeningBrace(), new ExpressionVariable("e.1"), new ClosingBrace()); // (e.1)
			var expr = PassiveExpression.Build(new OpeningBrace(), new ClosingBrace()); // ()

			var result = pattern.Match(expr);
			Assert.IsTrue(result);
			Assert.IsTrue(pattern.Variables.ContainsKey("e.1"));
		}

		[TestMethod]
		public void ExpressionVariableInBraces_MatchesNonEmptyBraces()
		{
			var pattern = new Pattern(new OpeningBrace(), new ExpressionVariable("e.1"), new ClosingBrace()); // (e.1)
			var expr = PassiveExpression.Build(new OpeningBrace(), 123, new ClosingBrace()); // (123)

			var result = pattern.Match(expr);
			Assert.IsTrue(result);
			Assert.IsTrue(pattern.Variables.ContainsKey("e.1"));
			Assert.AreEqual(PassiveExpression.Build(123), pattern.Variables["e.1"].Value);
		}

		// non-elementary tests

		[TestMethod]
		public void ComplexPattern_MatchesExpression1()
		{
			var pattern = new Pattern(new OpeningBrace(), new ExpressionVariable("e.1"), '0', new ClosingBrace(), 
				new OpeningBrace(), new ExpressionVariable("e.2"), new SymbolVariable("s.3"), new ClosingBrace()); // (e.1 '0') (e.2 s.3)
			var expr = PassiveExpression.Build(new OpeningBrace(), '0', new ClosingBrace(),
				new OpeningBrace(), '0', new ClosingBrace()); // ('0') ('0')

			var result = pattern.Match(expr);
			Assert.IsTrue(result);
			Assert.IsTrue(pattern.Variables.ContainsKey("e.1"));
			Assert.IsTrue(pattern.Variables.ContainsKey("e.2"));
			Assert.IsTrue(pattern.Variables.ContainsKey("s.3"));
			Assert.AreEqual(PassiveExpression.Build(), pattern.Variables["e.1"].Value);
			Assert.AreEqual(PassiveExpression.Build(), pattern.Variables["e.2"].Value);
			Assert.AreEqual('0', pattern.Variables["s.3"].Value);
		}

		[TestMethod]
		public void ComplexPattern_MatchesExpression2()
		{
			var pattern = new Pattern(new OpeningBrace(), new ExpressionVariable("e.1"), new SymbolVariable("s.3"),
				new ClosingBrace(), new OpeningBrace(), new ExpressionVariable("e.2"), '0', new ClosingBrace()); // (e.1 s.3) (e.2 '0')
			var expr = PassiveExpression.Build(new OpeningBrace(), '0', '1', new ClosingBrace(),
				new OpeningBrace(), '0', new ClosingBrace()); // ('01') ('0')

			var result = pattern.Match(expr);
			Assert.IsTrue(result);
			Assert.IsTrue(pattern.Variables.ContainsKey("e.1"));
			Assert.IsTrue(pattern.Variables.ContainsKey("e.2"));
			Assert.IsTrue(pattern.Variables.ContainsKey("s.3"));
			Assert.AreEqual(PassiveExpression.Build('0'), pattern.Variables["e.1"].Value);
			Assert.AreEqual(PassiveExpression.Build(), pattern.Variables["e.2"].Value);
			Assert.AreEqual('1', pattern.Variables["s.3"].Value);
		}
	}
}
