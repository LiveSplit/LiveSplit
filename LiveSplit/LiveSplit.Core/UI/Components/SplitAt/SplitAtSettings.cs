using System;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components.SplitAt
{
    public partial class SplitAtSettings : UserControl
    {

        public int Spacing { get; set; }

        public int Weight { get; set; }

        public SplitAtSettings()
        {
            InitializeComponent();

            Spacing = 0;
            Weight = 1000;

            trkSpacing.DataBindings.Add("Value", this, "Spacing", false, DataSourceUpdateMode.OnPropertyChanged);
            trkWeight.DataBindings.Add("Value", this, "Weight", false, DataSourceUpdateMode.OnPropertyChanged);

            txtSpacing.TextChanged += Spacing_UpdateValue;
            txtWeight.TextChanged += Weight_UpdateValue;

            trkSpacing.ValueChanged += Spacing_UpdateText;
            trkWeight.ValueChanged += Weight_UpdateText;
            txtSpacing.LostFocus += Spacing_UpdateText;
            txtWeight.LostFocus += Weight_UpdateText;

            btnResetSpacing.Click += BtnResetSpacing_Click;
            btnResetWeight.Click += BtnResetWeight_Click;

            updateText(txtSpacing, trkSpacing);
            updateText(txtWeight, trkWeight);
        }

        private void Spacing_UpdateValue(object sender, EventArgs e)
        {
            updateValue(txtSpacing, trkSpacing);
        }

        private void Weight_UpdateValue(object sender, EventArgs e)
        {
            updateValue(txtWeight, trkWeight);
        }

        private void Spacing_UpdateText(object sender, EventArgs e)
        {
            updateText(txtSpacing, trkSpacing);
        }

        private void Weight_UpdateText(object sender, EventArgs e)
        {
            updateText(txtWeight, trkWeight);
        }

        private void updateValue(TextBox text, TrackBar track)
        {
            // Allow both "," and "." as separator
            var value = text.Text.Replace(",", ".").Replace("%", "");
            try
            {
                var number = float.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                track.Value = (int)(number * 10);
            }
            catch (Exception ex)
            {
                // Don't change
            }
        }

        private void updateText(TextBox text, TrackBar track)
        {
            if (!text.Focused)
            {
                text.Text = String.Format("{0:P1}", track.Value / 1000f);
            }
        }

        private void BtnResetWeight_Click(object sender, EventArgs e)
        {
            trkWeight.Value = 1000;
        }

        private void BtnResetSpacing_Click(object sender, EventArgs e)
        {
            trkSpacing.Value = 0;
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Spacing = SettingsHelper.ParseInt(element["Spacing"], 0);
            Weight = SettingsHelper.ParseInt(element["Weight"], 1000);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.0") ^
            SettingsHelper.CreateSetting(document, parent, "Spacing", Spacing) ^
            SettingsHelper.CreateSetting(document, parent, "Weight", Weight) ^
            SettingsHelper.CreateSetting(document, parent, "Type", "SplitAt");
        }
    }
}
