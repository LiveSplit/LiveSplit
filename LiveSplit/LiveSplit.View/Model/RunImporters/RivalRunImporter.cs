using LiveSplit.Web.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.Model.RunImporters
{
    public class RivalRunImporter : IRunImporter
    {
        private string gameName;
        private string categoryName;
        private Time pbTime;

        public RivalRunImporter(string gameName, string categoryName, Time pbTime)
        {
            this.gameName = gameName;
            this.categoryName = categoryName;
            this.pbTime = pbTime;
        }

        private TimingMethod getLeaderboardTimingMethod(IEnumerable<SpeedrunCom.Record> leaderboard)
        {
            var lastRecord = leaderboard.First();

            foreach (var record in leaderboard)
            {
                if (lastRecord.Time.RealTime > record.Time.RealTime)
                    return TimingMethod.GameTime;
                lastRecord = record;
            }

            return TimingMethod.RealTime;
        }

        public IRun Import(Form form = null)
        {
            var leaderboard = SpeedrunCom.Instance.GetLeaderboard(gameName, categoryName);
            var timingMethod = getLeaderboardTimingMethod(leaderboard);

            var record = leaderboard
                .TakeWhile(x => x.Time[timingMethod].HasValue
                    && pbTime[timingMethod].HasValue
                    && (int)x.Time[timingMethod].Value.TotalSeconds
                       < (int)pbTime[timingMethod].Value.TotalSeconds)
                .Reverse()
                .FirstOrDefault(x => x.RunAvailable);

            if (!record.RunAvailable)
            {
                MessageBox.Show(form, "Couldn't find any splits for runs that are faster than your Personal Best.", "No Splits Found", MessageBoxButtons.OK);
                return null;
            }

            return record.Run.Value;
        }

        public string ImportAsComparison(IRun run, Form form = null)
        {
            var rivalRun = Import(form);
            if (rivalRun != null)
            {
                if (run.CustomComparisons.Contains("Rival"))
                {
                    var result = MessageBox.Show(form, "There's already a Rival comparison. Do you want to replace it?", "Replace Comparison?", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                        return null;
                    else
                        run.CustomComparisons.Remove("Rival");
                }

                run.AddComparisonFromRun(rivalRun, "Rival", form);

                return "Rival";
            }
            return null;
        }
    }
}
