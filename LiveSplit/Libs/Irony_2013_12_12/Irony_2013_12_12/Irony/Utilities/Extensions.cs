using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Parsing {
  public static class ParsingEnumExtensions {

    public static bool IsSet(this TermFlags flags, TermFlags flag) {
      return (flags & flag) != 0;
    }
    public static bool IsSet(this LanguageFlags flags, LanguageFlags flag) {
      return (flags & flag) != 0;
    }
    public static bool IsSet(this ParseOptions options, ParseOptions option) {
      return (options & option) != 0;
    }
    public static bool IsSet(this TermListOptions options, TermListOptions option) {
      return (options & option) != 0;
    }
    public static bool IsSet(this ProductionFlags flags, ProductionFlags flag) {
      return (flags & flag) != 0;
    }
  }//class

}
