using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Irony.Ast; 
using Irony.Parsing; 

namespace Irony.Interpreter.Ast {

  //For now we do not support dotted namespace/type references like System.Collections or System.Collections.List.
  // Only references to objects like 'objFoo.Name' or 'objFoo.DoStuff()'
  public class MemberAccessNode : AstNode {
    AstNode _left;
    string _memberName;

    public override void Init(AstContext context, ParseTreeNode treeNode) {
      base.Init(context, treeNode);
      var nodes = treeNode.GetMappedChildNodes();
      _left = AddChild("Target", nodes[0]);
      var right = nodes[nodes.Count - 1];
      _memberName = right.FindTokenAndGetText();
      ErrorAnchor = right.Span.Location; 
      AsString = "." + _memberName;
    }

    protected override object DoEvaluate(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      object result = null; 
      var leftValue = _left.Evaluate(thread);
      if (leftValue == null)
        thread.ThrowScriptError("Target object is null.");
      var type = leftValue.GetType();
      var members = type.GetMember(_memberName); 
      if (members == null || members.Length == 0)
        thread.ThrowScriptError("Member {0} not found in object of type {1}.", _memberName, type);
      var member = members[0];
      switch (member.MemberType) {
        case MemberTypes.Property:
          var propInfo = member as PropertyInfo;
          result = propInfo.GetValue(leftValue, null);
          break; 
        case MemberTypes.Field:
          var fieldInfo = member as FieldInfo;
          result = fieldInfo.GetValue(leftValue);
          break; 
        case MemberTypes.Method:
          result = new ClrMethodBindingTargetInfo(type, _memberName, leftValue); //this bindingInfo works as a call target
          break; 
        default:
          thread.ThrowScriptError("Invalid member type ({0}) for member {1} of type {2}.", member.MemberType, _memberName, type);
          result = null; 
          break; 
      }//switch
      thread.CurrentNode = Parent; //standard epilog
      return result; 
    }

    public override void DoSetValue(ScriptThread thread, object value) {
      thread.CurrentNode = this;  //standard prolog
      var leftValue = _left.Evaluate(thread);
      if (leftValue == null)
        thread.ThrowScriptError("Target object is null.");
      var type = leftValue.GetType();
      var members = type.GetMember(_memberName);
      if (members == null || members.Length == 0)
        thread.ThrowScriptError("Member {0} not found in object of type {1}.", _memberName, type);
      var member = members[0];
      switch (member.MemberType) {
        case MemberTypes.Property:
          var propInfo = member as PropertyInfo;
          propInfo.SetValue(leftValue, value, null);
          break;
        case MemberTypes.Field:
          var fieldInfo = member as FieldInfo;
          fieldInfo.SetValue(leftValue, value);
          break;
        default:
          thread.ThrowScriptError("Cannot assign to member {0} of type {1}.", _memberName, type);
          break;
      }//switch
      thread.CurrentNode = Parent; //standard epilog
    }//method

  }//class


}//namespace
