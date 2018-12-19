using System;

namespace LiveSplit.Server
{
    public class ScriptFactory
    {
        public static readonly string[] AllScripts = { "JavaScript", "C#" };

        public static IScript Create(string language, string code)
        {
            var lowerLanguage = language.ToLower();

            if (lowerLanguage == "c#" || lowerLanguage == "cs")
                return new CSharpScript(code);

            throw new ArgumentException("The language does not exist", "language");
        }
    }
}
