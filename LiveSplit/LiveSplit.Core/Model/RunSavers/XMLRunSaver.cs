using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using static LiveSplit.UI.SettingsHelper;

namespace LiveSplit.Model.RunSavers
{
    public class XMLRunSaver : IRunSaver
    {
        public void Save(IRun run, Stream stream)
        {
            using (var lscRun = new LiveSplitCore.Run())
            {
                var savedRun = lscRun.SaveAsLss();

                var writer = new StreamWriter(stream);
                writer.Write(savedRun);
                writer.Flush();
            }
        }
    }
}
