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
        void ImportAsComparison(IRun run, Form form = null);
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
                    foreach (var segment in comparisonRun)
                    {
                        var runSegment = target.FirstOrDefault(x => x.Name == segment.Name);
                        if (runSegment != null)
                            runSegment.Comparisons[name] = segment.PersonalBestSplitTime;
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

        public static bool AddComparisonWithNameInput(this IRun target, 
            IRun comparisonRun, string name, Form form = null)
        {
            while (!AddComparisonFromRun(target, comparisonRun, name, form))
            {
                var result = InputBox.Show(form, "Enter Comparison Name", "Name:", ref name);
                if (result == DialogResult.Cancel)
                    return false;
            }
            return true;
        }
    }
}
