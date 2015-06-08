using LiveSplit.Model.Comparisons;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.View
{
    public partial class ChooseComparisonsDialog : Form
    {
        public IDictionary<string, bool> ComparisonGeneratorStates { get; set; }

        public ChooseComparisonsDialog()
        {
            InitializeComponent();

            comparisonsListBox.Items.AddRange(new []
            {
                BestSegmentsComparisonGenerator.ComparisonName,
                BestSplitTimesComparisonGenerator.ComparisonName,
                AverageSegmentsComparisonGenerator.ComparisonName,
                PercentileComparisonGenerator.ComparisonName,
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
            ComparisonGeneratorStates[(string)comparisonsListBox.Items[e.Index]] = e.NewValue == CheckState.Checked;
        }
    }
}
