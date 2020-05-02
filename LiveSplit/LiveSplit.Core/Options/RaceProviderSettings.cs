using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.Options
{
    public abstract class RaceProviderSettings : ICloneable
    {
        public bool Enabled = true;        
        public abstract string Name { get; set; }
        public abstract string DisplayName { get; }
        public abstract string WebsiteLink { get; }
        public abstract string RulesLink { get; }

        public abstract object Clone();

        public virtual Control GetSettingsControl()
        {
            var control = new UserControl();
            control.Controls.Add(new Label() {
                Text = "There are no options available for this racing service.",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.TopCenter
            });
            control.Dock = DockStyle.Fill;
            return control;
        }

        public virtual void FromXml(XmlElement element, Version version)
        {
            var enabled = element.Attributes["enabled"];
            if (!bool.TryParse(enabled.Value, out Enabled))
                Enabled = true;
        }

        public virtual XmlElement ToXml(XmlDocument document)
        {
            var parent = document.CreateElement("Plugin");

            var providerName = document.CreateAttribute("name");
            if (providerName != null)
                providerName.InnerText = Name.ToString();
            parent.Attributes.Append(providerName);

            var enabled = document.CreateAttribute("enabled");
            if (enabled != null)
                enabled.InnerText = Enabled.ToString();
            parent.Attributes.Append(enabled);

            return parent;
        }
    }

    public class UnloadedRaceProviderSettings : RaceProviderSettings
    {
        public override string Name { get; set; }
        public override string DisplayName => Name;
        private string Content { get; set; }
        public override string WebsiteLink => "";
        public override string RulesLink => "";

        public override object Clone()
        {
            return new UnloadedRaceProviderSettings()
            {
                Enabled = Enabled,
                Name = Name,
                Content = Content
            };     
        }

        public override Control GetSettingsControl()
        {
            var control = new UserControl();
            control.Controls.Add(new Label()
            {
                Text = "Plugin could not be loaded",
                Dock = DockStyle.Fill,
                ForeColor = System.Drawing.Color.Red,
                TextAlign = System.Drawing.ContentAlignment.TopCenter
            });
            control.Dock = DockStyle.Fill;
            return control;
        }

        public override void FromXml(XmlElement element, Version version)
        {
            base.FromXml(element, version);
            
            Content = element.InnerXml;

            if (element.Attributes["name"] != null)
            {
                Name = element.Attributes["name"].InnerText;
            }
        }

        public override XmlElement ToXml(XmlDocument document)
        {
            XmlElement element = base.ToXml(document);

            if (element.Attributes["name"] == null)
            {
                element.Attributes.Append(document.CreateAttribute("name"));
            }
            element.Attributes["name"].Value = Name;
           
            element.InnerXml = Content;
            return element;
        }
    }
}
