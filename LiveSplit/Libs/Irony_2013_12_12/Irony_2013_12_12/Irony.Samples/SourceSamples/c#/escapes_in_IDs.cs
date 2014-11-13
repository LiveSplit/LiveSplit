//example from c# spec, page 93
namespace Sample {
  class @class {
    public static void @static(bool @bool) {
      if (@bool)
        System.Console.WriteLine("true");
      else
        System.Console.WriteLine("false");
    }
  }
  class Class1 {
    static void M() {
      cl\u0061ss.st\u0061tic(true);
    }
  }

}

/*  code comment from spec:
[the code]  defines a class named 'class' with a static method named 'static' that takes a parameter named 
'bool'. Note that since Unicode escapes are not permitted in keywords, the token 'cl\u0061ss' is an 
identifier, and is the same identifier as '@class'. end example]
*/