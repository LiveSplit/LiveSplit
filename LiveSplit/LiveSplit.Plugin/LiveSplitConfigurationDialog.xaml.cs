using CLROBS;
using System.Windows;

namespace LiveSplit.Plugin
{
    /// <summary>
    /// Interaktionslogik für LiveSplitConfigurationDialog.xaml
    /// </summary>
    public partial class LiveSplitConfigurationDialog : Window
    {
        XElement config;

        public LiveSplitConfigurationDialog(XElement config)
        {
            this.config = config;
            InitializeComponent();

            txtSplitsPath.Text = config.GetString("splitspath");
            txtLayoutPath.Text = config.GetString("layoutpath");
            var anchorRight = config.GetInt("anchorright") == 1;
            var anchorBottom = config.GetInt("anchorbottom") == 1;
            if (anchorRight)
            {
                if (anchorBottom)
                    rdoBottomRight.IsChecked = true;
                else
                    rdoTopRight.IsChecked = true;
            }
            else
            {
                if (anchorBottom)
                    rdoBottomLeft.IsChecked = true;
                else
                    rdoTopLeft.IsChecked = true;
            }
        }

        private void btnSplits_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = txtSplitsPath.Text;
            dlg.Multiselect = false;
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                txtSplitsPath.Text = dlg.FileName;
            }
        }

        private void btnLayout_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "LiveSplit Layout (*.lsl)|*.lsl|All Files (*.*)|*.*";
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                txtLayoutPath.Text = dlg.FileName;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            config.SetString("splitspath", txtSplitsPath.Text);
            config.SetString("layoutpath", txtLayoutPath.Text);
            config.SetInt("anchorright", (rdoTopRight.IsChecked.Value || rdoBottomRight.IsChecked.Value) ? 1 : 0);
            config.SetInt("anchorbottom", (rdoBottomLeft.IsChecked.Value || rdoBottomRight.IsChecked.Value) ? 1 : 0);
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
