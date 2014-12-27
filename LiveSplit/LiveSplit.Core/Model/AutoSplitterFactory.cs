using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace LiveSplit.Model
{
    public class AutoSplitterFactory
    {
        public static AutoSplitterFactory Instance { get; protected set; }
        public IDictionary<String, AutoSplitter> AutoSplitters { get; set; }

        static AutoSplitterFactory()
        {
            try
            {
                Instance = new AutoSplitterFactory();
                Instance.Init();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        protected AutoSplitterFactory()
        {
        }

        public void Init()
        {
            if (AutoSplitters != null)
                return;

            var document = DownloadAutoSplitters();

            if (document != null)
            {
                AutoSplitters = document["AutoSplitters"].ChildNodes.OfType<XmlElement>().Where(element => element != null).Select(element =>
                    new AutoSplitter()
                    {
                        Description = element["Description"].InnerText,
                        URLs = element["URLs"].ChildNodes.OfType<XmlElement>().Select(x => x.InnerText).ToList(),
                        Type = (AutoSplitterType)Enum.Parse(typeof(AutoSplitterType), element["Type"].InnerText),
                        Games = element["Games"].ChildNodes.OfType<XmlElement>().Select(x => x.InnerText).ToList(),
                        ShowInLayoutEditor = element["ShowInLayoutEditor"] != null
                    }).SelectMany(x => x.Games.Select(y => new KeyValuePair<string, AutoSplitter>(y, x))).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public AutoSplitter Create(string game)
        {
            if (AutoSplitters == null)
                Init();

            if (AutoSplitters != null && !string.IsNullOrEmpty(game) && AutoSplitters.ContainsKey(game))
                return AutoSplitters[game];

            return null;
        }

        protected XmlDocument DownloadAutoSplitters()
        {
            var autoSplitters = new XmlDocument();
            try
            {
#if DEBUG
                autoSplitters.Load("http://livesplit.org/debugautosplitters.xml");
#else
                autoSplitters.Load("http://livesplit.org/LiveSplit.AutoSplitters.xml");
#endif
                autoSplitters.Save("LiveSplit.AutoSplitters.xml");
                return autoSplitters;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                if (File.Exists("LiveSplit.AutoSplitters.xml"))
                    autoSplitters.Load("LiveSplit.AutoSplitters.xml");
                else
                    return null;
                return autoSplitters;
            }
        }
    }
}
