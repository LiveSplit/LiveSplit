using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class UserOrGuest
    {
        public bool IsAnonymous { get { return UserName != null; } }
        public int UserID { get; private set; }
        public string UserName { get; private set; }
    }
}
