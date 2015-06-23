using LiveSplit.Model.Input;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace LiveSplit.View
{
    public partial class SettingsDialog : Form
    {
        public ISettings Settings { get; set; }
        public CompositeHook Hook { get; set; }

        public string SplitKey { get { return FormatKey(Settings.SplitKey); } }
        public string ResetKey { get { return FormatKey(Settings.ResetKey); } }
        public string SkipKey { get { return FormatKey(Settings.SkipKey); } }
        public string UndoKey { get { return FormatKey(Settings.UndoKey); } }
        public string PauseKey { get { return FormatKey(Settings.PauseKey); } }
        public string SwitchComparisonPrevious { get { return FormatKey(Settings.SwitchComparisonPrevious); } }
        public string SwitchComparisonNext { get { return FormatKey(Settings.SwitchComparisonNext); } }
        public string ToggleGlobalHotkeys { get { return FormatKey(Settings.ToggleGlobalHotkeys); } }
        public bool GlobalHotkeysEnabled { get { return Settings.GlobalHotkeysEnabled; } set { Settings.GlobalHotkeysEnabled = value; } }
        public bool DeactivateHotkeysForOtherPrograms { get { return Settings.DeactivateHotkeysForOtherPrograms; } set { Settings.DeactivateHotkeysForOtherPrograms = value; } }
        public float HotkeyDelay { get { return Settings.HotkeyDelay; } set { Settings.HotkeyDelay = Math.Max(value, 0); } }
        public bool WarnOnReset { get { return Settings.WarnOnReset; } set { Settings.WarnOnReset = value; } }
        public bool DoubleTapPrevention { get { return Settings.DoubleTapPrevention; } set { Settings.DoubleTapPrevention = value; } }

        public event EventHandler SumOfBestModeChanged;

        public string RaceViewer { get { return Settings.RaceViewer.Name; } set { Settings.RaceViewer = LiveSplit.Web.SRL.RaceViewer.FromName(value); } }

        public SettingsDialog(CompositeHook hook, ISettings settings)
        {
            InitializeComponent();
            Settings = settings;
            Hook = hook;

            txtStartSplit.DataBindings.Add("Text", this, "SplitKey");
            txtReset.DataBindings.Add("Text", this, "ResetKey");
            txtSkip.DataBindings.Add("Text", this, "SkipKey");
            txtUndo.DataBindings.Add("Text", this, "UndoKey");
            txtPause.DataBindings.Add("Text", this, "PauseKey");
            txtToggle.DataBindings.Add("Text", this, "ToggleGlobalHotkeys");
            txtSwitchPrevious.DataBindings.Add("Text", this, "SwitchComparisonPrevious");
            txtSwitchNext.DataBindings.Add("Text", this, "SwitchComparisonNext");
            chkGlobalHotkeys.DataBindings.Add("Checked", this, "GlobalHotkeysEnabled");
            chkWarnOnReset.DataBindings.Add("Checked", this, "WarnOnReset");
            chkDoubleTap.DataBindings.Add("Checked", this, "DoubleTapPrevention");
            txtDelay.DataBindings.Add("Text", this, "HotkeyDelay");
            cbxRaceViewer.DataBindings.Add("SelectedItem", this, "RaceViewer");

            SetClickEvents();
            chkGlobalHotkeys_CheckedChanged(this, null);
        }

        void chkSimpleSOB_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SimpleSumOfBest = chkSimpleSOB.Checked;
            if (SumOfBestModeChanged != null)
                SumOfBestModeChanged(this, null);
        }

        void SettingsDialog_Load(object sender, EventArgs e)
        {
            chkSimpleSOB.Checked = Settings.SimpleSumOfBest;
            chkGlobalHotkeys_CheckedChanged(null, null);
        }

        void chkGlobalHotkeys_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGlobalHotkeys.Checked)
            {
                chkDeactivateForOtherPrograms.Enabled = true;
                chkDeactivateForOtherPrograms.DataBindings.Add("Checked", this, "DeactivateHotkeysForOtherPrograms");
            }
            else
            {
                chkDeactivateForOtherPrograms.Enabled = false;
                chkDeactivateForOtherPrograms.DataBindings.Clear();
                chkDeactivateForOtherPrograms.Checked = false;
            }
        }

        private string FormatKey(KeyOrButton key)
        {
            if (key != null)
            {
                var keyString = key.ToString();
                if (key.IsButton)
                {
                    var lastSpaceIndex = keyString.LastIndexOf(' ');
                    if (lastSpaceIndex != -1)
                    {
                        keyString = keyString.Substring(0, lastSpaceIndex);
                    }
                }

                return keyString;
            }
            return "None";
        }
        private void SetHotkeyHandlers(TextBox txtBox, Action<KeyOrButton> keySetCallback)
        {
            var oldText = txtBox.Text;
            txtBox.Text = "Set Hotkey...";
            txtBox.Select(0, 0);
            KeyEventHandler handlerDown = null;
            KeyEventHandler handlerUp = null;
            EventHandler leaveHandler = null;
            EventHandlerT<GamepadButton> gamepadButtonPressed = null;
            Action unregisterEvents = () =>
            {
                txtBox.KeyDown -= handlerDown;
                txtBox.KeyUp -= handlerUp;
                txtBox.Leave -= leaveHandler;
                Hook.AnyGamepadButtonPressed -= gamepadButtonPressed;
            };
            handlerDown = (s, x) =>
            {
                var key = x.KeyCode == Keys.Escape ? null : new KeyOrButton(x.KeyCode | x.Modifiers);
                if (x.KeyCode == Keys.ControlKey || x.KeyCode == Keys.ShiftKey || x.KeyCode == Keys.Menu)
                    return;
                keySetCallback(key);
                unregisterEvents();
                txtBox.Select(0, 0);
                chkGlobalHotkeys.Select();
                txtBox.Text = FormatKey(key);
            };
            handlerUp = (s, x) =>
            {
                var key = x.KeyCode == Keys.Escape ? null : new KeyOrButton(x.KeyCode | x.Modifiers);
                if (x.KeyCode == Keys.ControlKey || x.KeyCode == Keys.ShiftKey || x.KeyCode == Keys.Menu)
                {
                    keySetCallback(key);
                    unregisterEvents();
                    txtBox.Select(0, 0);
                    chkGlobalHotkeys.Select();
                    txtBox.Text = FormatKey(key);
                }
            };
            leaveHandler = (s, x) =>
            {
                unregisterEvents();
                txtBox.Text = oldText;
            };
            gamepadButtonPressed = (s, x) =>
            {
                var key = new KeyOrButton(x);
                keySetCallback(key);
                unregisterEvents();
                Action action = () =>
                {
                    txtBox.Select(0, 0);
                    chkGlobalHotkeys.Select();
                    txtBox.Text = FormatKey(key);
                };
                if (InvokeRequired)
                    Invoke(action);
                else
                    action();
            };
            txtBox.KeyDown += handlerDown;
            txtBox.KeyUp += handlerUp;
            txtBox.Leave += leaveHandler;
            Hook.AnyGamepadButtonPressed += gamepadButtonPressed;
        }
        private void Split_Set_Enter(object sender, EventArgs e)
        {
            SetHotkeyHandlers((TextBox)sender, x => { Settings.SplitKey = x; });
        }
        private void Reset_Set_Enter(object sender, EventArgs e)
        {
            SetHotkeyHandlers((TextBox)sender, x => { Settings.ResetKey = x; });
        }
        private void Skip_Set_Enter(object sender, EventArgs e)
        {
            SetHotkeyHandlers((TextBox)sender, x => { Settings.SkipKey = x; });
        }
        private void Undo_Set_Enter(object sender, EventArgs e)
        {
            SetHotkeyHandlers((TextBox)sender, x => { Settings.UndoKey = x; });
        }
        private void Pause_Set_Enter(object sender, EventArgs e)
        {
            SetHotkeyHandlers((TextBox)sender, x => { Settings.PauseKey = x; });
        }
        private void Toggle_Set_Enter(object sender, EventArgs e)
        {
            SetHotkeyHandlers((TextBox)sender, x => { Settings.ToggleGlobalHotkeys = x; });
        }
        private void Switch_Previous_Set_Enter(object sender, EventArgs e)
        {
            SetHotkeyHandlers((TextBox)sender, x => { Settings.SwitchComparisonPrevious = x; });
        }
        private void Switch_Next_Set_Enter(object sender, EventArgs e)
        {
            SetHotkeyHandlers((TextBox)sender, x => { Settings.SwitchComparisonNext = x; });
        }
        private void ClickControl(object sender, EventArgs e)
        {
            chkGlobalHotkeys.Select();
        }
        private void SetClickEvents()
        {
            //shitty code to remove "set hotkey" if you click out of it
            tableLayoutPanel1.Click += ClickControl;
            tableLayoutPanel2.Click += ClickControl;
            groupBox1.Click += ClickControl;
            label1.Click += ClickControl;
            label2.Click += ClickControl;
            label3.Click += ClickControl;
            label4.Click += ClickControl;
            label5.Click += ClickControl;
            label6.Click += ClickControl;
            label7.Click += ClickControl;
            label8.Click += ClickControl;
            label9.Click += ClickControl;
            label10.Click += ClickControl;
            label11.Click += ClickControl;
            /*lblDisplayInterval.Click += ClickControl;
            lblRefreshRate.Click += ClickControl;*/
            Click += ClickControl;
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

        private void btnOBSInstall_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("The OBS Plugin is still in Beta. It might reduce the framerate of your stream. Also, your splits will close if you lose internet connection. These issues will be fixed in the future. The plugin will also not automatically update in this version, so you will need to reinstall the plugin when there is a new update.\n\nAre you sure you would like to install the OBS Plugin?", "OBS Plugin Installation Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                var psi = new ProcessStartInfo();
                psi.Arguments = "obsplugin";
                psi.FileName = "LiveSplit.Register.exe";
                psi.Verb = "runas";

                try
                {
                    var process = new Process();
                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        private void btnChooseComparisons_Click(object sender, EventArgs e)
        {
            var generatorStates = new Dictionary<string, bool>(Settings.ComparisonGeneratorStates);
            var result = (new ChooseComparisonsDialog() { ComparisonGeneratorStates = generatorStates }).ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
                Settings.ComparisonGeneratorStates = generatorStates;
        }

    }
}
