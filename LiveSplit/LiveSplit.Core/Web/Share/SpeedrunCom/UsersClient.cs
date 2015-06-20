using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class UsersClient
    {
        private SpeedrunComClient baseClient;

        public UsersClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }

        public User GetUser(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
