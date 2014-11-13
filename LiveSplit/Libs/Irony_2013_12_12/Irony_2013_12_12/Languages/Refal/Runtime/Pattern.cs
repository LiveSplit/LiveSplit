// Refal5.NET runtime
// Written by Alexey Yakovlev <yallie@yandex.ru>
// http://refal.codeplex.com

using System;
using System.Collections.Generic;

namespace Refal.Runtime
{
	/// <summary>
	/// Pattern is much like expression, but it can include free variables.
	/// </summary>
	public class Pattern : PassiveExpression
	{
		public IDictionary<string, Variable> Variables { get; private set; }

		public Pattern(params object[] terms)
		{
			Variables = new Dictionary<string, Variable>();

			foreach (object term in terms)
				Add(term);
		}

		public new PatternItem this[int index]
		{
			get { return List[index] as PatternItem; }
		}

		public override int Add(object item)
		{
			// handle variables in a special way
			if (item is Variable)
			{
				// don't add duplicate variables, add same instances instead
				Variable variable = (Variable)item;
				if (Variables.ContainsKey(variable.Name))
					return base.Add(Variables[variable.Name]);

				// index variables by names
				Variables[variable.Name] = variable;
				int index = base.Add(variable);
				variable.FirstOccurance = index;
				return index;
			}

			// don't wrap pattern item
			if (item is PatternItem)
				return base.Add(item);

			// decompose char[] into chars, wrap each char as symbol
			if (item is char[])
			{
				int index = -1;
				foreach (char c in (char[])item)
					index = List.Add(new Symbol(c));
				return index;
			}

			// flatten PassiveExpressions
			if (item is PassiveExpression)
			{
				var index = -1;
				foreach (var exItem in (PassiveExpression)item)
				{
					index = Add(exItem);
				}
				return index;
			}

			// wrap any other object as symbol pattern
			return base.Add(new Symbol(item));
		}

		public object GetVariable(string name)
		{
			if (Variables[name] == null)
				return null;

			return (Variables[name]).Value;
		}

		public void CopyBoundVariables(Pattern pattern)
		{
			foreach (string name in pattern.Variables.Keys)
			{
				if (Variables.ContainsKey(name))
				{
					Variable var = Variables[name];
					var.Value = pattern.GetVariable(name);
					// first occurance of the variable is in another pattern
					var.FirstOccurance = -1;
				}
				else
				{
					// copy bound variable from another pattern
					Variable var = pattern.Variables[name];
					Variables[name] = var;
					var.FirstOccurance = -1;
				}
			}
		}

		public void ClearBoundValues(int startFromIndex)
		{
			foreach (string name in Variables.Keys)
			{
				Variable var = Variables[name];

				if (var.FirstOccurance > startFromIndex) // not >=!
					var.Value = null;
			}
		}

		public bool Match(PassiveExpression expression)
		{
			int exIndex = 0, patIndex = 0;

			while (patIndex < Count || exIndex < expression.Count)
			{
				if (patIndex >= Count)
				{
					if (!RollBackToLastPartialMatch(ref exIndex, ref patIndex))
						return false;

					continue;
				}

				switch (this[patIndex].Match(expression, ref exIndex, patIndex++))
				{
					case MatchResult.Success:
						continue;
						
					case MatchResult.PartialSuccess:
						SaveRollBackInfo(exIndex, patIndex - 1);
						continue;
				}

				if (!RollBackToLastPartialMatch(ref exIndex, ref patIndex))
					return false;
			}

			return true;
		}

		// stack holding positions of the last expression variables
		Stack<RollbackInfo> rollBackStack = null;

		struct RollbackInfo
		{
			public int exIndex;
			public int patIndex; 
		}

		private void SaveRollBackInfo(int exIndex, int patIndex)
		{
			RollbackInfo info = new RollbackInfo();
			info.exIndex = exIndex;
			info.patIndex = patIndex;

			if (rollBackStack == null)
				rollBackStack = new Stack<RollbackInfo>();

			rollBackStack.Push(info);
		}

		private bool RollBackToLastPartialMatch(ref int exIndex, ref int patIndex)
		{
			if (rollBackStack == null || rollBackStack.Count == 0)
				return false;

			// restore state up to the last expression variable
			var info = rollBackStack.Pop();
			exIndex = info.exIndex;
			patIndex = info.patIndex;

			// clear values of all bound variables starting later than patIndex
			ClearBoundValues(patIndex);
			return true;
		}
	}
}
