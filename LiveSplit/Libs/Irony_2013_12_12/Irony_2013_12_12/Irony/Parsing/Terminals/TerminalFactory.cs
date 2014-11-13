#region License
/* **********************************************************************************
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion
//Authors: Roman Ivantsov, Philipp Serr

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Irony.Parsing {
  public static class TerminalFactory {

    public static StringLiteral CreateCSharpString(string name) {
      StringLiteral term = new StringLiteral(name, "\"", StringOptions.AllowsAllEscapes);
      term.AddPrefix("@", StringOptions.NoEscapes | StringOptions.AllowsLineBreak | StringOptions.AllowsDoubledQuote);
      return term;
    }
    public static StringLiteral CreateCSharpChar(string name) {
      StringLiteral term = new StringLiteral(name, "'", StringOptions.IsChar);
      return term;
    }

    public static StringLiteral CreateVbString(string name) {
      StringLiteral term = new StringLiteral(name);
      term.AddStartEnd("\"", StringOptions.NoEscapes | StringOptions.AllowsDoubledQuote);
      term.AddSuffix("$", TypeCode.String);
      term.AddSuffix("c", TypeCode.Char);
      return term;
    }

    public static StringLiteral CreatePythonString(string name) {
      StringLiteral term = new StringLiteral(name);
      term.AddStartEnd("'", StringOptions.AllowsAllEscapes);
      term.AddStartEnd("'''", StringOptions.AllowsAllEscapes | StringOptions.AllowsLineBreak);
      term.AddStartEnd("\"", StringOptions.AllowsAllEscapes);
      term.AddStartEnd("\"\"\"", StringOptions.AllowsAllEscapes | StringOptions.AllowsLineBreak);

      term.AddPrefix("u", StringOptions.AllowsAllEscapes);
      term.AddPrefix("r", StringOptions.NoEscapes );
      term.AddPrefix("ur", StringOptions.NoEscapes);
 
      return term;
    }

		//http://www.ecma-international.org/publications/files/ECMA-ST/Ecma-334.pdf section 9.4.4
    public static NumberLiteral CreateCSharpNumber(string name) {
      NumberLiteral term = new NumberLiteral(name);
      term.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64 };
      term.DefaultFloatType = TypeCode.Double;
      term.AddPrefix("0x", NumberOptions.Hex);
      term.AddSuffix("u", TypeCode.UInt32, TypeCode.UInt64);
      term.AddSuffix("l", TypeCode.Int64, TypeCode.UInt64);
      term.AddSuffix("ul", TypeCode.UInt64);
      term.AddSuffix("f", TypeCode.Single);
			term.AddSuffix("d", TypeCode.Double);
      term.AddSuffix("m", TypeCode.Decimal);
      return term;
    }
    //http://www.microsoft.com/downloads/details.aspx?FamilyId=6D50D709-EAA4-44D7-8AF3-E14280403E6E&displaylang=en section 2
    public static NumberLiteral CreateVbNumber(string name) {
      NumberLiteral term = new NumberLiteral(name);
      term.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64 };
      //term.DefaultFloatType = TypeCode.Double; it is default
      term.AddPrefix("&H", NumberOptions.Hex);
      term.AddPrefix("&O", NumberOptions.Octal);
      term.AddSuffix("S", TypeCode.Int16);
      term.AddSuffix("I", TypeCode.Int32);
      term.AddSuffix("%", TypeCode.Int32);
      term.AddSuffix("L", TypeCode.Int64);
      term.AddSuffix("&", TypeCode.Int64);
      term.AddSuffix("D", TypeCode.Decimal);
      term.AddSuffix("@", TypeCode.Decimal);
      term.AddSuffix("F", TypeCode.Single);
      term.AddSuffix("!", TypeCode.Single);
      term.AddSuffix("R", TypeCode.Double);
      term.AddSuffix("#", TypeCode.Double);
      term.AddSuffix("US", TypeCode.UInt16);
      term.AddSuffix("UI", TypeCode.UInt32);
      term.AddSuffix("UL", TypeCode.UInt64);
      return term;
    }
    //http://docs.python.org/ref/numbers.html
    public static NumberLiteral CreatePythonNumber(string name) {
      NumberLiteral term = new NumberLiteral(name, NumberOptions.AllowStartEndDot);
      //default int types are Integer (32bit) -> LongInteger (BigInt); Try Int64 before BigInt: Better performance?
      term.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
      // term.DefaultFloatType = TypeCode.Double; -- it is default
      //float type is implementation specific, thus try decimal first (higher precision)
      //term.DefaultFloatTypes = new TypeCode[] { TypeCode.Decimal, TypeCode.Double };
      term.AddPrefix("0x", NumberOptions.Hex);
      term.AddPrefix("0", NumberOptions.Octal);
      term.AddSuffix("L", TypeCode.Int64, NumberLiteral.TypeCodeBigInt);
      term.AddSuffix("J", NumberLiteral.TypeCodeImaginary);
      return term;
    }

    // About exponent symbols, extract from R6RS:
    //  ... representations of number objects may be written with an exponent marker that indicates the desired precision 
    // of the inexact representation. The letters s, f, d, and l specify the use of short, single, double, and long precision, respectively.
    // ...
    // In addition, the exponent marker e specifies the default precision for the implementation. The default precision 
    //  has at least as much precision as double, but implementations may wish to allow this default to be set by the user.
    public static NumberLiteral CreateSchemeNumber(string name) {
      NumberLiteral term = new NumberLiteral(name);
      term.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
      term.DefaultFloatType = TypeCode.Double; // it is default
      term.AddExponentSymbols("eE", TypeCode.Double); //default precision for platform, double 
      term.AddExponentSymbols("sSfF", TypeCode.Single); 
      term.AddExponentSymbols("dDlL", TypeCode.Double); 
      term.AddPrefix("#b", NumberOptions.Binary);
      term.AddPrefix("#o", NumberOptions.Octal);
      term.AddPrefix("#x", NumberOptions.Hex);
      term.AddPrefix("#d", NumberOptions.None);
      term.AddPrefix("#i", NumberOptions.None); // inexact prefix, has no effect
      term.AddPrefix("#e", NumberOptions.None); // exact prefix, has no effect
      term.AddSuffix("J", NumberLiteral.TypeCodeImaginary);
      return term;
    }


    public static IdentifierTerminal CreateCSharpIdentifier(string name) {
      IdentifierTerminal id = new IdentifierTerminal(name, IdOptions.AllowsEscapes | IdOptions.CanStartWithEscape);
      id.AddPrefix("@", IdOptions.IsNotKeyword);
      //From spec:
      //Start char is "_" or letter-character, which is a Unicode character of classes Lu, Ll, Lt, Lm, Lo, or Nl 
      id.StartCharCategories.AddRange(new UnicodeCategory[] {
         UnicodeCategory.UppercaseLetter, //Ul
         UnicodeCategory.LowercaseLetter, //Ll
         UnicodeCategory.TitlecaseLetter, //Lt
         UnicodeCategory.ModifierLetter,  //Lm
         UnicodeCategory.OtherLetter,     //Lo
         UnicodeCategory.LetterNumber     //Nl
      });
      //Internal chars
      /* From spec:
      identifier-part-character: letter-character | decimal-digit-character | connecting-character |  combining-character |
          formatting-character
*/
      id.CharCategories.AddRange(id.StartCharCategories); //letter-character categories
      id.CharCategories.AddRange(new UnicodeCategory[] {
        UnicodeCategory.DecimalDigitNumber, //Nd
        UnicodeCategory.ConnectorPunctuation, //Pc
        UnicodeCategory.SpacingCombiningMark, //Mc
        UnicodeCategory.NonSpacingMark,       //Mn
        UnicodeCategory.Format                //Cf
      });
      //Chars to remove from final identifier
      id.CharsToRemoveCategories.Add(UnicodeCategory.Format);
      return id;
    }

    public static IdentifierTerminal CreatePythonIdentifier(string name) {
      IdentifierTerminal id = new IdentifierTerminal("Identifier"); //defaults are OK
      return id;
    }

    //Covers simple identifiers like abcd, and also quoted versions: [abc d], "abc d".
    public static IdentifierTerminal CreateSqlExtIdentifier(Grammar grammar, string name) {
      var id = new IdentifierTerminal(name);
      StringLiteral term = new StringLiteral(name + "_qouted");
      term.AddStartEnd("[", "]", StringOptions.NoEscapes);
      term.AddStartEnd("\"", StringOptions.NoEscapes);
      term.SetOutputTerminal(grammar, id); //term will be added to NonGrammarTerminals automatically 
      return id;
    }

  }//class
}//namespace
