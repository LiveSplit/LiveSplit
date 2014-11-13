/*
 * Created by SharpDevelop.
 * User: Nadvi
 * Date: 3/5/2013
 * Time: 5:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;
using System.Globalization;

namespace Irony.Samples.EMFOCL2 {
  /// <summary>
  /// Description of EMOFOCL2Grammer.
  /// </summary>
  [Language("SLAng", "2", "SLAng grammar")]
  public class SLAngGrammer : Irony.Parsing.Grammar {
    public SLAngGrammer()
      : base() {
      StringLiteral stringLiteral = TerminalFactory.CreateCSharpString("StringLiteral");
      StringLiteral charLiteral = TerminalFactory.CreateCSharpChar("CharLiteral");
      NumberLiteral number = TerminalFactory.CreateCSharpNumber("Number");
      IdentifierTerminal identifier = TerminalFactory.CreateCSharpIdentifier("Identifier");
      CommentTerminal delimitedComment = new CommentTerminal("DelimitedComment", "/[", "]/");
      CommentTerminal SingleLineComment = new CommentTerminal("SingleLineComment", "--", "\r", "\n", "\u2085", "\u2028", "\u2029");
      NonGrammarTerminals.Add(SingleLineComment);
      NonGrammarTerminals.Add(delimitedComment);

      #region Terminals
      KeyTerm colon = ToTerm(":");
      KeyTerm dot = ToTerm(".");
      KeyTerm comma = ToTerm(",");
      KeyTerm propertyIs = ToTerm("->");
      KeyTerm lbr = ToTerm("{");
      KeyTerm rbr = ToTerm("}");
      KeyTerm lpar = ToTerm("(");
      KeyTerm rpar = ToTerm(")");
      KeyTerm scopeIn = ToTerm("::");
      KeyTerm suchThat = ToTerm("|");
      KeyTerm _package = ToTerm("package");
      KeyTerm context = ToTerm("context");
      KeyTerm body = ToTerm("body");
      KeyTerm def = ToTerm("def");
      KeyTerm derive = ToTerm("derive");
      KeyTerm In = ToTerm("in");
      KeyTerm inv = ToTerm("inv");
      KeyTerm let = ToTerm("let");
      KeyTerm post = ToTerm("post");
      KeyTerm pre = ToTerm("pre");
      KeyTerm Static = ToTerm("static");
      KeyTerm self = ToTerm("self");
      KeyTerm result = ToTerm("result");
      KeyTerm equalsTo = ToTerm("=");
      KeyTerm _null = ToTerm("null");
      KeyTerm invalid = ToTerm("invalid");
      KeyTerm not = ToTerm("not");
      KeyTerm and = ToTerm("and");
      KeyTerm or = ToTerm("or");
      KeyTerm xor = ToTerm("xor");
      #endregion

      #region NonTerminal
      var specification = new NonTerminal("specification");
      var specificationBody = new NonTerminal("specificationBody");
      var package = new NonTerminal("package");
      var packages = new NonTerminal("packages");
      var oclPackage = new NonTerminal("oclPackage");
      var emofOclPackage = new NonTerminal("emofOclPackage");
      var packageBody = new NonTerminal("packageBody");
      var packageBodyItem = new NonTerminal("packageBodyItem");
      var contextExp = new NonTerminal("contextExp");
      var classType = new NonTerminal("classType");
      var enumeration = new NonTerminal("enumeration");
      var primitive = new NonTerminal("primitive");
      var primitiveType = new NonTerminal("primitiveType");
      var primitiveTypes = new NonTerminal("primitiveTypes");
      var delimitedIdentifier = new NonTerminal("delimitedIdentifier");
      var classSignature = new NonTerminal("classSignature");
      var classBody = new NonTerminal("classBody");
      var classBodyItem = new NonTerminal("classBodyItem");
      var invExp = new NonTerminal("invExp");
      var functionDefination = new NonTerminal("functionDefination");
      var attributeDiclaration = new NonTerminal("attributeDiclaration");
      var emofAttributeDiclaration = new NonTerminal("emofAttributeDiclaration");
      var oclAttributeDiclaration = new NonTerminal("oclAttributeDiclaration");
      var type = new NonTerminal("type");
      var expression = new NonTerminal("expression");
      var customType = new NonTerminal("customType");
      var collectionTpye = new NonTerminal("collectionTpye");
      var fullType = new NonTerminal("fullType");
      var globalType = new NonTerminal("globalType");
      var setType = new NonTerminal("setType");
      var orderedSetType = new NonTerminal("orderedSetType");
      var sequenceType = new NonTerminal("sequenceType");
      var bagType = new NonTerminal("bagType");
      var functionSignature = new NonTerminal("functionSignature");
      var functionSignatureBody = new NonTerminal("functionSignatureBody");
      var functionBody = new NonTerminal("functionBody");
      var oclFunctionSignature = new NonTerminal("oclFunctionSignature");
      var emofFunctionSignature = new NonTerminal("emofFunctionSignature");
      var argumentList = new NonTerminal("argumentList");
      var variableDiclaration = new NonTerminal("variableDiclaration");
      var emofFunctionBody = new NonTerminal("emofFunctionBody");
      var oclFunctionBody = new NonTerminal("oclFunctionBody");
      var singleValueCollectionMember = new NonTerminal("singleValueCollectionMember");
      var booleanValueCollection = new NonTerminal("booleanValueCollection");
      var letExp = new NonTerminal("letExp");
      var ifThenElseExp = new NonTerminal("ifThenElseExp");
      var collectionValueExp = new NonTerminal("collectionValueExp");
      var sum = new NonTerminal("sum");
      var size = new NonTerminal("size");
      var first = new NonTerminal("first");
      var last = new NonTerminal("last");
      var at = new NonTerminal("at");
      var indexOf = new NonTerminal("indexOf");
      var countOperation = new NonTerminal("countOperation");
      var maxOperation = new NonTerminal("maxOperation");
      var minOperation = new NonTerminal("minOperation");
      var valueExp = new NonTerminal("valueExp");
      var includesOperation = new NonTerminal("includesOperation");
      var includesAllOperation = new NonTerminal("includesAllOperation");
      var excludesOperation = new NonTerminal("excludesOperation");
      var excludesAllOperation = new NonTerminal("excludesAllOperation");
      var isEmptyOperation = new NonTerminal("isEmptyOperation");
      var notEmptyOperation = new NonTerminal("notEmptyOperation");
      var specialIteratorBodyExp = new NonTerminal("specialIteratorBodyExp");
      var existsExp = new NonTerminal("existsExp");
      var forAllExp = new NonTerminal("forAllExp");
      var isUniqueExp = new NonTerminal("isUniqueExp");
      var anyExp = new NonTerminal("anyExp");
      var oneExp = new NonTerminal("oneExp");
      var selectExp = new NonTerminal("selectExp");
      var includingOperation = new NonTerminal("includingOperation");
      var excludingOperation = new NonTerminal("excludingOperation");
      var collect = new NonTerminal("collect");
      var subOrderedSet = new NonTerminal("subOrderedSet");
      var symmetricDifference = new NonTerminal("symmetricDifference");
      var prepend = new NonTerminal("prepend");
      var append = new NonTerminal("append");
      var insertAt = new NonTerminal("insertAt");
      var intersection = new NonTerminal("intersection");
      var union = new NonTerminal("union");
      var asBag = new NonTerminal("asBag");
      var asSequence = new NonTerminal("asSequence");
      var asOrderdSet = new NonTerminal("asOrderdSet");
      var asSet = new NonTerminal("asSet");
      var flatten = new NonTerminal("flatten");
      var subSequence = new NonTerminal("subSequence");
      var andExp = new NonTerminal("andExp");
      var orExp = new NonTerminal("orExp");
      var notExp = new NonTerminal("notExp");
      var xorExp = new NonTerminal("xorExp");
      var impliesExp = new NonTerminal("impliesExp");
      var comparisonOperator = new NonTerminal("comparisonOperator");
      var arithmaticOperator = new NonTerminal("arithmaticOperator");
      var oclInvExp = new NonTerminal("oclInvExp");
      var emofInvExp = new NonTerminal("emofInvExp");
      var operationContextExp = new NonTerminal("operationContextExp");
      var attributeContextExp = new NonTerminal("attributeContextExp");
      var classContextExp = new NonTerminal("classContextExp");
      var classContextSignature = new NonTerminal("classContextSignature");
      var attributeContextbody = new NonTerminal("attributeContextbody");
      var attributeContextbodyItem = new NonTerminal("attributeContextbodyItem");
      var deriveExp = new NonTerminal("deriveExp");
      var initExp = new NonTerminal("initExp");
      var operationContextBodyExps = new NonTerminal("operationContextBodyExps");
      var operationContextSignature = new NonTerminal("operationContextSignature");
      var operationContextBodyItem = new NonTerminal("operationContextBodyItem");
      var preCondition = new NonTerminal("preCondition");
      var postCondition = new NonTerminal("postCondition");
      var bodyExp = new NonTerminal("bodyExp");
      var iterateExp = new NonTerminal("iterateExp");
      var productExp = new NonTerminal("productExp");
      var tupleExp = new NonTerminal("tupleExp");
      var rejectExp = new NonTerminal("rejectExp");
      var collection = new NonTerminal("collection");
      var tuplElementList = new NonTerminal("tuplElementList");
      var tupleElement = new NonTerminal("tupleElement");
      var binaryOperator = new NonTerminal("binaryOperator");
      var parameterList = new NonTerminal("parameterList");
      var structuralExp = new NonTerminal("structuralExp");
      var setLiteral = new NonTerminal("setLiteral");
      var bagLiteral = new NonTerminal("orderedSetLiteral");
      var orderedSetLiteral = new NonTerminal("");
      var sequenceLiteral = new NonTerminal("sequenceLiteral");
      var tupleLiteral = new NonTerminal("tupleLiteral");
      var literalList = new NonTerminal("literalList");
      var iteratorList = new NonTerminal("iteratorList");
      var iteratorInitializationList = new NonTerminal("iteratorInitializationList");
      var iteratorInitialization = new NonTerminal("iteratorInitialization");
      var collectionValue = new NonTerminal("collectionValue");
      var ArrayType = new NonTerminal("ArrayType");
      var binaryBooleanOperator = new NonTerminal("binaryBooleanOperator");
      var oclIsTypeOf = new NonTerminal("oclIsTypeOf");
      var ValueExp = new NonTerminal("ValueExp");
      var unValueExp = new NonTerminal("unValueExp");
      var instanceMemberAccess = new NonTerminal("instanceMemberAccess");
      var instanceMathodAccess = new NonTerminal("instanceMathodAccess");
      #endregion

      #region Rules

      #region specification
      specification.Rule = "specification" + identifier + lbr + specificationBody + rbr;
      specificationBody.Rule = MakePlusRule(packages, package);
      #endregion

      #region package
      package.Rule = oclPackage | emofOclPackage;
      oclPackage.Rule = _package + identifier + packageBody + "endpackage";
      emofOclPackage.Rule = _package + identifier + lbr + packageBody + rbr;
      packageBody.Rule = MakeStarRule(packageBody, null, packageBodyItem);
      packageBodyItem.Rule = primitive | enumeration | classType | contextExp | package;
      #endregion

      #region primitive
      primitive.Rule = "primitive" + primitiveTypes + identifier + "IDENTICAL";
      primitiveTypes.Rule = ToTerm("Real") | ToTerm("Integer") | ToTerm("String") | ToTerm("Boolean");
      #endregion

      #region enumeration
      enumeration.Rule = "enumeration" + identifier + lbr + delimitedIdentifier + rbr;
      delimitedIdentifier.Rule = MakePlusRule(delimitedIdentifier, comma, identifier);
      #endregion

      #region class
      classType.Rule = classSignature + lbr + classBody + rbr;
      classSignature.Rule = ToTerm("class") + identifier
                             | ToTerm("abstract") + ToTerm("class") + identifier
                             | ToTerm("abstract") + ToTerm("class") + identifier + ToTerm("extends") + identifier
                             | ToTerm("class") + identifier + ToTerm("extends") + type;
      classBody.Rule = MakeStarRule(classBody, classBodyItem);
      classBodyItem.Rule = attributeDiclaration | functionDefination | invExp;
      #endregion

      #region attribute
      attributeDiclaration.Rule = oclAttributeDiclaration | emofAttributeDiclaration;
      oclAttributeDiclaration.Rule = def + colon + identifier + type + ReduceHere()
                                     | def + colon + identifier + type + PreferShiftHere() + equalsTo + expression + ReduceHere()
                                     | def + colon + "attr" + identifier + type + PreferShiftHere() + equalsTo + expression + ReduceHere()
                                     | def + colon + "attr" + identifier + type + ReduceHere();
      emofAttributeDiclaration.Rule = identifier + colon + type + ReduceHere()
                                      | ToTerm("component") + identifier + colon + type + ToTerm("opposite") + identifier + ReduceHere()
                                      | ToTerm("component") + identifier + colon + type + ToTerm("unique") + ToTerm("opposite") + identifier + ReduceHere()
                                      | identifier + colon + type + equalsTo + expression + ReduceHere()
                                      | identifier + colon + type + ToTerm("opposite") + identifier + ReduceHere()
                                      | identifier + colon + type + ToTerm("unique") + ToTerm("opposite") + identifier + ReduceHere()
                                      | identifier + colon + type + ToTerm("unique") + ReduceHere();
      #endregion

      #region type
      type.Rule = primitiveTypes | customType;
      customType.Rule = fullType | globalType | ArrayType | collectionTpye | tupleExp;
      fullType.Rule = identifier + scopeIn + fullType | identifier + ReduceHere();
      globalType.Rule = PreferShiftHere() + scopeIn + fullType
                       | PreferShiftHere() + scopeIn + ArrayType;
      ArrayType.Rule = fullType + "[" + "*" + "]"
                       | fullType + "[" + identifier + "]"
                       | fullType + "[" + number + "]"
                       | fullType + "[" + number + comma + number + "]"
                       | fullType + "[" + number + comma + "*" + "]";
      collectionTpye.Rule = setType | orderedSetType | sequenceType | bagType | collection;
      setType.Rule = "Set(" + customType + ")";
      orderedSetType.Rule = "OrderedSet(" + customType + ")";
      sequenceType.Rule = "Sequence(" + customType + ")";
      bagType.Rule = "Bag(" + customType + ")";
      collection.Rule = "Collection(" + customType + ")";
      tupleExp.Rule = "Tuple" + lbr + argumentList + rbr;
      #endregion

      #region function
      functionDefination.Rule = functionSignature + functionBody;
      functionSignature.Rule = oclFunctionSignature | emofFunctionSignature;
      oclFunctionSignature.Rule = def + colon + functionSignatureBody;
      functionSignatureBody.Rule = identifier + lpar + argumentList + rpar + colon + type
                                   | identifier + lpar + argumentList + rpar + colon + type + ToTerm("ordered") + ToTerm("unique")
                                   | identifier + lpar + argumentList + rpar + colon;
      argumentList.Rule = MakeStarRule(argumentList, comma, variableDiclaration);
      variableDiclaration.Rule = identifier + PreferShiftHere() + colon + type;
      emofFunctionSignature.Rule = functionSignatureBody;
      functionBody.Rule = oclFunctionBody | emofFunctionBody;
      oclFunctionBody.Rule = equalsTo + expression;
      emofFunctionBody.Rule = equalsTo + lbr + expression + rbr;
      #endregion

      #region expression
      expression.Rule = valueExp | structuralExp;
      #endregion

      #region valueExp
      valueExp.Rule = // ValueExp + ReduceHere() |
                    unValueExp + ReduceHere()
                    | valueExp + PreferShiftHere() + binaryOperator + valueExp
                    | ToTerm("not") + valueExp
                    | lpar + valueExp + rpar;
      #endregion

      #region unValueExp

      unValueExp.Rule = booleanValueCollection
                             | ToTerm("false") | ToTerm("true")
                             | collectionValue
                             | tupleLiteral
                             | singleValueCollectionMember
                             | collectionValueExp
                             | iterateExp
                             | stringLiteral
                             | charLiteral
                             | number
                             | self
                             | oclIsTypeOf
                             | unValueExp + lpar + parameterList + rpar
                             | instanceMemberAccess
                             | instanceMathodAccess
                             | ToTerm("-") + number;
      #endregion

      #region ValueExp
      ValueExp.Rule = fullType + dot + instanceMemberAccess
                          | globalType + dot + instanceMemberAccess
                          | ValueExp + dot + identifier + lpar + parameterList + rpar;
      #endregion

      #region instanceMemberAccess
      instanceMemberAccess.Rule = MakePlusRule(instanceMemberAccess, dot, identifier);
      instanceMathodAccess.Rule = unValueExp + dot + identifier + lpar + parameterList + rpar;
      #endregion

      #region parameterList
      parameterList.Rule = MakeStarRule(parameterList, comma, expression);
      #endregion

      #region booleanExp
      booleanValueCollection.Rule = includesOperation | includesAllOperation | excludesOperation | excludesAllOperation
                        | isEmptyOperation | notEmptyOperation | existsExp | forAllExp | isUniqueExp | anyExp | oneExp;
      #endregion

      #region oclBuildInMethods
      oclIsTypeOf.Rule = valueExp + dot + ToTerm("oclIsTypeOf") + lpar + type + rpar;
      #endregion

      #region binaryOperator

      binaryOperator.Rule = comparisonOperator | arithmaticOperator | binaryBooleanOperator;
      arithmaticOperator.Rule = ToTerm("/") | ToTerm("div") | ToTerm("*") | ToTerm("+") | ToTerm("-");
      comparisonOperator.Rule = ToTerm(">") | ToTerm("<") | ToTerm(">=") | ToTerm("<=") | ToTerm("<>") | equalsTo;
      binaryBooleanOperator.Rule = ToTerm("and") | ToTerm("or") | ToTerm("xor") | ToTerm("implies");

      #endregion

      #region booleanValueCollection
      includesOperation.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("includes") + lpar + valueExp + rpar;
      includesAllOperation.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("includesAll") + lpar + identifier + rpar;
      excludesOperation.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("excludes") + lpar + identifier + rpar;
      excludesAllOperation.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("excludesAll") + lpar + identifier + rpar;
      isEmptyOperation.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("isEmpty") + lpar + rpar;
      notEmptyOperation.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("notEmpty") + lpar + rpar;
      existsExp.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("exists") + lpar + specialIteratorBodyExp + rpar;
      forAllExp.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("forAll") + lpar + specialIteratorBodyExp + rpar;
      isUniqueExp.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("isUnique") + lpar + specialIteratorBodyExp + rpar;
      anyExp.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("any") + lpar + specialIteratorBodyExp + rpar;
      oneExp.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("one") + lpar + specialIteratorBodyExp + rpar;
      specialIteratorBodyExp.Rule = delimitedIdentifier + colon + type + suchThat + valueExp
                                  | argumentList + suchThat + valueExp
                                  | valueExp;
      #endregion

      #region collectionValue
      collectionValue.Rule = setLiteral | bagLiteral | orderedSetLiteral | sequenceLiteral;
      setLiteral.Rule = "Set" + lbr + literalList + rbr
                         | "Set" + lpar + type + rpar + lbr + literalList + rbr;
      bagLiteral.Rule = "Bag" + lbr + literalList + rbr
                        | "Bag" + lpar + type + rpar + lbr + literalList + rbr;
      orderedSetLiteral.Rule = "OrderedSet" + lbr + literalList + rbr
                               | "OrderedSet" + lpar + type + rpar + lbr + literalList + rbr;
      sequenceLiteral.Rule = "Sequence" + lbr + literalList + rbr
                             | "Sequence" + lpar + type + rpar + lbr + literalList + rbr;
      literalList.Rule = MakeStarRule(literalList, comma, valueExp);
      tupleLiteral.Rule = "Tuple" + lbr + tuplElementList + rbr;
      tuplElementList.Rule = MakePlusRule(tuplElementList, comma, tupleElement);
      tupleElement.Rule = variableDiclaration + equalsTo + valueExp | identifier + equalsTo + valueExp;
      collectionValueExp.Rule = includingOperation | excludingOperation | selectExp | rejectExp
                               | union | intersection | insertAt | append | prepend | symmetricDifference | subOrderedSet | collect | productExp
                               | subSequence | flatten | asSet | asOrderdSet | asSequence | asBag;
      includingOperation.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("including") + lpar + identifier + rpar;
      excludingOperation.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("excluding") + lpar + identifier + rpar;
      selectExp.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("select") + lpar + specialIteratorBodyExp + rpar;
      rejectExp.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("reject") + lpar + specialIteratorBodyExp + rpar;
      union.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("union") + lpar + valueExp + rpar;
      intersection.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("intersection") + lpar + identifier + rpar;
      insertAt.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("insertAt") + lpar + number + comma + identifier + rpar;
      append.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("append") + lpar + identifier + rpar;
      prepend.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("prepend") + lpar + identifier + rpar;
      symmetricDifference.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("symmetricDifference") + lpar + identifier + rpar;
      subOrderedSet.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("subOrderedSet") + lpar + number + comma + number + rpar;
      collect.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("collect") + lpar + specialIteratorBodyExp + rpar
                   | valueExp + PreferShiftHere() + propertyIs + ToTerm("collect") + lpar + identifier + rpar
                   | valueExp + dot + identifier + lpar + argumentList + rpar
                   | valueExp + PreferShiftHere() + propertyIs + identifier + lpar + argumentList + rpar;
      productExp.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("product") + lpar + identifier + rpar;
      subSequence.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("subSequence") + lpar + number + comma + number + rpar;
      flatten.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("flatten") + lpar + rpar;
      asSet.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("asSet") + lpar + rpar;
      asOrderdSet.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("asOrderdSet") + lpar + rpar;
      asSequence.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("asSequence") + lpar + rpar;
      asBag.Rule = valueExp + PreferShiftHere() + propertyIs + ToTerm("asBag") + lpar + rpar;

      #endregion

      #region singleValueCollectionMember
      singleValueCollectionMember.Rule = maxOperation | minOperation | countOperation | at | indexOf | first | last | sum | size;
      maxOperation.Rule = valueExp + PreferShiftHere() + propertyIs + "max" + lpar + rpar;
      minOperation.Rule = valueExp + PreferShiftHere() + propertyIs + "min" + lpar + rpar;
      countOperation.Rule = valueExp + PreferShiftHere() + propertyIs + "count" + lpar + identifier + rpar;
      at.Rule = valueExp + PreferShiftHere() + propertyIs + "at" + lpar + identifier + rpar;
      indexOf.Rule = valueExp + PreferShiftHere() + propertyIs + "indexOf" + lpar + identifier + rpar;
      first.Rule = valueExp + PreferShiftHere() + propertyIs + "first" + lpar + rpar;
      last.Rule = valueExp + PreferShiftHere() + propertyIs + "last" + lpar + rpar;
      sum.Rule = valueExp + PreferShiftHere() + propertyIs + "sum" + lpar + rpar;
      size.Rule = valueExp + PreferShiftHere() + propertyIs + "size" + lpar + rpar;
      #endregion

      #region iterateExp
      iterateExp.Rule = valueExp + PreferShiftHere() + propertyIs + "iterate" + lpar + iteratorList + ";"
                        + iteratorInitializationList + suchThat + expression + rpar;
      iteratorList.Rule = MakePlusRule(iteratorList, comma, variableDiclaration);
      iteratorInitializationList.Rule = MakePlusRule(iteratorInitializationList, comma, iteratorInitialization);
      iteratorInitialization.Rule = variableDiclaration + equalsTo + valueExp;
      #endregion

      #region structuralExp
      structuralExp.Rule = ifThenElseExp | letExp;
      #endregion

      #region ifThenElseExp
      ifThenElseExp.Rule = "if" + valueExp + "then" + expression + "endif"
                          | "if" + valueExp + "then" + expression + "else" + expression + "endif";
      #endregion

      #region letExp
      letExp.Rule = let + variableDiclaration + equalsTo + valueExp + In + expression
                   | let + identifier + equalsTo + valueExp + In + expression;
      #endregion

      #region invExp
      invExp.Rule = oclInvExp | emofInvExp;
      oclInvExp.Rule = inv + identifier + colon + expression
                       | inv + colon + expression;
      emofInvExp.Rule = "invariant" + lbr + expression + rbr;
      #endregion

      #region contextExp
      contextExp.Rule = classContextExp | attributeContextExp | operationContextExp;
      #endregion

      #region classContextExp
      classContextExp.Rule = classContextSignature + classBody;
      classContextSignature.Rule = context + type
                                   | context + identifier + colon + type;
      classBody.Rule = MakePlusRule(classBody, null, classBodyItem);
      classBodyItem.Rule = invExp | attributeDiclaration | functionDefination;
      #endregion

      #region attributeContextExp
      attributeContextExp.Rule = context + type + colon + type + attributeContextbody;
      attributeContextbody.Rule = MakePlusRule(attributeContextbody, null, attributeContextbodyItem);
      attributeContextbodyItem.Rule = initExp | deriveExp;
      #endregion

      #region initExp
      initExp.Rule = ToTerm("init") + colon + equalsTo + valueExp;
      #endregion

      #region deriveExp
      deriveExp.Rule = ToTerm("derive") + colon + expression;
      #endregion

      #region operationContextExp
      operationContextExp.Rule = operationContextSignature + invExp + operationContextBodyExps;
      operationContextSignature.Rule = context + customType + PreferShiftHere() + scopeIn + functionSignature;
      operationContextBodyExps.Rule = MakePlusRule(operationContextBodyExps, null, operationContextBodyItem);
      operationContextBodyItem.Rule = preCondition | bodyExp | postCondition;
      #endregion

      #region preCondition
      preCondition.Rule = pre + identifier + colon + valueExp
                         | pre + colon + valueExp;
      #endregion

      #region bodyExp
      bodyExp.Rule = body + colon + expression;
      #endregion

      #region postCondition
      postCondition.Rule = post + colon + valueExp
                           | post + identifier + colon + valueExp;
      #endregion

      #endregion

      #region Operator,punctuation
      MarkPunctuation(",", "(", ")", "{", "}", "[", "]", ":");
      MarkMemberSelect(".", "->"); //dot, propertyIs);
      RegisterOperators(1, lpar, rpar);
      RegisterOperators(2, "let", "in");
      RegisterOperators(3, letExp, In);
      RegisterOperators(4, dot, scopeIn);
      RegisterOperators(5, not);
      RegisterOperators(6, "*", "/");
      RegisterOperators(7, letExp, In);
      RegisterOperators(8, dot, scopeIn);
      RegisterOperators(9, "+", "-");
      RegisterOperators(10, "<", ">", "<=", ">=");
      RegisterOperators(11, "=", "<>");
      RegisterOperators(12, and);
      RegisterOperators(12, "div", "implies");
      RegisterOperators(13, or);
      RegisterOperators(14, xor);
      RegisterOperators(15, "=", "<>");
      #endregion
      this.Root = specification;
    }

    public override void SkipWhitespace(ISourceStream source) {
      while (!source.EOF()) {
        var ch = source.PreviewChar;
        switch (ch) {
          case ' ':
          case '\t':
          case '\r':
          case '\n':
          case '\v':
          case '\u2085':
          case '\u2028':
          case '\u2029':
            source.PreviewPosition++;
            break;
          default:
            //Check unicode class Zs
            UnicodeCategory chCat = char.GetUnicodeCategory(ch);
            if (chCat == UnicodeCategory.SpaceSeparator) //it is whitespace, continue moving
              continue;//while loop 
            //Otherwize return
            return;
        }//switch
      }//while
    }

  }
}