using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LiveSplit.Options;
using LiveSplit.UI;
using LiveSplit.Model;
using System.Collections.Generic;
using SpeedrunComSharp;
using LiveSplit.Web.Share;
using System.Diagnostics;
using System.Text;
using LiveSplit.Utils;

namespace LiveSplit.View
{
    public partial class MetadataControl : UserControl
    {
        public class VariableBinding
        {
            public RunMetadata Metadata { get; set; }
            public Variable Variable { get; set; }
            public event EventHandler VariableChanged;

            public string Value
            {
                get
                {
                    if (Metadata.VariableValueNames.ContainsKey(Variable.Name))
                        return Metadata.VariableValueNames[Variable.Name];

                    return string.Empty;
                }
                set
                {
                    var choice = Variable.Values.FirstOrDefault(x => x.Value == value);
                    string variableValue = null;
                    if (choice == null)
                    {
                        if (Variable.IsUserDefined)
                            variableValue = value;
                    }
                    else
                    {
                        variableValue = choice.Value;
                    }
                    if (Metadata.VariableValueNames.ContainsKey(Variable.Name))
                    {
                        if (variableValue == null)
                            Metadata.VariableValueNames.Remove(Variable.Name);
                        else
                            Metadata.VariableValueNames[Variable.Name] = variableValue;
                    }
                    else if (variableValue != null)
                        Metadata.VariableValueNames.Add(Variable.Name, variableValue);

                    VariableChanged?.Invoke(this, new MetadataChangedEventArgs(true));
                }
            }
        }

        public const int VariablesFirstRowIndex = 3;
        public const int VariablesPerRow = 2;
        public const int ColumnsPerVariable = 3;
        public const AnchorStyles AnchorLeftRight = AnchorStyles.Left | AnchorStyles.Right;

        public RunMetadata Metadata { get; set; }
        public event EventHandler MetadataChanged;

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
            if (Metadata != null)
                Metadata.PropertyChanged += Metadata_Changed;
        }

        void Metadata_Changed(object sender, EventArgs e)
        {
            var metadataChanged = MetadataChanged;
            if (metadataChanged != null)
                metadataChanged(this, e);
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
                cmbRegion.DataBindings.Add("SelectedItem", Metadata, "RegionName");
                cmbPlatform.DataBindings.Add("SelectedItem", Metadata, "PlatformName");
                refreshRules();

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
                        DropDownStyle = variable.IsUserDefined ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList,
                        FormattingEnabled = true,
                        Anchor = AnchorLeftRight,
                        Visible = false
                    };

                    variableComboBox.Items.Add(string.Empty);
                    variableComboBox.Items.AddRange(variable.Values.Select(x => x.Value).ToArray());

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
                    variableBinding.VariableChanged += Metadata_Changed;

                    variableComboBox.DataBindings.Add("Text", variableBinding, "Value");
                    variableComboBox.SelectedIndexChanged += VariableComboBox_SelectedIndexChanged;

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

            RefreshAssociateButton();
        }

        private void refreshRules()
        {
            var additionalRules = new List<string>();

            if (Metadata.Game.Ruleset.DefaultTimingMethod != SpeedrunComSharp.TimingMethod.RealTime)
            {
                var timingText = Metadata.Game.Ruleset.DefaultTimingMethod == SpeedrunComSharp.TimingMethod.RealTimeWithoutLoads
                    ? "are timed without the loading times"
                    : "are timed with the Game Time";

                additionalRules.Add(timingText);
            }

            if (Metadata.Game.Ruleset.RequiresVideo)
            {
                additionalRules.Add("require video proof");
            }

            if (additionalRules.Any())
            {
                var firstRule = additionalRules.First();
                var lastRule = additionalRules.Last();

                var stringBuilder = new StringBuilder();
                stringBuilder.Append("Runs of this game ");

                foreach (var additionalRule in additionalRules)
                {
                    if (additionalRule != firstRule)
                    {
                        if (additionalRule != lastRule)
                            stringBuilder.Append(", ");
                        else
                            stringBuilder.Append(" and ");
                    }

                    stringBuilder.Append(additionalRule);
                }

                stringBuilder.AppendLine(".");
                stringBuilder.AppendLine();

                tbxRules.SelectionFont = new Font(tbxRules.Font, FontStyle.Bold);
                tbxRules.AppendText(stringBuilder.ToString());
                tbxRules.SelectionFont = new Font(tbxRules.Font, FontStyle.Regular);
            }

            if (Metadata.Category != null)
            {
                tbxRules.AppendBBCode(Metadata.Category.Rules ?? string.Empty);
            }
        }

        public void RefreshAssociateButton()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(RefreshAssociateButton));
                return;
            }
            
            if (string.IsNullOrEmpty(Metadata.RunID))
            {
                btnSubmit.Enabled = true;
                btnAssociate.Text = "Associate with Speedrun.com...";
            }
            else
            {
                btnSubmit.Enabled = false;
                btnAssociate.Text = "Show on Speedrun.com...";
            }
        }

        private void associateRun()
        {
            var url = "";
            var result = InputBox.Show("Enter Speedrun.com URL", "Speedrun.com Run URL:", ref url);

            if (result == DialogResult.OK)
            {
                try
                {
                    var run = SpeedrunCom.Client.Runs.GetRunFromSiteUri(url);
                    if (run != null)
                    {
                        Metadata.LiveSplitRun.PatchRun(run);
                        RefreshInformation();
                    }
                    else
                    {
                        MessageBox.Show(this, "The URL provided is not a valid speedrun.com Run URL.", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    MessageBox.Show(this, "The run could not be associated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void showRunOnSpeedrunCom()
        {
            try
            {
                Process.Start(Metadata.Run.WebLink.AbsoluteUri);
            }
            catch { }
        }

        private void btnAssociate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Metadata.RunID))
            {
                associateRun();
            }
            else
            {
                showRunOnSpeedrunCom();
            }
        }

        private void tbxRules_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process.Start(e.LinkText);
            }
            catch { }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string reason;
            var isValid = SpeedrunCom.ValidateRun(Metadata.LiveSplitRun, out reason);

            if (!isValid)
            {
                MessageBox.Show(this, reason, "Submitting Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var submitDialog = new SpeedrunComSubmitDialog(Metadata))
            {
                var result = submitDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    RefreshAssociateButton();
                }
            }
        }

        private void cmbRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            Metadata.RegionName = cmbRegion.SelectedItem.ToString();
        }

        private void cmbPlatform_SelectedIndexChanged(object sender, EventArgs e)
        {
            Metadata.PlatformName = cmbPlatform.SelectedItem.ToString();
        }

        private void VariableComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var variableComboBox = (ComboBox)sender;
            var variableBinding = (VariableBinding)variableComboBox.DataBindings[0].DataSource;
            variableBinding.Value = variableComboBox.SelectedItem.ToString();
        }
    }
}
