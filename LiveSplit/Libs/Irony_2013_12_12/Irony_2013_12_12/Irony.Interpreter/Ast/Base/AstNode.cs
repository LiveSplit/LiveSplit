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
using System.Linq.Expressions;
using System.Text;
using System.CodeDom;
using System.Xml;
using System.IO;

using Irony.Ast;
using Irony.Parsing;
using Irony.Interpreter;

namespace Irony.Interpreter.Ast {

  public static class CustomExpressionTypes {
    public const ExpressionType NotAnExpression =(ExpressionType) (-1);
  }

  public class AstNodeList : List<AstNode> { }

  //Base AST node class
  public partial class AstNode : IAstNodeInit, IBrowsableAstNode, IVisitableNode {
    public AstNode Parent;
    public BnfTerm Term;
    public SourceSpan Span { get; set; }
    public AstNodeFlags Flags;
    protected ExpressionType ExpressionType = CustomExpressionTypes.NotAnExpression;
    protected object LockObject = new object();

    //Used for pointing to error location. For most nodes it would be the location of the node itself.
    // One exception is BinExprNode: when we get "Division by zero" error evaluating 
    //  x = (5 + 3) / (2 - 2)
    // it is better to point to "/" as error location, rather than the first "(" - which is the start 
    // location of binary expression. 
    public SourceLocation ErrorAnchor;
    //UseType is set by parent
    public NodeUseType UseType = NodeUseType.Unknown;
    // Role is a free-form string used as prefix in ToString() representation of the node. 
    // Node's parent can set it to "property name" or role of the child node in parent's node currentFrame.Context. 
    public string Role;
    // Default AstNode.ToString() returns 'Role: AsString', which is used for showing node in AST tree. 
    public virtual string AsString { get; protected set; }
    public readonly AstNodeList ChildNodes = new AstNodeList();  //List of child nodes

    //Reference to Evaluate method implementation. Initially set to DoEvaluate virtual method. 
    public EvaluateMethod Evaluate;
    public ValueSetterMethod SetValue; 

    // Public default constructor
    public AstNode() {
      this.Evaluate = DoEvaluate;
      this.SetValue = DoSetValue;
    }
    public SourceLocation Location { get { return Span.Location; } }

    #region IAstNodeInit Members
    public virtual void Init(AstContext context, ParseTreeNode treeNode) {
      this.Term = treeNode.Term;
      Span = treeNode.Span;
      ErrorAnchor = this.Location;
      treeNode.AstNode = this;
      AsString = (Term == null ? this.GetType().Name : Term.Name);
    }
    #endregion

    //ModuleNode - computed on demand
    public AstNode ModuleNode {
      get {
        if (_moduleNode == null) {
          _moduleNode = (Parent == null) ? this : Parent.ModuleNode;
        }
        return _moduleNode;
      }
      set { _moduleNode = value; }
    }  AstNode _moduleNode;


    #region virtual methods: DoEvaluate, SetValue, IsConstant, SetIsTail, GetDependentScopeInfo
    public virtual void Reset() {
      _moduleNode = null;
      Evaluate = DoEvaluate;
      foreach (var child in ChildNodes)
        child.Reset();
    }

    //By default the Evaluate field points to this method.
    protected virtual object DoEvaluate(ScriptThread thread) {
      //These 2 lines are standard prolog/epilog statements. Place them in every Evaluate and SetValue implementations.
      thread.CurrentNode = this;  //standard prolog
      thread.CurrentNode = Parent; //standard epilog
      return null; 
    }

    public virtual void DoSetValue(ScriptThread thread, object value) {
      //Place the prolog/epilog lines in every implementation of SetValue method (see DoEvaluate above)
    }

    public virtual bool IsConstant() {
      return false; 
    }

    /// <summary>
    /// Sets a flag indicating that the node is in tail position. The value is propagated from parent to children. 
    /// Should propagate this call to appropriate children.
    /// </summary>
    public virtual void SetIsTail() {
      Flags |= AstNodeFlags.IsTail;
    }

    /// <summary>
    /// Dependent scope is a scope produced by the node. For ex, FunctionDefNode defines a scope
    /// </summary>
    public virtual ScopeInfo DependentScopeInfo {
      get {return _dependentScope; }
      set { _dependentScope = value; }
    } ScopeInfo _dependentScope;

    #endregion

    #region IBrowsableAstNode Members
    public virtual System.Collections.IEnumerable GetChildNodes() {
      return ChildNodes;
    }
    public int Position { 
      get { return Span.Location.Position; } 
    }
    #endregion

    #region Visitors, Iterators
    //the first primitive Visitor facility
    public virtual void AcceptVisitor(IAstVisitor visitor) {
      visitor.BeginVisit(this);
      if (ChildNodes.Count > 0)
        foreach(AstNode node in ChildNodes)
          node.AcceptVisitor(visitor);
      visitor.EndVisit(this);
    }

    //Node traversal 
    public IEnumerable<AstNode> GetAll() {
      AstNodeList result = new AstNodeList();
      AddAll(result);
      return result; 
    }
    private void AddAll(AstNodeList list) {
      list.Add(this);
      foreach (AstNode child in this.ChildNodes)
        if (child != null) 
          child.AddAll(list);
    }
    #endregion

    #region overrides: ToString
    public override string ToString() {
      return string.IsNullOrEmpty(Role) ? AsString : Role + ": " + AsString;
    }
    #endregion

    #region Utility methods: AddChild, HandleError

    protected AstNode AddChild(string role, ParseTreeNode childParseNode) {
      return AddChild(NodeUseType.Unknown, role, childParseNode);
    }

    protected AstNode AddChild(NodeUseType useType, string role, ParseTreeNode childParseNode) {
      var child = (AstNode)childParseNode.AstNode;
      if (child == null)
        child = new NullNode(childParseNode.Term); //put a stub to throw an exception with clear message on attempt to evaluate. 
      child.Role = role;
      child.Parent = this;
      ChildNodes.Add(child);
      return child;
    }

    #endregion

  }//class
}//namespace
