using LiveSplit.UI;
using LiveSplit.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.Model.RunImporters
{
    public class SpeedrunComRunImporter : IRunImporter
    {
        public IRun Import(Form form = null)
        {
            var dialog = new BrowseSpeedrunComDialog(false);
            var result = dialog.ShowDialog(form);
            if (result == DialogResult.OK)
                return dialog.Run;
            return null;
        }

        public string ImportAsComparison(IRun run, Form form = null)
        {
            var dialog = new BrowseSpeedrunComDialog(true);
            var result = dialog.ShowDialog(form);
            var name = dialog.RunName;
            if (result == DialogResult.OK)
            {
                result = InputBox.Show(form, "Enter Comparison Name", "Name:", ref name);
            }
            if (result == DialogResult.OK)
            {
                return run.AddComparisonWithNameInput(dialog.Run, name, form);
            }
            return null;
        }
    }
}
