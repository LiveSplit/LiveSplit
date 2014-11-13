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
using Irony.Interpreter; 
using Irony.Interpreter.Evaluator;

namespace Irony.Samples {
  //The purpose of this class is pure convenience - to make expression evaluator grammar (which is in Irony.Interpreter assembly) to appear 
  // with other sample grammars. 
  [Language("SampleExpressionEvaluator", "1.0", "Multi-line expression evaluator")]
  public class SampleExpressionEvaluatorGrammar : ExpressionEvaluatorGrammar { }

}//namespace


