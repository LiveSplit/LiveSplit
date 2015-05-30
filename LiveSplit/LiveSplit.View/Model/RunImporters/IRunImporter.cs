using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private static bool AddComparisonFromRun(this IRun target,
            IRun comparisonRun, string name, Form form = null)
        {
            if (!target.Comparisons.Contains(name))
            {
                if (!name.StartsWith("[Race]"))
                {
                    target.CustomComparisons.Add(name);
                    if (comparisonRun.Count > 0 && target.Count > 0)
                    {
                        foreach (var segment in comparisonRun)
                        {
                            if (segment == comparisonRun.Last())
                                target.Last().Comparisons[name] = comparisonRun.Last().PersonalBestSplitTime;
                            else
                            {
                                var runSegment = target.FirstOrDefault(x => x.Name.Trim().ToLower() == segment.Name.Trim().ToLower());
                                if (runSegment != null)
                                    runSegment.Comparisons[name] = segment.PersonalBestSplitTime;
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
