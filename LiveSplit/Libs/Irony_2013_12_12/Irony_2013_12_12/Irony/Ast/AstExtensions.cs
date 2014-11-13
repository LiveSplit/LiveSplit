using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Irony.Parsing;

namespace Irony.Ast {
  public static class AstExtensions {

    public static ParseTreeNodeList GetMappedChildNodes(this ParseTreeNode node) {
      var term = node.Term; 
      if (!term.HasAstConfig())
        return node.ChildNodes; 
      var map = term.AstConfig.PartsMap;
      //If no map then mapped list is the same as original 
      if (map == null) return node.ChildNodes;
      //Create mapped list
      var result = new ParseTreeNodeList();
      for (int i = 0; i < map.Length; i++) {
        var index = map[i];
        result.Add(node.ChildNodes[index]);
      }
      return result;
    }


  }
}
