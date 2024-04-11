using System.IO;

namespace LiveSplit.Options.SettingsSavers
{
    public interface ISettingsSaver
    {
        void Save(ISettings settings, Stream stream);
    }
}
