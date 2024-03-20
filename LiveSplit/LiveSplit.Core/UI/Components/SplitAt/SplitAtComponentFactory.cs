using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components.SplitAt
{
    public class SplitAtComponentFactory : IComponentFactory
    {
        public string ComponentName => "New row/column";

        public string Description => "Puts the components following this into a new row/column (horizontal/vertical layout)";

        public ComponentCategory Category => ComponentCategory.Other;

        public IComponent Create(LiveSplitState state) => new SplitAtComponent();

        public string UpdateName
        {
            get { throw new NotSupportedException(); }
        }

        public string XMLURL
        {
            get { throw new NotSupportedException(); }
        }

        public string UpdateURL
        {
            get { throw new NotSupportedException(); }
        }

        public Version Version
        {
            get { throw new NotSupportedException(); }
        }
    }
}