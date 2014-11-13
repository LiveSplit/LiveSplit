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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Irony.Ast;

namespace Irony.Parsing { 
  [Flags]
  public enum TermFlags {
    None = 0,
    IsOperator =         0x01,
    IsOpenBrace =        0x02,
    IsCloseBrace =       0x04,
    IsBrace = IsOpenBrace | IsCloseBrace,
    IsLiteral  =         0x08,

    IsConstant =         0x10,
    IsPunctuation =      0x20,
    IsDelimiter =        0x40,
    IsReservedWord =    0x080,
    IsMemberSelect =    0x100,
    InheritPrecedence = 0x200, // Signals that non-terminal must inherit precedence and assoc values from its children. 
                               // Typically set for BinOp nonterminal (where BinOp.Rule = '+' | '-' | ...) 

    IsNonScanner =    0x01000,  // indicates that tokens for this terminal are NOT produced by scanner 
    IsNonGrammar =    0x02000,  // if set, parser would eliminate the token from the input stream; terms in Grammar.NonGrammarTerminals have this flag set
    IsTransient =     0x04000,  // Transient non-terminal - should be replaced by it's child in the AST tree.
    IsNotReported =   0x08000,  // Exclude from expected terminals list on syntax error
    
    //calculated flags
    IsNullable =     0x010000,
    IsVisible =      0x020000,
    IsKeyword =      0x040000,
    IsMultiline =    0x100000,
    //internal flags
    IsList              = 0x200000,
    IsListContainer     = 0x400000,
    //Indicates not to create AST node; mainly to suppress warning message on some special nodes that AST node type is not specified
    //Automatically set by MarkTransient method
    NoAstNode           = 0x800000,
    //A flag to suppress automatic AST creation for child nodes in global AST construction. Will be used to supress full 
    // "compile" of method bodies in modules. The module might be large, but the running code might 
    // be actually using only a few methods or global members; so in this case it makes sense to "compile" only global/public
    // declarations, including method headers but not full bodies. The body will be compiled on the first call. 
    // This makes even more sense when processing module imports. 
    AstDelayChildren    = 0x1000000,
  
  }

  //Basic Backus-Naur Form element. Base class for Terminal, NonTerminal, BnfExpression, GrammarHint
  public abstract class BnfTerm {
    #region consructors
    public BnfTerm(string name) : this(name, name) { }
    public BnfTerm(string name, string errorAlias, Type nodeType) : this(name, errorAlias) {
      AstConfig.NodeType = nodeType;
    }
    public BnfTerm(string name, string errorAlias, AstNodeCreator nodeCreator) : this(name, errorAlias) {
      AstConfig.NodeCreator = nodeCreator;  
    }
    public BnfTerm(string name, string errorAlias) {
      Name = name;
      ErrorAlias = errorAlias;
      _hashCode = (_hashCounter++).GetHashCode(); 
    }
    #endregion

    #region virtuals and overrides
    public virtual void Init(GrammarData grammarData) {
      GrammarData = grammarData;
    }

    public virtual string GetParseNodeCaption(ParseTreeNode node) {
      if (GrammarData != null)
        return GrammarData.Grammar.GetParseNodeCaption(node);
      else 
        return Name; 
    }

    public override string ToString() {
      return Name;
    }

    //Hash code - we use static counter to generate hash codes
    private static int _hashCounter;
    private int _hashCode;
    public override int GetHashCode() {
      return _hashCode; 
    }
    #endregion 

    public const int NoPrecedence = 0;

    #region properties: Name, DisplayName, Key, Options
    public string Name;
  
    //ErrorAlias is used in error reporting, e.g. "Syntax error, expected <list-of-display-names>". 
    public string ErrorAlias;
    public TermFlags Flags;
    protected GrammarData GrammarData;
    public int Precedence = NoPrecedence;
    public Associativity Associativity = Associativity.Neutral;

    public Grammar Grammar { 
      get { return GrammarData.Grammar; } 
    }
    public void SetFlag(TermFlags flag) {
      SetFlag(flag, true);
    }
    public void SetFlag(TermFlags flag, bool value) {
      if (value)
        Flags |= flag;
      else
        Flags &= ~flag;
    }

    #endregion

    #region events: Shifting
    public event EventHandler<ParsingEventArgs> Shifting;
    public event EventHandler<AstNodeEventArgs> AstNodeCreated; //an event fired after AST node is created. 

    protected internal void OnShifting(ParsingEventArgs args) {
      if (Shifting != null)
        Shifting(this, args);
    }

    protected internal void OnAstNodeCreated(ParseTreeNode parseNode) {
      if (this.AstNodeCreated == null || parseNode.AstNode == null) return;
      AstNodeEventArgs args = new AstNodeEventArgs(parseNode);
      AstNodeCreated(this, args);
    }

    #endregion

    #region AST node creations: AstNodeType, AstNodeCreator, AstNodeCreated
    //We autocreate AST config on first GET;
    public AstNodeConfig AstConfig {
      get {
       if (_astConfig == null) 
         _astConfig = new Ast.AstNodeConfig(); 
        return _astConfig; 
      }
      set {_astConfig = value; }
    } AstNodeConfig _astConfig;

    public bool HasAstConfig() {
      return _astConfig != null; 
    }
    
    #endregion


    #region Kleene operator Q()
    NonTerminal _q; 
    public BnfExpression Q()
    {
      if (_q != null)
        return _q; 
      _q = new NonTerminal(this.Name + "?");
      _q.Rule = this | Grammar.CurrentGrammar.Empty;
      return _q; 
    }
    #endregion

    #region Operators: +, |, implicit
    public static BnfExpression operator +(BnfTerm term1, BnfTerm term2) {
      return Op_Plus(term1, term2);
    }
    public static BnfExpression operator +(BnfTerm term1, string symbol2) {
      return Op_Plus(term1, Grammar.CurrentGrammar.ToTerm(symbol2));
    }
    public static BnfExpression operator +( string symbol1, BnfTerm term2) {
      return Op_Plus(Grammar.CurrentGrammar.ToTerm(symbol1), term2);
    }

    //Alternative 
    public static BnfExpression operator |(BnfTerm term1, BnfTerm term2) {
      return Op_Pipe(term1, term2);
    }
    public static BnfExpression operator |(BnfTerm term1, string symbol2) {
      return Op_Pipe(term1, Grammar.CurrentGrammar.ToTerm(symbol2));
    }
    public static BnfExpression operator |(string symbol1, BnfTerm term2) {
      return Op_Pipe(Grammar.CurrentGrammar.ToTerm(symbol1), term2);
    }

    //BNF operations implementation -----------------------
    // Plus/sequence
    internal static BnfExpression Op_Plus(BnfTerm term1, BnfTerm term2) {
      //Check term1 and see if we can use it as result, simply adding term2 as operand
      BnfExpression expr1 = term1 as BnfExpression;
      if (expr1 == null || expr1.Data.Count > 1) //either not expression at all, or Pipe-type expression (count > 1)
        expr1 = new BnfExpression(term1);
      expr1.Data[expr1.Data.Count - 1].Add(term2);
      return expr1;
    }

    //Pipe/Alternative
    //New version proposed by the codeplex user bdaugherty
    internal static BnfExpression Op_Pipe(BnfTerm term1, BnfTerm term2) {
      BnfExpression expr1 = term1 as BnfExpression;
      if (expr1 == null)
        expr1 = new BnfExpression(term1);
      BnfExpression expr2 = term2 as BnfExpression;
      if (expr2 == null)
        expr2 = new BnfExpression(term2);
      expr1.Data.AddRange(expr2.Data);
      return expr1;
    }


    #endregion


  }//class

  public class BnfTermList : List<BnfTerm> { }
  public class BnfTermSet : HashSet<BnfTerm> {  }



}//namespace

