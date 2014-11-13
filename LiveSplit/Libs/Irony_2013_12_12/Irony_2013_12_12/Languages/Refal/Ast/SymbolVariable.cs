// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

namespace Refal
{
	/// <summary>
	/// Variable of form s.X that can be bound to single symbol.
	/// </summary>
	public class SymbolVariable : Variable
	{
		public override string Index
		{
			get { return base.Index; }
			protected set { base.Index = "s." + value; }
		}

		public override Runtime.Variable CreateVariable()
		{
			return new Runtime.SymbolVariable(Index);
		}
	}
}
