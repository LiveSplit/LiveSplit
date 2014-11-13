using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.SRL
{
    public enum SRLIRCRights
    {
        Normal, Voice, Operator
    }

    public static class SRLIRCRightsHelper
    {
        public static SRLIRCRights FromIrcChannelUser(IrcChannelUser user)
        {
            if (user.Modes.Contains('o'))
                return SRLIRCRights.Operator;
            else if (user.Modes.Contains('v'))
                return SRLIRCRights.Voice;
            return SRLIRCRights.Normal;
        }
    }
}
