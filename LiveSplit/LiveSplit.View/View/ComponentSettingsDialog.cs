using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Windows.Forms;
using System.Xml;

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
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Component.SetSettings(ComponentSettings);
        }

        protected void AddComponent(IComponent component)
        {
            var settingsControl = Component.GetSettingsControl(LayoutMode.Vertical);
            AddControl(component.ComponentName, settingsControl);
            ComponentSettings = component.GetSettings(new XmlDocument());
        }

        protected void AddControl(string name, Control control)
        {
            panel.Controls.Add(control);
            Name = name + " Settings";
        }

        private void LayoutSettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing 
                && DialogResult != DialogResult.OK)
            {
                btnCancel_Click(this, null);
            }
        }
    }
}
