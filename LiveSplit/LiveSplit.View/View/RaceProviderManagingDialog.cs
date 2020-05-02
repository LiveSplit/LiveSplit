using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.View
{
    public partial class RaceProviderManagingDialog : Form
    {
        public IList<RaceProviderSettings> Settings { get; set; }

        public RaceProviderManagingDialog(IList<RaceProviderSettings> settings)
        {
            InitializeComponent();
            websiteLink.LinkClicked += LinkClicked;
            rulesLink.LinkClicked += LinkClicked;
            Settings = settings;

            foreach (var raceProvider in Settings)
            {
                providerListBox.Items.Add(raceProvider.DisplayName, raceProvider.Enabled);
            }
        }

        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = sender as LinkLabel;
            if (link != null && !string.IsNullOrEmpty(link.Text))
                Process.Start($"{link.Text}");
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void providerListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            Settings[e.Index].Enabled = e.NewValue == CheckState.Checked;
        }

        private void providerListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (providerListBox.SelectedIndex < 0)
                return;

            websiteLink.Text = Settings[providerListBox.SelectedIndex].WebsiteLink;
            rulesLink.Text = Settings[providerListBox.SelectedIndex].RulesLink;
            settingsUIPanel.Controls.Clear();
            settingsUIPanel.Controls.Add(Settings[providerListBox.SelectedIndex].GetSettingsControl());
            settingsUIPanel.Controls[0].Visible = true;
            websiteTextLabel.Visible = !string.IsNullOrEmpty(Settings[providerListBox.SelectedIndex].WebsiteLink);
            rulesTextLabel.Visible = !string.IsNullOrEmpty(Settings[providerListBox.SelectedIndex].RulesLink);
        }
    }
}
