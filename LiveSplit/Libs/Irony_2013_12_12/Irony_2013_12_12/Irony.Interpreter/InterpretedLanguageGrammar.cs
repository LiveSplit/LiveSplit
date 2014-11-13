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

using Irony.Ast; 
using Irony.Parsing;
using Irony.Interpreter.Ast;

namespace Irony.Interpreter {
  /// <summary> Base class for languages that use Irony Interpreter to execute scripts. </summary>
  public abstract class InterpretedLanguageGrammar : Grammar, ICanRunSample {
       // making the class abstract so it won't load into Grammar Explorer
    public InterpretedLanguageGrammar(bool caseSensitive)
      : base(caseSensitive) {
      this.LanguageFlags = LanguageFlags.CreateAst;
    }

    // This method allows custom implementation of running a sample in Grammar Explorer
    // By default it evaluates a parse tree using default interpreter.
    // Irony's interpeter has one restriction: once a script (represented by AST node) is evaluated in ScriptApp, 
    // its internal fields in AST nodes become tied to this particular instance of ScriptApp (more precisely DataMap).
    // If you want to evaluate the AST tree again, you have to do it in the context of the same DataMap. 
    // Grammar Explorer may call RunSample method repeatedly for evaluation of the same parsed script. So we keep ScriptApp instance in 
    // the field, and if we get the same script node, then we reuse the ScriptApp thus satisfying the requirement. 
    private ScriptApp _app;
    private ParseTree _prevSample;
    
    public virtual string RunSample(RunSampleArgs args) {
      if (_app == null || args.ParsedSample != _prevSample)
        _app = new ScriptApp(args.Language); 
      _prevSample = args.ParsedSample;

      //for (int i = 0; i < 1000; i++)  //for perf measurements, to execute 1000 times
        _app.Evaluate(args.ParsedSample);
      return _app.OutputBuffer.ToString();
    }

    public virtual LanguageRuntime CreateRuntime(LanguageData language) {
      return new LanguageRuntime(language); 
    }

    public override void BuildAst(LanguageData language, ParseTree parseTree) {
      var opHandler = new OperatorHandler(language.Grammar.CaseSensitive);
      Util.Check(!parseTree.HasErrors(), "ParseTree has errors, cannot build AST.");
      var astContext = new InterpreterAstContext(language, opHandler);
      var astBuilder = new AstBuilder(astContext);
      astBuilder.BuildAst(parseTree);
    }
  } //grammar class

}
