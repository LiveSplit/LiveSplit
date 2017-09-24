using System;
using System.Text;

namespace LiveSplit.Server
{
    public class CSharpScript : IScript
    {
        protected dynamic CompiledCode { get; set; }

        public dynamic this[string name]
        {
            get
            {
                return CompiledCode[name];
            }
            set
            {
                CompiledCode[name] = value;
            }
        }

        public CSharpScript(string code)
        {
            using (var provider =
                new Microsoft.CSharp.CSharpCodeProvider())
            {
                var builder = new StringBuilder();
                builder
                    .AppendLine("using System;")
                    .AppendLine("using System.Collections.Generic;")
                    .AppendLine("using System.Linq;")
                    .AppendLine("using System.Reflection;")
                    .AppendLine("using System.Text;")
                    .AppendLine("public class CompiledScript")
                    .AppendLine("{")
                        .AppendLine("private Dictionary<String, dynamic> dict = new Dictionary<string,dynamic>();")
                        .AppendLine("public dynamic this[string name]")
                        .AppendLine("{")
                            .AppendLine("get")
                            .AppendLine("{")
                                .AppendLine("return dict[name];")
                            .AppendLine("}")
                            .AppendLine("set")
                            .AppendLine("{")
                                .AppendLine("if (dict.ContainsKey(name))")
                                    .AppendLine("dict[name] = value;")
                                .AppendLine("else")
                                    .AppendLine("dict.Add(name, value);")
                            .AppendLine("}")
                        .AppendLine("}")
                        .AppendLine("public dynamic Execute()")
                        .AppendLine("{")
                            .Append(code)
                            .Append("return null;")
                        .AppendLine("}")
                    .AppendLine("}");

                var parameters = new System.CodeDom.Compiler.CompilerParameters()
                    {
                        GenerateInMemory = true,
                        CompilerOptions = "/optimize",
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

                var res = provider.CompileAssemblyFromSource(parameters, builder.ToString());

                if (res.Errors.HasErrors)
                {
                    var errorMessage = "";
                    foreach (var error in res.Errors)
                    {
                        errorMessage += error + "\r\n";
                    }
                    throw new ArgumentException(errorMessage, "code");
                }

                var type = res.CompiledAssembly.GetType("CompiledScript");
                CompiledCode = Activator.CreateInstance(type);
            }
        }

        public dynamic Run()
        {
            return CompiledCode.Execute();
        }
    }
}
