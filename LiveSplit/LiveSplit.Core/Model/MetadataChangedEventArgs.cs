using System;
namespace LiveSplit.Model
{
    public class MetadataChangedEventArgs : EventArgs
    {
        public bool ClearRunID { get; set; }

        public MetadataChangedEventArgs(bool clearRunID)
        {
            ClearRunID = clearRunID;
        }
    }
}
