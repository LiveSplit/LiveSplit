using System;
using System.Linq;

namespace LiveSplit.Updates
{
    public static class Git
    {
        public static readonly string Revision = GitInfo.revision.Replace("\r", "").Replace("\n", "");
        public static readonly string Describe = GitInfo.version.Replace("\r", "").Replace("\n", "");
        private static readonly string[] DescribeSplit = Describe.Split('-');
        public static readonly bool IsDirty = DescribeSplit.Last() == "dirty";
        public static readonly string LastTag = DescribeSplit[0];
        public static readonly int CommitsSinceLastTag = int.Parse(DescribeSplit[1]);
        public static readonly string Version = new[] { LastTag }
            .Concat(CommitsSinceLastTag > 0 ? new[] { CommitsSinceLastTag.ToString() } : new string[0])
#if DEBUG
            .Concat(new[] { "debug" })
#endif
            .Concat(IsDirty ? new[] { "dirty" } : new string[0])
            .Aggregate((a, b) => a + "-" + b);
        public static readonly string Branch = GitInfo.branch.Replace("\r", "").Replace("\n", "");
        public static readonly Uri RevisionUri = new Uri("https://github.com/LiveSplit/LiveSplit/tree/" + (CommitsSinceLastTag > 0 ? Revision : LastTag));
    }
}
