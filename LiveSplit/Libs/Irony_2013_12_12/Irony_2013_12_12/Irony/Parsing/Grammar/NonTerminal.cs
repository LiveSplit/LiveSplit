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
using System.Reflection;
using Irony.Ast; 

namespace Irony.Parsing {

  internal class IntList : List<int> { }

  public partial class NonTerminal : BnfTerm {

    #region constructors
    public NonTerminal(string name)  : base(name, null) { }  //by default display name is null
    public NonTerminal(string name, string errorAlias) : base(name, errorAlias) { }
    public NonTerminal(string name, string errorAlias, Type nodeType) : base(name, errorAlias, nodeType ) { }
    public NonTerminal(string name, string errorAlias,  AstNodeCreator nodeCreator) : base(name, errorAlias, nodeCreator) {}
    public NonTerminal(string name, Type nodeType) : base(name, null, nodeType) { }
    public NonTerminal(string name, AstNodeCreator nodeCreator) : base(name, null, nodeCreator) { }
    public NonTerminal(string name, BnfExpression expression)
      : this(name) { 
      Rule = expression;
    }
    #endregion

    #region properties/fields: Rule, ErrorRule

    public BnfExpression Rule; 
    //Separate property for specifying error expressions. This allows putting all such expressions in a separate section
    // in grammar for all non-terminals. However you can still put error expressions in the main Rule property, just like
    // in YACC
    public BnfExpression ErrorRule;

    //A template for representing ParseTreeNode in the parse tree. Can contain '#{i}' fragments referencing 
    // child nodes by index
    public string NodeCaptionTemplate;
    //Converted template with index list
    private string _convertedTemplate;
    private IntList _captionParameters;

    // Productions are used internally by Parser builder
    internal ProductionList Productions = new ProductionList();
    #endregion

    #region Events: Reduced
    //Note that Reduced event may be called more than once for a List node 
    public event EventHandler<ReducedEventArgs> Reduced;
    internal void OnReduced(ParsingContext context, Production reducedProduction, ParseTreeNode resultNode) {
      if (Reduced != null)
        Reduced(this, new ReducedEventArgs(context, reducedProduction, resultNode));
    }
    #endregion

    #region overrides: ToString, Init
    public override string ToString() {
      return Name;
    }
    public override void Init(GrammarData grammarData) {
      base.Init(grammarData);
      if (!string.IsNullOrEmpty(NodeCaptionTemplate))
        ConvertNodeCaptionTemplate();
    }
    #endregion

    // Contributed by Alexey Yakovlev (yallie)
    #region Grammar hints
    // Adds a hint at the end of all productions
    public void AddHintToAll(GrammarHint hint) {
      if (this.Rule == null)
        throw new Exception("Rule property must be set on non-terminal before calling AddHintToAll.");
      foreach (var plusList in this.Rule.Data)
        plusList.Add(hint);
    }

    #endregion

    #region NodeCaptionTemplate utilities
    //We replace original tag '#{i}'  (where i is the index of the child node to put here)
    // with the tag '{k}', where k is the number of the parameter. So after conversion the template can 
    // be used in string.Format() call, with parameters set to child nodes captions
    private void ConvertNodeCaptionTemplate() {
      _captionParameters = new IntList();
      _convertedTemplate = NodeCaptionTemplate;
      var index = 0; 
      while(index < 100) {
        var strParam = "#{" + index + "}";
        if(_convertedTemplate.Contains(strParam)) {
          _convertedTemplate = _convertedTemplate.Replace(strParam, "{" + _captionParameters.Count + "}"); 
          _captionParameters.Add(index);
        }
        if (!_convertedTemplate.Contains("#{")) return;
        index++; 
      }//while
    }//method

    public string GetNodeCaption(ParseTreeNode node) {
      var paramValues = new string[_captionParameters.Count];
      for(int i = 0; i < _captionParameters.Count; i++) {
        var childIndex = _captionParameters[i];
        if(childIndex < node.ChildNodes.Count) {
          var child = node.ChildNodes[childIndex];
          //if child is a token, then child.ToString returns token.ToString which contains Value + Term; 
          // in this case we prefer to have Value only
          paramValues[i] = (child.Token != null? child.Token.ValueString : child.ToString());
        }
      }
      var result = string.Format(_convertedTemplate, paramValues); 
      return result; 
    }
    #endregion 

  }//class

  public class NonTerminalList : List<NonTerminal> {
    public override string ToString() {
      return string.Join(" ", this); 
    }
  }

  public class NonTerminalSet : HashSet<NonTerminal> {
    public override string ToString() {
      return string.Join(" ", this);
    }
  }


}//namespace
