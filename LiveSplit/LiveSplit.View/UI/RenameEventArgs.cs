using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.UI
{
    public class RenameEventArgs : EventArgs
    {
        public String OldName { get; set; }
        public String NewName { get; set; }
    }
}
