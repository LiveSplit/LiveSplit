// Refal5.NET interpreter
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

namespace Refal
{
	/// <summary>
	/// Variable of form e.X that can be bound to any expression.
	/// </summary>
	public class ExpressionVariable : Variable
	{
		public override string Index
		{
			get { return base.Index; }
			protected set { base.Index = "e." + value; }
		}

		public override Runtime.Variable CreateVariable()
		{
			return new Runtime.ExpressionVariable(Index);
		}
	}
}
