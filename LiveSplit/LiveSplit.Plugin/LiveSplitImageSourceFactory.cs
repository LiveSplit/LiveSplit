using CLROBS;
using LiveSplit.Options;
using System;

namespace LiveSplit.Plugin
{
    public class LiveSplitImageSourceFactory : AbstractImageSourceFactory
    {
        public LiveSplitImageSourceFactory()
        {
            ClassName = "LiveSplitImageSourceClass";
            DisplayName = "LiveSplit Splits";
        }
        public override ImageSource Create(XElement data)
        {
            return new LiveSplitImageSource(data);
        }
        public override bool ShowConfiguration(XElement config)
        {
            var dialog = new LiveSplitConfigurationDialog(config);
            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                if (config.Parent.GetInt("cx") == 0 || config.Parent.GetInt("cy") == 0)
                {
                    try
                    {
                        using (var layoutStream = System.IO.File.OpenRead(config.GetString("layoutpath")))
                        {
                            var xmlDoc = new System.Xml.XmlDocument();
                            xmlDoc.Load(layoutStream);

                            var parent = xmlDoc["Layout"];
                            var width = parent["VerticalWidth"];
                            config.Parent.SetInt("cx", int.Parse(width.InnerText));
                            var height = parent["VerticalHeight"];
                            config.Parent.SetInt("cy", int.Parse(height.InnerText)); //TODO Will break with horizontal
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                return true;
            }
            return false;
        }
    }
}
