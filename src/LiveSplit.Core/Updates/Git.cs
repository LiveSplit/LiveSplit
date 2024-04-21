using System;
using System.Linq;

namespace LiveSplit.Updates
{
    public static class Git
    {
        private static readonly string Revision = (
            string.IsNullOrWhiteSpace(GitInfo.revision)
            ? null
            : GitInfo.revision.Replace("\r", "").Replace("\n", "")
        );
        private static readonly string Describe = (
            string.IsNullOrWhiteSpace(GitInfo.version)
            ? null
            : GitInfo.version.Replace("\r", "").Replace("\n", "")
        );
        private static readonly string[] DescribeSplit = Describe?.Split('-');
        private static readonly bool IsDirty = DescribeSplit?.Last() == "dirty";
        public static readonly string LastTag = DescribeSplit?[0];
        public static readonly int CommitsSinceLastTag = (DescribeSplit == null || DescribeSplit.Length < 3) ? 0 : int.Parse(DescribeSplit[1]);
        public static readonly string Version = (
            Describe == null ? null : new[] { LastTag }
            .Concat(CommitsSinceLastTag > 0 ? new[] { CommitsSinceLastTag.ToString() } : new string[0])
#if DEBUG
            .Concat(new[] { "debug" })
#endif
            .Concat(IsDirty ? new[] { "dirty" } : new string[0])
            .Aggregate((a, b) => a + "-" + b)
        );
        public static readonly string Branch = (
            string.IsNullOrWhiteSpace(GitInfo.branch)
            ? null
            : GitInfo.branch.Replace("\r", "").Replace("\n", "")
        );
        public static readonly Uri RevisionUri = (
            LastTag == null || Revision == null
            ? null
            : new Uri("https://github.com/LiveSplit/LiveSplit/tree/" + (CommitsSinceLastTag > 0 ? Revision : LastTag))
        );
    }
}
