using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Model.RunSavers
{
    public interface IRunSaver
    {
        void Save(IRun run, Stream stream);
    }
}
