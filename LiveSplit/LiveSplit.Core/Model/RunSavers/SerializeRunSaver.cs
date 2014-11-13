using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Model.RunSavers
{
    public class SerializeRunSaver : IRunSaver
    {
        public void Save(IRun run, Stream stream)
        {
            var obj = (IRun)run.Clone();
            foreach (var segment in obj)
            {
                segment.SplitTime = default(Time);
            }

            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }
    }
}
