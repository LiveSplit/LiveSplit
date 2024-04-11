using System.IO;

namespace LiveSplit.Model.RunSavers
{
    public interface IRunSaver
    {
        void Save(IRun run, Stream stream);
    }
}
