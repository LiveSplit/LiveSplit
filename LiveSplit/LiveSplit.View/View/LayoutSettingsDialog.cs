using Fetze.WinFormsColor;
using LiveSplit.Options;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.View
{
    public partial class LayoutSettingsDialog : Form
    {
        public LiveSplit.Options.LayoutSettings Settings { get; set; }
        public LiveSplit.UI.ILayout Layout { get; set; }
        public List<XmlNode> ComponentSettings { get; set; }
        public List<IComponent> Components { get; set; }

        public LayoutSettingsDialog(LiveSplit.Options.LayoutSettings settings, LiveSplit.UI.ILayout layout, IComponent tabComponent = null)
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
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            var i = 0;
            foreach(var component in Components)
            {
                component.SetSettings(ComponentSettings[i]);
                i++;
            }
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

        protected void AddNewTab(String name, Control control)
        {
            var page = new TabPage(name);
            control.Location = new Point(0, 0);
            page.Controls.Add(control);
            page.AutoScroll = true;
            page.Name = name;
            tabControl.TabPages.Add(page);
        }

        private void LayoutSettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing 
                && DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                btnCancel_Click(this, null);
            }
        }
    }
}
