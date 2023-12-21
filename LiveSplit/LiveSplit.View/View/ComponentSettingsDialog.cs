using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Options;

namespace LiveSplit.View
{
    public partial class ComponentSettingsDialog : Form
    {
        public XmlNode ComponentSettings { get; set; }
        public IComponent Component { get; set; }

        public ComponentSettingsDialog(IComponent component)
        {
            InitializeComponent();
            Component = component;
            AddComponent(component);
            RefreshText();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Component.SetSettings(ComponentSettings);
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected void AddComponent(IComponent component)
        {
            var settingsControl = Component.GetSettingsControl(LayoutMode.Vertical);
            AddControl(component.ComponentName, settingsControl);
            ComponentSettings = component.GetSettings(new XmlDocument());
        }

        protected void AddControl(string name, Control control)
        {
            control.Location = new Point(0, 0);
            panel.Controls.Add(control);
            Name = name + " Settings";
        }

        private void RefreshText ()
        {
            btnCancel.Text = Languages.Instance.GetText("btnCancel", "Cancel");
            btnOK.Text = Languages.Instance.GetText("btnOK", "OK");
            Text = Languages.Instance.GetText("ComponentSettingsDialog", "Component Settings");
            foreach (Control item in panel.Controls)
            {
                foreach (Control item_1 in item.Controls)
                {
                    foreach (Control item_2 in item_1.Controls)
                    {
                        if (item_2.Controls.Count > 1)
                        {
                            foreach (Control item_3 in item_2.Controls)
                            {
                                if (item_3.Name == "btnCheckAll")
                                {
                                    item_3.Text = Languages.Instance.GetText("btnCheckAll", "Check All");
                                }
                                if (item_3.Name == "btnUncheckAll")
                                {
                                    item_3.Text = Languages.Instance.GetText("btnUncheckAll", "Uncheck All");
                                }
                                if (item_3.Name == "btnResetToDefault")
                                {
                                    item_3.Text = Languages.Instance.GetText("btnResetToDefault", "Reset to Default");
                                }
                                if (item_3.Name == "checkboxStart")
                                {
                                    item_3.Text = Languages.Instance.GetText("checkboxStart", "Start");
                                    item_3.Size = new Size(item_3.Width + 20, item_3.Height);
                                }
                                if (item_3.Name == "checkboxSplit")
                                {
                                    item_3.Text = Languages.Instance.GetText("checkboxSplit", "Split");
                                    item_3.Size = new Size(item_3.Width + 20, item_3.Height);
                                }
                                if (item_3.Name == "checkboxReset")
                                {
                                    item_3.Text = Languages.Instance.GetText("checkboxReset", "Reset");
                                    item_3.Size = new Size(item_3.Width + 20, item_3.Height);
                                }
                            }
                        }
                        if (item_2.Name == "labelScriptPath")
                        {
                            item_2.Text = Languages.Instance.GetText("labelScriptPath", "Script Path:");
                        }
                        if (item_2.Name == "btnSelectFile")
                        {
                            item_2.Text = Languages.Instance.GetText("btnSelectFile", "Browse...");
                        }
                        if (item_2.Name == "labelOptions")
                        {
                            item_2.Text = Languages.Instance.GetText("labelOptions", "Options:");
                        }
                        if (item_2.Name == "labelCustomSettings")
                        {
                            item_2.Text = Languages.Instance.GetText("labelCustomSettings", "Advanced:");
                        }
                    }
                }
            }
        }
    }
}
