using System.Linq;

namespace LiveSplit.Updates
{
    public static class Git
    {
        public static readonly string Revision = GitInfo.revision;
        public static readonly string Version = GitInfo.version
            .Replace("\r", "").Replace("\n", "")
            .Split('-').Where((x, i) => i != 2 && !(i == 1 && x == "0"))
            .Aggregate((a, b) => a + "-" + b);
        public static readonly string Branch = GitInfo.branch.Replace("\r", "").Replace("\n", "");
    }
}
