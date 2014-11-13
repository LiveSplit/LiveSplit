#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion
//Authors: Roman Ivantsov - initial implementation and some later edits
//         Philipp Serr - implementation of advanced features for c#, python, VB

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Diagnostics;
using Irony.Ast;

namespace Irony.Parsing {
  using BigInteger = System.Numerics.BigInteger; //Microsoft.Scripting.Math.BigInteger;
  using Complex64 = System.Numerics.Complex;
  using Irony.Ast; // Microsoft.Scripting.Math.Complex64;

  [Flags]
  public enum NumberOptions {
    None = 0,
    Default = None,

    AllowStartEndDot  = 0x01,     //python : http://docs.python.org/ref/floating.html
    IntOnly           = 0x02,
    NoDotAfterInt     = 0x04,     //for use with IntOnly flag; essentially tells terminal to avoid matching integer if 
                                  // it is followed by dot (or exp symbol) - leave to another terminal that will handle float numbers
    AllowSign         = 0x08,
    DisableQuickParse = 0x10,
    AllowLetterAfter  = 0x20,      // allow number be followed by a letter or underscore; by default this flag is not set, so "3a" would not be 
                                   //  recognized as number followed by an identifier
    AllowUnderscore   = 0x40,      // Ruby allows underscore inside number: 1_234

    //The following should be used with base-identifying prefixes
    Binary = 0x0100, //e.g. GNU GCC C Extension supports binary number literals
    Octal =  0x0200,
    Hex =    0x0400,
  }


  public class NumberLiteral : CompoundTerminalBase {
    
    //Flags for internal use
    public enum NumberFlagsInternal : short {
      HasDot = 0x1000,
      HasExp = 0x2000,
    }
    //nested helper class
    public class ExponentsTable : Dictionary<char, TypeCode> { }

    #region Public Consts
    //currently using TypeCodes for identifying numeric types
    public const TypeCode TypeCodeBigInt = (TypeCode)30;
    public const TypeCode TypeCodeImaginary = (TypeCode)31;
    #endregion

    #region constructors and initialization
    public NumberLiteral(string name) : this(name, NumberOptions.Default) {
    }
    public NumberLiteral(string name, NumberOptions options, Type astNodeType)  : this(name, options) {
      base.AstConfig.NodeType = astNodeType;
    }
    public NumberLiteral(string name, NumberOptions options, AstNodeCreator astNodeCreator)  : this(name, options) {
      base.AstConfig.NodeCreator = astNodeCreator;
    }
    public NumberLiteral(string name, NumberOptions options) : base(name) {
      Options = options;
      base.SetFlag(TermFlags.IsLiteral);
    }
    public void AddPrefix(string prefix, NumberOptions options) {
      PrefixFlags.Add(prefix, (short) options);
      Prefixes.Add(prefix);
    }
    public void AddExponentSymbols(string symbols, TypeCode floatType) {
      foreach(var exp in symbols)
        _exponentsTable[exp] = floatType;
    }
    #endregion

    #region Public fields/properties: ExponentSymbols, Suffixes
    public NumberOptions Options;
    public char DecimalSeparator = '.';

    //Default types are assigned to literals without suffixes; first matching type used
    public TypeCode[] DefaultIntTypes = new TypeCode[] { TypeCode.Int32 };
    public TypeCode DefaultFloatType = TypeCode.Double;
    private ExponentsTable _exponentsTable = new ExponentsTable(); 

    public bool IsSet(NumberOptions option) {
      return (Options & option) != 0;
    }
    #endregion

    #region Private fields: _quickParseTerminators
    #endregion

    #region overrides
    public override void Init(GrammarData grammarData) {
      base.Init(grammarData);
      //Default Exponent symbols if table is empty 
      if(_exponentsTable.Count == 0 && !IsSet(NumberOptions.IntOnly)) {
        _exponentsTable['e'] = DefaultFloatType;
        _exponentsTable['E'] = DefaultFloatType;
      }
      if (this.EditorInfo == null) 
        this.EditorInfo = new TokenEditorInfo(TokenType.Literal, TokenColor.Number, TokenTriggers.None);
    }

    public override IList<string> GetFirsts() {
      StringList result = new StringList();
      result.AddRange(base.Prefixes);
      //we assume that prefix is always optional, so number can always start with plain digit
      result.AddRange(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
      // Python float numbers can start with a dot
      if (IsSet(NumberOptions.AllowStartEndDot))
        result.Add(DecimalSeparator.ToString());
      if (IsSet(NumberOptions.AllowSign))
        result.AddRange(new string[] {"-", "+"} );
      return result;
    }

    //Most numbers in source programs are just one-digit instances of 0, 1, 2, and maybe others until 9
    // so we try to do a quick parse for these, without starting the whole general process
    protected override Token QuickParse(ParsingContext context, ISourceStream source) {
      if (IsSet(NumberOptions.DisableQuickParse)) return null;
      char current = source.PreviewChar;
      //it must be a digit followed by a whitespace or delimiter
      if (!char.IsDigit(current))  return null;
      if (!Grammar.IsWhitespaceOrDelimiter(source.NextPreviewChar))
        return null; 
      int iValue = current - '0';
      object value = null;
      switch (DefaultIntTypes[0]) {
        case TypeCode.Int32: value = iValue; break;
        case TypeCode.UInt32: value = (UInt32)iValue; break;
        case TypeCode.Byte: value = (byte)iValue; break;
        case TypeCode.SByte: value = (sbyte) iValue; break;
        case TypeCode.Int16: value = (Int16)iValue; break;
        case TypeCode.UInt16: value = (UInt16)iValue; break;
        default: return null; 
      }
      source.PreviewPosition++;
      return source.CreateToken(this.OutputTerminal, value);
    }

    protected override void InitDetails(ParsingContext context, CompoundTokenDetails details) {
      base.InitDetails(context, details);
      details.Flags = (short) this.Options;
    }

    protected override void ReadPrefix(ISourceStream source, CompoundTokenDetails details) {
      //check that is not a  0 followed by dot; 
      //this may happen in Python for number "0.123" - we can mistakenly take "0" as octal prefix
      if (source.PreviewChar == '0' && source.NextPreviewChar == '.') return;
      base.ReadPrefix(source, details);
    }//method

    protected override bool ReadBody(ISourceStream source, CompoundTokenDetails details) {
      //remember start - it may be different from source.TokenStart, we may have skipped prefix
      int start = source.PreviewPosition;
      char current = source.PreviewChar;
      if (IsSet(NumberOptions.AllowSign) && (current == '-' || current == '+')) {
        details.Sign = current.ToString();
        source.PreviewPosition++;
      }
      //Figure out digits set
      string digits = GetDigits(details);
      bool isDecimal = !details.IsSet((short) (NumberOptions.Binary | NumberOptions.Octal | NumberOptions.Hex));
      bool allowFloat = !IsSet(NumberOptions.IntOnly);
      bool foundDigits = false;

      while (!source.EOF()) {
        current = source.PreviewChar;
        //1. If it is a digit, just continue going; the same for '_' if it is allowed
        if (digits.IndexOf(current) >= 0 || IsSet(NumberOptions.AllowUnderscore) && current == '_') {
          source.PreviewPosition++;
          foundDigits = true; 
          continue;
        }
        //2. Check if it is a dot in float number
        bool isDot = current == DecimalSeparator;
        if (allowFloat && isDot) {
          //If we had seen already a dot or exponent, don't accept this one;
          bool hasDotOrExp = details.IsSet((short) (NumberFlagsInternal.HasDot | NumberFlagsInternal.HasExp));
          if (hasDotOrExp) break; //from while loop
          //In python number literals (NumberAllowPointFloat) a point can be the first and last character,
          //We accept dot only if it is followed by a digit
          if (digits.IndexOf(source.NextPreviewChar) < 0 && !IsSet(NumberOptions.AllowStartEndDot))
            break; //from while loop
          details.Flags |= (int) NumberFlagsInternal.HasDot;
          source.PreviewPosition++;
          continue;
        }
        //3. Check if it is int number followed by dot or exp symbol
        bool isExpSymbol = (details.ExponentSymbol == null) && _exponentsTable.ContainsKey(current);
        if (!allowFloat && foundDigits && (isDot || isExpSymbol)) {
          //If no partial float allowed then return false - it is not integer, let float terminal recognize it as float
          if (IsSet(NumberOptions.NoDotAfterInt)) return false;  
          //otherwise break, it is integer and we're done reading digits
          break;
        }


        //4. Only for decimals - check if it is (the first) exponent symbol
        if (allowFloat && isDecimal && isExpSymbol) {
          char next = source.NextPreviewChar;
          bool nextIsSign = next == '-' || next == '+';
          bool nextIsDigit = digits.IndexOf(next) >= 0;
          if (!nextIsSign && !nextIsDigit)
            break;  //Exponent should be followed by either sign or digit
          //ok, we've got real exponent
          details.ExponentSymbol = current.ToString(); //remember the exp char
          details.Flags |= (int) NumberFlagsInternal.HasExp;
          source.PreviewPosition++;
          if (nextIsSign)
            source.PreviewPosition++; //skip +/- explicitly so we don't have to deal with them on the next iteration
          continue;
        }
        //4. It is something else (not digit, not dot or exponent) - we're done
        break; //from while loop
      }//while
      int end = source.PreviewPosition;
      if (!foundDigits) 
        return false; 
      details.Body = source.Text.Substring(start, end - start);
      return true;
    }

    protected internal override void OnValidateToken(ParsingContext context) {
      if (!IsSet(NumberOptions.AllowLetterAfter)) {
        var current = context.Source.PreviewChar;
        if(char.IsLetter(current) || current == '_') {
          context.CurrentToken = context.CreateErrorToken(Resources.ErrNoLetterAfterNum); // "Number cannot be followed by a letter." 
        }
      }
      base.OnValidateToken(context);
    }

    protected override bool ConvertValue(CompoundTokenDetails details) {
      if (String.IsNullOrEmpty(details.Body)) {
        details.Error = Resources.ErrInvNumber;  // "Invalid number.";
        return false;
      }
      AssignTypeCodes(details); 
      //check for underscore
      if (IsSet(NumberOptions.AllowUnderscore) && details.Body.Contains("_"))
        details.Body = details.Body.Replace("_", string.Empty); 

      //Try quick paths
      switch (details.TypeCodes[0]) {
        case TypeCode.Int32: 
          if (QuickConvertToInt32(details)) return true;
          break;
        case TypeCode.Double:
          if (QuickConvertToDouble(details)) return true;
          break;
      }

      //Go full cycle
      details.Value = null;
      foreach (TypeCode typeCode in details.TypeCodes) {
        switch (typeCode) {
          case TypeCode.Single:   case TypeCode.Double:  case TypeCode.Decimal:  case TypeCodeImaginary:
            return ConvertToFloat(typeCode, details);
          case TypeCode.SByte:    case TypeCode.Byte:    case TypeCode.Int16:    case TypeCode.UInt16:
          case TypeCode.Int32:    case TypeCode.UInt32:  case TypeCode.Int64:    case TypeCode.UInt64:
            if (details.Value == null) //if it is not done yet
              TryConvertToLong(details, typeCode == TypeCode.UInt64); //try to convert to Long/Ulong and place the result into details.Value field;
            if(TryCastToIntegerType(typeCode, details)) //now try to cast the ULong value to the target type 
              return true;
            break;
          case TypeCodeBigInt:
            if (ConvertToBigInteger(details)) return true;
            break; 
        }//switch
      }
      return false; 
    }//method

    private void AssignTypeCodes(CompoundTokenDetails details) {
      //Type could be assigned when we read suffix; if so, just exit
      if (details.TypeCodes != null) return; 
      //Decide on float types
      var hasDot = details.IsSet((short)(NumberFlagsInternal.HasDot));
      var hasExp = details.IsSet((short)(NumberFlagsInternal.HasExp));
      var isFloat = (hasDot || hasExp); 
      if (!isFloat) {  
        details.TypeCodes = DefaultIntTypes;
        return; 
      }
      //so we have a float. If we have exponent symbol then use it to select type
      if (hasExp) {
        TypeCode code;
        if (_exponentsTable.TryGetValue(details.ExponentSymbol[0], out code)) {
          details.TypeCodes = new TypeCode[] {code};
          return; 
        }
      }//if hasExp
      //Finally assign default float type
      details.TypeCodes = new TypeCode[] {DefaultFloatType};
    }

    #endregion

    #region private utilities
    private bool QuickConvertToInt32(CompoundTokenDetails details) {
      int radix = GetRadix(details);
      if (radix == 10 && details.Body.Length > 10) return false;    //10 digits is maximum for int32; int32.MaxValue = 2 147 483 647
      try {
        //workaround for .Net FX bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=278448
        int iValue = 0;
        if (radix == 10)
          iValue =  Convert.ToInt32(details.Body, CultureInfo.InvariantCulture);
        else
          iValue = Convert.ToInt32(details.Body, radix);
        details.Value = iValue;
        return true;
      } catch {
        return false;
      }
    }//method

    private bool QuickConvertToDouble(CompoundTokenDetails details) {
      if (details.IsSet((short)(NumberOptions.Binary | NumberOptions.Octal | NumberOptions.Hex))) return false; 
      if (details.IsSet((short)(NumberFlagsInternal.HasExp))) return false; 
      if (DecimalSeparator != '.') return false;
      double dvalue;
      if (!double.TryParse(details.Body, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dvalue)) return false;
      details.Value = dvalue;
      return true; 
    }


    private bool ConvertToFloat(TypeCode typeCode, CompoundTokenDetails details) {
      //only decimal numbers can be fractions
      if (details.IsSet((short)(NumberOptions.Binary | NumberOptions.Octal | NumberOptions.Hex))) {
        details.Error = Resources.ErrInvNumber; //  "Invalid number.";
        return false;
      }
      string body = details.Body;
      //Some languages allow exp symbols other than E. Check if it is the case, and change it to E
      // - otherwise .NET conversion methods may fail
      if (details.IsSet((short)NumberFlagsInternal.HasExp) && details.ExponentSymbol.ToUpper() != "E")
        body = body.Replace(details.ExponentSymbol, "E");

      //'.' decimal seperator required by invariant culture
      if (details.IsSet((short)NumberFlagsInternal.HasDot) && DecimalSeparator != '.')
        body = body.Replace(DecimalSeparator, '.');

      switch (typeCode) {
        case TypeCode.Double:
        case TypeCodeImaginary:
          double dValue;
          if (!Double.TryParse(body, NumberStyles.Float, CultureInfo.InvariantCulture, out dValue)) return false;
          if (typeCode == TypeCodeImaginary)
            details.Value = new Complex64(0, dValue);
          else
            details.Value = dValue; 
          return true;
        case TypeCode.Single:
          float fValue;
          if (!Single.TryParse(body, NumberStyles.Float, CultureInfo.InvariantCulture, out fValue)) return false;
          details.Value = fValue;
          return true; 
        case TypeCode.Decimal:
          decimal decValue;
          if (!Decimal.TryParse(body, NumberStyles.Float, CultureInfo.InvariantCulture, out decValue)) return false;
          details.Value = decValue;
          return true;  
      }//switch
      return false; 
    }
    private bool TryCastToIntegerType(TypeCode typeCode, CompoundTokenDetails details) {
      if (details.Value == null) return false;
      try {
        if (typeCode != TypeCode.UInt64)
          details.Value = Convert.ChangeType(details.Value, typeCode, CultureInfo.InvariantCulture);
        return true;
      } catch (Exception) {
        details.Error = string.Format(Resources.ErrCannotConvertValueToType, details.Value, typeCode.ToString());
        return false;
      }
    }//method

    private bool TryConvertToLong(CompoundTokenDetails details, bool useULong) {
      try {
        int radix = GetRadix(details);
        //workaround for .Net FX bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=278448
        if (radix == 10)
            if (useULong)
              details.Value = Convert.ToUInt64(details.Body, CultureInfo.InvariantCulture);
            else
              details.Value = Convert.ToInt64(details.Body, CultureInfo.InvariantCulture);
        else
            if (useULong)
              details.Value = Convert.ToUInt64(details.Body, radix);
            else
              details.Value = Convert.ToInt64(details.Body, radix);
        return true; 
      } catch(OverflowException) {
        details.Error = string.Format(Resources.ErrCannotConvertValueToType, details.Value, TypeCode.Int64.ToString());
        return false;
      }
    }

    private bool ConvertToBigInteger(CompoundTokenDetails details) {
      //ignore leading zeros and sign
      details.Body = details.Body.TrimStart('+').TrimStart('-').TrimStart('0');
      if (string.IsNullOrEmpty(details.Body))
        details.Body = "0"; 
      int bodyLength = details.Body.Length;
      int radix = GetRadix(details);
      int wordLength = GetSafeWordLength(details);
      int sectionCount = GetSectionCount(bodyLength, wordLength);
      ulong[] numberSections = new ulong[sectionCount]; //big endian

      try {
        int startIndex = details.Body.Length - wordLength;
        for (int sectionIndex = sectionCount - 1; sectionIndex >= 0; sectionIndex--) {
          if (startIndex < 0) {
            wordLength += startIndex;
            startIndex = 0;
          }
          //workaround for .Net FX bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=278448
          if (radix == 10)
            numberSections[sectionIndex] = Convert.ToUInt64(details.Body.Substring(startIndex, wordLength));
          else
            numberSections[sectionIndex] = Convert.ToUInt64(details.Body.Substring(startIndex, wordLength), radix);

          startIndex -= wordLength;
        }
      } catch {
        details.Error = Resources.ErrInvNumber;//  "Invalid number.";
        return false;
      }
      //produce big integer
      ulong safeWordRadix = GetSafeWordRadix(details);
      BigInteger bigIntegerValue = numberSections[0];
      for (int i = 1; i < sectionCount; i++)
        bigIntegerValue = checked(bigIntegerValue * safeWordRadix + numberSections[i]);
      if (details.Sign == "-")
        bigIntegerValue = -bigIntegerValue;
      details.Value = bigIntegerValue;
      return true;
    }

    private int GetRadix(CompoundTokenDetails details) {
      if (details.IsSet((short)NumberOptions.Hex))
        return 16;
      if (details.IsSet((short)NumberOptions.Octal))
        return 8;
      if (details.IsSet((short)NumberOptions.Binary))
        return 2;
      return 10;
    }
    private string GetDigits(CompoundTokenDetails details) {
      if (details.IsSet((short)NumberOptions.Hex))
        return Strings.HexDigits;
      if (details.IsSet((short)NumberOptions.Octal))
        return Strings.OctalDigits;
      if (details.IsSet((short)NumberOptions.Binary))
        return Strings.BinaryDigits;
      return Strings.DecimalDigits;
    }
    private int GetSafeWordLength(CompoundTokenDetails details) {
      if (details.IsSet((short)NumberOptions.Hex))
        return 15;
      if (details.IsSet((short)NumberOptions.Octal))
        return 21; //maxWordLength 22
      if (details.IsSet((short)NumberOptions.Binary))
        return 63;
      return 19; //maxWordLength 20
    }
    private int GetSectionCount(int stringLength, int safeWordLength) {
      int quotient = stringLength / safeWordLength;
      int remainder = stringLength - quotient * safeWordLength;
      return remainder == 0 ? quotient : quotient + 1;
    }

    //radix^safeWordLength
    private ulong GetSafeWordRadix(CompoundTokenDetails details) {
      if (details.IsSet((short)NumberOptions.Hex))
        return 1152921504606846976;
      if (details.IsSet((short)NumberOptions.Octal))
        return 9223372036854775808;
      if (details.IsSet((short) NumberOptions.Binary))
        return 9223372036854775808;
      return 10000000000000000000;
    }
    private static bool IsIntegerCode(TypeCode code) {
      return (code >= TypeCode.SByte && code <= TypeCode.UInt64);
    }
    #endregion


  }//class


}
