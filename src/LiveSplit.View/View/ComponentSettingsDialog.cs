using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Drawing;
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
    }
}
