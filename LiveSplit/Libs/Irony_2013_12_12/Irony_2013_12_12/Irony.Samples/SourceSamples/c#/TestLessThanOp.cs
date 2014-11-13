//Simple test for "<" operator, to check it is properly distinguished from "<" in type argument.
namespace Test {
  public class TestClass {
    public void DoStuff(int arg) {
      List<T> x;
      List<Dictionary<string, object[,]>> genericVar; 
      if (myGenType<T>.Prop < arg)
        arg = 5;
    }
  }//class
}//namespace