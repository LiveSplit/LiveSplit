using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    internal struct Embeds
    {
        private Dictionary<string, bool> embedDictionary;

        public bool this[string name]
        {
            get
            {
                MakeSureInit();

                if (embedDictionary.ContainsKey(name))
                    return embedDictionary[name];
                else
                    return false;
            }
            set
            {
                MakeSureInit();

                if (embedDictionary.ContainsKey(name))
                    embedDictionary[name] = value;
                else
                    embedDictionary.Add(name, value);
            }
        }

        private void MakeSureInit()
        {
            if (embedDictionary == null)
                embedDictionary = new Dictionary<string, bool>();
        }

        public override string ToString()
        {
            MakeSureInit();

            if (!embedDictionary.Values.Any(x => x))
                return "";

            return "embed=" +
                embedDictionary
                .Where(x => x.Value)
                .Select(x => HttpUtility.UrlPathEncode(x.Key))
                .Aggregate((a, b) => a + "," + b);
        }
    }
}
