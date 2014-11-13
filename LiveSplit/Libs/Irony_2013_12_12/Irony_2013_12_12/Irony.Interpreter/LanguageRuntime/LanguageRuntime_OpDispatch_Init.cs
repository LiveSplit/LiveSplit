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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Numerics;
using System.Diagnostics;
using Irony.Parsing;

namespace Irony.Interpreter { 

  //Initialization of Runtime
  public partial class LanguageRuntime {
    private static ExpressionType[] _overflowOperators = new ExpressionType[] { 
       ExpressionType.Add, ExpressionType.AddChecked, ExpressionType.Subtract, ExpressionType.SubtractChecked, 
       ExpressionType.Multiply, ExpressionType.MultiplyChecked, ExpressionType.Power};
    
    // Smart boxing: boxes for a bunch of integers are preallocated
    private object[] _boxes = new object[4096];
    private const int _boxesMiddle = 2048;
    // Note: ran some primitive tests, and it appears that use of smart boxing makes it slower 
    //  by about 5-10%; so disabling it for now
    public bool SmartBoxingEnabled = false;
    bool _supportsComplex;
    bool _supportsBigInt;
    bool _supportsRational;


    protected virtual void InitOperatorImplementations() {
      _supportsComplex = this.Language.Grammar.LanguageFlags.IsSet(LanguageFlags.SupportsComplex);
      _supportsBigInt = this.Language.Grammar.LanguageFlags.IsSet(LanguageFlags.SupportsBigInt);
      _supportsRational = this.Language.Grammar.LanguageFlags.IsSet(LanguageFlags.SupportsRational);
      // TODO: add support for Rational
      if (SmartBoxingEnabled)
        InitBoxes();
      InitTypeConverters();
      InitBinaryOperatorImplementationsForMatchedTypes();
      InitUnaryOperatorImplementations();
      CreateBinaryOperatorImplementationsForMismatchedTypes();
      CreateOverflowHandlers();
    }

    //The value of smart boxing is questionable - so far did not see perf improvements, so currently it is disabled
    private void InitBoxes() {
      for (int i = 0; i < _boxes.Length; i++)
        _boxes[i] = i - _boxesMiddle;
    }

    #region Utility methods for adding converters and binary implementations
    protected OperatorImplementation AddConverter(Type fromType, Type toType, UnaryOperatorMethod method) {
      var key = new OperatorDispatchKey(ExpressionType.ConvertChecked, fromType, toType);
      var impl = new OperatorImplementation(key, toType, method);
      OperatorImplementations[key] = impl;
      return impl;
    }

    protected OperatorImplementation AddBinaryBoxed(ExpressionType op, Type baseType, 
         BinaryOperatorMethod boxedBinaryMethod, BinaryOperatorMethod noBoxMethod) {
      // first create implementation without boxing
      var noBoxImpl = AddBinary(op, baseType, noBoxMethod);
      if (!SmartBoxingEnabled)
        return noBoxImpl; 
      //The boxedImpl will overwrite noBoxImpl in the dictionary
      var boxedImpl = AddBinary(op, baseType, boxedBinaryMethod);
      boxedImpl.NoBoxImplementation = noBoxImpl;
      return boxedImpl;
    }

    protected OperatorImplementation AddBinary(ExpressionType op, Type baseType, BinaryOperatorMethod binaryMethod) {
      return AddBinary(op, baseType, binaryMethod, null);
    }

    protected OperatorImplementation AddBinary(ExpressionType op, Type commonType, 
                     BinaryOperatorMethod binaryMethod, UnaryOperatorMethod resultConverter) {
      var key = new OperatorDispatchKey(op, commonType, commonType);
      var impl = new OperatorImplementation(key, commonType, binaryMethod, null, null, resultConverter);
      OperatorImplementations[key] = impl;
      return impl;
    }

    protected OperatorImplementation AddUnary(ExpressionType op, Type commonType, UnaryOperatorMethod unaryMethod) {
      var key = new OperatorDispatchKey(op, commonType);
      var impl = new OperatorImplementation(key, commonType, null, unaryMethod, null, null);
      OperatorImplementations[key] = impl;
      return impl;
    }

    #endregion

    #region Initializing type converters
    public virtual void InitTypeConverters() {
      Type targetType; 

      //->string
      targetType = typeof(string);
      AddConverter(typeof(char), targetType, ConvertAnyToString);
      AddConverter(typeof(sbyte), targetType, ConvertAnyToString);
      AddConverter(typeof(byte), targetType, ConvertAnyToString);
      AddConverter(typeof(Int16), targetType, ConvertAnyToString);
      AddConverter(typeof(UInt16), targetType, ConvertAnyToString);
      AddConverter(typeof(Int32), targetType, ConvertAnyToString);
      AddConverter(typeof(UInt32), targetType, ConvertAnyToString);
      AddConverter(typeof(Int64), targetType, ConvertAnyToString);
      AddConverter(typeof(UInt64), targetType, ConvertAnyToString);
      AddConverter(typeof(Single), targetType, ConvertAnyToString);
      if (_supportsBigInt)
        AddConverter(typeof(BigInteger), targetType, ConvertAnyToString);
      if (_supportsComplex)
        AddConverter(typeof(Complex), targetType, ConvertAnyToString);

      //->Complex
      if (_supportsComplex) {
        targetType = typeof(Complex);
        AddConverter(typeof(sbyte), targetType, ConvertAnyToComplex);
        AddConverter(typeof(byte), targetType, ConvertAnyToComplex);
        AddConverter(typeof(Int16), targetType, ConvertAnyToComplex);
        AddConverter(typeof(UInt16), targetType, ConvertAnyToComplex);
        AddConverter(typeof(Int32), targetType, ConvertAnyToComplex);
        AddConverter(typeof(UInt32), targetType, ConvertAnyToComplex);
        AddConverter(typeof(Int64), targetType, ConvertAnyToComplex);
        AddConverter(typeof(UInt64), targetType, ConvertAnyToComplex);
        AddConverter(typeof(Single), targetType, ConvertAnyToComplex);
        if (_supportsBigInt) 
          AddConverter(typeof(BigInteger), targetType, ConvertBigIntToComplex);
      }
      //->BigInteger
      if (_supportsBigInt) {
        targetType = typeof(BigInteger);
        AddConverter(typeof(sbyte), targetType, ConvertAnyIntToBigInteger);
        AddConverter(typeof(byte), targetType, ConvertAnyIntToBigInteger);
        AddConverter(typeof(Int16), targetType, ConvertAnyIntToBigInteger);
        AddConverter(typeof(UInt16), targetType, ConvertAnyIntToBigInteger);
        AddConverter(typeof(Int32), targetType, ConvertAnyIntToBigInteger);
        AddConverter(typeof(UInt32), targetType, ConvertAnyIntToBigInteger);
        AddConverter(typeof(Int64), targetType, ConvertAnyIntToBigInteger);
        AddConverter(typeof(UInt64), targetType, ConvertAnyIntToBigInteger);
      }
 
      //->Double
      targetType = typeof(double);
      AddConverter(typeof(sbyte), targetType, value => (double)(sbyte)value);
      AddConverter(typeof(byte), targetType, value => (double)(byte)value);
      AddConverter(typeof(Int16), targetType, value => (double)(Int16)value);
      AddConverter(typeof(UInt16), targetType, value => (double)(UInt16)value);
      AddConverter(typeof(Int32), targetType, value => (double)(Int32)value);
      AddConverter(typeof(UInt32), targetType, value => (double)(UInt32)value);
      AddConverter(typeof(Int64), targetType, value => (double)(Int64)value);
      AddConverter(typeof(UInt64), targetType, value => (double)(UInt64)value);
      AddConverter(typeof(Single), targetType, value => (double)(Single)value);
      if (_supportsBigInt)
        AddConverter(typeof(BigInteger), targetType, value => ((double) (BigInteger)value));

      //->Single
      targetType = typeof(Single);
      AddConverter(typeof(sbyte), targetType, value => (Single)(sbyte)value);
      AddConverter(typeof(byte), targetType, value => (Single)(byte)value);
      AddConverter(typeof(Int16), targetType, value => (Single)(Int16)value);
      AddConverter(typeof(UInt16), targetType, value => (Single)(UInt16)value);
      AddConverter(typeof(Int32), targetType, value => (Single)(Int32)value);
      AddConverter(typeof(UInt32), targetType, value => (Single)(UInt32)value);
      AddConverter(typeof(Int64), targetType, value => (Single)(Int64)value);
      AddConverter(typeof(UInt64), targetType, value => (Single)(UInt64)value);
      if (_supportsBigInt)
        AddConverter(typeof(BigInteger), targetType, value => (Single)(BigInteger)value);
      
      //->UInt64
      targetType = typeof(UInt64);
      AddConverter(typeof(sbyte), targetType, value => (UInt64)(sbyte)value);
      AddConverter(typeof(byte), targetType, value => (UInt64)(byte)value);
      AddConverter(typeof(Int16), targetType, value => (UInt64)(Int16)value);
      AddConverter(typeof(UInt16), targetType, value => (UInt64)(UInt16)value);
      AddConverter(typeof(Int32), targetType, value => (UInt64)(Int32)value);
      AddConverter(typeof(UInt32), targetType, value => (UInt64)(UInt32)value);
      AddConverter(typeof(Int64), targetType, value => (UInt64)(Int64)value);

      //->Int64
      targetType = typeof(Int64);
      AddConverter(typeof(sbyte), targetType, value => (Int64)(sbyte)value);
      AddConverter(typeof(byte), targetType, value => (Int64)(byte)value);
      AddConverter(typeof(Int16), targetType, value => (Int64)(Int16)value);
      AddConverter(typeof(UInt16), targetType, value => (Int64)(UInt16)value);
      AddConverter(typeof(Int32), targetType, value => (Int64)(Int32)value);
      AddConverter(typeof(UInt32), targetType, value => (Int64)(UInt32)value);
      
      //->UInt32
      targetType = typeof(UInt32);
      AddConverter(typeof(sbyte), targetType, value => (UInt32)(sbyte)value);
      AddConverter(typeof(byte), targetType, value => (UInt32)(byte)value);
      AddConverter(typeof(Int16), targetType, value => (UInt32)(Int16)value);
      AddConverter(typeof(UInt16), targetType, value => (UInt32)(UInt16)value);
      AddConverter(typeof(Int32), targetType, value => (UInt32)(Int32)value);

      //->Int32
      targetType = typeof(Int32);
      AddConverter(typeof(sbyte), targetType, value => (Int32)(sbyte)value);
      AddConverter(typeof(byte), targetType, value => (Int32)(byte)value);
      AddConverter(typeof(Int16), targetType, value => (Int32)(Int16)value);
      AddConverter(typeof(UInt16), targetType, value => (Int32)(UInt16)value);

      //->UInt16
      targetType = typeof(UInt16);
      AddConverter(typeof(sbyte), targetType, value => (UInt16)(sbyte)value);
      AddConverter(typeof(byte), targetType, value => (UInt16)(byte)value);
      AddConverter(typeof(Int16), targetType, value => (UInt16)(Int16)value);

      //->Int16
      targetType = typeof(Int16);
      AddConverter(typeof(sbyte), targetType, value => (Int16)(sbyte)value);
      AddConverter(typeof(byte), targetType, value => (Int16)(byte)value);

      //->byte
      targetType = typeof(byte);
      AddConverter(typeof(sbyte), targetType, value => (byte)(sbyte)value);
    }

    // Some specialized convert implementation methods
    public static object ConvertAnyToString(object value) {
      return value == null ? string.Empty : value.ToString();
    }

    public static object ConvertBigIntToComplex(object value) {
      BigInteger bi = (BigInteger)value;
      return new Complex((double) bi, 0);
    }

    public static object ConvertAnyToComplex(object value) {
      double d = Convert.ToDouble(value);
      return new Complex(d, 0);
    }
    public static object ConvertAnyIntToBigInteger(object value) {
      long l = Convert.ToInt64(value);
      return new BigInteger(l);
    }
    #endregion

    #region Binary operators implementations
    // Generates of binary implementations for matched argument types
    public virtual void InitBinaryOperatorImplementationsForMatchedTypes() {

      // For each operator, we add a series of implementation methods for same-type operands. They are saved as OperatorImplementation
      // records in OperatorImplementations table. This happens at initialization time. 
      // After this initialization (for same-type operands), system adds implementations for all type pairs (ex: int + double), 
      // using these same-type implementations and appropriate type converters.
      // Note that arithmetics on byte, sbyte, int16, uint16 are performed in Int32 format (the way it's done in c# I guess)
      // so the result is always Int32. We do not define operators for sbyte, byte, int16 and UInt16 types - they will 
      // be processed using Int32 implementation, with appropriate type converters.
      ExpressionType op;

      op = ExpressionType.AddChecked;
      AddBinaryBoxed(op, typeof(Int32), (x, y) => _boxes[checked((Int32)x + (Int32)y) + _boxesMiddle], 
                                         (x, y) => checked((Int32)x + (Int32)y));
      AddBinary(op, typeof(UInt32), (x, y) => checked((UInt32)x + (UInt32)y));
      AddBinary(op, typeof(Int64), (x, y) => checked((Int64)x + (Int64)y));
      AddBinary(op, typeof(UInt64), (x, y) => checked((UInt64)x + (UInt64)y));
      AddBinary(op, typeof(Single), (x, y) => (Single)x + (Single)y);
      AddBinary(op, typeof(double), (x, y) => (double)x + (double)y);
      AddBinary(op, typeof(decimal), (x, y) => (decimal)x + (decimal)y);
      if (_supportsBigInt)
        AddBinary(op, typeof(BigInteger), (x, y) => (BigInteger)x + (BigInteger)y);
      if (_supportsComplex)
        AddBinary(op, typeof(Complex), (x, y) => (Complex)x + (Complex)y);
      AddBinary(op, typeof(string), (x, y) => (string)x + (string)y);
      AddBinary(op, typeof(char), (x, y) => ((char)x).ToString() + (char)y); //force to concatenate as strings

      op = ExpressionType.SubtractChecked;
      AddBinaryBoxed(op, typeof(Int32), (x, y) => _boxes[checked((Int32)x - (Int32)y) + _boxesMiddle],
                                         (x, y) => checked((Int32)x - (Int32)y));
      AddBinary(op, typeof(UInt32), (x, y) => checked((UInt32)x - (UInt32)y));
      AddBinary(op, typeof(Int64), (x, y) => checked((Int64)x - (Int64)y));
      AddBinary(op, typeof(UInt64), (x, y) => checked((UInt64)x - (UInt64)y));
      AddBinary(op, typeof(Single), (x, y) => (Single)x - (Single)y);
      AddBinary(op, typeof(double), (x, y) => (double)x - (double)y);
      AddBinary(op, typeof(decimal), (x, y) => (decimal)x - (decimal)y);
      if (_supportsBigInt)
        AddBinary(op, typeof(BigInteger), (x, y) => (BigInteger)x - (BigInteger)y);
      if (_supportsComplex)
        AddBinary(op, typeof(Complex), (x, y) => (Complex)x - (Complex)y);

      op = ExpressionType.MultiplyChecked;
      AddBinaryBoxed(op, typeof(Int32), (x, y) => _boxes[checked((Int32)x * (Int32)y) + _boxesMiddle],
                                         (x, y) => checked((Int32)x * (Int32)y));
      AddBinary(op, typeof(UInt32), (x, y) => checked((UInt32)x * (UInt32)y));
      AddBinary(op, typeof(Int64), (x, y) => checked((Int64)x * (Int64)y));
      AddBinary(op, typeof(UInt64), (x, y) => checked((UInt64)x * (UInt64)y));
      AddBinary(op, typeof(Single), (x, y) => (Single)x * (Single)y);
      AddBinary(op, typeof(double), (x, y) => (double)x * (double)y);
      AddBinary(op, typeof(decimal), (x, y) => (decimal)x * (decimal)y);
      if (_supportsBigInt)
        AddBinary(op, typeof(BigInteger), (x, y) => (BigInteger)x * (BigInteger)y);
      if (_supportsComplex)
        AddBinary(op, typeof(Complex), (x, y) => (Complex)x * (Complex)y);

      op = ExpressionType.Divide;
      AddBinary(op, typeof(Int32), (x, y) => checked((Int32)x / (Int32)y));
      AddBinary(op, typeof(UInt32), (x, y) => checked((UInt32)x / (UInt32)y));
      AddBinary(op, typeof(Int64), (x, y) => checked((Int64)x / (Int64)y));
      AddBinary(op, typeof(UInt64), (x, y) => checked((UInt64)x / (UInt64)y));
      AddBinary(op, typeof(Single), (x, y) => (Single)x / (Single)y);
      AddBinary(op, typeof(double), (x, y) => (double)x / (double)y);
      AddBinary(op, typeof(decimal), (x, y) =>(decimal)x / (decimal)y);
      if (_supportsBigInt)
        AddBinary(op, typeof(BigInteger), (x, y) => (BigInteger)x / (BigInteger)y);
      if (_supportsComplex)
        AddBinary(op, typeof(Complex), (x, y) => (Complex)x / (Complex)y);

      op = ExpressionType.Modulo;
      AddBinary(op, typeof(Int32), (x, y) => checked((Int32)x % (Int32)y));
      AddBinary(op, typeof(UInt32), (x, y) => checked((UInt32)x % (UInt32)y));
      AddBinary(op, typeof(Int64), (x, y) => checked((Int64)x % (Int64)y));
      AddBinary(op, typeof(UInt64), (x, y) => checked((UInt64)x % (UInt64)y));
      AddBinary(op, typeof(Single), (x, y) => (Single)x % (Single)y);
      AddBinary(op, typeof(double), (x, y) => (double)x % (double)y);
      AddBinary(op, typeof(decimal), (x, y) => (decimal)x % (decimal)y);
      if (_supportsBigInt)
        AddBinary(op, typeof(BigInteger), (x, y) => (BigInteger)x % (BigInteger)y);

      // For bitwise operator, we provide explicit implementations for "small" integer types
      op = ExpressionType.And;
      AddBinary(op, typeof(bool), (x, y) => (bool)x & (bool)y);
      AddBinary(op, typeof(sbyte), (x, y) => (sbyte)x & (sbyte)y);
      AddBinary(op, typeof(byte), (x, y) => (byte)x & (byte)y);
      AddBinary(op, typeof(Int16), (x, y) => (Int16)x & (Int16)y);
      AddBinary(op, typeof(UInt16), (x, y) => (UInt16)x & (UInt16)y);
      AddBinary(op, typeof(Int32), (x, y) => (Int32)x & (Int32)y);
      AddBinary(op, typeof(UInt32), (x, y) => (UInt32)x & (UInt32)y);
      AddBinary(op, typeof(Int64), (x, y) => (Int64)x & (Int64)y);
      AddBinary(op, typeof(UInt64), (x, y) => (UInt64)x & (UInt64)y);

      op = ExpressionType.Or;
      AddBinary(op, typeof(bool), (x, y) => (bool)x | (bool)y);
      AddBinary(op, typeof(sbyte), (x, y) => (sbyte)x | (sbyte)y);
      AddBinary(op, typeof(byte), (x, y) => (byte)x | (byte)y);
      AddBinary(op, typeof(Int16), (x, y) => (Int16)x | (Int16)y);
      AddBinary(op, typeof(UInt16), (x, y) => (UInt16)x | (UInt16)y);
      AddBinary(op, typeof(Int32), (x, y) => (Int32)x | (Int32)y);
      AddBinary(op, typeof(UInt32), (x, y) => (UInt32)x | (UInt32)y);
      AddBinary(op, typeof(Int64), (x, y) => (Int64)x | (Int64)y);
      AddBinary(op, typeof(UInt64), (x, y) => (UInt64)x | (UInt64)y);

      op = ExpressionType.ExclusiveOr; 
      AddBinary(op, typeof(bool), (x, y) => (bool)x ^ (bool)y);
      AddBinary(op, typeof(sbyte), (x, y) => (sbyte)x ^ (sbyte)y);
      AddBinary(op, typeof(byte), (x, y) => (byte)x ^ (byte)y);
      AddBinary(op, typeof(Int16), (x, y) => (Int16)x ^ (Int16)y);
      AddBinary(op, typeof(UInt16), (x, y) => (UInt16)x ^ (UInt16)y);
      AddBinary(op, typeof(Int32), (x, y) => (Int32)x ^ (Int32)y);
      AddBinary(op, typeof(UInt32), (x, y) => (UInt32)x ^ (UInt32)y);
      AddBinary(op, typeof(Int64), (x, y) => (Int64)x ^ (Int64)y);
      AddBinary(op, typeof(UInt64), (x, y) => (UInt64)x ^ (UInt64)y);

      op = ExpressionType.LessThan;
      AddBinary(op, typeof(Int32), (x, y) => checked((Int32)x < (Int32)y), BoolResultConverter);
      AddBinary(op, typeof(UInt32), (x, y) => checked((UInt32)x < (UInt32)y), BoolResultConverter);
      AddBinary(op, typeof(Int64), (x, y) => checked((Int64)x < (Int64)y), BoolResultConverter);
      AddBinary(op, typeof(UInt64), (x, y) => checked((UInt64)x < (UInt64)y), BoolResultConverter);
      AddBinary(op, typeof(Single), (x, y) => (Single)x < (Single)y, BoolResultConverter);
      AddBinary(op, typeof(double), (x, y) => (double)x < (double)y, BoolResultConverter);
      AddBinary(op, typeof(decimal), (x, y) => (decimal)x < (decimal)y);
      if (_supportsBigInt)
        AddBinary(op, typeof(BigInteger), (x, y) => (BigInteger)x < (BigInteger)y, BoolResultConverter);

      op = ExpressionType.GreaterThan;
      AddBinary(op, typeof(Int32), (x, y) => checked((Int32)x > (Int32)y), BoolResultConverter);
      AddBinary(op, typeof(UInt32), (x, y) => checked((UInt32)x > (UInt32)y), BoolResultConverter);
      AddBinary(op, typeof(Int64), (x, y) => checked((Int64)x > (Int64)y), BoolResultConverter);
      AddBinary(op, typeof(UInt64), (x, y) => checked((UInt64)x > (UInt64)y), BoolResultConverter);
      AddBinary(op, typeof(Single), (x, y) => (Single)x > (Single)y, BoolResultConverter);
      AddBinary(op, typeof(double), (x, y) => (double)x > (double)y, BoolResultConverter);
      AddBinary(op, typeof(decimal), (x, y) => (decimal)x > (decimal)y);
      if (_supportsBigInt)
        AddBinary(op, typeof(BigInteger), (x, y) => (BigInteger)x > (BigInteger)y, BoolResultConverter);

      op = ExpressionType.LessThanOrEqual;
      AddBinary(op, typeof(Int32), (x, y) => checked((Int32)x <= (Int32)y), BoolResultConverter);
      AddBinary(op, typeof(UInt32), (x, y) => checked((UInt32)x <= (UInt32)y), BoolResultConverter);
      AddBinary(op, typeof(Int64), (x, y) => checked((Int64)x <= (Int64)y), BoolResultConverter);
      AddBinary(op, typeof(UInt64), (x, y) => checked((UInt64)x <= (UInt64)y), BoolResultConverter);
      AddBinary(op, typeof(Single), (x, y) => (Single)x <= (Single)y, BoolResultConverter);
      AddBinary(op, typeof(double), (x, y) => (double)x <= (double)y, BoolResultConverter);
      AddBinary(op, typeof(decimal), (x, y) => (decimal)x <= (decimal)y);
      if (_supportsBigInt)
        AddBinary(op, typeof(BigInteger), (x, y) => (BigInteger)x <= (BigInteger)y, BoolResultConverter);

      op = ExpressionType.GreaterThanOrEqual;
      AddBinary(op, typeof(Int32), (x, y) => checked((Int32)x >= (Int32)y), BoolResultConverter);
      AddBinary(op, typeof(UInt32), (x, y) => checked((UInt32)x >= (UInt32)y), BoolResultConverter);
      AddBinary(op, typeof(Int64), (x, y) => checked((Int64)x >= (Int64)y), BoolResultConverter);
      AddBinary(op, typeof(UInt64), (x, y) => checked((UInt64)x >= (UInt64)y), BoolResultConverter);
      AddBinary(op, typeof(Single), (x, y) => (Single)x >= (Single)y, BoolResultConverter);
      AddBinary(op, typeof(double), (x, y) => (double)x >= (double)y, BoolResultConverter);
      AddBinary(op, typeof(decimal), (x, y) => (decimal)x >= (decimal)y);
      if (_supportsBigInt)
        AddBinary(op, typeof(BigInteger), (x, y) => (BigInteger)x >= (BigInteger)y, BoolResultConverter);

      op = ExpressionType.Equal;
      AddBinary(op, typeof(Int32), (x, y) => checked((Int32)x == (Int32)y), BoolResultConverter);
      AddBinary(op, typeof(UInt32), (x, y) => checked((UInt32)x == (UInt32)y), BoolResultConverter);
      AddBinary(op, typeof(Int64), (x, y) => checked((Int64)x == (Int64)y), BoolResultConverter);
      AddBinary(op, typeof(UInt64), (x, y) => checked((UInt64)x == (UInt64)y), BoolResultConverter);
      AddBinary(op, typeof(Single), (x, y) => (Single)x == (Single)y, BoolResultConverter);
      AddBinary(op, typeof(double), (x, y) => (double)x == (double)y, BoolResultConverter);
      AddBinary(op, typeof(decimal), (x, y) => (decimal)x == (decimal)y);
      if (_supportsBigInt)
        AddBinary(op, typeof(BigInteger), (x, y) => (BigInteger)x == (BigInteger)y, BoolResultConverter);

      op = ExpressionType.NotEqual;
      AddBinary(op, typeof(Int32), (x, y) => checked((Int32)x != (Int32)y), BoolResultConverter);
      AddBinary(op, typeof(UInt32), (x, y) => checked((UInt32)x != (UInt32)y), BoolResultConverter);
      AddBinary(op, typeof(Int64), (x, y) => checked((Int64)x != (Int64)y), BoolResultConverter);
      AddBinary(op, typeof(UInt64), (x, y) => checked((UInt64)x != (UInt64)y), BoolResultConverter);
      AddBinary(op, typeof(Single), (x, y) => (Single)x != (Single)y, BoolResultConverter);
      AddBinary(op, typeof(double), (x, y) => (double)x != (double)y, BoolResultConverter);
      AddBinary(op, typeof(decimal), (x, y) => (decimal)x != (decimal)y);
      if (_supportsBigInt)
        AddBinary(op, typeof(BigInteger), (x, y) => (BigInteger)x != (BigInteger)y, BoolResultConverter);

    }//method

    public virtual void InitUnaryOperatorImplementations() {
      var op = ExpressionType.UnaryPlus;
      AddUnary(op, typeof(sbyte), x => +(sbyte)x);
      AddUnary(op, typeof(byte), x => +(byte)x);
      AddUnary(op, typeof(Int16), x => +(Int16)x);
      AddUnary(op, typeof(UInt16), x => +(UInt16)x);
      AddUnary(op, typeof(Int32), x => +(Int32)x);
      AddUnary(op, typeof(UInt32), x => +(UInt32)x);
      AddUnary(op, typeof(Int64), x => +(Int64)x);
      AddUnary(op, typeof(UInt64), x => +(UInt64)x);
      AddUnary(op, typeof(Single), x => +(Single)x);
      AddUnary(op, typeof(double), x => +(double)x);
      AddUnary(op, typeof(decimal), x => +(decimal)x);
      if (_supportsBigInt)
        AddUnary(op, typeof(BigInteger), x => +(BigInteger)x);

      op = ExpressionType.Negate;
      AddUnary(op, typeof(sbyte), x => -(sbyte)x);
      AddUnary(op, typeof(byte), x => -(byte)x);
      AddUnary(op, typeof(Int16), x => -(Int16)x);
      AddUnary(op, typeof(UInt16), x => -(UInt16)x);
      AddUnary(op, typeof(Int32), x => -(Int32)x);
      AddUnary(op, typeof(UInt32), x => -(UInt32)x);
      AddUnary(op, typeof(Int64), x => -(Int64)x);
      AddUnary(op, typeof(Single), x => -(Single)x);
      AddUnary(op, typeof(double), x => -(double)x);
      AddUnary(op, typeof(decimal), x => -(decimal)x);
      if (_supportsBigInt)
        AddUnary(op, typeof(BigInteger), x => -(BigInteger)x);
      if (_supportsComplex)
        AddUnary(op, typeof(Complex), x => -(Complex)x);

      op = ExpressionType.Not;
      AddUnary(op, typeof(bool), x => !(bool)x);
      AddUnary(op, typeof(sbyte), x => ~(sbyte)x);
      AddUnary(op, typeof(byte), x => ~(byte)x);
      AddUnary(op, typeof(Int16), x => ~(Int16)x);
      AddUnary(op, typeof(UInt16), x => ~(UInt16)x);
      AddUnary(op, typeof(Int32), x => ~(Int32)x);
      AddUnary(op, typeof(UInt32), x => ~(UInt32)x);
      AddUnary(op, typeof(Int64), x => ~(Int64)x);

    }

    // Generates binary implementations for mismatched argument types
    public virtual void CreateBinaryOperatorImplementationsForMismatchedTypes() {
      // find all data types are there
      var allTypes = new HashSet<Type>();
      var allBinOps = new HashSet<ExpressionType>();
      foreach (var kv in OperatorImplementations) {
        allTypes.Add(kv.Key.Arg1Type);
        if (kv.Value.BaseBinaryMethod != null)
          allBinOps.Add(kv.Key.Op);      
      }
      foreach (var arg1Type in allTypes)
        foreach (var arg2Type in allTypes)
          if (arg1Type != arg2Type)
            foreach (ExpressionType op in allBinOps)
              CreateBinaryOperatorImplementation(op, arg1Type, arg2Type);
    }//method

    // Creates a binary implementations for an operator with mismatched argument types.
    // Determines common type, retrieves implementation for operator with both args of common type, then creates
    // implementation for mismatched types using type converters (by converting to common type)
    public OperatorImplementation CreateBinaryOperatorImplementation(ExpressionType op, Type arg1Type, Type arg2Type) {
      Type commonType = GetCommonTypeForOperator(op, arg1Type, arg2Type);
      if (commonType == null)
        return null;
      //Get base method for the operator and common type 
      var baseImpl = FindBaseImplementation(op, commonType);
      if (baseImpl == null) { //Try up-type
        commonType = GetUpType(commonType);
        if (commonType == null)
          return null; 
        baseImpl = FindBaseImplementation(op, commonType);
      }
      if (baseImpl == null)
        return null; 
      //Create implementation and save it in implementations table
      var impl = CreateBinaryOperatorImplementation(op, arg1Type, arg2Type, commonType, baseImpl.BaseBinaryMethod, baseImpl.ResultConverter);
      OperatorImplementations[impl.Key] = impl;
      return impl;
    }

    protected virtual OperatorImplementation CreateBinaryOperatorImplementation(ExpressionType op, Type arg1Type, Type arg2Type, 
                   Type commonType, BinaryOperatorMethod method, UnaryOperatorMethod resultConverter) {
      OperatorDispatchKey key = new OperatorDispatchKey(op, arg1Type, arg2Type);
      UnaryOperatorMethod arg1Converter = arg1Type == commonType ? null : GetConverter(arg1Type, commonType);
      UnaryOperatorMethod arg2Converter = arg2Type == commonType ? null : GetConverter(arg2Type, commonType);
      var impl = new OperatorImplementation(
        key, commonType, method, arg1Converter, arg2Converter, resultConverter);
      return impl; 
    }

    // Creates overflow handlers. For each implementation, checks if operator can overflow; 
    // if yes, creates and sets an overflow handler - another implementation that performs
    // operation using "upper" type that wouldn't overflow. For ex: (int * int) has overflow handler (int64 * int64) 
    protected virtual void CreateOverflowHandlers() {
      foreach (var impl in OperatorImplementations.Values) {
        if (!CanOverflow(impl))
          continue;
        var key = impl.Key;
        var upType = GetUpType(impl.CommonType);
        if (upType == null)
          continue;
        var upBaseImpl = FindBaseImplementation(key.Op, upType);
        if (upBaseImpl == null)
          continue;
        impl.OverflowHandler = CreateBinaryOperatorImplementation(key.Op, key.Arg1Type, key.Arg2Type, upType,
                                            upBaseImpl.BaseBinaryMethod, upBaseImpl.ResultConverter);
        // Do not put OverflowHandler into OperatoImplementations table! - it will override some other, non-overflow impl
      }
    }

    private OperatorImplementation FindBaseImplementation(ExpressionType op, Type commonType) {
      var baseKey = new OperatorDispatchKey(op, commonType, commonType);
      OperatorImplementation baseImpl;
      OperatorImplementations.TryGetValue(baseKey, out baseImpl);
      return baseImpl;
    }

    // Important: returns null if fromType == toType
    public virtual UnaryOperatorMethod GetConverter(Type fromType, Type toType) {
      if (fromType == toType)
        return (x => x);
      var key = new OperatorDispatchKey(ExpressionType.ConvertChecked, fromType, toType);
      OperatorImplementation impl;
      if (!OperatorImplementations.TryGetValue(key, out impl))
        return null;
      return impl.Arg1Converter;
    }
    #endregion

    #region Utilities

    private static bool CanOverflow(OperatorImplementation impl) {
      if (!CanOverflow(impl.Key.Op))
        return false;
      if (impl.CommonType == typeof(Int32) && IsSmallInt(impl.Key.Arg1Type) && IsSmallInt(impl.Key.Arg2Type))
        return false;
      if (impl.CommonType == typeof(double) || impl.CommonType == typeof(Single))
        return false;
      if (impl.CommonType == typeof(BigInteger))
        return false;
      return true;
    }

    private static bool CanOverflow(ExpressionType expression) {
      return _overflowOperators.Contains(expression);
    }


    private static bool IsSmallInt(Type type) {
      return type == typeof(byte) || type == typeof(sbyte) || type == typeof(Int16) || type == typeof(UInt16);
    }

    /// <summary>
    /// Returns the type to which arguments should be converted to perform the operation
    /// for a given operator and arguments types.
    /// </summary>
    /// <param name="op">Operator.</param>
    /// <param name="argType1">The type of the first argument.</param>
    /// <param name="argType2">The type of the second argument</param>
    /// <returns>A common type for operation.</returns>
    protected virtual Type GetCommonTypeForOperator(ExpressionType op, Type argType1, Type argType2) {
      if (argType1 == argType2)
        return argType1;

      //TODO: see how to handle properly null/NoneValue in expressions
      // var noneType = typeof(NoneClass);
      // if (argType1 == noneType || argType2 == noneType) return noneType; 
      
      // Check for unsigned types and convert to signed versions
      var t1 = GetSignedTypeForUnsigned(argType1);
      var t2 = GetSignedTypeForUnsigned(argType2);
      // The type with higher index in _typesSequence is the commont type 
      var index1 = _typesSequence.IndexOf(t1);
      var index2 = _typesSequence.IndexOf(t2);
      if (index1 >= 0 && index2 >= 0)
        return _typesSequence[Math.Max(index1, index2)];
      //If we have some custom type, 
      return null;
    }//method

    // If a type is one of "unsigned" int types, returns next bigger signed type
    protected virtual Type GetSignedTypeForUnsigned(Type type) {
      if (!_unsignedTypes.Contains(type))  return type;
      if (type == typeof(byte) || type == typeof(UInt16)) return typeof(int);
      if (type == typeof(UInt32)) return typeof(Int64);
      if (type == typeof(UInt64)) return typeof(Int64); //let's remain in Int64
      return typeof(BigInteger);
    }

    /// <summary>
    /// Returns the "up-type" to use in operation instead of the type that caused overflow.
    /// </summary>
    /// <param name="type">The base type for operation that caused overflow.</param>
    /// <returns>The type to use for operation.</returns>
    /// <remarks>
    /// Can be overwritten in language implementation to implement different type-conversion policy.
    /// </remarks>
    protected virtual Type GetUpType(Type type) {
      // In fact we do not need to care about unsigned types - they are eliminated from common types for operations,
      //  so "type" parameter can never be unsigned type. But just in case...
      if (_unsignedTypes.Contains(type))
        return GetSignedTypeForUnsigned(type); //it will return "upped" type in fact
      if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(UInt16) || type == typeof(Int16)) 
        return typeof(int);
      if (type == typeof(Int32)) 
        return typeof(Int64);
      if (type == typeof(Int64)) 
          return typeof(BigInteger);
      return null;
    }

    //Note bool type at the end - if any of operands is of bool type, convert the other to bool as well
    static TypeList _typesSequence = new TypeList(
        typeof(sbyte), typeof(Int16), typeof(Int32), typeof(Int64), typeof(BigInteger), // typeof(Rational)
        typeof(Single), typeof(Double), typeof(Complex),
        typeof(bool), typeof(char), typeof(string)
    );
    static TypeList _unsignedTypes = new TypeList(
      typeof(byte), typeof(UInt16), typeof(UInt32), typeof(UInt64)
    );
    #endregion

  }//class

}//namespace
