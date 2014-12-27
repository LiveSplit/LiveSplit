using System;

namespace LiveSplit.UI
{
    public class RenameEventArgs : EventArgs
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
    }
}
