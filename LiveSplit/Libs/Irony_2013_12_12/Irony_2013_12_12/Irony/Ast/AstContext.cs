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
using Irony.Parsing; 

namespace Irony.Ast {
  public class AstContext {
    public readonly LanguageData Language;
    public Type DefaultNodeType;
    public Type DefaultLiteralNodeType; //default node type for literals
    public Type DefaultIdentifierNodeType; //default node type for identifiers

    public Dictionary<object, object> Values = new Dictionary<object, object>();
    public LogMessageList Messages;

    public AstContext(LanguageData language) {
      Language = language;
    }

    public void AddMessage(ErrorLevel level, SourceLocation location, string message, params object[] args) {
      if (args != null && args.Length > 0)
        message = string.Format(message, args);
      Messages.Add(new LogMessage(level, location, message, null));
    }

  }//class
}//ns
