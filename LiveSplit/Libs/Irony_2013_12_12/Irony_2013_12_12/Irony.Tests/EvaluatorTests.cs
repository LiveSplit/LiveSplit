using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Interpreter;
using Irony.Interpreter.Evaluator; 

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
  public class EvaluatorTests {

    [TestMethod]
    public void TestEvaluator_Ops() {
      var eval = new ExpressionEvaluator();
      string script; 
      object result; 

      //Simple computation
      script = "2*3";
      result = eval.Evaluate(script);
      Assert.AreEqual(6, result, "Unexpected computation result"); 

      //Using variables
      script = @"
x=2
y=4
x * y
";
      result = eval.Evaluate(script);
      Assert.AreEqual(8, result, "Unexpected computation result");

      //Operator precedence
      script = @"
x=2
y=3
x + y * 5
";
      result = eval.Evaluate(script);
      Assert.AreEqual(17, result, "Unexpected computation result"); 
 
      //parenthesis
      script = @"
x=3
y=2
1 + (x - y) * 5
";
      result = eval.Evaluate(script);
      Assert.AreEqual(6, result, "Unexpected computation result"); 

      //strings
      script = @"
x='2'
y='3'
x + y + 4
";
      result = eval.Evaluate(script);
      Assert.AreEqual("234", result, "Unexpected computation result"); 

      //string with embedded expressions
      script = @"
x = 4
y = 7 
'#{x} * #{y} = #{x * y}'
";
      result = eval.Evaluate(script);
      Assert.AreEqual("4 * 7 = 28", result, "Unexpected computation result");

      //various operators
      script = @"
x = 1 + 2 * 3      # =7
y = --x            # = 6
z =  x * 1.5       # = 9
z -= y             # = 3   
";
      result = eval.Evaluate(script);
      Assert.AreEqual(3.0, (double) result, 0.0001, "Unexpected computation result");

      //&&, || operators
      script = @"x = (1 > 0) || (1/0)";
      result = eval.Evaluate(script);
      Assert.AreEqual(true, result, "Unexpected computation result");

      //Operator precedence test
      script = @"2+3*3*3";
      result = eval.Evaluate(script);
      Assert.AreEqual(29, result, "Operator precedence test failed.");

      script = @"x = (1 < 0) && (1/0)";
      result = eval.Evaluate(script);
      Assert.AreEqual(false, result, "Unexpected computation result"); 
    }

    [TestMethod]
    public void TestEvaluator_BuiltIns() {
      var eval = new ExpressionEvaluator();
      string script;
      object result;

      //Using methods imported from System.Math class
      script = @"abs(-1.0) + Log10(100.0) + sqrt(9) + floor(4.5) + sin(PI/2)";
      result = eval.Evaluate(script);
      Assert.IsTrue(result is double, "Result is not double.");
      Assert.AreEqual(11.0, (double) result, 0.001, "Unexpected computation result");

      //Using methods imported from System.Environment
      script = @"report = '#{MachineName}-#{OSVersion}-#{UserName}'";
      result = eval.Evaluate(script);
      var expected = string.Format("{0}-{1}-{2}", Environment.MachineName, Environment.OSVersion, Environment.UserName); 
      Assert.AreEqual(expected, result, "Unexpected computation result");

      //Using special built-in methods print and format
      eval.ClearOutput(); 
      script = @"print(format('{0} * {1} = {2}', 3, 4, 3 * 4))";
      eval.Evaluate(script);
      result = eval.GetOutput(); 
      Assert.AreEqual("3 * 4 = 12\r\n", result, "Unexpected computation result");

      //Add custom built-in method SayHello and test it
      eval.Runtime.BuiltIns.AddMethod(SayHello, "SayHello", 1, 1, "name"); 
      script = @"SayHello('John')";
      result = eval.Evaluate(script);
      Assert.AreEqual("Hello, John!", result, "Unexpected computation result");
    }

    //custom built-in method added to evaluator in Built-in tests
    public static string SayHello(ScriptThread thread, object[] args) {
      return "Hello, " + args[0] + "!";
    }

    [TestMethod]
    public void TestEvaluator_Iif() {
      var eval = new ExpressionEvaluator();
      string script;
      object result;

      //Test '? :' operator
      script = @"1 < 0 ? 1/0 : 'false' "; // Notice that (1/0) is not evaluated
      result = eval.Evaluate(script);
      Assert.AreEqual("false", result, "Unexpected computation result");

      //Test iif special form
      script = @"iif(1 > 0, 'true', 1/0) "; //Notice that (1/0) is not evaluated
      result = eval.Evaluate(script);
      Assert.AreEqual("true", result, "Unexpected computation result");
    }

    [TestMethod]
    public void TestEvaluator_MemberAccess() {
      var eval = new ExpressionEvaluator();
      eval.Globals["foo"] = new Foo(); 
      string script;
      object result;

      //Test access to field, prop, calling a method
      script = @"foo.Field + ',' + foo.Prop + ',' + foo.GetStuff()";
      result = eval.Evaluate(script);
      Assert.AreEqual("F,P,S", result, "Unexpected computation result");

      script = @"
foo.Field = 'FF'
foo.Prop = 'PP'
R = foo.Field + foo.Prop ";
      result = eval.Evaluate(script);
      Assert.AreEqual("FFPP", result, "Unexpected computation result");

      //Test access to indexed properties
      script = @"foo[3]";
      result = eval.Evaluate(script);
      Assert.AreEqual("#3", result, "Unexpected computation result");

      script = @"foo['a']";
      result = eval.Evaluate(script);
      Assert.AreEqual("V-a", result, "Unexpected computation result");

      // Test with string literal
      script = @" '0123'.Substring(1) + 'abcd'.Length    ";
      result = eval.Evaluate(script);
      Assert.AreEqual("1234", result, "Unexpected computation result");
    }

    //A class used for member access testing
    public class Foo {
      public string Field = "F";
      public string Prop { get; set; }

      public Foo() {
        Prop = "P";
      }
      public string GetStuff() {
        return "S";
      }
      public string this[int i] {
        get { return "#" + i; }
        set { }
      }
      public string this[string key] {
        get { return "V-" + key; }
        set { }
      }
    }

    [TestMethod]
    public void TestEvaluator_ArrayDictDataRow() {
      var eval = new ExpressionEvaluator();
      //Create an array, a dictionary and a data row and add them to Globals
      eval.Globals["primes"] = new int[] { 3, 5, 7, 11, 13 };
      var nums = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
      nums["one"] = "1";
      nums["two"] = "2";
      nums["three"] = "3";
      eval.Globals["nums"] = nums;
      var t = new System.Data.DataTable();
      t.Columns.Add("Name", typeof(string));
      t.Columns.Add("Age", typeof(int));
      var row = t.NewRow();
      row["Name"] = "John";
      row["Age"] = 30;
      eval.Globals["row"] = row;        
      
      string script;
      object result;

      //Test array
      script = @"primes[3]";
      result = eval.Evaluate(script);
      Assert.AreEqual(11, result, "Unexpected computation result");
      script = @"
primes[3] = 12345
primes[3]";
      result = eval.Evaluate(script);
      Assert.AreEqual(12345, result, "Unexpected computation result");

      //Test dict
      script = @"nums['three'] + nums['two'] + nums['one']";
      result = eval.Evaluate(script);
      Assert.AreEqual("321", result, "Unexpected computation result");
      script = @"
nums['two'] = '22'
nums['three'] + nums['two'] + nums['one']
";
      result = eval.Evaluate(script);
      Assert.AreEqual("3221", result, "Unexpected computation result");

      //Test data row
      script = @"row['Name'] + ', ' + row['age']";
      result = eval.Evaluate(script);
      Assert.AreEqual("John, 30", result, "Unexpected computation result");
      script = @"
row['Name'] = 'Jon'
row['Name'] + ', ' + row['age']";
      result = eval.Evaluate(script);
      Assert.AreEqual("Jon, 30", result, "Unexpected computation result");
    }


  }//class
}//namespace
