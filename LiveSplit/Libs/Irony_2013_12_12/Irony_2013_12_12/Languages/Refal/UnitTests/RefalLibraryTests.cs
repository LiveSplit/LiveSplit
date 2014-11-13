using System;
using Refal.Runtime;

namespace Refal.UnitTests
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
	/// Refal runtime library tests.
	/// Written by Alexey Yakovlev, yallie@yandex.ru.
	/// http://refal.codeplex.com
	/// </summary>
	[TestClass]
	public class RefalLibraryTests
	{
		/// <summary>
		/// Initialized by MSTest Framework
		/// </summary>
		public TestContext TestContext { get; set; }

		// int32, int32 -> int32

		[TestMethod]
		public void RefalLibrary_TestAddInt32()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 123, new ClosingBrace(), 321);
			var result = refal.Add(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 444);
		}

		[TestMethod]
		public void RefalLibrary_TestAddInt32Negative()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), '-', 123, new ClosingBrace(), 321);
			var result = refal.Add(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 198);
		}

		[TestMethod]
		public void RefalLibrary_TestSubInt32()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 321, new ClosingBrace(), 123);
			var result = refal.Sub(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 198);
		}

		[TestMethod]
		public void RefalLibrary_TestSubInt32Negative()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 123, new ClosingBrace(), 321);
			var result = refal.Sub(expr);

			// result should be '-' 198 (two symbols)
			Assert.AreEqual(result.Count, 2);
			Assert.IsTrue(result[0] is char);
			Assert.IsTrue(result[1] is int);
			Assert.AreEqual((char)result[0], '-');
			Assert.AreEqual((int)result[1], 198);
		}

		[TestMethod]
		public void RefalLibrary_TestMulInt32()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 321, new ClosingBrace(), 123);
			var result = refal.Mul(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 39483);
		}

		[TestMethod]
		public void RefalLibrary_TestMulInt32NegNeg()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), '-', 321, new ClosingBrace(), '-', 123);
			var result = refal.Mul(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 39483);
		}

		[TestMethod]
		public void RefalLibrary_TestMulInt32PosNeg()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 321, new ClosingBrace(), '-', 123);
			var result = refal.Mul(expr);

			// result should be '-' 198 (two symbols)
			Assert.AreEqual(result.Count, 2);
			Assert.IsTrue(result[0] is char);
			Assert.IsTrue(result[1] is int);
			Assert.AreEqual((char)result[0], '-');
			Assert.AreEqual((int)result[1], 39483);
		}

		[TestMethod]
		public void RefalLibrary_TestMulInt32NegPos()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), '-', 321, new ClosingBrace(), 123);
			var result = refal.Mul(expr);

			// result should be '-' 198 (two symbols)
			Assert.AreEqual(result.Count, 2);
			Assert.IsTrue(result[0] is char);
			Assert.IsTrue(result[1] is int);
			Assert.AreEqual((char)result[0], '-');
			Assert.AreEqual((int)result[1], 39483);
		}

		[TestMethod]
		public void RefalLibrary_TestDivInt32()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 321, new ClosingBrace(), 123);
			var result = refal.Div(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 2);
		}

		[TestMethod]
		public void RefalLibrary_TestDivInt32PosNeg()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), '-', 321, new ClosingBrace(), 123);
			var result = refal.Div(expr);

			Assert.AreEqual(result.Count, 2);
			Assert.IsTrue(result[0] is char);
			Assert.IsTrue(result[1] is int);
			Assert.AreEqual((char)result[0], '-');
			Assert.AreEqual((int)result[1], 2);
		}

		[TestMethod]
		public void RefalLibrary_TestDivInt32NegPos()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 321, new ClosingBrace(), '-', 123);
			var result = refal.Div(expr);

			Assert.AreEqual(result.Count, 2);
			Assert.IsTrue(result[0] is char);
			Assert.IsTrue(result[1] is int);
			Assert.AreEqual((char)result[0], '-');
			Assert.AreEqual((int)result[1], 2);
		}

		[TestMethod]
		public void RefalLibrary_TestDivInt32NegNeg()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), '-', 321, new ClosingBrace(), '-', 123);
			var result = refal.Div(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 2);
		}

		// int64, int64 -> int32

		[TestMethod]
		public void RefalLibrary_TestAddInt64GivingInt32()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 123L, new ClosingBrace(), 321L);
			var result = refal.Add(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 444);
		}

		[TestMethod]
		public void RefalLibrary_TestAddInt64NegativeGivingInt32()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), '-', 123L, new ClosingBrace(), 321L);
			var result = refal.Add(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 198);
		}

		[TestMethod]
		public void RefalLibrary_TestSubInt64GivingInt32()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 321L, new ClosingBrace(), 123L);
			var result = refal.Sub(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 198);
		}

		[TestMethod]
		public void RefalLibrary_TestSubInt64GivingInt32Negative()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 123L, new ClosingBrace(), 321L);
			var result = refal.Sub(expr);

			// result should be '-' 198 (two symbols)
			Assert.AreEqual(result.Count, 2);
			Assert.IsTrue(result[0] is char);
			Assert.IsTrue(result[1] is int);
			Assert.AreEqual((char)result[0], '-');
			Assert.AreEqual((int)result[1], 198);
		}

		[TestMethod]
		public void RefalLibrary_TestMulInt64GivingInt32()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 321L, new ClosingBrace(), 123L);
			var result = refal.Mul(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 39483);
		}

		[TestMethod]
		public void RefalLibrary_TestMulInt64GivingInt32Negative()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 321L, new ClosingBrace(), '-', 123L);
			var result = refal.Mul(expr);

			// result should be '-' 198 (two symbols)
			Assert.AreEqual(result.Count, 2);
			Assert.IsTrue(result[0] is char);
			Assert.IsTrue(result[1] is int);
			Assert.AreEqual((char)result[0], '-');
			Assert.AreEqual((int)result[1], 39483);
		}

		[TestMethod]
		public void RefalLibrary_TestDivInt64GivingInt32()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 321L, new ClosingBrace(), 123L);
			var result = refal.Div(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 2);
		}

		[TestMethod]
		public void RefalLibrary_TestDivInt64GivingInt32Negative()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), '-', 321L, new ClosingBrace(), '-', 123L);
			var result = refal.Div(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is int);
			Assert.AreEqual((int)result[0], 2);
		}

		// int64, int64 -> int64

		[TestMethod]
		public void RefalLibrary_TestAddInt64()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 123123123123, new ClosingBrace(), 321321321321);
			var result = refal.Add(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is long);
			Assert.AreEqual((long)result[0], 444444444444);
		}

		[TestMethod]
		public void RefalLibrary_TestAddInt64Negative()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), '-', 123123123123, new ClosingBrace(), 321321321321);
			var result = refal.Add(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is long);
			Assert.AreEqual((long)result[0], 198198198198);
		}

		[TestMethod]
		public void RefalLibrary_TestSubInt64()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 321321321321, new ClosingBrace(), 123123123123L);
			var result = refal.Sub(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is long);
			Assert.AreEqual((long)result[0], 198198198198);
		}

		[TestMethod]
		public void RefalLibrary_TestSubInt64Negative()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), '-', 123123123123, new ClosingBrace(), 321321321321);
			var result = refal.Sub(expr);

			// result should be '-' 444444444444 (two symbols)
			Assert.AreEqual(result.Count, 2);
			Assert.IsTrue(result[0] is char);
			Assert.IsTrue(result[1] is long);
			Assert.AreEqual((char)result[0], '-');
			Assert.AreEqual((long)result[1], 444444444444);
		}

		[TestMethod]
		public void RefalLibrary_TestMulInt64()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 321321L, new ClosingBrace(), 123123L);
			var result = refal.Mul(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is long);
			Assert.AreEqual((long)result[0], 39562005483);
		}

		[TestMethod]
		public void RefalLibrary_TestMulInt64Negative()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), '-', 321321L, new ClosingBrace(), '-', 123123L);
			var result = refal.Mul(expr);

			Assert.AreEqual(result.Count, 1);
			Assert.IsTrue(result[0] is long);
			Assert.AreEqual((long)result[0], 39562005483);
		}

		[TestMethod]
		public void RefalLibrary_TestDivInt64()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(new OpeningBrace(), 321321321321321, new ClosingBrace(), '-', 123L);
			var result = refal.Div(expr);

			// result should be '-' 2612368466027 (two symbols)
			Assert.AreEqual(result.Count, 2);
			Assert.IsTrue(result[0] is char);
			Assert.IsTrue(result[1] is long);
			Assert.AreEqual((char)result[0], '-');
			Assert.AreEqual((long)result[1], 2612368466027);
		}

		[TestMethod]
		public void RefalLibrary_TestChr()
		{
			var refal = new RefalLibrary(null);
			var expr = PassiveExpression.Build(13, 10, 0x20, "NewLine");
			var result = refal.Chr(expr);

			// result should be '\r', '\n', ' ' and Environment.NewLine
			var expected = ("\r\n " + Environment.NewLine).ToCharArray();
			Assert.AreEqual(result.Count, expected.Length);

			for (int i = 0; i < result.Count; i++)
			{
				Assert.IsTrue(result[i] is char);
				Assert.AreEqual(expected[i], (char)result[i]);
			}
		}
	}
}
