using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class RunStatus
    {
        public RunStatusType Type { get; private set; }
        public string ExaminerUserID { get; private set; }
        public string Reason { get; private set; }

        private RunStatus() { }

        private static RunStatusType ParseType(string type)
        {
            switch (type)
            {
                case "new":
                    return RunStatusType.New;
                case "verified":
                    return RunStatusType.Verified;
                case "rejected":
                    return RunStatusType.Rejected;
            }

            throw new ArgumentException("type");
        }

        public static RunStatus Parse(SpeedrunComClient client, dynamic statusElement)
        {
            var status = new RunStatus();

            status.Type = ParseType(statusElement.status as string);

            if (status.Type == RunStatusType.Rejected 
                || status.Type == RunStatusType.Verified)
            {
                status.ExaminerUserID = statusElement.examiner as string;
            }

            if (status.Type == RunStatusType.Rejected)
            {
                status.Reason = statusElement.reason as string;
            }

            return status;
        }

        public override string ToString()
        {
            if (Type == RunStatusType.Rejected)
                return "Rejected:" + Reason;
            else
                return Type.ToString();
        }
    }
}
