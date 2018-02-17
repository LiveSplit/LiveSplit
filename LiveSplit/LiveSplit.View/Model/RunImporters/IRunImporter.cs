using LiveSplit.UI;
using LiveSplit.Web.SRL;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.Model.RunImporters
{
    public interface IRunImporter
    {
        IRun Import(Form form = null);
        string ImportAsComparison(IRun run, Form form = null);
    }

    public static class IRunImportRunExtensions
    {
        private static int FindMatchingIndex(IRun target, ISegment segment, int startIndex)
        {
            for (int i = startIndex; i < target.Count; i++)
            {
                var targetSeg = target[i];
                if (targetSeg.Name.Trim().ToLower() == segment.Name.Trim().ToLower())
                    return i;
            }
            return -1;
        }

        private static bool AddComparisonFromRun(this IRun target,
            IRun comparisonRun, string name, Form form = null)
        {
            if (!target.Comparisons.Contains(name))
            {
                if (!SRLComparisonGenerator.IsRaceComparison(name))
                {
                    target.CustomComparisons.Add(name);
                    var maxMatched = -1;
                    foreach (var segment in comparisonRun)
                    {
                        if (segment == comparisonRun.Last())
                            target.Last().Comparisons[name] = comparisonRun.Last().PersonalBestSplitTime;
                        else
                        {
                            var matchingIndex = FindMatchingIndex(target, segment, maxMatched + 1);
                            if (matchingIndex >= 0)
                            {
                                target[matchingIndex].Comparisons[name] = segment.PersonalBestSplitTime;
                                maxMatched = matchingIndex;
                            }
                        }
                    }
                    target.HasChanged = true;
                    target.FixSplits();
                }
                else
                {
                    var result = MessageBox.Show(form, "A Comparison name cannot start with [Race].", "Invalid Comparison Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (result == DialogResult.Retry)
                        return false;
                }
            }
            else
            {
                var result = MessageBox.Show(form, "A Comparison with this name already exists.", "Comparison Already Exists", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (result == DialogResult.Retry)
                    return false;
            }
            return true;
        }

        public static string AddComparisonWithNameInput(this IRun target, 
            IRun comparisonRun, string name, Form form = null)
        {
            while (!AddComparisonFromRun(target, comparisonRun, name, form))
            {
                var result = InputBox.Show(form, "Enter Comparison Name", "Name:", ref name);
                if (result == DialogResult.Cancel)
                    return null;
            }
            return name;
        }
    }
}
