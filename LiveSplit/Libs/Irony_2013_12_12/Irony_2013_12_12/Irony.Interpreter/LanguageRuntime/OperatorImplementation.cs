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
using System.Text;
using System.Linq.Expressions;
using Irony.Parsing;

namespace Irony.Interpreter {

  public delegate object UnaryOperatorMethod(object arg);
  public delegate object BinaryOperatorMethod(object arg1, object arg2);

  #region OperatorDispatchKey class
  /// <summary>
  /// The struct is used as a key for the dictionary of operator implementations. 
  /// Contains types of arguments for a method or operator implementation.
  /// </summary>
  public struct OperatorDispatchKey {
    public static readonly OperatorDispatchKeyComparer Comparer = new OperatorDispatchKeyComparer();
    public readonly ExpressionType Op;
    public readonly Type Arg1Type;
    public readonly Type Arg2Type;
    public readonly int HashCode;

    //For binary operators
    public OperatorDispatchKey(ExpressionType op, Type arg1Type, Type arg2Type) {
      Op = op;
      Arg1Type = arg1Type;
      Arg2Type = arg2Type;
      int h0 = (int)Op;
      int h1 = Arg1Type.GetHashCode();
      int h2 = Arg2Type.GetHashCode();
      HashCode = unchecked(h0 << 8  ^  h1 << 4 ^ h2);
    }

    //For unary operators
    public OperatorDispatchKey(ExpressionType op, Type arg1Type) {
      Op = op;
      Arg1Type = arg1Type;
      Arg2Type = null;
      int h0 = (int)Op;
      int h1 = Arg1Type.GetHashCode();
      int h2 = 0;
      HashCode = unchecked(h0 << 8 ^ h1 << 4 ^ h2);
    }

    public override int GetHashCode() {
      return HashCode;
    }

    public override string ToString() {
      return  Op + "(" +  Arg1Type + ", " + Arg2Type + ")";
    }
  }//class
  #endregion

  #region OperatorDispatchKeyComparer class
  // Note: I believe (guess) that a custom Comparer provided to a Dictionary is a bit more efficient 
  // than implementing IComparable on the key itself
  public class OperatorDispatchKeyComparer : IEqualityComparer<OperatorDispatchKey> {
    public bool  Equals(OperatorDispatchKey x, OperatorDispatchKey y) {
    return x.HashCode == y.HashCode && x.Op == y.Op && x.Arg1Type == y.Arg1Type && x.Arg2Type == y.Arg2Type;
    }
    public int  GetHashCode(OperatorDispatchKey obj){
      return obj.HashCode;
    }
  }//class
  #endregion 

  public class TypeConverterTable : Dictionary<OperatorDispatchKey, UnaryOperatorMethod> {
    public TypeConverterTable(int capacity) : base(capacity, OperatorDispatchKey.Comparer) {}

  }//class

  public class OperatorImplementationTable : Dictionary<OperatorDispatchKey, OperatorImplementation> {
    public OperatorImplementationTable(int capacity) : base(capacity, OperatorDispatchKey.Comparer) { }
  }

  ///<summary>
  ///The OperatorImplementation class represents an implementation of an operator for specific argument types.
  ///</summary>
  ///<remarks>
  /// The OperatorImplementation is used for holding implementation for binary operators, unary operators, 
  /// and type converters (special case of unary operators)
  /// it holds 4 method references for binary operators:
  /// converters for both arguments, implementation method and converter for the result.
  /// For unary operators (and type converters) the implementation is in Arg1Converter
  /// operator (arg1 is used); the converter method is stored in Arg1Converter; the target type is in CommonType
  ///</remarks>
  public sealed class OperatorImplementation {
    public readonly OperatorDispatchKey Key;
    // The type to which arguments are converted and no-conversion method for this type. 
    public readonly Type CommonType;
    public readonly BinaryOperatorMethod BaseBinaryMethod;
    //converters
    internal UnaryOperatorMethod Arg1Converter;
    internal UnaryOperatorMethod Arg2Converter;
    internal UnaryOperatorMethod ResultConverter;
    //A reference to the actual binary evaluator method - one of EvaluateConvXXX 
    public BinaryOperatorMethod EvaluateBinary; 
    // An overflow handler - the implementation to handle arithmetic overflow
    public OperatorImplementation OverflowHandler; 
    // No-box counterpart for implementations with auto-boxed output. If this field <> null, then this is 
    // implementation with auto-boxed output
    public OperatorImplementation NoBoxImplementation; 

    //Constructor for binary operators
    public OperatorImplementation(OperatorDispatchKey key, Type resultType, BinaryOperatorMethod baseBinaryMethod, 
        UnaryOperatorMethod arg1Converter, UnaryOperatorMethod arg2Converter,  UnaryOperatorMethod resultConverter) {
      Key = key;
      CommonType = resultType;
      Arg1Converter = arg1Converter;
      Arg2Converter = arg2Converter;
      ResultConverter = resultConverter;
      BaseBinaryMethod = baseBinaryMethod;
      SetupEvaluationMethod();
    }

    //Constructor for unary operators and type converters
    public OperatorImplementation(OperatorDispatchKey key, Type type, UnaryOperatorMethod method) {
      Key = key;
      CommonType = type;
      Arg1Converter = method;
      Arg2Converter = null;
      ResultConverter = null;
      BaseBinaryMethod = null;
    }

    public override string ToString() {
      return "[OpImpl for " + Key.ToString() + "]";
    }

    public void SetupEvaluationMethod() {
      if (BaseBinaryMethod == null)
        //special case - it is unary method, the method itself in Arg1Converter; LanguageRuntime.ExecuteUnaryOperator will handle this properly
        return; 
      // Binary operator
      if (ResultConverter == null) {
        //without ResultConverter
        if (Arg1Converter == null && Arg2Converter == null)
          EvaluateBinary = EvaluateConvNone;
        else if (Arg1Converter != null && Arg2Converter == null)
          EvaluateBinary = EvaluateConvLeft;
        else if (Arg1Converter == null && Arg2Converter != null)
          EvaluateBinary = EvaluateConvRight;
        else // if (Arg1Converter != null && arg2Converter != null)
          EvaluateBinary = EvaluateConvBoth;
      } else {
        //with result converter
        if (Arg1Converter == null && Arg2Converter == null)
          EvaluateBinary = EvaluateConvNoneConvResult;
        else if (Arg1Converter != null && Arg2Converter == null)
          EvaluateBinary = EvaluateConvLeftConvResult;
        else if (Arg1Converter == null && Arg2Converter != null)
          EvaluateBinary = EvaluateConvRightConvResult;
        else // if (Arg1Converter != null && Arg2Converter != null)
          EvaluateBinary = EvaluateConvBothConvResult;
      }
    }

    private object EvaluateConvNone(object arg1, object arg2) {
      return BaseBinaryMethod(arg1, arg2);
    }
    private object EvaluateConvLeft(object arg1, object arg2) {
      return BaseBinaryMethod(Arg1Converter(arg1), arg2);
    }
    private object EvaluateConvRight(object arg1, object arg2) {
      return BaseBinaryMethod(arg1, Arg2Converter(arg2));
    }
    private object EvaluateConvBoth(object arg1, object arg2) {
      return BaseBinaryMethod(Arg1Converter(arg1), Arg2Converter(arg2));
    }

    private object EvaluateConvNoneConvResult(object arg1, object arg2) {
      return ResultConverter(BaseBinaryMethod(arg1, arg2));
    }
    private object EvaluateConvLeftConvResult(object arg1, object arg2) {
      return ResultConverter(BaseBinaryMethod(Arg1Converter(arg1), arg2));
    }
    private object EvaluateConvRightConvResult(object arg1, object arg2) {
      return ResultConverter(BaseBinaryMethod(arg1, Arg2Converter(arg2)));
    }
    private object EvaluateConvBothConvResult(object arg1, object arg2) {
      return ResultConverter(BaseBinaryMethod(Arg1Converter(arg1), Arg2Converter(arg2)));
    }
  }//class



}//namespace
