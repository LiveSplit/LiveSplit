using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Options.SettingsSavers
{
    public interface ISettingsSaver
    {
        void Save(ISettings settings, Stream stream);
    }
}
