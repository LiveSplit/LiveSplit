using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Irony.Ast;
using Irony.Parsing; 

namespace Irony.Interpreter.Ast {

  public class IndexedAccessNode : AstNode {
    AstNode _target, _index;

    public override void Init(AstContext context, ParseTreeNode treeNode) {
      base.Init(context, treeNode);
      var nodes = treeNode.GetMappedChildNodes();
      _target = AddChild("Target", nodes.First());
      _index = AddChild("Index", nodes.Last()); 
      AsString = "[" + _index + "]";
    }

    protected override object DoEvaluate(ScriptThread thread) {
      thread.CurrentNode = this;  //standard prolog
      object result = null; 
      var targetValue = _target.Evaluate(thread);
      if (targetValue == null)
        thread.ThrowScriptError("Target object is null.");
      var type = targetValue.GetType();
      var indexValue = _index.Evaluate(thread); 
      //string and array are special cases
      if (type == typeof(string)) {
        var sTarget = targetValue as string;
        var iIndex = Convert.ToInt32(indexValue); 
        result = sTarget[iIndex];
      } else if (type.IsArray) {
        var arr = targetValue as Array;
        var iIndex = Convert.ToInt32(indexValue);
        result = arr.GetValue(iIndex); 
      } else if (targetValue is IDictionary) {
        var dict = (IDictionary)targetValue;
        result = dict[indexValue];
      } else {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.InvokeMethod;
        result = type.InvokeMember("get_Item", flags, null, targetValue, new object[] { indexValue }); 
      }
      thread.CurrentNode = Parent; //standard epilog
      return result; 
    }

    public override void DoSetValue(ScriptThread thread, object value) {
      thread.CurrentNode = this;  //standard prolog
      var targetValue = _target.Evaluate(thread);
      if (targetValue == null)
        thread.ThrowScriptError("Target object is null.");
      var type = targetValue.GetType();
      var indexValue = _index.Evaluate(thread);
      //string and array are special cases
      if (type == typeof(string)) {
        thread.ThrowScriptError("String is read-only.");
      } else if (type.IsArray) {
        var arr = targetValue as Array;
        var iIndex = Convert.ToInt32(indexValue);
        arr.SetValue(value, iIndex);
      } else if (targetValue is IDictionary) {
        var dict = (IDictionary)targetValue;
        dict[indexValue] = value;
      } else {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.InvokeMethod;
        type.InvokeMember("set_Item", flags, null, targetValue, new object[] { indexValue, value }); 
      }
      thread.CurrentNode = Parent; //standard epilog
    }//method

  }//class


}//namespace
