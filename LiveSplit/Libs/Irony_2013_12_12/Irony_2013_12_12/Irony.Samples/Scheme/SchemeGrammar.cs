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
using System.Text;
using Irony.Parsing;
using Irony.Interpreter.Ast;

namespace Irony.Samples.Scheme {
  [Language("Scheme", "1.0", "Sample Scheme grammar")]
  public class SchemeGrammar : Grammar {
    // It is loosely based on R6RS specs.  
    // See Grammar Errors tab in GrammarExplorer for remaining conflicts.
    public SchemeGrammar() {

      #region Terminals
      ConstantTerminal Constant = new ConstantTerminal("Constant", typeof(LiteralValueNode));
      Constant.Add("#T", 1);
      Constant.Add("#t", 1);
      Constant.Add("#F", null);
      Constant.Add("#f", null);
      Constant.Add("'()", null);
      Constant.Add(@"#\nul", '\u0000');
      Constant.Add(@"#\alarm", '\u0007');
      Constant.Add(@"#\backspace", '\b');
      Constant.Add(@"#\tab", '\t');
      Constant.Add(@"#\linefeed", '\n');
      Constant.Add(@"#\vtab", '\v');
      Constant.Add(@"#\page", '\f');
      Constant.Add(@"#\return", '\r');
      Constant.Add(@"#\esc", '\u001B');
      Constant.Add(@"#\space", ' ');
      Constant.Add(@"#\delete", '\u007F');

      // TODO: build SchemeCharLiteral
      // the following is nonsense, just to put something there
      var charLiteral = new StringLiteral("Char", "'", StringOptions.None); 
      var stringLiteral = new StringLiteral("String", "\"", StringOptions.AllowsAllEscapes);
      //Identifiers. Note: added "-", just to allow IDs starting with "->" 
      var SimpleIdentifier = new IdentifierTerminal("SimpleIdentifier", "_+-*/.@?!<>=", "_+-*/.@?!<>=$%&:^~");
      //                                                           name                extraChars      extraFirstChars  
      var Number = TerminalFactory.CreateSchemeNumber("Number");
      var Byte = new NumberLiteral("Byte", NumberOptions.IntOnly); 

      //Comments
      Terminal Comment = new CommentTerminal("Comment", "#|", "|#");
      Terminal LineComment = new CommentTerminal("LineComment", ";", "\n");
      NonGrammarTerminals.Add(Comment); //add comments explicitly to this list as it is not reachable from Root
      NonGrammarTerminals.Add(LineComment);
      #endregion

      #region NonTerminals
      var Module = new NonTerminal("Module");
      var Library = new NonTerminal("Library");
      var LibraryList = new NonTerminal("Library+");
      var Script = new NonTerminal("Script");

      var Abbreviation = new NonTerminal("Abbreviation");
      var Vector = new NonTerminal("Vector");
      var ByteList = new NonTerminal("ByteList");
      var ByteVector = new NonTerminal("ByteVector");
      var Datum = new NonTerminal("Datum"); //Datum in R6RS terms
      var DatumOpt = new NonTerminal("DatumOpt"); //Datum in R6RS terms
      var DatumList = new NonTerminal("Datum+");
      var DatumListOpt = new NonTerminal("Datum*");
      var Statement = new NonTerminal("Statement");
      var Atom = new NonTerminal("Atom");
      var CompoundDatum = new NonTerminal("CompoundDatum");
      var AbbrevPrefix = new NonTerminal("AbbrevPrefix");

      var LibraryName = new NonTerminal("LibraryName");
      var LibraryBody = new NonTerminal("LibraryBody");
      var ImportSection = new NonTerminal("ImportSection");
      var ExportSection = new NonTerminal("ExportSection");
      var ImportSpec = new NonTerminal("ImportSpec");
      var ImportSpecList = new NonTerminal("ImportSpecList");
      var ExportSpec = new NonTerminal("ExportSpec");
      var ExportSpecList = new NonTerminal("ExportSpecList");
      var LP = new NonTerminal("LP"); //"(" or "["
      var RP = new NonTerminal("RP"); // ")" or "]"
      var Identifier = new NonTerminal("Identifier");
      var IdentifierList = new NonTerminal("IdentifierList");
      var IdentifierListOpt = new NonTerminal("IdentifierListOpt");
      var PeculiarIdentifier = new NonTerminal("PeculiarIdentifier");
      var LibraryVersion = new NonTerminal("LibraryVersion");
      var VersionListOpt = new NonTerminal("VersionListOpt");

      var FunctionCall = new NonTerminal("FunctionCall", typeof(FunctionCallNode));
      var FunctionRef = new NonTerminal("FunctionRef"); //transient
      var SpecialForm = new NonTerminal("SpecialForm"); //transient
      var DefineVarForm = new NonTerminal("DefineVarForm", typeof(AssignmentNode));
      var DefineFunForm = new NonTerminal("DefineFunForm", typeof(FunctionDefNode));
      var LambdaForm = new NonTerminal("LambdaForm", typeof(LambdaNode));
      var IfForm = new NonTerminal("IfForm", typeof(IfNode));
      var CondForm = new NonTerminal("CondForm");
      var CondClause = new NonTerminal("CondClause");
      var CondClauseList = new NonTerminal("CondClauseList");
      var CondElseOpt = new NonTerminal("CondElseOpt");
      var BeginForm = new NonTerminal("BeginForm", typeof(StatementListNode));
      var LetForm = new NonTerminal("LetForm"); //not implemented
      var LetRecForm = new NonTerminal("LetRecForm"); //not implemented
      var LetPair = new NonTerminal("LetPair");
      var LetPairList = new NonTerminal("LetPairList");
      #endregion

      #region Rules
      base.Root = Module; 

      LP.Rule = ToTerm("(") | "[";  //R6RS allows mix & match () and []
      RP.Rule = ToTerm(")") | "]";

      // Module.Rule = LibraryListOpt + Script; -- this brings conflicts
      Module.Rule = LibraryList + Script | Script;
      LibraryList.Rule = MakePlusRule(LibraryList, Library);
      Script.Rule = ImportSection + DatumList | DatumList; 

      //Library
      // the following doesn't work - brings conflicts that incorrectly resolved by default shifting
      //Library.Rule = LP + "library" + LibraryName + ExportSectionOpt + ImportSectionOpt + DatumListOpt + RP;
      Library.Rule = LP + "library" + LibraryName + LibraryBody + RP;
      //Note - we should be using DatumListOpt, but that brings 2 conflicts, so for now it is just DatumList
      //Note that the following style of BNF expressions is strongly discouraged - all productions should be of the same length, 
      // so that the process of mapping child nodes to parent's properties is straightforward.
      LibraryBody.Rule = ExportSection + ImportSection + DatumList
                       | ExportSection + DatumList
                       | ImportSection + DatumList
                       | DatumList; 
      LibraryName.Rule = LP + IdentifierList + LibraryVersion.Q() + RP;
      LibraryVersion.Rule = LP + VersionListOpt + RP; //zero or more subversion numbers
      VersionListOpt.Rule = MakeStarRule(VersionListOpt, Number);
      ExportSection.Rule = LP + "export" + ExportSpecList + RP;
      ImportSection.Rule = LP + "import" + ImportSpecList + RP;
      ExportSpecList.Rule = MakePlusRule(ExportSpecList, ExportSpec);
      ImportSpecList.Rule = MakePlusRule(ImportSpecList, ImportSpec);
      ExportSpec.Rule = Identifier | LP + "rename"  +  LP + Identifier + Identifier + RP + RP;
      ImportSpec.Rule = LP + Identifier + RP;   // - much more complex in R6RS

      //Datum
      Datum.Rule = Atom | CompoundDatum;
      DatumOpt.Rule = Empty | Datum;
      DatumList.Rule = MakePlusRule(DatumList, Datum);
      DatumListOpt.Rule = MakeStarRule(DatumListOpt, Datum);
      Atom.Rule = Number | Identifier | stringLiteral | Constant | charLiteral | ".";
      CompoundDatum.Rule = Statement | Abbreviation | Vector | ByteVector;
      Identifier.Rule = SimpleIdentifier | PeculiarIdentifier;
      IdentifierList.Rule = MakePlusRule(IdentifierList, Identifier);
      IdentifierListOpt.Rule = MakeStarRule(IdentifierListOpt, Identifier);

      //TODO: create PeculiarIdentifier custom terminal instead of var 
      // or just custom SchemeIdentifier terminal
      PeculiarIdentifier.Rule = ToTerm("+") | "-" | "..."; // |"->" + subsequent; (should be!) 
      Abbreviation.Rule = AbbrevPrefix + Datum;
      AbbrevPrefix.Rule = ToTerm("'") | "`" | ",@" | "," | "#'" | "#`" | "#,@" | "#,";
      Vector.Rule = "#(" + DatumListOpt + ")";
      ByteVector.Rule = "#vu8(" + ByteList + ")";
      ByteList.Rule = MakeStarRule(ByteList, Byte);

      Statement.Rule = FunctionCall | SpecialForm;

      FunctionCall.Rule = LP + FunctionRef + DatumListOpt + RP;
      FunctionRef.Rule = Identifier | Statement;

      SpecialForm.Rule = DefineVarForm | DefineFunForm | LambdaForm | IfForm | CondForm | BeginForm | LetForm | LetRecForm;
      DefineVarForm.Rule = LP + "define" + Identifier + Datum + RP;
      DefineVarForm.AstConfig.PartsMap = new int[] { 1, 2 };

      DefineFunForm.Rule = LP + "define" + LP + Identifier + IdentifierListOpt + RP + DatumList + RP;
      LambdaForm.Rule = LP + "lambda" + LP + IdentifierListOpt + RP + DatumList + RP;
      IfForm.Rule = LP + "if" + Datum + Datum + DatumOpt + RP;

      CondForm.Rule = LP + "cond" + CondClauseList + CondElseOpt + RP;
      CondClauseList.Rule = MakePlusRule(CondClauseList, CondClause);
      CondClause.Rule = LP + Datum + DatumList + RP;
      CondElseOpt.Rule = Empty | LP + "else" + DatumList + RP;
      LetForm.Rule = LP + "let" + LP + LetPairList + RP + DatumList + RP;
      LetRecForm.Rule = LP + "letrec" + LP + LetPairList + RP + DatumList + RP;
      BeginForm.Rule = LP + "begin" + DatumList + RP;
      LetPairList.Rule = MakePlusRule(LetPairList, LetPair);
      LetPair.Rule = LP + Identifier + Datum + RP;
      #endregion 

      //Register brace  pairs
      RegisterBracePair("(", ")"); 
      RegisterBracePair("[", "]");

      MarkPunctuation(LP, RP);
      MarkTransient(Datum, CompoundDatum, Statement, SpecialForm, Atom, FunctionRef); 

      //Scheme is tail-recursive language
      base.LanguageFlags |= LanguageFlags.TailRecursive;// | LanguageFlags.CreateAst; 

    }//constructor


  }//class

  


}//namespace
