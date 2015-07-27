using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.View
{
    public partial class LayoutSettingsDialog : Form
    {
        public LiveSplit.UI.LayoutSettings Settings { get; set; }
        public UI.ILayout Layout { get; set; }
        public List<XmlNode> ComponentSettings { get; set; }
        public List<IComponent> Components { get; set; }

        public LayoutSettingsDialog(LiveSplit.UI.LayoutSettings settings, UI.ILayout layout, IComponent tabComponent = null)
        {
            InitializeComponent();
            Settings = settings;
            Layout = layout;
            ComponentSettings = new List<XmlNode>();
            Components = new List<IComponent>();
            AddNewTab("Layout", new LayoutSettingsControl(settings, layout));
            AddComponents(tabComponent);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].SetSettings(ComponentSettings[i]);
            }

            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected void AddComponents(IComponent tabComponent = null)
        {
            foreach (var component in Layout.Components)
            {
                var settingsControl = component.GetSettingsControl(Layout.Mode);
                if (settingsControl != null)
                {
                    AddNewTab(component.ComponentName, settingsControl);
                    ComponentSettings.Add(component.GetSettings(new XmlDocument()));
                    Components.Add(component);
                    if (component == tabComponent)
                    {
                        tabControl.SelectTab(tabControl.TabPages.Count - 1);
                    }
                }
            }
        }

        protected void AddNewTab(string name, Control control)
        {
            var page = new TabPage(name);
            control.Location = new Point(0, 0);
            page.Controls.Add(control);
            page.AutoScroll = true;
            page.Name = name;
            tabControl.TabPages.Add(page);
        }
    }
}
