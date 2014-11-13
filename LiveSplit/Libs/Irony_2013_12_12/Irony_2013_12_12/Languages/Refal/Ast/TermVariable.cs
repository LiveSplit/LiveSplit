// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

namespace Refal
{
	/// <summary>
	/// Variable of form t.X that can be bound either to a symbol or to expression in a structure braces.
	/// </summary>
	public class TermVariable : Variable
	{
		public override string Index
		{
			get { return base.Index; }
			protected set { base.Index = "t." + value; }
		}

		public override Runtime.Variable CreateVariable()
		{
			return new Runtime.TermVariable(Index);
		}
	}
}
