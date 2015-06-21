using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class VariableValue
    {
        public string VariableID { get; private set; }
        public string VariableChoiceID { get; private set; }

        private VariableValue() { }

        internal static VariableValue Parse(SpeedrunComClient client, KeyValuePair<string, dynamic> valueElement)
        {
            var value = new VariableValue();

            value.VariableID = valueElement.Key;
            value.VariableChoiceID = valueElement.Value as string;

            return value;
        }

        public override string ToString()
        {
            return (VariableID ?? "") + " " + (VariableChoiceID ?? "");
        }
    }
}
