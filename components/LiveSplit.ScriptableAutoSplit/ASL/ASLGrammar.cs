using System.Linq;

using Irony.Parsing;

namespace LiveSplit.ASL;

[Language("asl", "1.0", "Auto Split Language grammar")]
public class ASLGrammar : Grammar
{
    public ASLGrammar()
        : base(true)
    {
        StringLiteral string_lit = TerminalFactory.CreateCSharpString("string");
        NumberLiteral number = TerminalFactory.CreateCSharpNumber("number");
        number.Options |= NumberOptions.AllowSign;
        IdentifierTerminal identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
        var code = new CustomTerminal("code", MatchCodeTerminal);

        var single_line_comment = new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
        var delimited_comment = new CommentTerminal("DelimitedComment", "/*", "*/");
        NonGrammarTerminals.Add(single_line_comment);
        NonGrammarTerminals.Add(delimited_comment);

        var state = new KeyTerm("state", "state");
        var init = new KeyTerm("init", "init");
        var exit = new KeyTerm("exit", "exit");
        var update = new KeyTerm("update", "update");
        var start = new KeyTerm("start", "start");
        var split = new KeyTerm("split", "split");
        var reset = new KeyTerm("reset", "reset");
        var startup = new KeyTerm("startup", "startup");
        var shutdown = new KeyTerm("shutdown", "shutdown");
        var isLoading = new KeyTerm("isLoading", "isLoading");
        var gameTime = new KeyTerm("gameTime", "gameTime");
        var onStart = new KeyTerm("onStart", "onStart");
        var onSplit = new KeyTerm("onSplit", "onSplit");
        var onReset = new KeyTerm("onReset", "onReset");
        KeyTerm comma = ToTerm(",", "comma");
        KeyTerm semi = ToTerm(";", "semi");

        var root = new NonTerminal("root");
        var state_def = new NonTerminal("stateDef");
        var version = new NonTerminal("version");
        var state_list = new NonTerminal("stateList");
        var method_list = new NonTerminal("methodList");
        var var_list = new NonTerminal("varList");
        var var = new NonTerminal("var");
        var module = new NonTerminal("module");
        var method = new NonTerminal("method");
        var offset_list = new NonTerminal("offsetList");
        var offset = new NonTerminal("offset");
        var method_type = new NonTerminal("methodType");

        root.Rule = state_list + method_list;
        version.Rule = (comma + string_lit) | Empty;
        state_def.Rule = state + "(" + string_lit + version + ")" + "{" + var_list + "}";
        state_list.Rule = MakeStarRule(state_list, state_def);
        method_list.Rule = MakeStarRule(method_list, method);
        var_list.Rule = MakeStarRule(var_list, semi, var);
        module.Rule = (string_lit + comma) | Empty;
        var.Rule = (identifier + identifier + ":" + module + offset_list) | Empty;
        method.Rule = (method_type + "{" + code + "}") | Empty;
        offset_list.Rule = MakePlusRule(offset_list, comma, offset);
        offset.Rule = number;
        method_type.Rule = init | exit | update | start | split | isLoading | gameTime | reset | startup | shutdown | onStart | onSplit | onReset;

        Root = root;

        MarkTransient(var_list, method_list, offset, method_type);

        LanguageFlags = LanguageFlags.NewLineBeforeEOF;
    }

    private static Token MatchCodeTerminal(Terminal terminal, ParsingContext context, ISourceStream source)
    {
        string remaining = source.Text[source.Location.Position..];
        int stack = 1;
        string token = "";

        while (stack > 0)
        {
            int index = remaining.IndexOf('}') + 1;
            string cut = remaining[..index];

            token += cut;
            remaining = remaining[index..];
            stack += cut.Count(x => x == '{');
            stack--;
        }

        token = token[..^1];
        source.PreviewPosition += token.Length;

        return source.CreateToken(terminal.OutputTerminal);
    }
}
