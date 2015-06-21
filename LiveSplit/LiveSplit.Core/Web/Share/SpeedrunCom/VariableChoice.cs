using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class VariableChoice
    {
        public string ID { get; private set; }
        public string Value { get; private set; }

        private VariableChoice() { }

        public static VariableChoice Parse(SpeedrunComClient client, KeyValuePair<string, dynamic> choiceElement)
        {
            var choice = new VariableChoice();

            choice.ID = choiceElement.Key;
            choice.Value = choiceElement.Value as string;

            return choice;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
