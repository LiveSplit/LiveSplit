using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Parsing {

  /// <summary>Base class for more specific reduce actions. </summary>
  public partial class ReduceParserAction: ParserAction {
    public readonly Production Production;
  
    public ReduceParserAction(Production production) {
      Production = production; 
    }
    public override string ToString() {
      return string.Format(Resources.LabelActionReduce, Production.ToStringQuoted());
    }

    /// <summary>Factory method for creating a proper type of reduce parser action. </summary>
    /// <param name="production">A Production to reduce.</param>
    /// <returns>Reduce action.</returns>
    public static ReduceParserAction Create(Production production) {
      var nonTerm = production.LValue;
      //List builder (non-empty production for list non-terminal) is a special case 
      var isList = nonTerm.Flags.IsSet(TermFlags.IsList);
      var isListBuilderProduction = isList && production.RValues.Count > 0 && production.RValues[0] == production.LValue;
      if (isListBuilderProduction)
        return new ReduceListBuilderParserAction(production);
      else if (nonTerm.Flags.IsSet(TermFlags.IsListContainer))
        return new ReduceListContainerParserAction(production);
      else if (nonTerm.Flags.IsSet(TermFlags.IsTransient))
        return new ReduceTransientParserAction(production);
      else
        return new ReduceParserAction(production);
    }

    public override void Execute(ParsingContext context) {
      var savedParserInput = context.CurrentParserInput; 
      context.CurrentParserInput = GetResultNode(context);
      CompleteReduce(context);
      context.CurrentParserInput = savedParserInput;
    }

    protected virtual ParseTreeNode GetResultNode(ParsingContext context) {
      var childCount = Production.RValues.Count;
      int firstChildIndex = context.ParserStack.Count - childCount;
      var span = context.ComputeStackRangeSpan(childCount);
      var newNode = new ParseTreeNode(Production.LValue, span);
      for (int i = 0; i < childCount; i++) {
        var childNode = context.ParserStack[firstChildIndex + i];
        if (childNode.IsPunctuationOrEmptyTransient()) continue; //skip punctuation or empty transient nodes
        newNode.ChildNodes.Add(childNode);
      }//for i
      return newNode;
    }
    //Completes reduce: pops child nodes from the stack and pushes result node into the stack
    protected void CompleteReduce(ParsingContext context) {
      var resultNode = context.CurrentParserInput; 
      var childCount = Production.RValues.Count;
      //Pop stack
      context.ParserStack.Pop(childCount);
      //Copy comment block from first child; if comments precede child node, they precede the parent as well. 
      if (resultNode.ChildNodes.Count > 0)
        resultNode.Comments = resultNode.ChildNodes[0].Comments;
      //Inherit precedence and associativity, to cover a standard case: BinOp->+|-|*|/; 
      // BinOp node should inherit precedence from underlying operator symbol. 
      //TODO: this special case will be handled differently. A ToTerm method should be expanded to allow "combined" terms like "NOT LIKE".
      // OLD COMMENT: A special case is SQL operator "NOT LIKE" which consists of 2 tokens. We therefore inherit "max" precedence from any children
      if (Production.LValue.Flags.IsSet(TermFlags.InheritPrecedence))
        InheritPrecedence(resultNode);
      //Push new node into stack and move to new state
      //First read the state from top of the stack 
      context.CurrentParserState = context.ParserStack.Top.State;
      if (context.TracingEnabled) 
        context.AddTrace(Resources.MsgTracePoppedState, Production.LValue.Name);
      #region comments on special case
      //Special case: if a non-terminal is Transient (ex: BinOp), then result node is not this NonTerminal, but its its child (ex: symbol). 
      // Shift action will invoke OnShifting on actual term being shifted (symbol); we need to invoke Shifting even on NonTerminal itself
      // - this would be more expected behavior in general. ImpliedPrecHint relies on this
      #endregion
      if (resultNode.Term != Production.LValue) //special case
        Production.LValue.OnShifting(context.SharedParsingEventArgs);
      // Shift to new state - execute shift over the non-terminal of the production. 
      var shift = context.CurrentParserState.Actions[Production.LValue];
      // Execute shift to new state
      shift.Execute(context);
      //Invoke Reduce event
      Production.LValue.OnReduced(context, Production, resultNode);
    }

    //This operation helps in situation when Bin expression is declared as BinExpr.Rule = expr + BinOp + expr; 
    // where BinOp is an OR-combination of operators. 
    // During parsing, when 'expr, BinOp, expr' is on the top of the stack, 
    // and incoming symbol is operator, we need to use precedence rule for deciding on the action. 
    private void InheritPrecedence(ParseTreeNode node) {
      for (int i = 0; i < node.ChildNodes.Count; i++) {
        var child = node.ChildNodes[i];
        if (child.Precedence == Terminal.NoPrecedence) continue;
        node.Precedence = child.Precedence;
        node.Associativity = child.Associativity;
        return; 
      }
    }
  
  }//class

  /// <summary>Reduces non-terminal marked as Transient by MarkTransient method. </summary>
  public class ReduceTransientParserAction : ReduceParserAction {
    public ReduceTransientParserAction(Production production) : base(production) { }

    protected override ParseTreeNode GetResultNode(ParsingContext context) {
      var topIndex = context.ParserStack.Count - 1;
      var childCount = Production.RValues.Count;
      for (int i = 0; i < childCount; i++) {
        var child = context.ParserStack[topIndex - i];
        if (child.IsPunctuationOrEmptyTransient()) continue;
        return child;
      }
      //Otherwise return an empty transient node; if it is part of the list, the list will skip it
      var span = context.ComputeStackRangeSpan(childCount);
      return new ParseTreeNode(Production.LValue, span); 
      
    }
  }//class

  /// <summary>Reduces list created by MakePlusRule or MakeListRule methods. </summary>
  public class ReduceListBuilderParserAction : ReduceParserAction {
    
    public ReduceListBuilderParserAction(Production production) : base(production) { }

    protected override ParseTreeNode GetResultNode(ParsingContext context) {
      int childCount = Production.RValues.Count;
      int firstChildIndex = context.ParserStack.Count - childCount;
      var listNode = context.ParserStack[firstChildIndex]; //get the list already created - it is the first child node
      listNode.Span = context.ComputeStackRangeSpan(childCount);
      var listMember = context.ParserStack.Top; //next list member is the last child - at the top of the stack
      if (listMember.IsPunctuationOrEmptyTransient())
        return listNode;
      listNode.ChildNodes.Add(listMember);
      return listNode; 
    }
  }//class

  //List container is an artificial non-terminal created by MakeStarRule method; the actual list is a direct child. 
  public class ReduceListContainerParserAction : ReduceParserAction {
    public ReduceListContainerParserAction(Production production) : base(production) { }

    protected override ParseTreeNode GetResultNode(ParsingContext context) {
      int childCount = Production.RValues.Count;
      int firstChildIndex = context.ParserStack.Count - childCount;
      var span = context.ComputeStackRangeSpan(childCount);
      var newNode = new ParseTreeNode(Production.LValue, span);
      if (childCount > 0) { //if it is not empty production - might happen for MakeStarRule
        var listNode = context.ParserStack[firstChildIndex]; //get the transient list with all members - it is the first child node
        newNode.ChildNodes.AddRange(listNode.ChildNodes);    //copy all list members
      }
      return newNode; 
      
    }
  }//class

}//ns
