using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Interpreter {
  public static class Util {
    public static string SafeFormat(this string template, params object[] args) {
      if (args == null || args.Length == 0) return template;
      try { 
        template = string.Format(template, args); 
      }  catch (Exception ex) {
          template = template + "(message formatting failed: " + ex.Message + " Args: " + string.Join(",", args) + ")"; 
      }
      return template;
    }//method

    public static void Check(bool condition, string messageTemplate, params object[] args) {
      if (condition) return; 
      throw new Exception(messageTemplate.SafeFormat(args));
    }

  }//class
}
