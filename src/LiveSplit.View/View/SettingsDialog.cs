using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using LiveSplit.Model.Input;
using LiveSplit.Options;
using LiveSplit.UI;
using LiveSplit.Utils;
using LiveSplit.Web;
using LiveSplit.Web.Share;

namespace LiveSplit.View;

public partial class SettingsDialog : Form
{
    public ISettings Settings { get; set; }
    public CompositeHook Hook { get; set; }
    public string SelectedHotkeyProfile { get; set; }

    public string SplitKey => FormatKey(Settings.HotkeyProfiles[SelectedHotkeyProfile].SplitKey);
    public string ResetKey => FormatKey(Settings.HotkeyProfiles[SelectedHotkeyProfile].ResetKey);
    public string SkipKey => FormatKey(Settings.HotkeyProfiles[SelectedHotkeyProfile].SkipKey);
    public string UndoKey => FormatKey(Settings.HotkeyProfiles[SelectedHotkeyProfile].UndoKey);
    public string PauseKey => FormatKey(Settings.HotkeyProfiles[SelectedHotkeyProfile].PauseKey);
    public string SwitchComparisonPrevious => FormatKey(Settings.HotkeyProfiles[SelectedHotkeyProfile].SwitchComparisonPrevious);
    public string SwitchComparisonNext => FormatKey(Settings.HotkeyProfiles[SelectedHotkeyProfile].SwitchComparisonNext);
    public string ToggleGlobalHotkeys => FormatKey(Settings.HotkeyProfiles[SelectedHotkeyProfile].ToggleGlobalHotkeys);
    public float HotkeyDelay
    {
        get => Settings.HotkeyProfiles[SelectedHotkeyProfile].HotkeyDelay;
        set => Settings.HotkeyProfiles[SelectedHotkeyProfile].HotkeyDelay = Math.Max(value, 0);
    }
    public bool GlobalHotkeysEnabled
    {
        get => Settings.HotkeyProfiles[SelectedHotkeyProfile].GlobalHotkeysEnabled;
        set => Settings.HotkeyProfiles[SelectedHotkeyProfile].GlobalHotkeysEnabled = value;
    }
    public bool DoubleTapPrevention
    {
        get => Settings.HotkeyProfiles[SelectedHotkeyProfile].DoubleTapPrevention;
        set => Settings.HotkeyProfiles[SelectedHotkeyProfile].DoubleTapPrevention = value;
    }
    public bool DeactivateHotkeysForOtherPrograms
    {
        get => Settings.HotkeyProfiles[SelectedHotkeyProfile].DeactivateHotkeysForOtherPrograms;
        set => Settings.HotkeyProfiles[SelectedHotkeyProfile].DeactivateHotkeysForOtherPrograms = value;
    }
    public bool AllowGamepadsAsHotkeys
    {
        get => Settings.HotkeyProfiles[SelectedHotkeyProfile].AllowGamepadsAsHotkeys;
        set => Settings.HotkeyProfiles[SelectedHotkeyProfile].AllowGamepadsAsHotkeys = value;
    }
    public bool EnableDPIAwareness
    {
        get => Settings.EnableDPIAwareness;
        set => Settings.EnableDPIAwareness = value;
    }

    public int RefreshRate
    {
        get => Settings.RefreshRate;
        set => Settings.RefreshRate = Math.Min(Math.Max(value, 20), 300);
    }

    public int ServerPort
    {
        get => Settings.ServerPort;
        set => Settings.ServerPort = value;
    }

    public event EventHandler SumOfBestModeChanged;

    public string RaceViewer { get => Settings.RaceViewer.Name; set => Settings.RaceViewer = Web.SRL.RaceViewer.FromName(value); }

    public SettingsDialog(CompositeHook hook, ISettings settings, string hotkeyProfile)
    {
        InitializeComponent();
        Settings = settings;
        Hook = hook;

        InitializeHotkeyProfiles(hotkeyProfile);
        SetClickEvents(this);

        chkGlobalHotkeys.DataBindings.Add("Checked", this, "GlobalHotkeysEnabled");
        chkDoubleTap.DataBindings.Add("Checked", this, "DoubleTapPrevention");
        txtDelay.DataBindings.Add("Text", this, "HotkeyDelay");
        chkWarnOnReset.DataBindings.Add("Checked", Settings, "WarnOnReset");
        cbxRaceViewer.DataBindings.Add("SelectedItem", this, "RaceViewer");
        chkAllowGamepads.DataBindings.Add("Checked", this, "AllowGamepadsAsHotkeys");
        chkEnableDPIAwareness.DataBindings.Add("Checked", this, "EnableDPIAwareness");

        txtRefreshRate.DataBindings.Add("Text", this, "RefreshRate");
        txtServerPort.DataBindings.Add("Text", this, "ServerPort");
        cbxServerStartup.SelectedIndex = (int)Settings.ServerStartup;

        UpdateDisplayedHotkeyValues();
        RefreshRemoveButton();
        RefreshLogOutButton();

        if (Environment.OSVersion.Version.Major < LiveSplit.Options.Settings.DPI_AWARENESS_OS_MIN_VERSION)
        {
            settings.EnableDPIAwareness = false;
            chkEnableDPIAwareness.Checked = false;
            chkEnableDPIAwareness.Enabled = false;
        }
    }

    private void InitializeHotkeyProfiles(string hotkeyProfile)
    {
        cmbHotkeyProfiles.Items.AddRange(Settings.HotkeyProfiles.Keys.ToArray());
        cmbHotkeyProfiles.SelectedItem = hotkeyProfile;
    }

    private void UpdateDisplayedHotkeyValues()
    {
        txtStartSplit.Text = SplitKey;
        txtReset.Text = ResetKey;
        txtSkip.Text = SkipKey;
        txtUndo.Text = UndoKey;
        txtPause.Text = PauseKey;
        txtToggle.Text = ToggleGlobalHotkeys;
        txtSwitchPrevious.Text = SwitchComparisonPrevious;
        txtSwitchNext.Text = SwitchComparisonNext;

        chkGlobalHotkeys.Checked = GlobalHotkeysEnabled;
        chkDoubleTap.Checked = DoubleTapPrevention;
        txtDelay.Text = HotkeyDelay.ToString();
        chkDeactivateForOtherPrograms.Checked = DeactivateHotkeysForOtherPrograms;
        chkAllowGamepads.Checked = AllowGamepadsAsHotkeys;

        chkGlobalHotkeys_CheckedChanged(null, null);
    }

    private void RefreshRemoveButton()
    {
        btnRemoveProfile.Enabled = Settings.HotkeyProfiles.Count > 1;
    }

    private void RefreshLogOutButton()
    {
        btnLogOut.Enabled = WebCredentials.AnyCredentialsExist();
    }

    private void chkSimpleSOB_CheckedChanged(object sender, EventArgs e)
    {
        Settings.SimpleSumOfBest = chkSimpleSOB.Checked;
        SumOfBestModeChanged?.Invoke(this, null);
    }

    private void SettingsDialog_Load(object sender, EventArgs e)
    {
        chkSimpleSOB.Checked = Settings.SimpleSumOfBest;
        chkGlobalHotkeys_CheckedChanged(null, null);
    }

    private void chkGlobalHotkeys_CheckedChanged(object sender, EventArgs e)
    {
        if (chkGlobalHotkeys.Checked)
        {
            chkDeactivateForOtherPrograms.Enabled = true;
            chkDeactivateForOtherPrograms.DataBindings.Clear();
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
            string keyString = key.ToString();
            if (key.IsButton)
            {
                int lastSpaceIndex = keyString.LastIndexOf(' ');
                if (lastSpaceIndex != -1)
                {
                    keyString = keyString[..lastSpaceIndex];
                }
            }

            return keyString;
        }

        return "None";
    }
    private void SetHotkeyHandlers(TextBox txtBox, Action<KeyOrButton> keySetCallback)
    {
        string oldText = txtBox.Text;
        txtBox.Text = "Set Hotkey...";
        txtBox.Select(0, 0);
        KeyEventHandler handlerDown = null;
        KeyEventHandler handlerUp = null;
        EventHandler leaveHandler = null;
        EventHandlerT<GamepadButton> gamepadButtonPressed = null;
        void unregisterEvents()
        {
            txtBox.KeyDown -= handlerDown;
            txtBox.KeyUp -= handlerUp;
            txtBox.Leave -= leaveHandler;
            Hook.AnyGamepadButtonPressed -= gamepadButtonPressed;
        }

        handlerDown = (s, x) =>
        {
            KeyOrButton key = x.KeyCode == Keys.Escape ? null : new KeyOrButton(x.KeyCode | x.Modifiers);
            if (x.KeyCode is Keys.ControlKey or Keys.ShiftKey or Keys.Menu)
            {
                return;
            }

            keySetCallback(key);
            unregisterEvents();
            txtBox.Select(0, 0);
            chkGlobalHotkeys.Select();
            txtBox.Text = FormatKey(key);
        };
        handlerUp = (s, x) =>
        {
            KeyOrButton key = x.KeyCode == Keys.Escape ? null : new KeyOrButton(x.KeyCode | x.Modifiers);
            if (x.KeyCode is Keys.ControlKey or Keys.ShiftKey or Keys.Menu)
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

            this.InvokeIfRequired(() =>
            {
                txtBox.Select(0, 0);
                chkGlobalHotkeys.Select();
                txtBox.Text = FormatKey(key);
            });
        };
        txtBox.KeyDown += handlerDown;
        txtBox.KeyUp += handlerUp;
        txtBox.Leave += leaveHandler;
        Hook.AnyGamepadButtonPressed += gamepadButtonPressed;
    }
    private void Split_Set_Enter(object sender, EventArgs e)
    {
        SetHotkeyHandlers((TextBox)sender, x => Settings.HotkeyProfiles[SelectedHotkeyProfile].SplitKey = x);
    }
    private void Reset_Set_Enter(object sender, EventArgs e)
    {
        SetHotkeyHandlers((TextBox)sender, x => Settings.HotkeyProfiles[SelectedHotkeyProfile].ResetKey = x);
    }
    private void Skip_Set_Enter(object sender, EventArgs e)
    {
        SetHotkeyHandlers((TextBox)sender, x => Settings.HotkeyProfiles[SelectedHotkeyProfile].SkipKey = x);
    }
    private void Undo_Set_Enter(object sender, EventArgs e)
    {
        SetHotkeyHandlers((TextBox)sender, x => Settings.HotkeyProfiles[SelectedHotkeyProfile].UndoKey = x);
    }
    private void Pause_Set_Enter(object sender, EventArgs e)
    {
        SetHotkeyHandlers((TextBox)sender, x => Settings.HotkeyProfiles[SelectedHotkeyProfile].PauseKey = x);
    }
    private void Toggle_Set_Enter(object sender, EventArgs e)
    {
        SetHotkeyHandlers((TextBox)sender, x => Settings.HotkeyProfiles[SelectedHotkeyProfile].ToggleGlobalHotkeys = x);
    }
    private void Switch_Previous_Set_Enter(object sender, EventArgs e)
    {
        SetHotkeyHandlers((TextBox)sender, x => Settings.HotkeyProfiles[SelectedHotkeyProfile].SwitchComparisonPrevious = x);
    }
    private void Switch_Next_Set_Enter(object sender, EventArgs e)
    {
        SetHotkeyHandlers((TextBox)sender, x => Settings.HotkeyProfiles[SelectedHotkeyProfile].SwitchComparisonNext = x);
    }

    private void ClickControl(object sender, EventArgs e)
    {
        chkGlobalHotkeys.Select();
    }

    private void SetClickEvents(Control control)
    {
        foreach (Control childControl in control.Controls)
        {
            if (childControl is TableLayoutPanel or Label or GroupBox)
            {
                SetClickEvents(childControl);
            }
        }

        control.Click += ClickControl;
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

    private void btnChooseComparisons_Click(object sender, EventArgs e)
    {
        var generatorStates = new Dictionary<string, bool>(Settings.ComparisonGeneratorStates);
        int hcpHistorySize = Settings.HcpHistorySize;
        int hcpNBestRuns = Settings.HcpNBestRuns;

        var dialog = new ChooseComparisonsDialog()
        {
            ComparisonGeneratorStates = generatorStates,
            HcpHistorySize = hcpHistorySize,
            HcpNBestRuns = hcpNBestRuns
        };

        DialogResult result = dialog.ShowDialog(this);

        if (result == DialogResult.OK)
        {
            Settings.ComparisonGeneratorStates = dialog.ComparisonGeneratorStates;
            Settings.HcpHistorySize = dialog.HcpHistorySize;
            Settings.HcpNBestRuns = dialog.HcpNBestRuns;
        }
    }

    private void cmbHotkeyProfiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectedHotkeyProfile = cmbHotkeyProfiles.SelectedItem.ToString();
        UpdateDisplayedHotkeyValues();
    }

    private void btnRemoveProfile_Click(object sender, EventArgs e)
    {
        if (Settings.HotkeyProfiles.Count > 1)
        {
            Settings.HotkeyProfiles.Remove(SelectedHotkeyProfile);
            cmbHotkeyProfiles.Items.Remove(SelectedHotkeyProfile);
            cmbHotkeyProfiles.SelectedItem = Settings.HotkeyProfiles.Keys.Last();
            RefreshRemoveButton();
        }
    }

    private void btnRenameProfile_Click(object sender, EventArgs e)
    {
        string newName = SelectedHotkeyProfile;
        DialogResult result = InputBox.Show("Rename Hotkey Profile", "Hotkey Profile Name:", ref newName);
        if (result == DialogResult.OK)
        {
            if (!Settings.HotkeyProfiles.ContainsKey(newName))
            {
                for (int i = 0; i < Settings.RecentSplits.Count; i++)
                {
                    RecentSplitsFile file = Settings.RecentSplits[i];
                    if (file.LastHotkeyProfile == SelectedHotkeyProfile)
                    {
                        var newFile = new RecentSplitsFile(file.Path, file.LastTimingMethod, newName, file.GameName, file.CategoryName);
                        Settings.RecentSplits.RemoveAt(i);
                        Settings.RecentSplits.Insert(i, newFile);
                    }
                }

                HotkeyProfile curProfile = Settings.HotkeyProfiles[SelectedHotkeyProfile];
                Settings.HotkeyProfiles.Remove(SelectedHotkeyProfile);
                Settings.HotkeyProfiles[newName] = curProfile;
                cmbHotkeyProfiles.Items.Remove(SelectedHotkeyProfile);
                cmbHotkeyProfiles.Items.Add(newName);
                cmbHotkeyProfiles.SelectedItem = newName;
            }
            else if (newName != SelectedHotkeyProfile)
            {
                result = MessageBox.Show(this, "A Hotkey Profile with this name already exists.", "Hotkey Profile Already Exists", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (result == DialogResult.Retry)
                {
                    btnRenameProfile_Click(sender, e);
                }
            }
        }
    }

    private void btnNewProfile_Click(object sender, EventArgs e)
    {
        string name = "";
        DialogResult result = InputBox.Show("New Hotkey Profile", "Hotkey Profile Name:", ref name);
        if (result == DialogResult.OK)
        {
            if (!Settings.HotkeyProfiles.ContainsKey(name))
            {
                var newProfile = (HotkeyProfile)Settings.HotkeyProfiles[SelectedHotkeyProfile].Clone();
                Settings.HotkeyProfiles.Add(name, newProfile);
                cmbHotkeyProfiles.Items.Add(name);
                cmbHotkeyProfiles.SelectedItem = name;
                RefreshRemoveButton();
            }
            else
            {
                result = MessageBox.Show(this, "A Hotkey Profile with this name already exists.", "Hotkey Profile Already Exists", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (result == DialogResult.Retry)
                {
                    btnNewProfile_Click(sender, e);
                }
            }
        }
    }

    private void btnLogOut_Click(object sender, EventArgs e)
    {
        SpeedrunCom.ClearAccessToken();
        Twitch.Instance.ClearAccessToken();
        WebCredentials.DeleteAllCredentials();
        RefreshLogOutButton();
    }

    private void btnChooseRaceProvider_Click(object sender, EventArgs e)
    {
        var newSettings = Settings.RaceProvider.Select(x => (RaceProviderSettings)x.Clone()).ToList();
        var dialog = new RaceProviderManagingDialog(newSettings);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            Settings.RaceProvider = newSettings;
        }
    }

    private void cbxServerStartup_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Enum.IsDefined(typeof(ServerStartupType), cbxServerStartup.SelectedIndex))
        {
            Settings.ServerStartup = (ServerStartupType)cbxServerStartup.SelectedIndex;
        }
    }
}
