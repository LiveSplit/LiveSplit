using LiveSplit.Web.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.Model.RunImporters
{
    public class LeaderboardImporter
    {
        private string gameName;
        private string categoryName;

        public LeaderboardImporter(string gameName, string categoryName)
        {
            this.gameName = gameName;
            this.categoryName = categoryName;
        }

        public IEnumerable<string> ImportComparisons(IRun run, Form form = null)
        {
            var leaderboard = SpeedrunCom.Instance.GetLeaderboard(gameName, categoryName);

            foreach (var record in leaderboard.Where(x => x.RunAvailable))
            {
                var recordRun = record.Run.Value;

                if (recordRun != null 
                    && !(recordRun.LastOrDefault().PersonalBestSplitTime.RealTime == run.LastOrDefault().PersonalBestSplitTime.RealTime
                         && recordRun.LastOrDefault().PersonalBestSplitTime.GameTime == run.LastOrDefault().PersonalBestSplitTime.GameTime))
                {
                    var name = record.Runner;

                    if (run.CustomComparisons.Contains(name))
                    {
                        var result = MessageBox.Show(form, 
                            string.Format("There's already a comparison called {0}. Do you want to replace it?", name), 
                            "Replace Comparison?", MessageBoxButtons.YesNoCancel);

                        if (result == DialogResult.No)
                            continue;
                        else if (result == DialogResult.Cancel)
                            yield break;
                        else
                            run.CustomComparisons.Remove(name);
                    }

                    run.AddComparisonFromRun(recordRun, name, form);

                    yield return name;
                }
            }
        }
    }
}
