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

        private List<Control> dynamicControls;
        private List<VariableBinding> variableBindings;

        public MetadataControl()
        {
            InitializeComponent();
            dynamicControls = new List<Control>();
            variableBindings = new List<VariableBinding>();
        }

        private void MetadataControl_Load(object sender, EventArgs e)
        {
            RefreshInformation();
        }

        private int getDynamicControlRowIndex(int controlIndex)
        {
            return controlIndex / VariablesPerRow + VariablesFirstRowIndex;
        }

        private int getDynamicControlColumnIndex(int controlIndex)
        {
            return ColumnsPerVariable * (controlIndex % VariablesPerRow);
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

            foreach (var control in dynamicControls)
            {
                tableLayoutPanel1.Controls.Remove(control);
            }

            dynamicControls.Clear();
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

                var controlIndex = 0;

                if (Metadata.Game != null && Metadata.Game.Ruleset.EmulatorsAllowed)
                {
                    var emulatedRow = getDynamicControlRowIndex(controlIndex);
                    var emulatedColumn = getDynamicControlColumnIndex(controlIndex);

                    var emulatedCheckBox = new CheckBox
                    {
                        Text = "Uses Emulator",
                        Anchor = AnchorLeftRight,
                        Margin = new Padding(7, 3, 3, 3),
                        Height = 21,
                        Visible = false
                    };

                    tableLayoutPanel1.Controls.Add(emulatedCheckBox, emulatedColumn, emulatedRow);
                    tableLayoutPanel1.SetColumnSpan(emulatedCheckBox, 2);

                    emulatedCheckBox.DataBindings.Add("Checked", Metadata, "UsesEmulator", false, DataSourceUpdateMode.OnPropertyChanged);

                    dynamicControls.Add(emulatedCheckBox);
                    
                    controlIndex++;
                }

                foreach (var variable in Metadata.VariableValues.Keys)
                {
                    var variableLabel = new Label()
                    {
                        Text = variable.Name + ":",
                        AutoSize = true,
                        Anchor = AnchorLeftRight,
                        Visible = false
                    };

                    var variableComboBox = new ComboBox()
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        FormattingEnabled = true,
                        Anchor = AnchorLeftRight,
                        Visible = false
                    };

                    variableComboBox.Items.Add(string.Empty);
                    variableComboBox.Items.AddRange(variable.Choices.Select(x => x.Value).ToArray());

                    var variableRow = getDynamicControlRowIndex(controlIndex);
                    var variableLabelColumn = getDynamicControlColumnIndex(controlIndex);
                    var variableComboBoxColumn = variableLabelColumn + 1;

                    tableLayoutPanel1.Controls.Add(variableLabel, variableLabelColumn, variableRow);
                    tableLayoutPanel1.Controls.Add(variableComboBox, variableComboBoxColumn, variableRow);

                    var variableBinding = new VariableBinding()
                    {
                        Metadata = Metadata,
                        Variable = variable
                    };

                    variableComboBox.DataBindings.Add("SelectedItem", variableBinding, "Value", false, DataSourceUpdateMode.OnPropertyChanged);

                    dynamicControls.Add(variableLabel);
                    dynamicControls.Add(variableComboBox);
                    variableBindings.Add(variableBinding);

                    controlIndex++;
                }
            }

            foreach (var control in dynamicControls)
            {
                control.Visible = true;
            }

            cmbRegion.Enabled = cmbRegion.Items.Count > 1;
            cmbPlatform.Enabled = cmbPlatform.Items.Count > 1;
        }
    }
}
