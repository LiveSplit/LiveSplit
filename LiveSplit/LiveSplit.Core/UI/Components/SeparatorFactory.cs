using System;

namespace LiveSplit.UI.Components
{
    public class SeparatorFactory : IComponentFactory
    {
        public string ComponentName
            => "Separator";

        public string Description
            => "Shows a line to separate components.";

        public ComponentCategory Category
            => ComponentCategory.Other;

        public IComponent Create(Model.LiveSplitState state)
            => new SeparatorComponent();

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
