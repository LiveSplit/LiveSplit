using System.IO;
using System.Linq;

namespace LiveSplit.Model.RunSavers
{
    public class WSplitRunSaver : IRunSaver
    {
        protected string Escape(string text)
        {
            return text.Replace(@",", @"‡");
        }

        public void Save(IRun run, Stream stream)
        {
            var writer = new StreamWriter(stream);

            writer.Write("Title=");

            if (!string.IsNullOrEmpty(run.GameName))
            {
                writer.Write(Escape(run.GameName));

                if (!string.IsNullOrEmpty(run.CategoryName))
                    writer.Write(" - ");
            }

            writer.WriteLine(Escape(run.CategoryName));

            writer.Write("Attempts=");
            writer.WriteLine(Escape(run.AttemptCount.ToString()));

            writer.Write("Offset=");
            writer.WriteLine(Escape(run.Offset.TotalSeconds.ToString()));

            writer.WriteLine("Size=200,200");

            foreach (var segment in run)
            {
                writer.Write(Escape(segment.Name));
                writer.Write(",0,");
                writer.Write(Escape(segment.PersonalBestSplitTime.RealTime.HasValue ? segment.PersonalBestSplitTime.RealTime.Value.TotalSeconds.ToString() : "0"));
                writer.Write(',');
                writer.WriteLine(Escape(segment.BestSegmentTime.RealTime.HasValue ? segment.BestSegmentTime.RealTime.Value.TotalSeconds.ToString() : "0"));
            }

            writer.Write("Icons=");
            writer.Write(Enumerable.Repeat("\"\",", run.Count - 1));
            writer.WriteLine("\"\"");

            writer.Flush();
        }
    }
}
