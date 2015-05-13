using LiveSplit.Web.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.Model.RunImporters
{
    public class WorldRecordRunImporter : IRunImporter
    {
        public string Game { get; set; }
        public string Category { get; set; }

        public WorldRecordRunImporter(string game, string category)
        {
            Game = game;
            Category = category;
        }

        public IRun Import(Form form = null)
        {
            var worldRecord = SpeedrunCom.Instance.GetWorldRecord(Game, Category);

            if (!worldRecord.RunAvailable)
            {
                MessageBox.Show(form, "Couldn't find splits for the World Record Run.", "No Splits Found", MessageBoxButtons.OK);
                return null;
            }

            return worldRecord.Run.Value;
        }

        public string ImportAsComparison(IRun run, Form form = null)
        {
            var worldRecordRun = Import(form);
            if (worldRecordRun != null)
            {
                if (run.CustomComparisons.Contains("World Record"))
                {
                    var result = MessageBox.Show(form, "There's already a World Record comparison. Do you want to replace it?", "Replace Comparison?", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                        return null;
                    else
                        run.CustomComparisons.Remove("World Record");
                }

                run.AddComparisonFromRun(worldRecordRun, "World Record", form);

                return "World Record";
            }
            return null;
        }
    }
}
