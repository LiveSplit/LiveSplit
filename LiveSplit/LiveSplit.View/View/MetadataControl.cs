using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Fetze.WinFormsColor;
using LiveSplit.Options;
using LiveSplit.UI;
using LiveSplit.Model;
using System.Collections.Generic;
using SpeedrunComSharp;

namespace LiveSplit.View
{
    public partial class MetadataControl : UserControl
    {
        public class VariableBinding
        {
            public RunMetadata Metadata { get; set; }
            public Variable Variable { get; set; }

            public string Value
            {
                get
                {
                    return Metadata.VariableValueNames[Variable.Name];
                }
                set
                {
                    var choice = Variable.Choices.FirstOrDefault(x => x.Value == value);
                    var choiceId = choice != null ? choice.ID : string.Empty;
                    if (Metadata.VariableValueIDs.ContainsKey(Variable.ID))
                        Metadata.VariableValueIDs[Variable.ID] = choiceId;
                    else
                        Metadata.VariableValueIDs.Add(Variable.ID, choiceId);
                }
            }
        }

        public const int VariablesFirstRowIndex = 3;
        public const int VariablesPerRow = 2;
        public const int ColumnsPerVariable = 3;
        public const AnchorStyles AnchorLeftRight = AnchorStyles.Left | AnchorStyles.Right;

        public RunMetadata Metadata { get; set; }

        private List<Label> variableLabels;
        private List<ComboBox> variableComboBoxes;
        private List<VariableBinding> variableBindings;

        public MetadataControl()
        {
            InitializeComponent();
            variableLabels = new List<Label>();
            variableComboBoxes = new List<ComboBox>();
            variableBindings = new List<VariableBinding>();
        }

        private void MetadataControl_Load(object sender, EventArgs e)
        {
            RefreshInformation();
        }

        public void RefreshInformation()
        {
            if (Metadata == null)
                return;

            cmbRegion.Items.Clear();
            cmbPlatform.Items.Clear();
            cmbRegion.DataBindings.Clear();
            cmbPlatform.DataBindings.Clear();
            tbxRules.Clear();

            foreach (var control in variableLabels.Cast<Control>().Concat(variableComboBoxes))
            {
                tableLayoutPanel1.Controls.Remove(control);
            }

            variableLabels.Clear();
            variableComboBoxes.Clear();
            variableBindings.Clear();

            if (Metadata.Game != null)
            {
                cmbRegion.Items.Add(string.Empty);
                cmbPlatform.Items.Add(string.Empty);
                cmbRegion.Items.AddRange(Metadata.Game.Regions.Select(x => x.Name).ToArray());
                cmbPlatform.Items.AddRange(Metadata.Game.Platforms.Select(x => x.Name).ToArray());
                cmbRegion.DataBindings.Add("SelectedItem", Metadata, "RegionName", false, DataSourceUpdateMode.OnPropertyChanged);
                cmbPlatform.DataBindings.Add("SelectedItem", Metadata, "PlatformName", false, DataSourceUpdateMode.OnPropertyChanged);

                if (Metadata.Category != null)
                {
                    tbxRules.Text = Metadata.Category.Rules ?? string.Empty;
                }

                var variableIndex = 0;
                foreach (var variable in Metadata.VariableValues.Keys)
                {
                    var variableLabel = new Label()
                    {
                        Text = variable.Name + ":",
                        AutoSize = true,
                        Anchor = AnchorLeftRight
                    };

                    var variableComboBox = new ComboBox()
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        FormattingEnabled = true,
                        Anchor = AnchorLeftRight
                    };

                    variableComboBox.Items.Add(string.Empty);
                    variableComboBox.Items.AddRange(variable.Choices.Select(x => x.Value).ToArray());

                    var variableRow = variableIndex / VariablesPerRow + VariablesFirstRowIndex;
                    var variableLabelColumn = ColumnsPerVariable * (variableIndex % VariablesPerRow);
                    var variableComboBoxColumn = variableLabelColumn + 1;

                    tableLayoutPanel1.Controls.Add(variableLabel, variableLabelColumn, variableRow);
                    tableLayoutPanel1.Controls.Add(variableComboBox, variableComboBoxColumn, variableRow);

                    var variableBinding = new VariableBinding()
                    {
                        Metadata = Metadata,
                        Variable = variable
                    };

                    variableComboBox.DataBindings.Add("SelectedItem", variableBinding, "Value", false, DataSourceUpdateMode.OnPropertyChanged);

                    variableLabels.Add(variableLabel);
                    variableComboBoxes.Add(variableComboBox);
                    variableBindings.Add(variableBinding);

                    variableIndex++;
                }
            }

            cmbRegion.Enabled = cmbRegion.Items.Count > 1;
            cmbPlatform.Enabled = cmbPlatform.Items.Count > 1;
        }
    }
}
