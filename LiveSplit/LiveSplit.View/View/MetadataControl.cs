using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Fetze.WinFormsColor;
using LiveSplit.Options;
using LiveSplit.UI;
using LiveSplit.Model;

namespace LiveSplit.View
{
    public partial class MetadataControl : UserControl
    {
        public RunMetadata Metadata { get; set; }

        public MetadataControl()
        {
            InitializeComponent();
        }

        private void MetadataControl_Load(object sender, EventArgs e)
        {
            RefreshInformation();
        }

        public void RefreshInformation()
        {
            cmbRegion.Items.Clear();
            cmbPlatform.Items.Clear();
            cmbRegion.DataBindings.Clear();
            cmbPlatform.DataBindings.Clear();
            tbxRules.Clear();

            if (Metadata.Game != null)
            {
                cmbRegion.Items.Add(string.Empty);
                cmbPlatform.Items.Add(string.Empty);
                cmbRegion.Items.AddRange(Metadata.Game.Regions.Select(x => x.Name).ToArray());
                cmbPlatform.Items.AddRange(Metadata.Game.Platforms.Select(x => x.Name).ToArray());
                cmbRegion.DataBindings.Add("SelectedItem", Metadata, "RegionName", false, DataSourceUpdateMode.OnPropertyChanged);
                cmbPlatform.DataBindings.Add("SelectedItem", Metadata, "PlatformName", false, DataSourceUpdateMode.OnPropertyChanged);
                
                if (Metadata.Category != null)
                {
                    tbxRules.Text = Metadata.Category.Rules ?? string.Empty;
                }
            }

            cmbRegion.Enabled = cmbRegion.Items.Count > 1;
            cmbPlatform.Enabled = cmbPlatform.Items.Count > 1;
        }
    }
}
