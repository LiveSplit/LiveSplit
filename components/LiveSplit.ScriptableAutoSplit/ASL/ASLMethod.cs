using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;

using LiveSplit.Model;

namespace LiveSplit.ASL;

public class ASLMethod
{
    public ASLScript.Methods ScriptMethods { get; set; }

    public string Name { get; }

    public bool IsEmpty { get; }

    public int LineOffset { get; }

    public Module Module { get; }

    private readonly dynamic _compiled_code;

    public ASLMethod(string code, string name = null, int script_line = 0)
    {
        if (code == null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        Name = name;
        IsEmpty = string.IsNullOrWhiteSpace(code);
        code = code.Replace("return;", "return null;"); // hack

        var options = new Dictionary<string, string> {
            { "CompilerVersion", "v4.0" }
        };

        using var provider = new Microsoft.CSharp.CSharpCodeProvider(options);
        string user_code_start_marker = "// USER_CODE_START";
        string source = $@"
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using LiveSplit.Options;
public class CompiledScript
{{
    public string version;
    public double refreshRate;
    void print(string s)
    {{
        Log.Info(s);
    }}
    public dynamic Execute(LiveSplitState timer, dynamic old, dynamic current, dynamic vars, Process game, dynamic settings)
    {{
        var memory = game;
        var modules = game != null ? game.ModulesWow64Safe() : null;
        {user_code_start_marker}
	    {code}
	    return null;
    }}
}}";

        if (script_line > 0)
        {
            int user_code_index = source.IndexOf(user_code_start_marker);
            int compiled_code_line = source.Take(user_code_index).Count(c => c == '\n') + 2;
            LineOffset = script_line - compiled_code_line;
        }

        var parameters = new CompilerParameters()
        {
            GenerateInMemory = true,
            CompilerOptions = "/optimize /d:TRACE /debug:pdbonly",
        };
        parameters.ReferencedAssemblies.Add("System.dll");
        parameters.ReferencedAssemblies.Add("System.Core.dll");
        parameters.ReferencedAssemblies.Add("System.Data.dll");
        parameters.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
        parameters.ReferencedAssemblies.Add("System.Drawing.dll");
        parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
        parameters.ReferencedAssemblies.Add("System.Xml.dll");
        parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
        parameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
        parameters.ReferencedAssemblies.Add("LiveSplit.Core.dll");

        CompilerResults res = provider.CompileAssemblyFromSource(parameters, source);
        if (res.Errors.HasErrors)
        {
            throw new ASLCompilerException(this, res.Errors);
        }

        Module = res.CompiledAssembly.ManifestModule;
        Type type = res.CompiledAssembly.GetType("CompiledScript");
        _compiled_code = Activator.CreateInstance(type);
    }

    public dynamic Call(LiveSplitState timer, ExpandoObject vars, ref string version, ref double refreshRate,
        dynamic settings, ExpandoObject old = null, ExpandoObject current = null, Process game = null)
    {
        // dynamic args can't be ref or out, this is a workaround
        _compiled_code.version = version;
        _compiled_code.refreshRate = refreshRate;
        dynamic ret;
        try
        {
            ret = _compiled_code.Execute(timer, old, current, vars, game, settings);
        }
        catch (Exception ex)
        {
            throw new ASLRuntimeException(this, ex);
        }

        version = _compiled_code.version;
        refreshRate = _compiled_code.refreshRate;
        return ret;
    }
}
