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
using System.Threading;
using System.Diagnostics;
using Irony.Parsing;

namespace Irony.Interpreter {

  //An abstraction of a Console. 
  public interface IConsoleAdaptor {
    bool Canceled { get; set; }
    void Write(string text);
    void WriteLine(string text);
    void SetTextStyle(ConsoleTextStyle style);
    int Read(); //reads a key
    string ReadLine(); //reads a line; returns null if Ctrl-C is pressed
    void SetTitle(string title);
  }

  //WARNING: Ctrl-C for aborting running script does NOT work when you run console app from Visual Studio 2010. 
  // Run executable directly from bin folder. 
  public class CommandLine {
    #region Fields and properties
    public readonly LanguageRuntime Runtime;
    public readonly IConsoleAdaptor _console;
    //Initialized from grammar
    public string Title;
    public string Greeting;
    public string Prompt; //default prompt
    public string PromptMoreInput; //prompt to show when more input is expected

    public readonly ScriptApp App;
    Thread _workerThread;
    public bool IsEvaluating { get; private set; }

    #endregion 

    public CommandLine(LanguageRuntime runtime, IConsoleAdaptor console = null) {
      Runtime = runtime;
      _console = console ?? new ConsoleAdapter();
      var grammar = runtime.Language.Grammar;
      Title = grammar.ConsoleTitle;
      Greeting = grammar.ConsoleGreeting;
      Prompt = grammar.ConsolePrompt;
      PromptMoreInput = grammar.ConsolePromptMoreInput;
      App = new ScriptApp(Runtime);
      App.ParserMode = ParseMode.CommandLine;
      // App.PrintParseErrors = false;
      App.RethrowExceptions = false;
    
    }

    public void Run() {
      try {
        RunImpl();
      } catch (Exception ex) {
        _console.SetTextStyle(ConsoleTextStyle.Error);
        _console.WriteLine(Resources.ErrConsoleFatalError);
        _console.WriteLine(ex.ToString());
        _console.SetTextStyle(ConsoleTextStyle.Normal);
        _console.WriteLine(Resources.MsgPressAnyKeyToExit);
        _console.Read();
      }
    }


    private void RunImpl() {

      _console.SetTitle(Title);
      _console.WriteLine(Greeting);
      string input;
      while (true) {
        _console.Canceled = false;
        _console.SetTextStyle(ConsoleTextStyle.Normal);
        string prompt = (App.Status == AppStatus.WaitingMoreInput ? PromptMoreInput : Prompt);

        //Write prompt, read input, check for Ctrl-C
        _console.Write(prompt);
        input = _console.ReadLine();
        if (_console.Canceled) 
          if (Confirm(Resources.MsgExitConsoleYN))
            return;
          else
            continue; //from the start of the loop

        //Execute
        App.ClearOutputBuffer(); 
        EvaluateAsync(input);
        //Evaluate(input);
        WaitForScriptComplete(); 
       
        switch (App.Status) {
          case AppStatus.Ready: //success
            _console.WriteLine(App.GetOutput());
            break;
          case  AppStatus.SyntaxError:
            _console.WriteLine(App.GetOutput()); //write all output we have
            _console.SetTextStyle(ConsoleTextStyle.Error);
            foreach (var err in App.GetParserMessages()) {
              _console.WriteLine(string.Empty.PadRight(prompt.Length + err.Location.Column) + "^"); //show err location
              _console.WriteLine(err.Message); //print message
            }
            break;
          case AppStatus.Crash:
          case AppStatus.RuntimeError:
            ReportException(); 
            break;
          default: break;
        }//switch
      }

    }//Run method

    private void WaitForScriptComplete() {
      _console.Canceled = false; 
      while(true) {
        Thread.Sleep(50);
        if(!IsEvaluating) return;
        if(_console.Canceled) {
          _console.Canceled = false; 
          if (Confirm(Resources.MsgAbortScriptYN))
            WorkerThreadAbort();
        }//if Canceled
      }
    }

    private void Evaluate(string script) {
      try {
        IsEvaluating = true;
        App.Evaluate(script);
      } finally {
        IsEvaluating = false; 
      }
    }

    private void EvaluateAsync(string script) {
      IsEvaluating = true; 
      _workerThread = new Thread(WorkerThreadStart);
      _workerThread.Start(script);
    }

    private void WorkerThreadStart(object data) {
      try {
        var script = data as string;
        App.Evaluate(script);
      } finally {
        IsEvaluating = false; 
      }
    }
    private void WorkerThreadAbort() {
      try {
        _workerThread.Abort();
        _workerThread.Join(50);
      } finally {
        IsEvaluating = false;
      }
    }

    private bool Confirm(string message) {
      _console.WriteLine(string.Empty);
      _console.Write(message);
      var input = _console.ReadLine();
      return Resources.ConsoleYesChars.Contains(input);
    }

    private void ReportException()  {
      _console.SetTextStyle(ConsoleTextStyle.Error);
      var ex = App.LastException;
      var scriptEx = ex as ScriptException;
      if (scriptEx != null)
        _console.WriteLine(scriptEx.Message + " " + Resources.LabelLocation + " " + scriptEx.Location.ToUiString());
      else {
        if (App.Status == AppStatus.Crash)
          _console.WriteLine(ex.ToString());   //Unexpected interpreter crash:  the full stack when debugging your language  
        else
        _console.WriteLine(ex.Message);

      }
      //
    }

  }//class
}
