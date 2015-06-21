using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class VariableScope
    {
        public VariableScopeType Type { get; private set; }
        public string LevelID { get; private set; }

        private VariableScope() { }

        private static VariableScopeType parseType(string type)
        {
            switch (type)
            {
                case "global":
                    return VariableScopeType.Global;
                case "full-game":
                    return VariableScopeType.FullGame;
                case "all-levels":
                    return VariableScopeType.AllLevels;
                case "single-level":
                    return VariableScopeType.SingleLevel;
            }

            throw new ArgumentException("type");
        }

        public static VariableScope Parse(SpeedrunComClient client, dynamic scopeElement)
        {
            var scope = new VariableScope();

            scope.Type = parseType(scopeElement.type as string);

            if (scope.Type == VariableScopeType.SingleLevel)
            {
                scope.LevelID = scopeElement.level as string;
            }

            return scope;
        }

        public override string ToString()
        {
            if (Type == VariableScopeType.SingleLevel)
                return "Single Level: " + (LevelID ?? "");
            else
                return Type.ToString();
        }
    }
}
