using LiveSplit.Model;

namespace LiveSplit.Options
{
    public struct RecentSplitsFile
    {
        public string GameName;
        public string CategoryName;
        public string Path;
        public TimingMethod LastTimingMethod;
        public string LastHotkeySet;

        public RecentSplitsFile(string path, IRun run, TimingMethod method, string hotkeySet)
            : this(path, method, hotkeySet)
        {
            if (run != null)
            {
                GameName = run.GameName;
                CategoryName = run.GetExtendedCategoryName();
            }
        }

        public RecentSplitsFile(string path, TimingMethod method, string hotkeySet, string gameName = null, string categoryName = null)
        {
            GameName = gameName;
            CategoryName = categoryName;
            Path = path;
            LastTimingMethod = method;
            LastHotkeySet = hotkeySet;
        }
    }
}
