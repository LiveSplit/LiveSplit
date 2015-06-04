using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class Category
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public CategoryType Type { get; private set; }
        public string Rules { get; private set; }
        public int Players { get; private set; }
        public bool IsMiscellaneous { get; private set; }
    }
}
