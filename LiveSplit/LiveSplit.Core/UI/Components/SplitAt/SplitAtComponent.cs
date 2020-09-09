using LiveSplit.Model;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components.SplitAt
{
    public class SplitAtComponent : LogicComponent
    {
        public override string ComponentName => "-- New row/column --";

        public SplitAtSettings Settings { get; set; }

        public SplitAtComponent()
        {
            Settings = new SplitAtSettings();
        }

        public override void Dispose()
        {
            
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return Settings;
        }

        public override void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            
        }
    }
}
