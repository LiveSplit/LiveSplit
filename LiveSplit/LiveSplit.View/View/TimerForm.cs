using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Model.Input;
using LiveSplit.Model.RunFactories;
using LiveSplit.Model.RunImporters;
using LiveSplit.Model.RunSavers;
using LiveSplit.Options;
using LiveSplit.Options.SettingsFactories;
using LiveSplit.Options.SettingsSavers;
using LiveSplit.Server;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.UI.LayoutFactories;
using LiveSplit.UI.LayoutSavers;
using LiveSplit.Updates;
using LiveSplit.Utils;
using LiveSplit.Web;
using LiveSplit.Web.Share;
using LiveSplit.Web.SRL;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using UpdateManager;

namespace LiveSplit.View
{
    public partial class TimerForm : Form
    {
        protected IComparisonGeneratorsFactory ComparisonGeneratorsFactory { get; set; }
        protected ComponentRenderer ComponentRenderer { get; set; }
        public LiveSplitState CurrentState { get; set; }
        protected ITimerModel Model { get; set; }
        protected CompositeHook Hook { get; set; }
        protected bool IsInDialogMode { get; set; }
        protected bool ResetMessageShown { get; set; }
        public new ILayout Layout
        {
            get { return CurrentState.Layout; }
            set { CurrentState.Layout = value; }
        }
        protected float OldSize { get; set; }
        protected int RefreshesRemaining { get; set; }
        public ISettings Settings { get; set; }
        protected Invalidator Invalidator { get; set; }
        protected bool InTimerOnlyMode { get; set; }

        private Image previousBackground { get; set; }
        private float previousOpacity { get; set; }
        private float previousBlur { get; set; }
        private Image blurredBackground { get; set; }
        private Image bakedBackground { get; set; }

        public CommandServer Server { get; set; }

        protected GraphicsCache GlobalCache { get; set; }

        protected StandardFormatsRunFactory RunFactory { get; set; }
        protected IRunSaver RunSaver { get; set; }
        protected ILayoutSaver LayoutSaver { get; set; }
        protected ISettingsSaver SettingsSaver { get; set; }

        protected float TotalPosition { get; set; }

        private bool DontRedraw = false;

        protected Region UpdateRegion { get; set; }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const string SETTINGS_PATH = "settings.cfg";
        const int WS_MINIMIZEBOX = 0x20000;
        const int CS_DBLCLKS = 0x8;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }

        protected bool MouseIsDown = false;
        protected Point MousePoint;

        private List<Action> RacesToRefresh = new List<Action>();
        private bool ShouldRefreshRaces = false;

        protected Task RefreshTask { get; set; }
        protected bool InvalidationRequired { get; set; }

        public string BasePath { get; set; }
        protected IEnumerable<RaceProviderAPI> RaceProvider { get; set; }

        private bool MousePassThrough
        {
            set
            {
                const int GWL_EXSTYLE = -20;

                const uint WS_EX_LAYERED = 0x00080000;
                const uint WS_EX_TRANSPARENT = 0x00000020;

                // If we're trying to set to false and it's already false, don't bother doing anything.
                // We can't do this for setting to true because setting Opacity may have messed the GWL_EXSTYLE flags up.
                if (!value && !MousePassThroughState)
                    return;
                MousePassThroughState = value;

                var prevWindowLong = GetWindowLong(Handle, GWL_EXSTYLE);
                if (value)
                {
                    if ((prevWindowLong & (WS_EX_LAYERED | WS_EX_TRANSPARENT)) != (WS_EX_LAYERED | WS_EX_TRANSPARENT))
                    {
                        // We have to add WS_EX_LAYERED, because WS_EX_TRANSPARENT won't work otherwise.
                        prevWindowLong |= WS_EX_LAYERED | WS_EX_TRANSPARENT;
                        SetWindowLong(Handle, GWL_EXSTYLE, prevWindowLong);
                    }
                }
                else
                {
                    // Not removing WS_EX_LAYERED because it may still be needed if Opacity != 1.
                    // It shouldn't really affect anything and setting Form.Opacity to 1 will remove it anyway:
                    prevWindowLong &= ~WS_EX_TRANSPARENT;
                    SetWindowLong(Handle, GWL_EXSTYLE, prevWindowLong);
                }
            }
        }
        private bool MousePassThroughState = false;

        private bool IsForegroundWindow
        {
            get
            {
                return GetForegroundWindow() == Handle;
            }
        }

        private float? ResizingInitialAspectRatio { get; set; } = null;

        [DllImport("user32.dll")]
        static extern int GetUpdateRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] bool bErase);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("user32")]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32")]
        private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public TimerForm(string splitsPath = null, string layoutPath = null, string basePath = "")
        {
            BasePath = basePath;
            InitializeComponent();
            Init(splitsPath, layoutPath);
        }

        private void Init(string splitsPath = null, string layoutPath = null)
        {
            LiveSplitCoreFactory.LoadLiveSplitCore();

            SetWindowTitle();

            SpeedrunCom.Authenticator = new SpeedrunComOAuthForm();

            GlobalCache = new GraphicsCache();
            Invalidator = new Invalidator(this);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            ComponentManager.BasePath = BasePath;

            CurrentState = new LiveSplitState(null, this, null, null, null);

            ComparisonGeneratorsFactory = new StandardComparisonGeneratorsFactory();

            Model = new DoubleTapPrevention(new TimerModel());

            ComponentManager.RaceProviderFactories = ComponentManager.LoadAllFactories<IRaceProviderFactory>();
            ComponentManager.RaceProviderFactories["SRL"] = new SRLFactory();
            RunFactory = new StandardFormatsRunFactory();
            RunSaver = new XMLRunSaver();
            LayoutSaver = new XMLLayoutSaver();
            SettingsSaver = new XMLSettingsSaver();
            LoadSettings();

            UpdateRaceProviderIntegration();

            CurrentState.CurrentHotkeyProfile = Settings.HotkeyProfiles.First().Key;

            UpdateRecentSplits();
            UpdateRecentLayouts();

            InTimerOnlyMode = false;
            var timerOnlyRun = new StandardRunFactory().Create(ComparisonGeneratorsFactory);

            IRun run = timerOnlyRun;
            try
            {
                if (!string.IsNullOrEmpty(splitsPath))
                {
                    UpdateStateFromSplitsPath(splitsPath);

                    run = LoadRunFromFile(splitsPath);
                }
                else if (Settings.RecentSplits.Count > 0)
                {
                    var lastSplitFile = Settings.RecentSplits.Last();
                    if (!string.IsNullOrEmpty(lastSplitFile.Path))
                    {
                        UpdateStateFromSplitsPath(lastSplitFile.Path);

                        run = LoadRunFromFile(lastSplitFile.Path);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            run.FixSplits();
            CurrentState.Run = run;
            CurrentState.Settings = Settings;

            try
            {
                if (!string.IsNullOrEmpty(layoutPath))
                {
                    Layout = LoadLayoutFromFile(layoutPath);
                }
                else
                {
                    if (Settings.RecentLayouts.Count > 0
                        && !string.IsNullOrEmpty(Settings.RecentLayouts.Last()))
                    {
                        Layout = LoadLayoutFromFile(Settings.RecentLayouts.Last());
                    }
                    else if (run == timerOnlyRun)
                    {
                        Layout = new TimerOnlyLayoutFactory().Create(CurrentState);
                        InTimerOnlyMode = true;
                    }
                    else
                    {
                        Layout = new StandardLayoutFactory().Create(CurrentState);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                Layout = new StandardLayoutFactory().Create(CurrentState);
            }

            CurrentState.LayoutSettings = Layout.Settings;
            CreateAutoSplitter();

            SwitchComparisonGenerators();
            SwitchComparison(Settings.LastComparison);
            Model.CurrentState = CurrentState;

            CurrentState.OnReset += CurrentState_OnReset;
            CurrentState.OnStart += CurrentState_OnStart;
            CurrentState.OnSplit += CurrentState_OnSplit;
            CurrentState.OnSkipSplit += CurrentState_OnSkipSplit;
            CurrentState.OnUndoSplit += CurrentState_OnUndoSplit;
            CurrentState.OnPause += CurrentState_OnPause;
            CurrentState.OnUndoAllPauses += CurrentState_OnUndoAllPauses;
            CurrentState.OnResume += CurrentState_OnResume;
            CurrentState.OnSwitchComparisonPrevious += CurrentState_OnSwitchComparisonPrevious;
            CurrentState.OnSwitchComparisonNext += CurrentState_OnSwitchComparisonNext;

            ComponentRenderer = new ComponentRenderer();

            StartPosition = FormStartPosition.Manual;

            SetLayout(Layout);

            RefreshTask = Task.Factory.StartNew(RefreshTimerWorker);

            InvalidationRequired = false;

            Hook = new CompositeHook(false);
            Hook.GamepadHookInitialized += Hook_GamepadHookInitialized;
            Hook.KeyOrButtonPressed += hook_KeyOrButtonPressed;
            Settings.RegisterHotkeys(Hook, CurrentState.CurrentHotkeyProfile);

            SizeChanged += TimerForm_SizeChanged;

            TopMost = Layout.Settings.AlwaysOnTop;
            BackColor = Color.Black;

            Server = new CommandServer(CurrentState);
            Server.Start();

            new System.Timers.Timer(1000) { Enabled = true }.Elapsed += RaceRefreshTimer_Elapsed;

            InitDragAndDrop();
        }

        private void InitDragAndDrop()
        {
            AllowDrop = true;
            DragDrop += TimerForm_DragDrop;
            DragEnter += TimerForm_DragEnter;
        }

        void UpdateRaceProviderIntegration()
        {
            if (RightClickMenu.InvokeRequired)
            {
                RightClickMenu.Invoke(new Action(UpdateRaceProviderIntegration), null);
                return;
            }

            int menuItemIndex = RightClickMenu.Items.IndexOf(shareMenuItem);
            int firstRaceProvider = menuItemIndex + 1;
            int lastRaceProvider = RightClickMenu.Items.IndexOfKey("endRaceSection")-1;
            if (lastRaceProvider-firstRaceProvider >= 0)
            {
                for (int i = 0; i < (lastRaceProvider - firstRaceProvider) + 1; i++)
                {
                    RightClickMenu.Items[firstRaceProvider].Tag = null;
                    RightClickMenu.Items[firstRaceProvider].MouseHover -= racingMenuItem_MouseHover;
                    RightClickMenu.Items[firstRaceProvider].MouseLeave -= racingMenuItem_MouseLeave;
                    RightClickMenu.Items.RemoveAt(firstRaceProvider);
                }
            }
                       
            RaceProvider = ComponentManager.RaceProviderFactories.Select(x => x.Value.Create(Model, Settings.RaceProvider.FirstOrDefault(y => y.Name == x.Key)));           
            foreach (var raceProvider in RaceProvider.Reverse())
            {
                if (Settings.RaceProvider.Any(x => x.DisplayName == raceProvider.ProviderName && !x.Enabled))
                    continue;
               
                raceProvider.RacesRefreshedCallback = RacesRefreshed;
                ToolStripMenuItem raceProviderItem = new ToolStripMenuItem()
                {
                    Name = $"{raceProvider.ProviderName}racesMenuItem",
                    Text = $"{raceProvider.ProviderName} Races",
					Tag = raceProvider
                };
                raceProviderItem.MouseHover += racingMenuItem_MouseHover;
                raceProviderItem.MouseLeave += racingMenuItem_MouseLeave;
                RightClickMenu.Items.Insert(menuItemIndex + 1, raceProviderItem);
                raceProvider.RefreshRacesListAsync();
            }
            var srlRaceProvider = RaceProvider.FirstOrDefault(x => x.ProviderName == "SRL");
            if (srlRaceProvider != null)
            {
                srlRaceProvider.JoinRace = SRL_JoinRace;
                srlRaceProvider.CreateRace = SRL_NewRace;
            }
            
        }

        void SetWindowTitle()
        {
            var lowestAvailableNumber = 0;
            var currentName = "LiveSplit";
            var processNames = Process.GetProcessesByName("LiveSplit").Select(x => x.MainWindowTitle);

            while (processNames.Contains(currentName))
            {
                currentName = string.Format("LiveSplit ({0})", ++lowestAvailableNumber);
            }

            Text = currentName;
        }

        void CurrentState_OnSwitchComparisonNext(object sender, EventArgs e)
        {
            RefreshComparisonItems();
        }

        void RefreshComparisonItems()
        {
            var numSeparators = 0;
            foreach (var item in comparisonMenuItem.DropDownItems.OfType<ToolStripItem>().Reverse())
            {
                if (item is ToolStripSeparator)
                    numSeparators++;
                if (item is ToolStripMenuItem)
                {
                    var toolItem = (ToolStripMenuItem)item;
                    if (numSeparators == 0)
                        toolItem.Checked = toolItem.Name == CurrentState.CurrentTimingMethod.ToString();
                    else
                        toolItem.Checked = toolItem.Text == CurrentState.CurrentComparison.EscapeMenuItemText();
                }
            }
        }

        void CurrentState_OnSwitchComparisonPrevious(object sender, EventArgs e)
        {
            RefreshComparisonItems();
        }

        private string GetShortenedGameAndGoal(string goal)
        {
            if (goal.Length > 65)
                return goal.Substring(0, 60) + "...";
            return goal;
        }

        private void RaceRefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ShouldRefreshRaces)
            {
                for (var i = 0; i < RacesToRefresh.Count; i++)
                {
                    var updateTitleAction = RacesToRefresh[i];
                    updateTitleAction();
                }
            }
        }

        void RacesRefreshed(RaceProviderAPI raceProvider)
        {
            ToolStripMenuItem racingMenuItem = RightClickMenu.Items.Find($"{raceProvider.ProviderName}racesMenuItem", false).FirstOrDefault() as ToolStripMenuItem;
            if (racingMenuItem == null)
                return;

            Action<ToolStripItem> addItem = null;
            Action clear = null;

            addItem = x =>
            {
                if (InvokeRequired)
                {
                    Invoke(addItem, x);
                }
                else if (RightClickMenu.InvokeRequired)
                {
                    RightClickMenu.Invoke(addItem, x);
                }
                else
                {
                    racingMenuItem.DropDownItems.Add(x);
                }
            };
            clear = () =>
            {
                if (InvokeRequired)
                {
                    Invoke(clear);
                }
                else if (RightClickMenu.InvokeRequired)
                {
                    RightClickMenu.Invoke(clear);
                }
                else
                {
                    RacesToRefresh.Clear();
                    racingMenuItem.DropDownItems.Clear();
                }
            };

            clear();

            foreach (var race in raceProvider.GetRaces())
            {
                if (race.State != 1)
                    continue;

                var gameAndGoal = GetShortenedGameAndGoal(string.Format("{0} - {1}", race.GameName, race.Goal));
                var entrants = race.NumEntrants;
                var plural = entrants == 1 ? "" : "s";
                var title = string.Format("{0} ({1} Entrant{2})", gameAndGoal, entrants, plural) as string;

                var item = new ToolStripMenuItem();
                item.Text = title.EscapeMenuItemText();
                item.Tag = race.Id;
                item.Click += (s, e) => { raceProvider.JoinRace?.Invoke(Model, race.Id); };
                addItem(item);

                SetGameImage(raceProvider, item, race);
            }

            if (racingMenuItem.DropDownItems.Count > 0)
                addItem(new ToolStripSeparator());

            foreach (var race in raceProvider.GetRaces())
            {
                if (race.State != 3)
                    continue;

                var gameAndGoal = GetShortenedGameAndGoal(string.Format("{0} - {1}", race.GameName, race.Goal));
                var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                startTime = startTime.AddSeconds(race.Starttime);

                var tsItem = new ToolStripMenuItem();

                Action updateTitleAction = null;
                updateTitleAction = () =>
                {
                    if (InvokeRequired)
                    {
                        if (!IsDisposed)
                        {
                            try
                            {
                                Invoke(updateTitleAction);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        try
                        {
                            var timeSpan = TimeStamp.CurrentDateTime - startTime;
                            if (timeSpan < TimeSpan.Zero)
                                timeSpan = TimeSpan.Zero;
                            var time = new RegularTimeFormatter().Format(timeSpan);
                            var title = string.Format("[{0}] {1} ({2}/{3} Finished)", time, gameAndGoal, race.Finishes, race.NumEntrants - race.Forfeits) as string;
                            tsItem.Text = title.EscapeMenuItemText();
                        }
                        catch { }
                    }
                };

                SetGameImage(raceProvider, tsItem, race);

                updateTitleAction();

                RacesToRefresh.Add(updateTitleAction);

                tsItem.Click += (s, ev) =>
                {
                    if (!race.IsParticipant(raceProvider.Username))
                        Settings.RaceViewer.ShowRace(race);
                    else
                    {
                        tsItem.Tag = race.Id;
                        raceProvider.JoinRace?.Invoke(Model, race.Id);
                    }
                };

                addItem(tsItem);
            }

            if (racingMenuItem.DropDownItems.Count > 0 && !(racingMenuItem.DropDownItems[racingMenuItem.DropDownItems.Count - 1] is ToolStripSeparator))
                addItem(new ToolStripSeparator());

            var newRaceItem = new ToolStripMenuItem();
            newRaceItem.Text = "New Race...";
            newRaceItem.Click += (s, e) => { raceProvider.CreateRace?.Invoke(Model); };
            addItem(newRaceItem);
        }

        void SetGameImage(RaceProviderAPI raceProvider, ToolStripMenuItem item, IRaceInfo race)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var image = raceProvider.GetGameImage(race.GameId);
                    this.InvokeIfRequired(() =>
                    {
                        try
                        {
                            item.Image = image;
                        }
                        catch { }
                    });
                }
                catch { }
            });
        }

        void SRL_JoinRace(ITimerModel model, string raceId)
        {
            if (ShowSRLRules())
            {
                var form = new SpeedRunsLiveForm(CurrentState, model, raceId);
                TopMost = false;
                form.Show(this);
                TopMost = CurrentState.LayoutSettings.AlwaysOnTop;
            }
        }

        void SRL_NewRace(ITimerModel model)
        {
            if (ShowSRLRules())
            {
                var gameName = CurrentState.Run.GameName;
                var gameCategory = CurrentState.Run.CategoryName;
                var inputBox = new NewRaceInputBox();
                TopMost = false;
                var result = inputBox.Show(ref gameName, ref gameCategory);
                if (result == DialogResult.OK)
                {
                    var id = SpeedRunsLiveAPI.Instance.GetGameIDFromName(gameName);
                    if (id == null)
                    {
                        id = "new";
                        gameCategory = gameName + " - " + gameCategory;
                        gameName = "New Game";
                    }
                    var form = new SpeedRunsLiveForm(CurrentState, model, gameName, id, gameCategory);
                    form.Show(this);
                }
                TopMost = CurrentState.LayoutSettings.AlwaysOnTop;
            }
        }

        void TimerForm_SizeChanged(object sender, EventArgs e)
        {
            CreateBakedBackground();
            if (RefreshesRemaining <= 0)
            {
                if (Layout.Mode == LayoutMode.Vertical)
                {
                    Layout.VerticalWidth = Size.Width;
                    Layout.VerticalHeight = Size.Height;
                }
                else
                {
                    Layout.HorizontalWidth = Size.Width;
                    Layout.HorizontalHeight = Size.Height;
                }
                MaintainMinimumSize();
            }
        }

        private void Hook_GamepadHookInitialized(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void CheckForUpdates()
        {
            UpdateHelper.Update(this, () => Invoke(new Action(() =>
                         {
                             Process.GetCurrentProcess().Kill();
                         })),
                        new IUpdateable[] {
                            new LiveSplitUpdateable(),
                            UpdateManagerUpdateable.Instance }
                            .Concat(ComponentManager.ComponentFactories.Values)
                            .Concat(ComponentManager.RaceProviderFactories.Values)
                            .ToArray());
        }

        void CurrentState_OnUndoSplit(object sender, EventArgs e)
        {
            this.InvokeIfRequired(() =>
            {
                pauseMenuItem.Enabled = true;
                splitMenuItem.Enabled = true;
                if (CurrentState.CurrentSplitIndex == 0)
                    undoSplitMenuItem.Enabled = false;
                if (CurrentState.CurrentSplitIndex < CurrentState.Run.Count - 1)
                    skipSplitMenuItem.Enabled = true;
            });
        }

        void CurrentState_OnSkipSplit(object sender, EventArgs e)
        {
            this.InvokeIfRequired(() =>
            {
                if (CurrentState.CurrentSplitIndex >= CurrentState.Run.Count - 1)
                    skipSplitMenuItem.Enabled = false;
                undoSplitMenuItem.Enabled = true;
            });
        }

        void CurrentState_OnSplit(object sender, EventArgs e)
        {
            this.InvokeIfRequired(() =>
            {
                if (CurrentState.CurrentSplitIndex == CurrentState.Run.Count)
                {
                    pauseMenuItem.Enabled = false;
                    splitMenuItem.Enabled = false;
                }
                if (CurrentState.CurrentSplitIndex >= CurrentState.Run.Count - 1)
                    skipSplitMenuItem.Enabled = false;
                undoSplitMenuItem.Enabled = true;
            });
        }

        void CurrentState_OnStart(object sender, EventArgs e)
        {
            this.InvokeIfRequired(() =>
            {
                pauseMenuItem.Enabled = true;
                resetMenuItem.Enabled = true;
                undoSplitMenuItem.Enabled = false;
                skipSplitMenuItem.Enabled = true;
                splitMenuItem.Text = "Split";
            });
        }

        void CurrentState_OnReset(object sender, TimerPhase e)
        {
            RegenerateComparisons();

            this.InvokeIfRequired(() =>
            {
                if (InTimerOnlyMode)
                {
                    var timerOnlyRun = new StandardRunFactory().Create(ComparisonGeneratorsFactory);
                    timerOnlyRun.Offset = CurrentState.Run.Offset;

                    SetRun(timerOnlyRun);
                }
                resetMenuItem.Enabled = false;
                pauseMenuItem.Enabled = false;
                undoPausesMenuItem.Enabled = false;
                undoSplitMenuItem.Enabled = false;
                skipSplitMenuItem.Enabled = false;
                splitMenuItem.Enabled = true;
                splitMenuItem.Text = "Start";
            });
        }

        void CurrentState_OnResume(object sender, EventArgs e)
        {
            this.InvokeIfRequired(() =>
            {
                splitMenuItem.Text = "Split";
                pauseMenuItem.Enabled = true;
            });
        }

        void CurrentState_OnPause(object sender, EventArgs e)
        {
            this.InvokeIfRequired(() =>
            {
                splitMenuItem.Text = "Resume";
                undoPausesMenuItem.Enabled = true;
                pauseMenuItem.Enabled = false;
            });
        }

        private void CurrentState_OnUndoAllPauses(object sender, EventArgs e)
        {
            this.InvokeIfRequired(() =>
            {
                undoPausesMenuItem.Enabled = false;
            });
        }

        private void AddSplitsFileToLRU(string filePath, IRun run, TimingMethod lastTimingMethod, string lastHotkeyProfile)
        {
            Settings.AddToRecentSplits(filePath, run, lastTimingMethod, lastHotkeyProfile);
            UpdateRecentSplits();
        }

        private void AddLayoutFileToLRU(string filePath)
        {
            Settings.AddToRecentLayouts(filePath);
            UpdateRecentLayouts();
        }

        protected void SetInTimerOnlyMode()
        {
            if (Layout.Components.Count() != 1 || Layout.Components.FirstOrDefault().ComponentName != "Timer")
                InTimerOnlyMode = false;
        }

        private void UpdateRecentSplits()
        {
            openSplitsMenuItem.DropDownItems.Clear();

            foreach (var game in Settings.RecentSplits
                .Reverse()
                .Where(x => !string.IsNullOrEmpty(x.Path))
                .GroupBy(x => x.GameName ?? ""))
            {
                var gameMenuItem = new ToolStripMenuItem();

                foreach (var category in game
                    .GroupBy(x => x.CategoryName ?? ""))
                {
                    var categoryMenuItem = new ToolStripMenuItem();
                    categoryMenuItem.Tag = "Category";

                    foreach (var splitsFile in category)
                    {
                        string fileName = Path.GetFileName(splitsFile.Path);

                        var menuItem = new ToolStripMenuItem(fileName.EscapeMenuItemText());
                        menuItem.Tag = "FileName";
                        menuItem.Click += (x, y) =>
                        {
                            OpenRunFromFile(splitsFile.Path);
                        };
                        categoryMenuItem.DropDownItems.Add(menuItem);
                    }

                    if (categoryMenuItem.DropDownItems.Count == 1)
                    {
                        categoryMenuItem = (ToolStripMenuItem)categoryMenuItem.DropDownItems[0];
                        if (!string.IsNullOrEmpty(category.Key))
                        {
                            categoryMenuItem.Text = category.Key.EscapeMenuItemText();
                            categoryMenuItem.Tag = "Category";
                        }
                    }
                    else
                    {
                        string categoryName;
                        if (string.IsNullOrEmpty(category.Key))
                            categoryName = "Unknown Category";
                        else
                        {
                            categoryName = category.Key;
                        }

                        categoryMenuItem.Text = categoryName.EscapeMenuItemText();
                    }

                    gameMenuItem.DropDownItems.Add(categoryMenuItem);
                }

                string gameName;
                if (string.IsNullOrEmpty(game.Key))
                {
                    gameName = "Unknown Game";

                    if (gameMenuItem.DropDownItems.Count == 1)
                    {
                        gameMenuItem = (ToolStripMenuItem)gameMenuItem.DropDownItems[0];
                        gameName = gameMenuItem.Text;
                        if (gameMenuItem.Text == "Unknown Category")
                            gameName = "Unknown";
                    }
                }
                else
                {
                    gameName = game.Key;

                    if (gameMenuItem.DropDownItems.Count == 1)
                    {
                        gameMenuItem = (ToolStripMenuItem)gameMenuItem.DropDownItems[0];
                        if ((string)gameMenuItem.Tag == "Category")
                        {
                            if (!gameMenuItem.Text.StartsWith("Unknown Category"))
                                gameName += " - " + gameMenuItem.Text;
                        }
                        else
                        {
                            gameName += " (" + gameMenuItem.Text + ")";
                        }
                    }
                }

                gameMenuItem.Text = gameName.EscapeMenuItemText();

                openSplitsMenuItem.DropDownItems.Add(gameMenuItem);
            }
            if (openSplitsMenuItem.DropDownItems.Count > 0)
                openSplitsMenuItem.DropDownItems.Add(new ToolStripSeparator());
            var openFromFileMenuItem = new ToolStripMenuItem("From File...");
            openFromFileMenuItem.Click += openSplitsFromFileMenuItem_Click;
            openSplitsMenuItem.DropDownItems.Add(openFromFileMenuItem);
            var openFromURLMenuItem = new ToolStripMenuItem("From URL...");
            openFromURLMenuItem.Click += openSplitsFromURLMenuItem_Click;
            openSplitsMenuItem.DropDownItems.Add(openFromURLMenuItem);
            var openFromSpeedrunComMenuItem = new ToolStripMenuItem("From Speedrun.com...");
            openFromSpeedrunComMenuItem.Click += openFromSpeedrunComMenuItem_Click;
            openSplitsMenuItem.DropDownItems.Add(openFromSpeedrunComMenuItem);
            openSplitsMenuItem.DropDownItems.Add(new ToolStripSeparator());
            var editSplitHistoryMenuItem = new ToolStripMenuItem("Edit History");
            editSplitHistoryMenuItem.Click += editSplitHistoryMenuItem_Click;
            openSplitsMenuItem.DropDownItems.Add(editSplitHistoryMenuItem);
        }

        void openFromSpeedrunComMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TopMost = false;
                IsInDialogMode = true;

                var runImporter = new SpeedrunComRunImporter();
                var run = runImporter.Import(this);

                if (run != null)
                {
                    if (!WarnUserAboutSplitsSave())
                        return;
                    if (!WarnAndRemoveTimerOnly(true))
                        return;
                    run.HasChanged = true;
                    SetRun(run);
                    CurrentState.CallRunManuallyModified();
                }
            }
            finally
            {
                TopMost = Layout.Settings.AlwaysOnTop;
                IsInDialogMode = false;
            }
        }

        void editSplitHistoryMenuItem_Click(object sender, EventArgs e)
        {
            var editHistoryDialog = new EditHistoryDialog(Settings.RecentSplits.Select(x => x.Path));
            if (editHistoryDialog.ShowDialog(this) != System.Windows.Forms.DialogResult.Cancel)
                Settings.RecentSplits = new List<RecentSplitsFile>(Settings.RecentSplits.Where(x => editHistoryDialog.History.Contains(x.Path)));
            UpdateRecentSplits();
        }

        private void UpdateRecentLayouts()
        {
            openLayoutMenuItem.DropDownItems.Clear();

            foreach (var item in Settings.RecentLayouts.Reverse().Where(x => !string.IsNullOrEmpty(x)))
            {
                var menuItem = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(item).EscapeMenuItemText());
                menuItem.Click += (x, y) => { OpenLayoutFromFile(item); };
                openLayoutMenuItem.DropDownItems.Add(menuItem);
            }
            if (openLayoutMenuItem.DropDownItems.Count > 0)
                openLayoutMenuItem.DropDownItems.Add(new ToolStripSeparator());
            var openLayoutFromFileMenuItem = new ToolStripMenuItem("From File...");
            openLayoutFromFileMenuItem.Click += openLayoutFromFileMenuItem_Click;
            openLayoutMenuItem.DropDownItems.Add(openLayoutFromFileMenuItem);
            var openFromURLMenuItem = new ToolStripMenuItem("From URL...");
            openFromURLMenuItem.Click += openLayoutFromURLMenuItem_Click;
            openLayoutMenuItem.DropDownItems.Add(openFromURLMenuItem);
            var defaultLayoutMenuItem = new ToolStripMenuItem("Default");
            defaultLayoutMenuItem.Click += (x, y) => { LoadDefaultLayout(); };
            openLayoutMenuItem.DropDownItems.Add(defaultLayoutMenuItem);
            openLayoutMenuItem.DropDownItems.Add(new ToolStripSeparator());
            var editLayoutHistoryMenuItem = new ToolStripMenuItem("Edit History");
            editLayoutHistoryMenuItem.Click += editLayoutHistoryMenuItem_Click;
            openLayoutMenuItem.DropDownItems.Add(editLayoutHistoryMenuItem);
        }

        void editLayoutHistoryMenuItem_Click(object sender, EventArgs e)
        {
            var editHistoryDialog = new EditHistoryDialog(Settings.RecentLayouts);
            if (editHistoryDialog.ShowDialog(this) != System.Windows.Forms.DialogResult.Cancel)
                Settings.RecentLayouts = editHistoryDialog.History;
            UpdateRecentLayouts();
        }

        void openLayoutFromURLMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IsInDialogMode = true;
                TopMost = false;
                string url = null;

                if (DialogResult.OK == InputBox.Show("Open Layout from URL", "URL:", ref url))
                {
                    try
                    {
                        var uri = new Uri(url);
                        if (uri.Host.ToLowerInvariant() == "ge.tt"
                            && uri.LocalPath.Length > 0
                            && !uri.LocalPath.Substring(1).Contains('/'))
                        {
                            uri = new Uri(string.Format("http://ge.tt/api/1/files{0}/0/blob?download", uri.LocalPath));
                        }

                        var request = WebRequest.Create(uri);

                        using (var response = request.GetResponse())
                        using (var stream = response.GetResponseStream())
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            try
                            {
                                var layout = new XMLLayoutFactory(memoryStream).Create(CurrentState);
                                layout.HasChanged = true;
                                SetLayout(layout);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                                DontRedraw = true;
                                MessageBox.Show(this, "The selected file was not recognized as a layout file. (" + ex.Message + ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                DontRedraw = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        DontRedraw = true;
                        MessageBox.Show(this, "The layout file couldn't be downloaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        DontRedraw = false;
                    }
                }
            }
            finally
            {
                IsInDialogMode = false;
                TopMost = Layout.Settings.AlwaysOnTop;
            }
        }

        void openSplitsFromURLMenuItem_Click(object sender, EventArgs e)
        {
            var runImporter = new URLRunImporter();
            var run = runImporter.Import(this);

            if (run != null)
            {
                if (!WarnUserAboutSplitsSave())
                    return;
                if (!WarnAndRemoveTimerOnly(true))
                    return;
                run.HasChanged = true;
                SetRun(run);
                CurrentState.CallRunManuallyModified();
            }
        }

        private void StartOrSplit()
        {
            if (CurrentState.CurrentPhase == TimerPhase.Running)
            {
                Model.Split();
            }
            else if (CurrentState.CurrentPhase == TimerPhase.Paused)
            {
                Model.Pause();
            }
            else if (CurrentState.CurrentPhase == TimerPhase.NotRunning)
            {
                Model.Start();
            }
            else if (CurrentState.CurrentPhase == TimerPhase.Ended)
            {
                Model.Reset();
            }
        }

        void hook_KeyOrButtonPressed(object sender, KeyOrButton e)
        {
            Action action = () =>
            {
                var hotkeyProfile = Settings.HotkeyProfiles[CurrentState.CurrentHotkeyProfile];

                if ((ActiveForm == this || hotkeyProfile.GlobalHotkeysEnabled) && !ResetMessageShown && !IsInDialogMode)
                {
                    if (hotkeyProfile.SplitKey == e)
                    {
                        if (hotkeyProfile.HotkeyDelay > 0)
                        {
                            var splitTimer = new System.Timers.Timer(hotkeyProfile.HotkeyDelay * 1000f);
                            splitTimer.Enabled = true;
                            splitTimer.Elapsed += splitTimer_Elapsed;
                        }
                        else
                            StartOrSplit();
                    }

                    else if (hotkeyProfile.UndoKey == e)
                    {
                        Model.UndoSplit();
                    }

                    else if (hotkeyProfile.SkipKey == e)
                    {
                        Model.SkipSplit();
                    }

                    else if (hotkeyProfile.ResetKey == e)
                    {
                        Reset();
                    }

                    else if (hotkeyProfile.PauseKey == e)
                    {
                        if (hotkeyProfile.HotkeyDelay > 0)
                        {
                            var pauseTimer = new System.Timers.Timer(hotkeyProfile.HotkeyDelay * 1000f);
                            pauseTimer.Enabled = true;
                            pauseTimer.Elapsed += pauseTimer_Elapsed;
                        }
                        else
                            Model.Pause();
                    }

                    else if (hotkeyProfile.SwitchComparisonPrevious == e)
                        Model.SwitchComparisonPrevious();

                    else if (hotkeyProfile.SwitchComparisonNext == e)
                        Model.SwitchComparisonNext();
                }

                if (hotkeyProfile.ToggleGlobalHotkeys == e)
                {
                    hotkeyProfile.GlobalHotkeysEnabled = !hotkeyProfile.GlobalHotkeysEnabled;
                    SetProgressBar();
                }
            };

            new Task(() =>
            {
                try
                {
                    Invoke(action);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }).Start();
        }

        void pauseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ((System.Timers.Timer)sender).Stop();
            Model.Pause();
        }

        void splitTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ((System.Timers.Timer)sender).Stop();
            StartOrSplit();
        }

        void RefreshTimerWorker()
        {
            while (true)
            {
                Thread.Sleep(25);
                try
                {
                    TimerElapsed();
                }
                catch { }
            }
        }

        void TimerElapsed()
        {
            try
            {
                this.InvokeIfRequired(() =>
                {
                    try
                    {
                        if (Hook != null)
                            Hook.Poll();

                        if (CurrentState.Run.IsAutoSplitterActive())
                            CurrentState.Run.AutoSplitter.Component.Update(null, CurrentState, 0, 0, Layout.Mode);

                        if (DontRedraw)
                            return;

                        if (RefreshesRemaining > 0 || InvalidationRequired)
                        {
                            InvalidateForm();
                            if (InvalidationRequired)
                                InvalidationRequired = false;
                        }
                        else
                        {
                            GlobalCache.Restart();
                            GlobalCache["LayoutHashCode"] = new XMLLayoutSaver().CreateLayoutNode(null, null, Layout);

                            if (GlobalCache.HasChanged)
                                InvalidateForm();
                            else
                            {
                                Invalidator.Restart();

                                this.InvokeIfRequired(() =>
                                {
                                    try
                                    {
                                        ComponentRenderer.Update(Invalidator, CurrentState, Width, Height, Layout.Mode);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error(ex);
                                        Invalidate();
                                    }
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        Invalidate();
                    }
                });
            }
            catch (Exception ex)
            {
                if (!(ex is ObjectDisposedException && ((ObjectDisposedException)ex).ObjectName == "TimerForm"))
                {
                    Log.Error(ex);
                    Invalidate();
                }
            }
        }

        private void FixSize()
        {
            ComponentRenderer.CalculateOverallSize(Layout.Mode);
            var currentSize = ComponentRenderer.OverallSize;
            if (RefreshesRemaining <= 0)
            {
                if (OldSize != currentSize)
                {
                    MinimumSize = new Size(0, 0);
                    if (Layout.Mode == LayoutMode.Vertical)
                        Height = (int)((currentSize / (double)OldSize) * Height + 0.5);
                    else
                        Width = (int)((currentSize / (double)OldSize) * Width + 0.5);
                    OldSize = currentSize;
                }
                var minSize = (int)(currentSize / 5 + 0.5f);
                if (Layout.Mode == LayoutMode.Vertical)
                    MinimumSize = new Size(25, Math.Max(minSize, 25));
                else
                    MinimumSize = new Size(Math.Max(minSize, 25), 25);
            }
        }

        private void KeepLayoutSize()
        {
            if (RefreshesRemaining > 0)
            {
                if (Layout.Mode == LayoutMode.Vertical)
                    Size = new Size(Layout.VerticalWidth, Layout.VerticalHeight);
                else
                    Size = new Size(Layout.HorizontalWidth, Layout.HorizontalHeight);

                if (OldSize != ComponentRenderer.OverallSize)
                    UpdateRefreshesRemaining();
                else
                    RefreshesRemaining--;
                OldSize = ComponentRenderer.OverallSize;
            }
        }

        private void UpdateRefreshesRemaining()
        {
            RefreshesRemaining = 5;
        }

        protected void InvalidateForm()
        {
            this.InvokeIfRequired(() =>
            {
                try
                {
                    UpdateAllComponents();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });

            Invalidate();
        }

        protected void UpdateAllComponents()
        {
            foreach (var component in Layout.Components)
                component.Update(null, CurrentState, Width, Height, Layout.Mode);
        }

        private void PaintForm(Graphics g, Region clip)
        {
            if (!clip.GetBounds(g).Equals(UpdateRegion.GetBounds(g)))
                UpdateRegion.Union(clip);

            DrawBackground(g);

            Opacity = Layout.Settings.Opacity;

            // Set MousePassThrough after setting Opacity, because setting Opacity can reset the Form's WS_EX_LAYERED flag.
            MousePassThrough = Layout.Settings.MousePassThroughWhileRunning && Model.CurrentState.CurrentPhase == TimerPhase.Running && !IsForegroundWindow;

            if (Layout.Settings.AntiAliasing)
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
            else
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.GammaCorrected;
            g.InterpolationMode = InterpolationMode.Bilinear;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            FixSize();

            var scaleFactor = Layout.Mode == LayoutMode.Vertical
                ? Height / ComponentRenderer.OverallSize
                : Width / ComponentRenderer.OverallSize;

            g.ResetTransform();
            g.TranslateTransform(-0.5f, -0.5f);
            g.ScaleTransform(scaleFactor, scaleFactor);
            float transformedWidth = Width;
            float transformedHeight = Height;

            if (Layout.Mode == LayoutMode.Vertical)
                transformedWidth /= scaleFactor;
            else
                transformedHeight /= scaleFactor;

            ComponentRenderer.Render(g, CurrentState, transformedWidth, transformedHeight, Layout.Mode, UpdateRegion);

            FixSize();
            KeepLayoutSize();
            MaintainMinimumSize();
        }

        private void TimerForm_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                var clip = e.Graphics.Clip;
                e.Graphics.Clip = new Region();
                PaintForm(e.Graphics, clip);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Invalidate();
            }
        }

        private void DrawBackground(Graphics g)
        {
            if (Layout.Settings.BackgroundType == BackgroundType.Image)
            {
                if (Layout.Settings.BackgroundImage != null)
                {
                    if (Layout.Settings.BackgroundImage != previousBackground
                        || Layout.Settings.ImageOpacity != previousOpacity
                        || Layout.Settings.ImageBlur != previousBlur)
                    {
                        CreateBakedBackground();
                    }
                    foreach (var rectangle in UpdateRegion.GetRegionScans(g.Transform))
                    {
                        var rect = Rectangle.Round(rectangle);
                        g.DrawImage(bakedBackground, rect, rect, GraphicsUnit.Pixel);
                    }
                }
            }
            else if (Layout.Settings.BackgroundColor != Color.Transparent
                || Layout.Settings.BackgroundType != BackgroundType.SolidColor
                && Layout.Settings.BackgroundColor2 != Color.Transparent)
            {
                var gradientBrush = new LinearGradientBrush(
                            new PointF(0, 0),
                            Layout.Settings.BackgroundType == BackgroundType.HorizontalGradient
                            ? new PointF(Size.Width, 0)
                            : new PointF(0, Size.Height),
                            Layout.Settings.BackgroundColor,
                            Layout.Settings.BackgroundType == BackgroundType.SolidColor
                            ? Layout.Settings.BackgroundColor
                            : Layout.Settings.BackgroundColor2);
                g.FillRectangle(gradientBrush, 0, 0, Size.Width, Size.Height);
            }
        }

        private void CreateBakedBackground()
        {
            var image = Layout.Settings.BackgroundImage;
            var opacity = Layout.Settings.ImageOpacity;
            var blur = Layout.Settings.ImageBlur;

            if (image != null)
            {
                if (blur > 0)
                {
                    if (blur != previousBlur || image != previousBackground)
                    {
                        if (blurredBackground != null)
                            blurredBackground.Dispose();
                        blurredBackground = ImageBlur.Generate(image, blur * 10);
                    }
                    image = blurredBackground;
                }

                var croppedWidth = (float)image.Width;
                var croppedHeight = (float)image.Height;

                if (image.Width / (float)image.Height > Width / (float)Height)
                {
                    croppedWidth = image.Height * (Width / (float)Height);
                }
                else
                {
                    croppedHeight = image.Width * (Height / (float)Width);
                }

                var bitmap = new Bitmap(Width, Height, image.PixelFormat);

                using (var graphics = Graphics.FromImage(bitmap))
                {
                    var matrix = new ColorMatrix();
                    matrix.Matrix33 = opacity;
                    var attributes = new ImageAttributes();
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(image,
                        new Rectangle(0, 0, Width, Height),
                        (image.Width - croppedWidth) / 2,
                        (image.Height - croppedHeight) / 2,
                        croppedWidth,
                        croppedHeight,
                        GraphicsUnit.Pixel,
                        attributes);
                }

                if (bakedBackground != null)
                    bakedBackground.Dispose();

                bakedBackground = bitmap;
                previousBackground = Layout.Settings.BackgroundImage;
                previousOpacity = opacity;
                previousBlur = blur;
            }
        }

        private void TimerForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MousePoint = new Point(e.X, e.Y);
                MouseIsDown = true;
            }
        }

        private void TimerForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseIsDown)
            {
                int x = Location.X - MousePoint.X + e.Location.X;
                int y = Location.Y - MousePoint.Y + e.Location.Y;
                Location = new Point(x, y);
            }
        }

        private void TimerForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                MouseIsDown = false;
            if (e.Button == MouseButtons.Right)
            {
                RightClickMenu.Show(this, e.Location);
                MouseIsDown = false;
            }
        }

        private void TimerForm_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                Model.ScrollUp();
            } else if (e.Delta < 0)
            {
                Model.ScrollDown();
            }
        }

        protected bool ShowSRLRules()
        {
            if (!Settings.AgreedToSRLRules)
            {
                Process.Start("http://speedrunslive.com/faq/rules/");
                var result = MessageBox.Show(this, "Please read through the rules of SpeedRunsLive carefully.\r\nDo you agree to these rules?", "SpeedRunsLive Rules", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Settings.AgreedToSRLRules = true;
                    return true;
                }
                return false;
            }
            return true;
        }

        protected override void WndProc(ref Message m)
        {
            const uint WM_NCHITTEST = 0x0084;
            const uint WM_MOUSEMOVE = 0x0200;
            const uint WM_PAINT = 0x000F;
            const uint WM_SIZING = 0x0214;

            const uint HTLEFT = 10;
            const uint HTRIGHT = 11;
            const uint HTBOTTOMRIGHT = 17;
            const uint HTBOTTOM = 15;
            const uint HTBOTTOMLEFT = 16;
            const uint HTTOP = 12;
            const uint HTTOPLEFT = 13;
            const uint HTTOPRIGHT = 14;

            const int RESIZE_HANDLE_SIZE = 10;
            bool handled = false;

            if (m.Msg == WM_NCHITTEST || m.Msg == WM_MOUSEMOVE)
            {
                Size formSize = Size;
                Point screenPoint = new Point(m.LParam.ToInt32());
                Point clientPoint = PointToClient(screenPoint);

                Dictionary<uint, Rectangle> boxes = new Dictionary<uint, Rectangle>() {
                    {HTBOTTOMLEFT, new Rectangle(0, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
                    {HTBOTTOM, new Rectangle(RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, formSize.Width - 2*RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
                    {HTBOTTOMRIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
                    {HTRIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2*RESIZE_HANDLE_SIZE)},
                    {HTTOPRIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
                    {HTTOP, new Rectangle(RESIZE_HANDLE_SIZE, 0, formSize.Width - 2*RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
                    {HTTOPLEFT, new Rectangle(0, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
                    {HTLEFT, new Rectangle(0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2*RESIZE_HANDLE_SIZE) }
                };

                foreach (KeyValuePair<uint, Rectangle> hitBox in boxes)
                {
                    if (hitBox.Value.Contains(clientPoint))
                    {
                        m.Result = (IntPtr)hitBox.Key;
                        handled = true;
                        break;
                    }
                }
            }

            if (m.Msg == WM_SIZING)
            {
                handled = WmSizingProc(ref m);
            }

            if (m.Msg == WM_PAINT)
            {
                if (hRgn != IntPtr.Zero)
                {
                    DeleteObject(hRgn);
                }
                hRgn = CreateRectRgn(0, 0, 0, 0);
                var x = GetUpdateRgn(Handle, hRgn, false);
                try
                {
                    UpdateRegion = Region.FromHrgn(hRgn);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }

            if (!handled)
            {
                try
                {
                    base.WndProc(ref m);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        private bool WmSizingProc(ref Message m)
        {
            const uint WMSZ_TOPLEFT = 4;
            const uint WMSZ_TOPRIGHT = 5;
            const uint WMSZ_BOTTOMLEFT = 7;
            const uint WMSZ_BOTTOMRIGHT = 8;

            if (!ResizingInitialAspectRatio.HasValue)
                return false;

            if (!ModifierKeys.HasFlag(Keys.Shift))
                return false;

            bool anchorLeft;
            bool anchorTop;

            switch ((uint)m.WParam)
            {
                case WMSZ_TOPLEFT:
                    anchorLeft = false;
                    anchorTop = false;
                    break;
                case WMSZ_TOPRIGHT:
                    anchorLeft = true;
                    anchorTop = false;
                    break;
                case WMSZ_BOTTOMLEFT:
                    anchorLeft = false;
                    anchorTop = true;
                    break;
                case WMSZ_BOTTOMRIGHT:
                    anchorLeft = true;
                    anchorTop = true;
                    break;

                default:
                    return false;
            }

            var rect = (Win32.RECT)Marshal.PtrToStructure(m.LParam, typeof(Win32.RECT));
            var currentAspectRatio = (float)rect.Width / rect.Height;

            if (currentAspectRatio >= ResizingInitialAspectRatio.Value)
            {
                var newWidth = (int)(rect.Height * ResizingInitialAspectRatio.Value);
                if (anchorLeft)
                    rect.Right = rect.Left + newWidth;
                else
                    rect.Left = rect.Right - newWidth;
            }
            else
            {
                var newHeight = (int)(rect.Width / ResizingInitialAspectRatio.Value);
                if (anchorTop)
                    rect.Bottom = rect.Top + newHeight;
                else
                    rect.Top = rect.Bottom - newHeight;
            }

            Marshal.StructureToPtr(rect, m.LParam, false);

            return true;
        }

        IntPtr hRgn = IntPtr.Zero;
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DeleteObject([In] IntPtr hObject);

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void Exit()
        {
            Close();
        }

        private void SetRun(IRun run)
        {
            foreach (var icon in CurrentState.Run.Select(x => x.Icon).Except(run.Select(x => x.Icon)))
            {
                if (icon != null)
                    icon.Dispose();
            }
            if (CurrentState.Run.GameIcon != null && CurrentState.Run.GameIcon != run.GameIcon)
            {
                CurrentState.Run.GameIcon.Dispose();
            }

            run.ComparisonGenerators = new List<IComparisonGenerator>(CurrentState.Run.ComparisonGenerators);
            foreach (var generator in run.ComparisonGenerators)
                generator.Run = run;
            run.FixSplits();
            DeactivateAutoSplitter();
            CurrentState.Run = run;
            InvalidationRequired = true;
            RegenerateComparisons();
            SwitchComparison(CurrentState.CurrentComparison);
            CreateAutoSplitter();
            UpdateRefreshesRemaining();
        }

        private void CreateAutoSplitter()
        {
            var splitter = AutoSplitterFactory.Instance.Create(CurrentState.Run.GameName);
            CurrentState.Run.AutoSplitter = splitter;
            if (splitter != null && CurrentState.Settings.ActiveAutoSplitters.Contains(CurrentState.Run.GameName))
            {
                splitter.Activate(CurrentState);
                if (splitter.IsActivated
                && CurrentState.Run.AutoSplitterSettings != null
                && CurrentState.Run.AutoSplitterSettings.GetAttribute("gameName") == CurrentState.Run.GameName)
                    CurrentState.Run.AutoSplitter.Component.SetSettings(CurrentState.Run.AutoSplitterSettings);
            }
        }

        private void DeactivateAutoSplitter()
        {
            if (CurrentState.Run.AutoSplitter != null)
                CurrentState.Run.AutoSplitter.Deactivate();
        }

        private void AddCurrentSplitsToLRU(TimingMethod lastTimingMethod, string lastHotkeyProfile)
        {
            if (CurrentState.Run != null && Settings.RecentSplits.Any(x => x.Path == CurrentState.Run.FilePath))
                AddSplitsFileToLRU(CurrentState.Run.FilePath, CurrentState.Run, lastTimingMethod, lastHotkeyProfile);
        }

        private IRun LoadRunFromFile(string filePath, TimingMethod? previousTimingMethod = null, string previousHotkeyProfile = null)
        {
            IRun run;

            using (var stream = File.OpenRead(filePath))
            {
                RunFactory.Stream = stream;
                RunFactory.FilePath = filePath;

                run = RunFactory.Create(ComparisonGeneratorsFactory);
            }

            if (previousTimingMethod.HasValue && previousHotkeyProfile != null)
                AddCurrentSplitsToLRU(previousTimingMethod.Value, previousHotkeyProfile);

            AddSplitsFileToLRU(filePath, run, CurrentState.CurrentTimingMethod, CurrentState.CurrentHotkeyProfile);

            return run;
        }

        private ILayout LoadLayoutFromFile(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                var layout = new XMLLayoutFactory(stream).Create(CurrentState);
                layout.FilePath = filePath;
                AddLayoutFileToLRU(filePath);
                return layout;
            }
        }

        private void OpenRunFromFile(string filePath)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                if (!WarnUserAboutSplitsSave())
                    return;
                if (!WarnAndRemoveTimerOnly(true))
                    return;

                var previousTimingMethod = CurrentState.CurrentTimingMethod;
                var previousHotkeyProfile = CurrentState.CurrentHotkeyProfile;

                UpdateStateFromSplitsPath(filePath);

                var run = LoadRunFromFile(filePath, previousTimingMethod, previousHotkeyProfile);
                SetRun(run);
                CurrentState.CallRunManuallyModified();
            }
            catch (Exception e)
            {
                Log.Error(e);
                DontRedraw = true;
                MessageBox.Show(this, "The selected file was not recognized as a splits file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DontRedraw = false;
            }
            Cursor.Current = Cursors.Arrow;
        }

        private void UpdateStateFromSplitsPath(string filePath)
        {
            var recentSplitsFile = Settings.RecentSplits.LastOrDefault(splitsFile => splitsFile.Path == filePath);
            if (recentSplitsFile.Path != null)
            {
                CurrentState.CurrentTimingMethod = recentSplitsFile.LastTimingMethod;
                if (Settings.HotkeyProfiles.ContainsKey(recentSplitsFile.LastHotkeyProfile))
                {
                    CurrentState.CurrentHotkeyProfile = recentSplitsFile.LastHotkeyProfile;
                    if (Hook != null)
                    {
                        Settings.UnregisterAllHotkeys(Hook);
                        Settings.RegisterHotkeys(Hook, CurrentState.CurrentHotkeyProfile);
                    }
                }
            }
        }

        private void OpenSplits()
        {
            using (var splitDialog = new OpenFileDialog())
            {
                IsInDialogMode = true;
                try
                {
                    if (Settings.RecentSplits.Any() && !string.IsNullOrEmpty(Settings.RecentSplits.Last().Path))
                        splitDialog.InitialDirectory = Path.GetDirectoryName(Settings.RecentSplits.Last().Path);
                    var result = splitDialog.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        OpenRunFromFile(splitDialog.FileName);
                    }
                }
                finally
                {
                    IsInDialogMode = false;
                }
            }
        }

        private bool SaveSplitsAs(bool promptPBMessage)
        {
            using (var splitDialog = new SaveFileDialog())
            {
                splitDialog.FileName = CurrentState.Run.GetExtendedFileName();
                splitDialog.Filter = "LiveSplit Splits (*.lss)|*.lss|All Files (*.*)|*.*";
                IsInDialogMode = true;
                try
                {
                    var result = splitDialog.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        CurrentState.Run.FilePath = splitDialog.FileName;
                        return SaveSplits(promptPBMessage);
                    }
                    return false;
                }
                finally
                {
                    IsInDialogMode = false;
                }
            }
        }

        private bool SaveSplits(bool promptPBMessage)
        {
            var savePath = CurrentState.Run.FilePath;

            if (savePath == null)
            {
                return SaveSplitsAs(promptPBMessage);
            }

            CurrentState.Run.FixSplits();

            var result = DialogResult.No;

            if (promptPBMessage && ((CurrentState.CurrentPhase == TimerPhase.Ended
                && CurrentState.Run.Last().PersonalBestSplitTime[CurrentState.CurrentTimingMethod] != null
                && CurrentState.Run.Last().SplitTime[CurrentState.CurrentTimingMethod] >= CurrentState.Run.Last().PersonalBestSplitTime[CurrentState.CurrentTimingMethod])
                || CurrentState.CurrentPhase == TimerPhase.Running
                || CurrentState.CurrentPhase == TimerPhase.Paused))
            {
                DontRedraw = true;
                result = MessageBox.Show(this, "This run did not beat your current splits. Would you like to save this run as a Personal Best?", "Save as Personal Best?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                DontRedraw = false;
                if (result == DialogResult.Yes)
                {
                    Model.ResetAndSetAttemptAsPB();
                }
                else if (result == DialogResult.Cancel)
                    return false;
            }

            var stateCopy = CurrentState;
            if (result == DialogResult.No)
            {
                var modelCopy = new TimerModel();
                stateCopy = CurrentState.Clone() as LiveSplitState;
                modelCopy.CurrentState = stateCopy;
                modelCopy.Reset();
            }

            try
            {
                if (!File.Exists(savePath))
                    File.Create(savePath).Close();

                using (var memoryStream = new MemoryStream())
                {
                    RunSaver.Save(stateCopy.Run, memoryStream);

                    using (var stream = File.Open(savePath, FileMode.Create, FileAccess.Write))
                    {
                        var buffer = memoryStream.GetBuffer();
                        stream.Write(buffer, 0, (int)memoryStream.Length);
                    }

                    CurrentState.Run.HasChanged = false;
                }

                AddSplitsFileToLRU(savePath, stateCopy.Run, CurrentState.CurrentTimingMethod, CurrentState.CurrentHotkeyProfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Splits could not be saved!", "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Error(ex);
                return false;
            }
            return true;
        }

        private bool SaveLayout()
        {
            var savePath = Layout.FilePath;
            if (Layout.Mode == LayoutMode.Vertical)
            {
                Layout.VerticalWidth = Width;
                Layout.VerticalHeight = Height;
            }
            else
            {
                Layout.HorizontalWidth = Width;
                Layout.HorizontalHeight = Height;
            }
            Layout.X = Location.X;
            Layout.Y = Location.Y;

            if (savePath == null)
            {
                return SaveLayoutAs();
            }

            try
            {
                if (!File.Exists(savePath))
                    File.Create(savePath).Close();

                using (var memoryStream = new MemoryStream())
                {
                    LayoutSaver.Save(Layout, memoryStream);

                    using (var stream = File.Open(savePath, FileMode.Create, FileAccess.Write))
                    {
                        var buffer = memoryStream.GetBuffer();
                        stream.Write(buffer, 0, (int)memoryStream.Length);
                    }

                    Layout.HasChanged = false;
                }

                AddLayoutFileToLRU(savePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Layout could not be saved!", "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Error(ex);
                return false;
            }
            return true;
        }

        private void EditSplits()
        {
            var runCopy = CurrentState.Run.Clone() as IRun;
            var activeAutoSplitters = new List<string>(CurrentState.Settings.ActiveAutoSplitters);
            var editor = new RunEditorDialog(CurrentState);
            editor.RunEdited += editor_RunEdited;
            editor.ComparisonRenamed += editor_ComparisonRenamed;
            editor.SegmentRemovedOrAdded += editor_SegmentRemovedOrAdded;
            try
            {
                TopMost = false;
                IsInDialogMode = true;
                if (CurrentState.CurrentPhase == TimerPhase.NotRunning)
                    editor.AllowChangingSegments = true;
                var result = editor.ShowDialog(this);
                if (result == DialogResult.Cancel)
                {
                    foreach (var image in runCopy.Select(x => x.Icon))
                        editor.ImagesToDispose.Remove(image);
                    editor.ImagesToDispose.Remove(runCopy.GameIcon);

                    CurrentState.Settings.ActiveAutoSplitters = activeAutoSplitters;
                    SetRun(runCopy);
                    CurrentState.CallRunManuallyModified();
                }
                foreach (var image in editor.ImagesToDispose)
                    image.Dispose();
            }
            finally
            {
                TopMost = Layout.Settings.AlwaysOnTop;
                IsInDialogMode = false;
            }
        }

        void editor_SegmentRemovedOrAdded(object sender, EventArgs e)
        {
            InvalidationRequired = true;
        }

        void editor_ComparisonRenamed(object sender, EventArgs e)
        {
            CurrentState.CallComparisonRenamed(e);
        }

        void editor_RunEdited(object sender, EventArgs e)
        {
            RegenerateComparisons();
            CurrentState.CallRunManuallyModified();
            WarnAndRemoveTimerOnly(false);
        }

        protected bool WarnAndRemoveTimerOnly(bool canCancel)
        {
            if (InTimerOnlyMode)
            {
                if (!WarnUserAboutLayoutSave(canCancel))
                    return false;

                InTimerOnlyMode = false;
                ILayout layout;
                try
                {
                    var lastLayoutPath = Settings.RecentLayouts.LastOrDefault(x => !string.IsNullOrEmpty(x));
                    if (lastLayoutPath != null)
                        layout = LoadLayoutFromFile(lastLayoutPath);
                    else
                        layout = new StandardLayoutFactory().Create(CurrentState);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    layout = new StandardLayoutFactory().Create(CurrentState);
                }
                layout.X = Location.X;
                layout.Y = Location.Y;
                SetLayout(layout);
            }
            return true;
        }

        private void EditLayout()
        {
            var editor = new LayoutEditorDialog(Layout, CurrentState, this);
            editor.OrientationSwitched += editor_OrientationSwitched;
            editor.LayoutResized += editor_LayoutResized;
            editor.LayoutSettingsAssigned += editor_LayoutSettingsAssigned;
            Layout.X = Location.X;
            Layout.Y = Location.Y;
            if (Layout.Mode == LayoutMode.Vertical)
            {
                Layout.VerticalWidth = Size.Width;
                Layout.VerticalHeight = Size.Height;
            }
            else
            {
                Layout.HorizontalWidth = Size.Width;
                Layout.HorizontalHeight = Size.Height;
            }
            var layoutCopy = (ILayout)Layout.Clone();
            var document = new XmlDocument();
            var componentSettings = Layout.Components.Select(x =>
                {
                    try
                    {
                        return x.GetSettings(document);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                        return null;
                    }
                }).ToList();
            try
            {
                TopMost = false;
                editor.ShowDialog(this);
                if (editor.DialogResult == DialogResult.Cancel)
                {
                    foreach (var component in layoutCopy.Components)
                        editor.ComponentsToDispose.Remove(component);
                    editor.ImagesToDispose.Remove(layoutCopy.Settings.BackgroundImage);

                    using (var enumerator = componentSettings.GetEnumerator())
                    {
                        foreach (var component in layoutCopy.Components)
                        {
                            if (enumerator.MoveNext())
                                component.SetSettings(enumerator.Current);
                        }
                    }
                    SetLayout(layoutCopy);
                }
                foreach (var component in editor.ComponentsToDispose)
                    component.Dispose();
                foreach (var image in editor.ImagesToDispose)
                    image.Dispose();
            }
            finally
            {
                TopMost = Layout.Settings.AlwaysOnTop;
            }
        }

        void editor_LayoutSettingsAssigned(object sender, EventArgs e)
        {
            InvalidationRequired = true;
        }

        void editor_LayoutResized(object sender, EventArgs e)
        {
            SetInTimerOnlyMode();
            if (Layout.Mode == LayoutMode.Horizontal)
            {
                Layout.VerticalWidth = UI.Layout.InvalidSize;
                Layout.VerticalHeight = UI.Layout.InvalidSize;
            }
            else
            {
                Layout.HorizontalWidth = UI.Layout.InvalidSize;
                Layout.HorizontalHeight = UI.Layout.InvalidSize;
            }
        }

        void editor_OrientationSwitched(object sender, EventArgs e)
        {
            ComponentRenderer.CalculateOverallSize(Layout.Mode);
            if (Layout.Mode == LayoutMode.Vertical)
            {
                Layout.HorizontalWidth = Size.Width;
                Layout.HorizontalHeight = Size.Height;
                if (Layout.VerticalHeight == UI.Layout.InvalidSize || Layout.VerticalWidth == UI.Layout.InvalidSize)
                {
                    Layout.VerticalWidth = 300;
                    Layout.VerticalHeight = (int)(ComponentRenderer.OverallSize + 0.5);
                }
            }
            else
            {
                Layout.VerticalWidth = Size.Width;
                Layout.VerticalHeight = Size.Height;
                if (Layout.HorizontalWidth == UI.Layout.InvalidSize || Layout.HorizontalHeight == UI.Layout.InvalidSize)
                {
                    Layout.HorizontalWidth = (int)(ComponentRenderer.OverallSize + 0.5);
                    Layout.HorizontalHeight = 45;
                }
            }
            TopMost = false;
            SetLayout(Layout);
        }

        private bool SaveLayoutAs()
        {
            using (var layoutDialog = new SaveFileDialog())
            {
                layoutDialog.Filter = "LiveSplit Layout (*.lsl)|*.lsl|All Files (*.*)|*.*";
                IsInDialogMode = true;
                try
                {
                    var result = layoutDialog.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        Layout.FilePath = layoutDialog.FileName;
                        return SaveLayout();
                    }
                    return false;
                }
                finally
                {
                    IsInDialogMode = false;
                }
            }
        }

        private void OpenAboutBox()
        {
            using (var aboutBox = new AboutBox())
            {
                try
                {
                    TopMost = false;
                    aboutBox.ShowDialog(this);
                }
                finally
                {
                    TopMost = Layout.Settings.AlwaysOnTop;
                }
            }
        }

        private void OpenLayout()
        {
            using (var layoutDialog = new OpenFileDialog())
            {
                layoutDialog.Filter = "LiveSplit Layout (*.lsl)|*.lsl|All Files (*.*)|*.*";
                IsInDialogMode = true;
                try
                {
                    if (Settings.RecentLayouts.Any() && !string.IsNullOrEmpty(Settings.RecentLayouts.Last()))
                        layoutDialog.InitialDirectory = Path.GetDirectoryName(Settings.RecentLayouts.Last());
                    var result = layoutDialog.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        OpenLayoutFromFile(layoutDialog.FileName);
                    }
                }
                finally
                {
                    IsInDialogMode = false;
                }
            }
        }

        private void OpenLayoutFromFile(string filePath)
        {
            if (WarnUserAboutLayoutSave(true))
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    var layout = LoadLayoutFromFile(filePath);
                    SetLayout(layout);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    DontRedraw = true;
                    MessageBox.Show(this, "The selected file was not recognized as a layout file. (" + e.Message + ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DontRedraw = false;
                }
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void LoadDefaultLayout()
        {
            if (WarnUserAboutLayoutSave(true))
            {
                var layout = new StandardLayoutFactory().Create(CurrentState);
                layout.X = Location.X;
                layout.Y = Location.Y;
                SetLayout(layout);
                Settings.AddToRecentLayouts("");
            }
        }

        private void SetLayout(ILayout layout)
        {
            if (Layout != null && Layout != layout)
            {
                if (Layout.Settings.BackgroundImage != null && Layout.Settings.BackgroundImage != layout.Settings.BackgroundImage)
                    Layout.Settings.BackgroundImage.Dispose();

                foreach (var component in Layout.Components.Except(layout.Components))
                    component.Dispose();

                foreach (var component in layout.Components.Except(Layout.Components).OfType<IDeactivatableComponent>())
                    component.Activated = true;
            }
            Layout = layout;
            ComponentRenderer.VisibleComponents = Layout.Components;
            CurrentState.LayoutSettings = layout.Settings;
            UpdateRefreshesRemaining();
            MinimumSize = new Size(0, 0);
            if (Layout.Mode == LayoutMode.Vertical)
            {
                if (Layout.VerticalWidth != UI.Layout.InvalidSize && Layout.VerticalHeight != UI.Layout.InvalidSize)
                    Size = new Size(Layout.VerticalWidth, Layout.VerticalHeight);
            }
            else
            {
                if (Layout.HorizontalWidth != UI.Layout.InvalidSize && Layout.HorizontalHeight != UI.Layout.InvalidSize)
                    Size = new Size(Layout.HorizontalWidth, Layout.HorizontalHeight);
            }
            var x = Math.Max(SystemInformation.VirtualScreen.X, Math.Min(Layout.X, SystemInformation.VirtualScreen.X + SystemInformation.VirtualScreen.Width - Width));
            var y = Math.Max(SystemInformation.VirtualScreen.Y, Math.Min(Layout.Y, SystemInformation.VirtualScreen.Y + SystemInformation.VirtualScreen.Height - Height));
            Location = new Point(x, y);
            TopMost = Layout.Settings.AlwaysOnTop;
            SetInTimerOnlyMode();
        }

        private void CloseSplits()
        {
            var needToChangeLayout = Layout.Components.Count() != 1 || Layout.Components.FirstOrDefault().ComponentName != "Timer";

            if (!WarnUserAboutSplitsSave())
                return;
            if (needToChangeLayout && !WarnUserAboutLayoutSave(true))
                return;

            AddCurrentSplitsToLRU(CurrentState.CurrentTimingMethod, CurrentState.CurrentHotkeyProfile);

            var run = new StandardRunFactory().Create(ComparisonGeneratorsFactory);
            Model.Reset();
            SetRun(run);
            Settings.AddToRecentSplits("", null, TimingMethod.RealTime, CurrentState.CurrentHotkeyProfile);
            InTimerOnlyMode = true;

            if (needToChangeLayout)
            {
                var layout = new TimerOnlyLayoutFactory().Create(CurrentState);
                layout.Settings = Layout.Settings;
                layout.X = Location.X;
                layout.Y = Location.Y;
                layout.Mode = Layout.Mode;
                SetLayout(layout);
                Settings.AddToRecentLayouts("");
            }
        }

        private void saveAsMenuItem_Click(object sender, EventArgs e)
        {
            SaveSplitsAs(true);
        }

        private void saveSplitsMenuItem_Click(object sender, EventArgs e)
        {
            SaveSplits(true);
        }

        private void editSplitsMenuItem_Click(object sender, EventArgs e)
        {
            EditSplits();
        }

        private void editLayoutMenuItem_Click(object sender, EventArgs e)
        {
            EditLayout();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            OpenAboutBox();
        }

        private void openSplitsFromFileMenuItem_Click(object sender, EventArgs e)
        {
            OpenSplits();
        }

        private void openLayoutFromFileMenuItem_Click(object sender, EventArgs e)
        {
            OpenLayout();
        }

        private void resetLayoutMenuItem_Click(object sender, EventArgs e)
        {
            LoadDefaultLayout();
        }

        private void saveLayoutAsMenuItem_Click(object sender, EventArgs e)
        {
            SaveLayoutAs();
        }

        private void saveLayoutMenuItem_Click(object sender, EventArgs e)
        {
            SaveLayout();
        }

        private bool WarnUserAboutSplitsSave()
        {
            if (InTimerOnlyMode)
            {
                Model.Reset();
                return true;
            }

            bool safeToContinue = true;
            if (CurrentState.Run.HasChanged)
            {
                try
                {
                    DontRedraw = true;
                    var result = MessageBox.Show(this, "Your splits have been updated but not yet saved.\nDo you want to save your splits now?", "Save Splits?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        safeToContinue = SaveSplits(false);
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return false;
                    }
                }
                finally
                {
                    DontRedraw = false;
                }
            }
            if (safeToContinue)
            {
                Model.Reset();
            }
            return safeToContinue;
        }

        private bool WarnUserAboutLayoutSave(bool canCancel)
        {
            bool safeToContinue = true;
            if (Layout.HasChanged)
            {
                try
                {
                    DontRedraw = true;
                    var buttons = canCancel ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo;
                    var result = MessageBox.Show(this, "Your layout has been updated but not yet saved.\nDo you want to save your layout now?", "Save Layout?", buttons, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        safeToContinue = SaveLayout();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return false;
                    }
                }
                finally
                {
                    DontRedraw = false;
                }
            }
            return safeToContinue;
        }

        private void TimerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!WarnUserAboutSplitsSave())
            {
                e.Cancel = true;
                return;
            }
            if (!WarnUserAboutLayoutSave(true))
            {
                e.Cancel = true;
                return;
            }

            Settings.LastComparison = CurrentState.CurrentComparison;
            AddCurrentSplitsToLRU(CurrentState.CurrentTimingMethod, CurrentState.CurrentHotkeyProfile);

            try
            {
                var settingsPath = Path.Combine(BasePath, SETTINGS_PATH);
                if (!File.Exists(settingsPath))
                    File.Create(settingsPath).Close();

                using (var memoryStream = new MemoryStream())
                {
                    SettingsSaver.Save(Settings, memoryStream);

                    using (var stream = File.Open(settingsPath, FileMode.Create, FileAccess.Write))
                    {
                        var buffer = memoryStream.GetBuffer();
                        stream.Write(buffer, 0, (int)memoryStream.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Settings could not be saved!", "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Error(ex);
            }

            foreach (var component in Layout.Components)
                component.Dispose();
            DeactivateAutoSplitter();
            Server.Stop();
        }

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            var editor = new SettingsDialog(Hook, Settings, CurrentState.CurrentHotkeyProfile);
            editor.SumOfBestModeChanged += editor_SumOfBestModeChanged;
            try
            {
                TopMost = false;
                var oldSettings = (ISettings)Settings.Clone();
                Settings.UnregisterAllHotkeys(Hook);
                var result = editor.ShowDialog(this);
                if (result == DialogResult.Cancel)
                {
                    var regenerate = Settings.SimpleSumOfBest != oldSettings.SimpleSumOfBest;
                    CurrentState.Settings = Settings = oldSettings;
                    if (regenerate)
                        RegenerateComparisons();
                }
                else
                {
                    SwitchComparisonGenerators();
                    CurrentState.CurrentHotkeyProfile = editor.SelectedHotkeyProfile;
                }
                Settings.RegisterHotkeys(Hook, CurrentState.CurrentHotkeyProfile);
                UpdateRaceProviderIntegration();
            }
            finally
            {
                SetProgressBar();
                TopMost = Layout.Settings.AlwaysOnTop;
            }
        }

        void editor_SumOfBestModeChanged(object sender, EventArgs e)
        {
            RegenerateComparisons();
        }

        private void LoadSettings()
        {
            try
            {
                using (var stream = File.OpenRead(Path.Combine(BasePath, SETTINGS_PATH)))
                {
                    Settings = new XMLSettingsFactory(stream).Create();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                Settings = new StandardSettingsFactory().Create();
            }
        }

        private void closeSplitsMenuItem_Click(object sender, EventArgs e)
        {
            CloseSplits();
        }

        private void MaintainMinimumSize()
        {
            if (Layout.Mode == LayoutMode.Vertical)
            {
                var minimumWidth = ComponentRenderer.MinimumWidth * (Height / ComponentRenderer.OverallSize);
                if (Width < minimumWidth)
                    Height = (int)(Height / (minimumWidth / Width) + 0.5f);
            }
            else
            {
                var minimumHeight = ComponentRenderer.MinimumHeight * (Width / ComponentRenderer.OverallSize);
                if (Height < minimumHeight)
                    Width = (int)(Width / (minimumHeight / Height) + 0.5f);
            }
        }

        private Image MakeScreenShot(bool transparent = false)
        {
            var image = new Bitmap(Width, Height);
            var graphics = Graphics.FromImage(image);

            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.FillRectangle(Brushes.Transparent, 0, 0, Width, Height);
            graphics.CompositingMode = CompositingMode.SourceOver;

            if (!transparent)
            {
                graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, Width, Height);
                graphics.FillRectangle(new SolidBrush(Layout.Settings.BackgroundColor), 0, 0, Width, Height);
            }

            var drawRegion = new Region(new Rectangle(0, 0, Width, Height));
            UpdateRegion = drawRegion;
            PaintForm(graphics, drawRegion);

            return image;
        }

        private void shareMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TopMost = false;
                IsInDialogMode = true;
                Settings.UnregisterAllHotkeys(Hook);
                new ShareRunDialog(
                    (LiveSplitState)(CurrentState.Clone()),
                    Settings,
                    () => MakeScreenShot(false)).ShowDialog(this);
                Settings.RegisterHotkeys(Hook, CurrentState.CurrentHotkeyProfile);
            }
            finally
            {
                TopMost = Layout.Settings.AlwaysOnTop;
                IsInDialogMode = false;
            }
        }

        private DialogResult WarnAboutResetting()
        {
            var warnUser = false;
            for (var index = 0; index < CurrentState.Run.Count; index++)
            {
                if (LiveSplitStateHelper.CheckBestSegment(CurrentState, index, CurrentState.CurrentTimingMethod))
                {
                    warnUser = true;
                    break;
                }
            }
            if (!warnUser && (CurrentState.Run.Last().SplitTime[CurrentState.CurrentTimingMethod] != null && CurrentState.Run.Last().PersonalBestSplitTime[CurrentState.CurrentTimingMethod] == null) || CurrentState.Run.Last().SplitTime[CurrentState.CurrentTimingMethod] < CurrentState.Run.Last().PersonalBestSplitTime[CurrentState.CurrentTimingMethod])
                warnUser = true;
            if (warnUser)
            {
                DontRedraw = true;
                var result = MessageBox.Show(this, "You have beaten some of your best times.\r\nDo you want to update them?", "Update Times?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                DontRedraw = false;
                return result;
            }
            return DialogResult.Yes;
        }

        private void Reset()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(Reset));
                return;
            }

            if (!ResetMessageShown)
            {
                var result = DialogResult.Yes;
                if (Settings.WarnOnReset && (!InTimerOnlyMode))
                {
                    ResetMessageShown = true;
                    result = WarnAboutResetting();
                }
                if (result == DialogResult.Yes)
                    Model.Reset();
                else if (result == DialogResult.No)
                    Model.Reset(false);
                ResetMessageShown = false;
            }
        }

        private void racingMenuItem_MouseHover(object sender, EventArgs e)
        {
            RaceProviderAPI raceProvider = (RaceProviderAPI)(sender as ToolStripMenuItem)?.Tag;
            raceProvider?.RefreshRacesListAsync();
            ShouldRefreshRaces = true;
        }

        private void racingMenuItem_MouseLeave(object sender, EventArgs e)
        {
            ShouldRefreshRaces = false;
        }

        private void resetMenuItem_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void pauseMenuItem_Click(object sender, EventArgs e)
        {
            Model.Pause();
        }

        private void undoPausesMenuItem_Click(object sender, EventArgs e)
        {
            Model.UndoAllPauses();
        }

        private void hotkeysMenuItem_Click(object sender, EventArgs e)
        {
            var hotkeyProfile = Settings.HotkeyProfiles[CurrentState.CurrentHotkeyProfile];

            if (hotkeysMenuItem.Checked)
                hotkeysMenuItem.Checked = hotkeyProfile.GlobalHotkeysEnabled = false;
            else
                hotkeysMenuItem.Checked = hotkeyProfile.GlobalHotkeysEnabled = true;

            SetProgressBar();
        }

        private void splitMenuItem_Click(object sender, EventArgs e)
        {
            StartOrSplit();
        }

        private void undoSplitMenuItem_Click(object sender, EventArgs e)
        {
            Model.UndoSplit();
        }

        private void skipSplitMenuItem_Click(object sender, EventArgs e)
        {
            Model.SkipSplit();
        }

        private void SetProgressBar()
        {
            try
            {
                var hotkeyProfile = Settings.HotkeyProfiles[CurrentState.CurrentHotkeyProfile];
                if (hotkeyProfile.ToggleGlobalHotkeys != null)
                {
                    TaskbarManager.Instance.SetProgressState(hotkeyProfile.GlobalHotkeysEnabled ? TaskbarProgressBarState.Normal : TaskbarProgressBarState.Error);
                    TaskbarManager.Instance.SetProgressValue(100, 100);
                }
                else
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void TimerForm_Shown(object sender, EventArgs e)
        {
            SetProgressBar();
        }

        private void RebuildComparisonsMenu()
        {
            comparisonMenuItem.DropDownItems.Clear();

            foreach (var customComparison in CurrentState.Run.CustomComparisons)
                AddActionToComparisonsMenu(customComparison);

            if (CurrentState.Run.ComparisonGenerators.Count > 0)
                comparisonMenuItem.DropDownItems.Add(new ToolStripSeparator());

            var raceSeparatorAdded = false;
            foreach (var generator in CurrentState.Run.ComparisonGenerators)
            {
                if (!raceSeparatorAdded && generator is SRLComparisonGenerator)
                {
                    comparisonMenuItem.DropDownItems.Add(new ToolStripSeparator());
                    raceSeparatorAdded = true;
                }
                AddActionToComparisonsMenu(generator.Name);
            }

            comparisonMenuItem.DropDownItems.Add(new ToolStripSeparator());

            var realTimeMenuItem = new ToolStripMenuItem("Real Time");
            realTimeMenuItem.Click += realTimeMenuItem_Click;
            realTimeMenuItem.Name = "RealTime";
            comparisonMenuItem.DropDownItems.Add(realTimeMenuItem);

            var gameTimeMenuItem = new ToolStripMenuItem("Game Time");
            gameTimeMenuItem.Click += gameTimeMenuItem_Click;
            gameTimeMenuItem.Name = "GameTime";
            comparisonMenuItem.DropDownItems.Add(gameTimeMenuItem);

            RefreshComparisonItems();
        }

        void gameTimeMenuItem_Click(object sender, EventArgs e)
        {
            CurrentState.CurrentTimingMethod = TimingMethod.GameTime;
            RefreshComparisonItems();
        }

        void realTimeMenuItem_Click(object sender, EventArgs e)
        {
            CurrentState.CurrentTimingMethod = TimingMethod.RealTime;
            RefreshComparisonItems();
        }

        private void RegenerateComparisons()
        {
            if (CurrentState != null && CurrentState.Run != null)
            {
                foreach (var generator in CurrentState.Run.ComparisonGenerators)
                    generator.Generate(CurrentState.Settings);
            }
        }

        private void SwitchComparisonGenerators()
        {
            var allGenerators = new StandardComparisonGeneratorsFactory().GetAllGenerators(CurrentState.Run);
            foreach (var generator in allGenerators)
            {
                var generatorInRun = CurrentState.Run.ComparisonGenerators.FirstOrDefault(x => x.Name == generator.Name);
                if (generatorInRun != null)
                    CurrentState.Run.ComparisonGenerators.Remove(generatorInRun);

                if (Settings.ComparisonGeneratorStates[generator.Name])
                    CurrentState.Run.ComparisonGenerators.Add(generator);

            }
            SwitchComparison(CurrentState.CurrentComparison);
            RegenerateComparisons();
        }

        private void SwitchComparison(string name)
        {
            if (!CurrentState.Run.Comparisons.Contains(name))
                name = Run.PersonalBestComparisonName;
            CurrentState.CurrentComparison = name;
        }

        private void AddActionToComparisonsMenu(string name)
        {
            var menuItem = new ToolStripMenuItem(name.EscapeMenuItemText());
            menuItem.Click += (s, e) => SwitchComparison(name);
            comparisonMenuItem.DropDownItems.Add(menuItem);
        }

        private void AddActionToControlMenu(string name, Action action)
        {
            var menuItem = new ToolStripMenuItem(name.EscapeMenuItemText());
            menuItem.Click += (s, e) => action();
            controlMenuItem.DropDownItems.Add(menuItem);
        }

        private void RebuildControlMenu()
        {
            controlMenuItem.DropDownItems.Clear();
            controlMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            splitMenuItem,
            resetMenuItem,
            undoSplitMenuItem,
            skipSplitMenuItem,
            pauseMenuItem,
            undoPausesMenuItem});

            controlMenuItem.DropDownItems.Add(new ToolStripSeparator());
            controlMenuItem.DropDownItems.Add(hotkeysMenuItem);

            hotkeysMenuItem.Checked = Settings.HotkeyProfiles[CurrentState.CurrentHotkeyProfile].GlobalHotkeysEnabled;

            var components = Layout.Components;
            if (CurrentState.Run.IsAutoSplitterActive())
                components = components.Concat(new[] { CurrentState.Run.AutoSplitter.Component });

            var componentControls =
                components
                .Select(x => x.ContextMenuControls)
                .Where(x => x != null && x.Any());

            foreach (var componentControlSection in componentControls)
            {
                controlMenuItem.DropDownItems.Add(new ToolStripSeparator());

                foreach (var componentControl in componentControlSection)
                {
                    AddActionToControlMenu(componentControl.Key, componentControl.Value);
                }
            }
        }

        private void RightClickMenu_Opening(object sender, CancelEventArgs e)
        {
            RebuildControlMenu();
            RebuildComparisonsMenu();
        }

        private void TimerForm_ResizeBegin(object sender, EventArgs e)
        {
            if (Size.Height > 0)
                ResizingInitialAspectRatio = (float)Size.Width / Size.Height;
        }

        private void TimerForm_ResizeEnd(object sender, EventArgs e)
        {
            ResizingInitialAspectRatio = null;
        }

        private void TimerForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop, false) is string[] fileList)
            {
                var lssOpened = false;
                var lslOpened = false;

                foreach (string fileToOpen in fileList)
                {
                    if (File.Exists(fileToOpen))
                    {
                        var extension = Path.GetExtension(fileToOpen).ToLower();

                        if (!lslOpened && extension == ".lsl")
                        {
                            lslOpened = true;
                            OpenLayoutFromFile(fileToOpen);
                        }
                        else if (!lssOpened)
                        {
                            lssOpened = true;
                            OpenRunFromFile(fileToOpen);
                        }
                    }
                }
            }
        }

        private void TimerForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }
}
