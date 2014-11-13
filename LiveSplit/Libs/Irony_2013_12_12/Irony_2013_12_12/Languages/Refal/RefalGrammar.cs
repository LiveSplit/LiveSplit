using System;
using System.Text;
using System.Collections.Generic;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Irony.Interpreter;

namespace Refal
{
	/// <summary>
	/// Refal-5 Grammar.
	/// Written by Alexey Yakovlev, yallie@yandex.ru.
	/// http://refal.codeplex.com
	/// </summary>
	[Language("Refal5", "0.2 alpha", "Refal-5.NET language interpreter")]
	public class RefalGrammar : InterpretedLanguageGrammar
	{
		public RefalGrammar() : base(true) // case sensitive
		{
			GrammarComments = "Refal-5 language interpreter based on Irony toolkit.";

			// Terminals
			var Number = new NumberLiteral("Number");
			Number.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
			Number.AddPrefix("0x", NumberOptions.Hex);
			Number.AddSuffix("u", TypeCode.UInt32);
			Number.AddSuffix("l", TypeCode.Int64);
			Number.AddSuffix("ul", TypeCode.UInt64);
			Number.Options |= NumberOptions.IntOnly;

			var CharLiteral = new StringLiteral("Char", "'", StringOptions.AllowsAllEscapes);
			var StringLiteral = new StringLiteral("String", "\"", StringOptions.AllowsAllEscapes);
			var Identifier = new IdentifierTerminal("Identifier", "_-", "");

			var LineComment = new CommentTerminal("LineComment", "*", "\n", "\r");
			LineComment.ValidateToken += LineComment_ValidateToken;

			var BlockComment = new CommentTerminal("BlockComment", "/*", "*/");
			NonGrammarTerminals.Add(LineComment);
			NonGrammarTerminals.Add(BlockComment);

			// Non-terminals
			var Program = new NonTerminal("Program", typeof(Program));
			var Definition = new NonTerminal("Definition");
			var Function = new NonTerminal("Function", typeof(DefinedFunction));
			var External = new NonTerminal("External", typeof(AuxiliaryNode));
			var IdentifierList = new NonTerminal("Identifier+", typeof(AuxiliaryNode));
			var Block = new NonTerminal("Block", typeof(Block));
			var Sentence = new NonTerminal("Sentence", typeof(Sentence));
			var RSentence = new NonTerminal("RSentence", typeof(AuxiliaryNode));
			var SentenceList = new NonTerminal("Sentence+", typeof(AuxiliaryNode));
			var Pattern = new NonTerminal("Pattern", typeof(Pattern));
			var PatternItem = new NonTerminal("PatternItem");
			var PatternInParentheses = new NonTerminal("(Pattern)", typeof(ExpressionInBraces));
			var Expression = new NonTerminal("Expression", typeof(Expression));
			var ExpressionItem = new NonTerminal("ExpressionItem");
			var ExpressionInParentheses = new NonTerminal("(Expression)", typeof(ExpressionInBraces));
			var Var = new NonTerminal("Variable", Variable.CreateVariableNode);
			var VarPrefix = new NonTerminal("VariablePrefix");
			var VarIndex = new NonTerminal("VariableIndex");
			var Symbol = new NonTerminal("Symbol", LiteralValueNodeHelper.InitNode);
			var Call = new NonTerminal("Call", typeof(FunctionCall));
			var FunctionName = new NonTerminal("FunctionName", typeof(AuxiliaryNode));
			var WhereOrWithClause = new NonTerminal("WhereOrWithClause", typeof(Conditions));
			var CommaOrAmpersand = new NonTerminal("CommaOrAmpersand");
			var RWhereOrWithClause = new NonTerminal("RWhereOrWithClause", typeof(AuxiliaryNode));
			var RExpressionOrWhereOrWithClause = new NonTerminal("RExpressionOrWhereOrWithClause");

			var SemicolonOpt = new NonTerminal("[;]", Empty | ";");
			var EntryOpt = new NonTerminal("[ENTRY]", Empty | "$ENTRY");
			var Extern = new NonTerminal("Extern", ToTerm("$EXTRN") | "$EXTERN" | "$EXTERNAL");

			// Rules
			Root = Program;

			Program.Rule = MakePlusRule(Program, Definition);
			Definition.Rule = Function | External;
			External.Rule = Extern + IdentifierList + ";";
			IdentifierList.Rule = MakePlusRule(IdentifierList, ToTerm(","), Identifier);

			Function.Rule = EntryOpt + Identifier + Block + SemicolonOpt;
			Block.Rule = "{" + SentenceList + SemicolonOpt + "}";
			SentenceList.Rule = MakePlusRule(SentenceList, ToTerm(";"), Sentence);

			Sentence.Rule = Pattern + RSentence;
			RSentence.Rule = "=" + Expression | WhereOrWithClause;
			Pattern.Rule = MakeStarRule(Pattern, PatternItem);
			PatternItem.Rule = Var | Symbol | PatternInParentheses;
			PatternInParentheses.Rule = "(" + Pattern + ")";
			Expression.Rule = MakeStarRule(Expression, ExpressionItem);
			ExpressionItem.Rule = Call | Var | Symbol | ExpressionInParentheses;
			ExpressionInParentheses.Rule = "(" + Expression + ")";

			Var.Rule = VarPrefix + "." + VarIndex;
			VarPrefix.Rule = ToTerm("e") | "s" | "t";
			VarIndex.Rule = Number | Identifier;
			Symbol.Rule = StringLiteral | CharLiteral | Number | "True" | "False" | Identifier;
			Call.Rule = "<" + FunctionName + Expression + ">";
			FunctionName.Rule = Identifier | "+" | "-" | "*" | "/";

			WhereOrWithClause.Rule = CommaOrAmpersand + Expression + ":" + RWhereOrWithClause;
			CommaOrAmpersand.Rule = ToTerm(",") | "&";
			RWhereOrWithClause.Rule = Block // with-clause
				| Pattern + RExpressionOrWhereOrWithClause; // where-clause
			RExpressionOrWhereOrWithClause.Rule = ToTerm("=") + Expression | WhereOrWithClause;

			// Punctuation, braces, transient terms, options
			MarkPunctuation("(", ")");
			MarkPunctuation("{", "}");
			MarkPunctuation("<", ">");
			MarkPunctuation("=", ",", "&", ";");

			RegisterBracePair("(", ")");
			RegisterBracePair("<", ">");
			RegisterBracePair("{", "}");

			MarkTransient(Definition, PatternItem, ExpressionItem, SemicolonOpt, EntryOpt, Extern, CommaOrAmpersand, VarPrefix, VarIndex, RExpressionOrWhereOrWithClause);
			LanguageFlags = LanguageFlags.CreateAst | LanguageFlags.SupportsBigInt | LanguageFlags.TailRecursive;
		}

		void LineComment_ValidateToken(object sender, ParsingEventArgs args)
		{
			// if "*" is allowed in the current parser state, suppress comments starting with "*"
			var parserState = args.Context.CurrentParserState;
			if (parserState.ExpectedTerminals.Contains(ToTerm("*")))
			{
				// rewind input stream and reject the token
//				args.Context.SetSourceLocation(args.Context.CurrentToken.Location);
				args.Context.CurrentToken = null;
			}
		}
	}
}
