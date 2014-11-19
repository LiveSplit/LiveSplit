using System;

namespace LiveSplit.UI
{
    public class RenameEventArgs : EventArgs
    {
        public String OldName { get; set; }
        public String NewName { get; set; }
    }
}
