using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LiveSplit.View
{
    public partial class ChooseComparisonsDialog : Form
    {
        public IDictionary<string, bool> ComparisonGeneratorStates { get; set; }
        protected bool DialogInitialized;

        public ChooseComparisonsDialog()
        {
            InitializeComponent();
            DialogInitialized = false;
            comparisonsListBox.Items.AddRange(new []
            {
                BestSegmentsComparisonGenerator.ComparisonName,
                BestSplitTimesComparisonGenerator.ComparisonName,
                AverageSegmentsComparisonGenerator.ComparisonName,
                MedianSegmentsComparisonGenerator.ComparisonName,
                WorstSegmentsComparisonGenerator.ComparisonName,
                PercentileComparisonGenerator.ComparisonName,
                LatestRunComparisonGenerator.ComparisonName,
                NoneComparisonGenerator.ComparisonName
            });
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void comparisonsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (DialogInitialized)
            {
                var generatorName = (string)comparisonsListBox.Items[e.Index];
                ComparisonGeneratorStates[generatorName] = e.NewValue == CheckState.Checked;
            }
        }

        private void ChooseComparisonsDialog_Load(object sender, EventArgs e)
        {
            foreach (var generator in ComparisonGeneratorStates)
            {
                comparisonsListBox.SetItemChecked(comparisonsListBox.Items.IndexOf(generator.Key), generator.Value);
            }
            DialogInitialized = true;
        }
    }
}
