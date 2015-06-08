using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Model.Input;
using LiveSplit.Model.RunFactories;
using LiveSplit.Model.RunImporters;
using LiveSplit.Model.RunSavers;
using LiveSplit.Options;
using LiveSplit.Options.SettingsFactories;
using LiveSplit.Options.SettingsSavers;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.UI.LayoutFactories;
using LiveSplit.UI.LayoutSavers;
using LiveSplit.Updates;
using LiveSplit.Web.Share;
using LiveSplit.Web.SRL;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
#if WITH_XSPLIT
using XSplit.Wpf;
#endif

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
        public ISettings Settings { get; set; }
        protected ILayout TimerOnlyLayout;
        protected IRun TimerOnlyRun { get; set; }
        protected Invalidator Invalidator { get; set; }
        protected bool InTimerOnlyMode { get; set; }

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

        protected bool MouseIsDown = false;
        protected Point MousePoint;

        protected Task RefreshTask { get; set; }
        protected int RefreshCounter { get; set; }

        public string BasePath { get; set; }

        public Bitmap BackBuffer { get; set; }
        public object BackBufferLock = new object();
        public bool DrawToBackBuffer { get; set; }

#if WITH_XSPLIT
        public TimedBroadcasterPlugin XSplit { get; set; }
#endif

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        static extern int GetUpdateRgn(IntPtr hWnd, IntPtr hRgn, bool bErase);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        /*protected override CreateParams CreateParams
        {
            get
            {
                // Add the layered extended style (WS_EX_LAYERED) to this window.
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= Win32.WS_EX_LAYERED;
                return createParams;
            }
        }*/
        
        public TimerForm(string splitsPath = null, string layoutPath = null, bool drawToBackBuffer = false, string basePath = "")
        {
            BasePath = basePath;
            InitializeComponent();
            Init(splitsPath, layoutPath);
            DrawToBackBuffer = drawToBackBuffer;
        }

        private void Init(string splitsPath = null, string layoutPath = null)
        {
            GlobalCache = new GraphicsCache();
            Invalidator = new Invalidator(this);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            ComponentManager.BasePath = BasePath;

            SpeedRunsLiveAPI.Instance.RacesRefreshed += SRL_RacesRefreshed;
            SpeedRunsLiveAPI.Instance.RefreshRacesListAsync();

            CurrentState = new LiveSplitState(null, this, null, null, null);

            ComparisonGeneratorsFactory = new StandardComparisonGeneratorsFactory();

            TimerOnlyLayout = new TimerOnlyLayoutFactory().Create(CurrentState);
            TimerOnlyRun = new StandardRunFactory().Create(ComparisonGeneratorsFactory);
            InTimerOnlyMode = false;

            Model = new DoubleTapPrevention(new TimerModel());
            //LiveSplit.Web.Share.Twitch.Instance.AutoUpdateModel = Model;

            RunFactory = new StandardFormatsRunFactory();
            RunSaver = new XMLRunSaver();
            LayoutSaver = new XMLLayoutSaver();
            SettingsSaver = new XMLSettingsSaver();
            LoadSettings();

            UpdateRecentSplits();
            UpdateRecentLayouts();

            IRun run = TimerOnlyRun;
            try
            {
                if (!string.IsNullOrEmpty(splitsPath))
                {
                    run = LoadRunFromFile(splitsPath, true);
                }
                else
                {
                    if (Settings.RecentSplits.Count > 0)
                        run = LoadRunFromFile(Settings.RecentSplits.Last(), true);
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
                    else if (run == TimerOnlyRun)
                    {
                        Layout = TimerOnlyLayout;
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

            CurrentState.CurrentTimingMethod = Settings.LastTimingMethod;

            RegenerateComparisons();
            SwitchComparison(Settings.LastComparison);
            Model.CurrentState = CurrentState;

            CurrentState.OnReset += CurrentState_OnReset;
            CurrentState.OnStart += CurrentState_OnStart;
            CurrentState.OnSplit += CurrentState_OnSplit;
            CurrentState.OnSkipSplit += CurrentState_OnSkipSplit;
            CurrentState.OnUndoSplit += CurrentState_OnUndoSplit;
            CurrentState.OnPause += CurrentState_OnPause;
            CurrentState.OnResume += CurrentState_OnResume;
            CurrentState.OnSwitchComparisonPrevious += CurrentState_OnSwitchComparisonPrevious;
            CurrentState.OnSwitchComparisonNext += CurrentState_OnSwitchComparisonNext;

            ComponentRenderer = new ComponentRenderer();

            StartPosition = FormStartPosition.Manual;

            SetLayout(Layout);

            OldSize = -20;

            RefreshTask = Task.Factory.StartNew(RefreshTimerWorker);

            RefreshCounter = 0;

            Hook = new CompositeHook();
            Hook.KeyOrButtonPressed += hook_KeyOrButtonPressed;
            Settings.RegisterHotkeys(Hook);

            RegisterTaskbarButtons();

            SizeChanged += TimerForm_SizeChanged;

            lock (BackBufferLock)
            {
                BackBuffer = new Bitmap(Width, Height);
#if WITH_XSPLIT
                /*try
                {
                    // Outputs a CosmoWright image every 50ms (20 FPS)
                    XSplit = TimedBroadcasterPlugin.CreateInstance(
                        "livesplit", BackBuffer, 50);

                    if (this.XSplit != null)
                    {
                        // The correct version of XSplit was installed (unless they use OBS), so we can start our output.
                        this.XSplit.StartTimer();
                    }
                }
                catch
                { }*/
#endif
            }

            TopMost = Layout.Settings.AlwaysOnTop;
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
                        toolItem.Checked = toolItem.Text == CurrentState.CurrentComparison;
                }
            }
        }

        void CurrentState_OnSwitchComparisonPrevious(object sender, EventArgs e)
        {
            RefreshComparisonItems();
        }

        void SRL_RacesRefreshed(object sender, EventArgs e)
        {
            Action<ToolStripItem> addItem = null;
            Action clear = null;

            addItem = x =>
            {
                if (InvokeRequired)
                {
                    Invoke(addItem, x);
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
                else
                {
                    racingMenuItem.DropDownItems.Clear();
                }
            };

            clear();
            foreach (var race in SpeedRunsLiveAPI.Instance.GetRaces())
            {
                if (race.state != 1)
                    continue;

                var game = race.game.name;
                var goal = race.goal;
                var entrants = race.numentrants;
                var plural = entrants == 1 ? "" : "s";
                var title = string.Format("{0} - {1} ({2} Entrant{3})", game, goal, entrants, plural);
                var item = new ToolStripMenuItem();
                item.Text = title;
                item.Tag = race.id;
                item.Click += Race_Click;
                addItem(item);

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var image = SpeedRunsLiveAPI.Instance.GetGameImage(race.game.abbrev);
                        Action setImage = () =>
                            {
                                try
                                {
                                    item.Image = image;
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex);
                                }
                            };
                        if (InvokeRequired)
                            Invoke(setImage);
                        else
                            setImage();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                });
            }

            if (racingMenuItem.DropDownItems.Count > 0)
                addItem(new ToolStripSeparator());

            foreach (var race in SpeedRunsLiveAPI.Instance.GetRaces())
            {
                if (race.state != 3)
                    continue;

                var game = race.game.name;
                var goal = race.goal;
                var entrants = race.numentrants;
                var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                startTime = startTime.AddSeconds(race.time);

                var finishedCount = 0;
                var forfeitedCount = 0;
                foreach (var entrant in race.entrants.Properties.Values)
                {
                    if (entrant.time >= 0)
                        finishedCount++;
                    if (entrant.statetext == "Forfeit")
                        forfeitedCount++;
                }

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
                                var timeSpan = DateTime.UtcNow.ToUniversalTime() - startTime;
                                if (timeSpan < TimeSpan.Zero)
                                    timeSpan = TimeSpan.Zero;
                                var time = new RegularTimeFormatter().Format(timeSpan);
                                var title = string.Format("[{0}] {1} - {2} ({3} / {4} Finished)", time, game, goal, finishedCount, entrants - forfeitedCount);
                                title = title.Replace("&", "&&");
                                tsItem.Text = title;
                            }
                            catch { }
                        }
                    };

                Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var image = SpeedRunsLiveAPI.Instance.GetGameImage(race.game.abbrev);
                            Action setImage = () =>
                            {
                                try
                                {
                                    tsItem.Image = image;
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex);
                                }
                            };
                            if (InvokeRequired)
                                Invoke(setImage);
                            else
                                setImage();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    });

                updateTitleAction();

                new System.Timers.Timer(500) { Enabled = true }.Elapsed += (s, ev) =>
                    {
                        updateTitleAction();
                    };
                tsItem.Click += (s, ev) =>
                    {
                        ShareSettings.Default.Reload();
                        var username = ShareSettings.Default.SRLIRCUsername;
                        var racers = ((IEnumerable<string>)race.entrants.Properties.Keys).Select(x => x.ToLower());
                        if (!racers.Contains((username ?? "").ToLower()))
                            Settings.RaceViewer.ShowRace(race);
                        else
                        {
                            tsItem.Tag = race.id;
                            Race_Click(tsItem, null);
                        }
                    };
                addItem(tsItem);
            }

            if (racingMenuItem.DropDownItems.Count > 0 && !(racingMenuItem.DropDownItems[racingMenuItem.DropDownItems.Count - 1] is ToolStripSeparator))
                addItem(new ToolStripSeparator());

            var newRaceItem = new ToolStripMenuItem();
            newRaceItem.Text = "New Race...";
            newRaceItem.Click += NewRace_Click;
            addItem(newRaceItem);
        }

        void Race_Click(object sender, EventArgs e)
        {
            if (ShowSRLRules())
            {
                var raceId = (sender as ToolStripMenuItem).Tag.ToString();
                var form = new SpeedRunsLiveForm(CurrentState, Model, raceId);
                TopMost = false;
                form.Show(this);
                TopMost = CurrentState.LayoutSettings.AlwaysOnTop;
            }
        }

        void NewRace_Click(object sender, EventArgs e)
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
                    var form = new SpeedRunsLiveForm(CurrentState, Model, gameName, id, gameCategory);
                    form.Show(this);
                }
                TopMost = CurrentState.LayoutSettings.AlwaysOnTop;
            }
        }

        void TimerForm_SizeChanged(object sender, EventArgs e)
        {
            if (OldSize > 0)
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
            }
            FixSize();
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
                            .ToArray());
        }

        void CurrentState_OnUndoSplit(object sender, EventArgs e)
        {
            Action x = () =>
            {
                pauseMenuItem.Enabled = true;
                splitMenuItem.Enabled = true;
                if (CurrentState.CurrentSplitIndex == 0)
                    undoSplitMenuItem.Enabled = false;
                if (CurrentState.CurrentSplitIndex < CurrentState.Run.Count - 1)
                    skipSplitMenuItem.Enabled = true;
            };

            if (InvokeRequired)
                Invoke(x);
            else
                x();
        }

        void CurrentState_OnSkipSplit(object sender, EventArgs e)
        {
            Action x = () =>
            {
                if (CurrentState.CurrentSplitIndex >= CurrentState.Run.Count - 1)
                    skipSplitMenuItem.Enabled = false;
                undoSplitMenuItem.Enabled = true;
            };

            if (InvokeRequired)
                Invoke(x);
            else
                x();
        }

        void CurrentState_OnSplit(object sender, EventArgs e)
        {
            Action x = () =>
            {
                if (CurrentState.CurrentSplitIndex == CurrentState.Run.Count)
                {
                    pauseMenuItem.Enabled = false;
                    splitMenuItem.Enabled = false;
                }
                if (CurrentState.CurrentSplitIndex >= CurrentState.Run.Count - 1)
                    skipSplitMenuItem.Enabled = false;
                undoSplitMenuItem.Enabled = true;
            };

            if (InvokeRequired)
                Invoke(x);
            else
                x();
        }

        void CurrentState_OnStart(object sender, EventArgs e)
        {
            Action x = () =>
                {
                    pauseMenuItem.Enabled = true;
                    resetMenuItem.Enabled = true;
                    undoSplitMenuItem.Enabled = false;
                    skipSplitMenuItem.Enabled = true;
                    splitMenuItem.Text = "Split";
                };

            if (InvokeRequired)
                Invoke(x);
            else
                x();
        }

        void CurrentState_OnReset(object sender, TimerPhase e)
        {
            RegenerateComparisons();
            Action x = () =>
            {
                if (InTimerOnlyMode)
                {
                    var offset = CurrentState.Run.Offset;
                    TimerOnlyRun = new StandardRunFactory().Create(ComparisonGeneratorsFactory);
                    TimerOnlyRun.Offset = offset;
                    var run = TimerOnlyRun;

                    SetRun(run);
                }
                resetMenuItem.Enabled = false;
                pauseMenuItem.Enabled = false;
                undoSplitMenuItem.Enabled = false;
                skipSplitMenuItem.Enabled = false;
                splitMenuItem.Enabled = true;
                splitMenuItem.Text = "Start";
            };

            if (InvokeRequired)
                Invoke(x);
            else
                x();
        }

        void CurrentState_OnResume(object sender, EventArgs e)
        {
            Action x = () =>
            {
                splitMenuItem.Text = "Split";
                pauseMenuItem.Enabled = true;
            };

            if (InvokeRequired)
                Invoke(x);
            else
                x();
        }

        void CurrentState_OnPause(object sender, EventArgs e)
        {
            Action x = () =>
            {
                splitMenuItem.Text = "Resume";
                pauseMenuItem.Enabled = false;
            };

            if (InvokeRequired)
                Invoke(x);
            else
                x();
        }

        private void RegisterTaskbarButtons()
        {
            return; //TODO Crashes with plugin when using OBS twice.
            /*if (TaskbarManager.IsPlatformSupported)
            {
                var taskbarSplitButton = new ThumbnailToolBarButton(Properties.Resources.Split, "Split");
                taskbarSplitButton.Click += (s, e) => { StartOrSplit(); };
                var taskbarSkipSplitButton = new ThumbnailToolBarButton(Properties.Resources.SkipSplit, "Skip Split");
                taskbarSkipSplitButton.Click += (s, e) => { Model.SkipSplit(); };
                var taskbarUnsplitButton = new ThumbnailToolBarButton(Properties.Resources.Unsplit, "Undo Split");
                taskbarUnsplitButton.Click += (s, e) => { Model.UndoSplit(); };
                var taskbarStopButton = new ThumbnailToolBarButton(Properties.Resources.Stop, "Reset Run");
                taskbarStopButton.Click += (s, e) => { Model.Reset(); };
                TaskbarManager.Instance.ThumbnailToolBars.AddButtons(this.Handle, taskbarSplitButton, taskbarSkipSplitButton, taskbarUnsplitButton, taskbarStopButton);
            }*/
        }

        private void UnregisterTaskbarButtons()
        {
            //TODO idk how to do this O.o
        }

        private void AddFileToLRU(string filePath)
        {
            Settings.AddToRecentSplits(filePath);
            UpdateRecentSplits();
        }

        protected void SetInTimerOnlyMode()
        {
            if (Layout.Components.Count() != 1 || Layout.Components.FirstOrDefault().ComponentName != "Timer")
                InTimerOnlyMode = false;
        }

        private void UpdateRecentSplits()
        {
            openSplitsMenuItem.DropDownItems.Clear();
            
            foreach (var item in Settings.RecentSplits.Reverse().Where(x => !string.IsNullOrEmpty(x)))
            {
                var menuItem = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(item));
                menuItem.Click += (x, y) => { OpenRunFromFile(item); };
                openSplitsMenuItem.DropDownItems.Add(menuItem);
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
            var clearSplitHistoryMenuItem = new ToolStripMenuItem("Clear History");
            clearSplitHistoryMenuItem.Click += clearSplitHistoryMenuItem_Click;
            openSplitsMenuItem.DropDownItems.Add(clearSplitHistoryMenuItem);
        }

        void openFromSpeedrunComMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TopMost = false;
                IsInDialogMode = true;

                var runImporter = new SpeedrunComRunImporter();
                var run = runImporter.Import(this);

                if (!WarnUserAboutSplitsSave())
                    return;

                if (run != null)
                {
                    if (InTimerOnlyMode)
                        RemoveTimerOnly();
                    run.HasChanged = true;
                    SetRun(run);
                }
            }
            finally
            {
                TopMost = Layout.Settings.AlwaysOnTop;
                IsInDialogMode = false;
            }
        }

        void clearSplitHistoryMenuItem_Click(object sender, EventArgs e)
        {
            Settings.RecentSplits.Clear();
            UpdateRecentSplits();
        }

        private void UpdateRecentLayouts()
        {
            openLayoutMenuItem.DropDownItems.Clear();
            
            foreach (var item in Settings.RecentLayouts.Reverse().Where(x => !string.IsNullOrEmpty(x)))
            {
                var menuItem = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(item));
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
            var clearLayoutHistoryMenuItem = new ToolStripMenuItem("Clear History");
            clearLayoutHistoryMenuItem.Click += clearLayoutHistoryMenuItem_Click;
            openLayoutMenuItem.DropDownItems.Add(clearLayoutHistoryMenuItem);
        }

        void clearLayoutHistoryMenuItem_Click(object sender, EventArgs e)
        {
            Settings.RecentLayouts.Clear();
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
                        using (var stream = request.GetResponse().GetResponseStream())
                        {
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
                                    MessageBox.Show(this, "The selected file was not recognized as a layout file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    DontRedraw = false;
                                }
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

            if (!WarnUserAboutSplitsSave())
                return;

            if (run != null)
            {
                if (InTimerOnlyMode)
                    RemoveTimerOnly();
                run.HasChanged = true;
                SetRun(run);
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
                if ((ActiveForm == this || Settings.GlobalHotkeysEnabled) && !ResetMessageShown && !IsInDialogMode)
                {
                    if (Settings.SplitKey == e)
                    {
                        if (Settings.HotkeyDelay > 0)
                        {
                            var splitTimer = new System.Timers.Timer(Settings.HotkeyDelay * 1000f);
                            splitTimer.Enabled = true;
                            splitTimer.Elapsed += splitTimer_Elapsed;
                        }
                        else
                            StartOrSplit();
                    }

                    else if (Settings.UndoKey == e)
                    {
                        Model.UndoSplit();
                    }

                    else if (Settings.SkipKey == e)
                    {
                        Model.SkipSplit();
                    }

                    else if (Settings.ResetKey == e)
                    {
                        Reset();
                    }

                    else if (Settings.PauseKey == e)
                    {
                        if (Settings.HotkeyDelay > 0)
                        {
                            var pauseTimer = new System.Timers.Timer(Settings.HotkeyDelay * 1000f);
                            pauseTimer.Enabled = true;
                            pauseTimer.Elapsed += pauseTimer_Elapsed;
                        }
                        else
                            Model.Pause();
                    }

                    else if (Settings.SwitchComparisonPrevious == e)
                        Model.SwitchComparisonPrevious();

                    else if (Settings.SwitchComparisonNext == e)
                        Model.SwitchComparisonNext();
                }

                if (ActiveForm == this && !ResetMessageShown && !IsInDialogMode)
                {
                    if (Settings.ScrollUp == e)
                        Model.ScrollUp();

                    else if (Settings.ScrollDown == e)
                        Model.ScrollDown();
                }

                if (Settings.ToggleGlobalHotkeys == e)
                {
                    Settings.GlobalHotkeysEnabled = !Settings.GlobalHotkeysEnabled;
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
            Model.Pause();
            ((System.Timers.Timer)sender).Stop();
        }

        void splitTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            StartOrSplit();
            ((System.Timers.Timer)sender).Stop();
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
                Action timerAction = () =>
                {
                    try
                    {
                        if (Hook != null)
                            Hook.Poll();

                        if (CurrentState.Run.IsAutoSplitterActive())
                            CurrentState.Run.AutoSplitter.Component.Update(null, CurrentState, 0, 0, Layout.Mode);

                        if (DontRedraw)
                            return;

                        if (OldSize <= 0 || (RefreshCounter > 0 && RefreshCounter % 5 == 0))
                        {
                            InvalidateForm();
                            if (RefreshCounter > 0)
                                RefreshCounter--;
                        }
                        else
                        {
                            GlobalCache.Restart();
                            GlobalCache["Layout"] = new XMLLayoutSaver().GetLayoutNode(new XmlDocument(), Layout).OuterXml;

                            if (GlobalCache.HasChanged || OldSize <= 0)
                                InvalidateForm();
                            else
                            {
                                Invalidator.Restart();
                                Action x = () =>
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
                                };

                                if (InvokeRequired)
                                    Invoke(x);
                                else
                                    x();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        Invalidate();
                    }
                };

                if (InvokeRequired)
                    Invoke(timerAction);
                else
                    timerAction();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Invalidate();
            }
        }

        protected void InvalidateForm()
        {
            Action x = () =>
            {
                try
                {
                    UpdateAllComponents();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            };

            if (InvokeRequired)
                Invoke(x);
            else
                x();

            Invalidate();
        }

        protected void UpdateAllComponents()
        {
            foreach (var component in Layout.Components)
                component.Update(null, CurrentState, Width, Height, Layout.Mode);
        }

        private void PaintForm(Graphics g, Region clip)
        {
            if (CurrentState.LayoutSettings.BackgroundColor != Color.Transparent
                || CurrentState.LayoutSettings.BackgroundGradient != GradientType.Plain
                && CurrentState.LayoutSettings.BackgroundColor2 != Color.Transparent)
            {
                var gradientBrush = new LinearGradientBrush(
                            new PointF(0, 0),
                            CurrentState.LayoutSettings.BackgroundGradient == GradientType.Horizontal
                            ? new PointF(Size.Width, 0)
                            : new PointF(0, Size.Height),
                            CurrentState.LayoutSettings.BackgroundColor,
                            CurrentState.LayoutSettings.BackgroundGradient == GradientType.Plain
                            ? CurrentState.LayoutSettings.BackgroundColor
                            : CurrentState.LayoutSettings.BackgroundColor2);
                g.FillRectangle(gradientBrush, 0, 0, Size.Width, Size.Height);
            }

            Opacity = Layout.Settings.Opacity;

            if (Layout.Settings.AntiAliasing)
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
            else
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.GammaCorrected;
            g.InterpolationMode = InterpolationMode.Bilinear;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var scaleFactor = Layout.Mode == LayoutMode.Vertical
                ? Height / Math.Max(ComponentRenderer.OverallHeight, 1f)
                : Width / Math.Max(ComponentRenderer.OverallWidth, 1f);

            g.ResetTransform();
            g.ScaleTransform(scaleFactor, scaleFactor);
            float transformedWidth = Width;
            float transformedHeight = Height;

            if (Layout.Mode == LayoutMode.Vertical)
                transformedWidth /= scaleFactor;
            else
                transformedHeight /= scaleFactor;

            BackColor = Color.Black;

            if (!clip.GetBounds(g).Equals(UpdateRegion.GetBounds(g)))
                UpdateRegion.Union(clip);

            ComponentRenderer.Render(g, CurrentState, transformedWidth, transformedHeight, Layout.Mode, UpdateRegion);
                
            var currentSize = Layout.Mode == LayoutMode.Vertical ? ComponentRenderer.OverallHeight : ComponentRenderer.OverallWidth;

            if (OldSize >= 0)
            {
                if (OldSize != currentSize)
                {
                    MinimumSize = new Size(0, 0);
                    if (Layout.Mode == LayoutMode.Vertical)
                        Height = (int)((currentSize / (double)OldSize) * Height + 0.5);
                    else
                        Width = (int)((currentSize / (double)OldSize) * Width + 0.5);
                }
                FixSize();
                if (Layout.Mode == LayoutMode.Vertical)
                    MinimumSize = new Size(100, (int)((ComponentRenderer.OverallHeight / 3) + 0.5f));
                else
                    MinimumSize = new Size((int)((ComponentRenderer.OverallWidth / 3) + 0.5f), 25);
            }
            else
            {
                if (Layout.Mode == LayoutMode.Vertical)
                    Size = new Size(Layout.VerticalWidth, Layout.VerticalHeight);
                else
                    Size = new Size(Layout.HorizontalWidth, Layout.HorizontalHeight);
                OldSize++;
            }

            if (OldSize == 0)
                RefreshCounter = 50;

            if (OldSize >= 0)
                OldSize = currentSize;
        }

        Random rng = new Random();

        private void TimerForm_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (DrawToBackBuffer)
                {
                    lock (BackBufferLock)
                    {
                        if (BackBuffer == null || BackBuffer.Size != Size)
                            BackBuffer = new Bitmap(Width, Height);
                        var graphics = Graphics.FromImage(BackBuffer);

                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        //graphics.Clip = e.Graphics.Clip;
                        graphics.FillRectangle(Brushes.Transparent, 0, 0, BackBuffer.Width, BackBuffer.Height);
                        graphics.CompositingMode = CompositingMode.SourceOver;

                        //var clip = e.Graphics.Clip;
                        graphics.Clip = new Region();
                        PaintForm(graphics, graphics.Clip);

                        var graphicsForm = e.Graphics;
                        graphicsForm.CompositingQuality = CompositingQuality.GammaCorrected;

                        graphicsForm.DrawImage(BackBuffer, 0, 0);
                        //SelectBitmap(BackBuffer);
                    }
                }
                else
                {
                    var clip = e.Graphics.Clip;
                    e.Graphics.Clip = new Region();
                    PaintForm(e.Graphics, clip);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        public void SelectBitmap(Bitmap bitmap)
        {
            SelectBitmap(bitmap, 255);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap">
        /// 
        /// </param>
        /// <param name="opacity">
        /// Specifies an alpha transparency value to be used on the entire source 
        /// bitmap. The SourceConstantAlpha value is combined with any per-pixel 
        /// alpha values in the source bitmap. The value ranges from 0 to 255. If 
        /// you set SourceConstantAlpha to 0, it is assumed that your image is 
        /// transparent. When you only want to use per-pixel alpha values, set 
        /// the SourceConstantAlpha value to 255 (opaque).
        /// </param>
        public void SelectBitmap(Bitmap bitmap, int opacity)
        {
            // Does this bitmap contain an alpha channel?
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ApplicationException("The bitmap must be 32bpp with alpha-channel.");
            }

            // Get device contexts
            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr hOldBitmap = IntPtr.Zero;

            try
            {
                // Get handle to the new bitmap and select it into the current 
                // device context.
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                hOldBitmap = Win32.SelectObject(memDc, hBitmap);

                // Set parameters for layered window update.
                Win32.Size newSize = new Win32.Size(bitmap.Width, bitmap.Height);
                Win32.Point sourceLocation = new Win32.Point(0, 0);
                Win32.Point newLocation = new Win32.Point(Left, Top);
                Win32.BLENDFUNCTION blend = new Win32.BLENDFUNCTION();
                blend.BlendOp = Win32.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = (byte)opacity;
                blend.AlphaFormat = Win32.AC_SRC_ALPHA;

                // Update the window.
                Win32.UpdateLayeredWindow(
                    Handle,     // Handle to the layered window
                    screenDc,        // Handle to the screen DC
                    ref newLocation, // New screen position of the layered window
                    ref newSize,     // New size of the layered window
                    memDc,           // Handle to the layered window surface DC
                    ref sourceLocation, // Location of the layer in the DC
                    0,               // Color key of the layered window
                    ref blend,       // Transparency of the layered window
                    Win32.ULW_ALPHA        // Use blend as the blend function
                    );
            }
            finally
            {
                // Release device context.
                Win32.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, hOldBitmap);
                    Win32.DeleteObject(hBitmap);
                }
                Win32.DeleteDC(memDc);
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
                this.Location = new Point(x, y);
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
            run.ComparisonGenerators = new List<IComparisonGenerator>(CurrentState.Run.ComparisonGenerators);
            foreach (var generator in run.ComparisonGenerators)
                generator.Run = run;
            run.FixSplits();
            var autoSplitterChanged = run.AutoSplitter != CurrentState.Run.AutoSplitter;
            if (autoSplitterChanged)
                DeactivateAutoSplitter();
            CurrentState.Run = run;
            RefreshCounter = 50;
            RegenerateComparisons();
            SwitchComparison(CurrentState.CurrentComparison);
            if (autoSplitterChanged)
                CreateAutoSplitter();
        }

        private void CreateAutoSplitter()
        {
            var splitter = AutoSplitterFactory.Instance.Create(CurrentState.Run.GameName);
            CurrentState.Run.AutoSplitter = splitter;
            if (splitter != null && CurrentState.Settings.ActiveAutoSplitters.Contains(CurrentState.Run.GameName))
            {
                splitter.Activate(CurrentState);
                if (splitter.Component != null
                    && CurrentState.Run.AutoSplitterSettings != null
                    && !CurrentState.Run.AutoSplitterSettings.IsEmpty
                    && CurrentState.Run.AutoSplitterSettings.Attributes["gameName"].InnerText == CurrentState.Run.GameName)
                    CurrentState.Run.AutoSplitter.Component.SetSettings(CurrentState.Run.AutoSplitterSettings);
            }
        }

        private void DeactivateAutoSplitter()
        {
            if (CurrentState.Run.AutoSplitter != null)
                CurrentState.Run.AutoSplitter.Deactivate();
        }

        private IRun LoadRunFromFile(string filePath, bool addToRecent)
        {
            IRun run;

            using (var stream = File.OpenRead(filePath))
            {
                RunFactory.Stream = stream;
                RunFactory.FilePath = filePath;

                run = RunFactory.Create(ComparisonGeneratorsFactory);
            }

            if (addToRecent)
                AddFileToLRU(filePath);
            RegenerateComparisons();
            if (InTimerOnlyMode)
                RemoveTimerOnly();
            return run;
        }

        private ILayout LoadLayoutFromFile(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                var layout = new XMLLayoutFactory(stream).Create(CurrentState);
                layout.FilePath = filePath;
                Settings.AddToRecentLayouts(filePath);
                UpdateRecentLayouts();
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

                var run = LoadRunFromFile(filePath, true);
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

        private void OpenSplits()
        {
            var splitDialog = new OpenFileDialog();
            IsInDialogMode = true;
            try
            {
                if (Settings.RecentSplits.Any() && !string.IsNullOrEmpty(Settings.RecentSplits.Last()))
                    splitDialog.InitialDirectory = Path.GetDirectoryName(Settings.RecentSplits.Last());
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

        private void SaveSplitsAs(bool promptPBMessage)
        {
            var splitDialog = new SaveFileDialog();
            splitDialog.Filter = "LiveSplit Splits (*.lss)|*.lss|All Files (*.*)|*.*";
            try
            {
                var result = splitDialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    CurrentState.Run.FilePath = splitDialog.FileName;
                    SaveSplits(promptPBMessage);
                }
            }
            finally
            {

            }
        }

        private void SaveSplits(bool promptPBMessage)
        {
            var savePath = CurrentState.Run.FilePath;

            if (savePath == null)
            {
                SaveSplitsAs(promptPBMessage);
                return;
            }

            CurrentState.Run.FixSplits();

            var stateCopy = CurrentState.Clone() as LiveSplitState;
            var modelCopy = new TimerModel();
            modelCopy.CurrentState = stateCopy;
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
                    Model.Reset();
                    modelCopy.SetRunAsPB();
                    modelCopy.UpdateAttemptHistory();
                    modelCopy.UpdateBestSegments();
                    modelCopy.UpdateSegmentHistory();
                    SetRun(stateCopy.Run);
                }
                else if (result == DialogResult.Cancel)
                    return;
            }

            if (result == DialogResult.Yes)
                modelCopy.Reset(false);
            else
                modelCopy.Reset();

            try
            {
                if (!File.Exists(savePath))
                    File.Create(savePath).Close();
                using (var stream = File.Open(savePath, FileMode.Create, FileAccess.Write))
                {
                    RunSaver.Save(stateCopy.Run, stream);
                    CurrentState.Run.HasChanged = false;
                }
                Settings.AddToRecentSplits(savePath);
                UpdateRecentSplits();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Save Failed", "Could Not Save File!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Error(ex);
            }
        }

        private void SaveLayout()
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
                SaveLayoutAs();
                return;
            }

            try
            {
                if (!File.Exists(savePath))
                    File.Create(savePath).Close();
                using (var stream = File.Open(savePath, FileMode.Create, FileAccess.Write))
                {
                    LayoutSaver.Save(Layout, stream);
                    Layout.HasChanged = false;
                }
                Settings.AddToRecentLayouts(savePath);
                UpdateRecentLayouts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Save Failed", "Could Not Save File!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Error(ex);
            }
        }

        private void EditSplits()
        {
            var runCopy = CurrentState.Run.Clone() as IRun;
            var autoSplitterSettings = CurrentState.Run.IsAutoSplitterActive()
                ? CurrentState.Run.AutoSplitter.Component.GetSettings(new XmlDocument()) 
                : null;
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
                    if (CurrentState.Run.IsAutoSplitterActive() && !CurrentState.Settings.ActiveAutoSplitters.Contains(CurrentState.Run.GameName))
                        CurrentState.Settings.ActiveAutoSplitters.Add(CurrentState.Run.GameName);
                    else if (!CurrentState.Run.IsAutoSplitterActive() && CurrentState.Settings.ActiveAutoSplitters.Contains(CurrentState.Run.GameName))
                        CurrentState.Settings.ActiveAutoSplitters.Remove(CurrentState.Run.GameName);
                    SetRun(runCopy);
                    CurrentState.CallRunManuallyModified();
                    if (CurrentState.Run.IsAutoSplitterActive())
                        CurrentState.Run.AutoSplitter.Component.SetSettings(autoSplitterSettings);
                }
            }
            finally
            {
                TopMost = Layout.Settings.AlwaysOnTop;
                IsInDialogMode = false;
            }
        }

        void editor_SegmentRemovedOrAdded(object sender, EventArgs e)
        {
            RefreshCounter = 50;
        }

        void editor_ComparisonRenamed(object sender, EventArgs e)
        {
            CurrentState.CallComparisonRenamed(e);
        }

        void editor_RunEdited(object sender, EventArgs e)
        {
            RegenerateComparisons();
            CurrentState.CallRunManuallyModified();
            if (InTimerOnlyMode)
                RemoveTimerOnly();
        }

        protected void RemoveTimerOnly()
        {
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
                    var enumerator = componentSettings.GetEnumerator();
                    foreach (var component in layoutCopy.Components)
                    {
                        enumerator.MoveNext();
                        if (enumerator.Current != null)
                            component.SetSettings(enumerator.Current);
                    }
                    SetLayout(layoutCopy);
                }
                foreach (var component in editor.ComponentsToDispose)
                    component.Dispose();
            }
            finally
            {
                TopMost = Layout.Settings.AlwaysOnTop;
            }
        }

        void editor_LayoutSettingsAssigned(object sender, EventArgs e)
        {
            RefreshCounter = 50;
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
            if (Layout.Mode == LayoutMode.Vertical)
            {
                Layout.HorizontalWidth = Size.Width;
                Layout.HorizontalHeight = Size.Height;
                if (Layout.VerticalHeight == UI.Layout.InvalidSize || Layout.VerticalWidth == UI.Layout.InvalidSize)
                {
                    Layout.VerticalWidth = 300;
                    Layout.VerticalHeight = (int)(ComponentRenderer.OverallHeight + 0.5);
                }
            }
            else
            {
                Layout.VerticalWidth = Size.Width;
                Layout.VerticalHeight = Size.Height;
                if (Layout.HorizontalWidth == UI.Layout.InvalidSize || Layout.HorizontalHeight == UI.Layout.InvalidSize)
                {
                    Layout.HorizontalWidth = (int)(ComponentRenderer.OverallWidth + 0.5);
                    Layout.HorizontalHeight = 45;
                }
            }
            TopMost = false;
            SetLayout(Layout);
        }

        private void SaveLayoutAs()
        {
            var layoutDialog = new SaveFileDialog();
            layoutDialog.Filter = "LiveSplit Layout (*.lsl)|*.lsl|All Files (*.*)|*.*";
            try
            {
                var result = layoutDialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    Layout.FilePath = layoutDialog.FileName;
                    SaveLayout();
                }
            }
            finally
            {
            }
        }
        private void OpenAboutBox()
        {
            var aboutBox = new AboutBox();
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

        private void OpenLayout()
        {
            var layoutDialog = new OpenFileDialog();
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

        private void OpenLayoutFromFile (string filePath)
        {
            if (WarnUserAboutLayoutSave())
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
                    MessageBox.Show(this, "The selected file was not recognized as a layout file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DontRedraw = false;
                }
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void LoadDefaultLayout()
        {
            if (WarnUserAboutLayoutSave())
            {
                var layoutFactory = new StandardLayoutFactory();
                var layout = layoutFactory.Create(CurrentState);
                layout.X = Location.X;
                layout.Y = Location.Y;
                SetLayout(layout);
                Settings.AddToRecentLayouts("");
            }
        }

        private void SetLayout(ILayout layout)
        {
            lock (BackBufferLock)
            {
                if (Layout != null && Layout != layout)
                {
                    foreach (var component in Layout.Components.Except(layout.Components))
                        component.Dispose();

                    foreach (var component in layout.Components.Except(Layout.Components).OfType<IDeactivatableComponent>())
                        component.Activated = true;
                }
                Layout = layout;
                ComponentRenderer.VisibleComponents = Layout.Components;
                CurrentState.LayoutSettings = layout.Settings;
                OldSize = -5;
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
        }

        private void CloseSplits()
        {
            if (!WarnUserAboutSplitsSave())
                return;
            if (!WarnUserAboutLayoutSave())
                return;
            TimerOnlyRun = new StandardRunFactory().Create(ComparisonGeneratorsFactory);
            var run = TimerOnlyRun;
            Model.Reset();
            SetRun(run);
            Settings.AddToRecentSplits("");
            InTimerOnlyMode = true;
            if (Layout.Components.Count() != 1 || Layout.Components.FirstOrDefault().ComponentName != "Timer")
            {
                TimerOnlyLayout = new TimerOnlyLayoutFactory().Create(CurrentState);
                var layout = TimerOnlyLayout;
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

            if (CurrentState.Run.HasChanged)
            {
                try
                {
                    DontRedraw = true;
                    var result = MessageBox.Show(this,"Your splits have been updated but not yet saved.\nDo you want to save your splits now?", "Save Splits?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveSplits(false);
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
            Model.Reset();
            return true;
        }

        private bool WarnUserAboutLayoutSave()
        {
            if (Layout.HasChanged)
            {
                try
                {
                    DontRedraw = true;
                    var result = MessageBox.Show(this, "Your layout has been updated but not yet saved.\nDo you want to save your layout now?", "Save Layout?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveLayout();
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
            return true;
        }

        private void TimerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.LastComparison = CurrentState.CurrentComparison;
            Settings.LastTimingMethod = CurrentState.CurrentTimingMethod;
            if (!WarnUserAboutSplitsSave())
            {
                e.Cancel = true;
                return;
            }
            if (!WarnUserAboutLayoutSave())
            {
                e.Cancel = true;
                return;
            }

            var settingsPath = Path.Combine(BasePath, SETTINGS_PATH);
            if (!File.Exists(settingsPath))
                File.Create(settingsPath).Close();
            using (var stream = File.Open(settingsPath, FileMode.Create, FileAccess.Write))
            {
                SettingsSaver.Save(Settings, stream);
            }

            foreach (var component in Layout.Components)
                component.Dispose();
            DeactivateAutoSplitter();
        }

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            var editor = new SettingsDialog(Hook, Settings);
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
                Settings.RegisterHotkeys(Hook);
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

        private void TimerForm_Load(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void closeSplitsMenuItem_Click(object sender, EventArgs e)
        {
            CloseSplits();
        }

        private void FixSize()
        {
            var currentSize = Layout.Mode == LayoutMode.Vertical ? ComponentRenderer.OverallHeight : ComponentRenderer.OverallWidth;
            if (OldSize >= 0)
            {
                if (Layout.Mode == LayoutMode.Vertical)
                {
                    var minimumWidth = ComponentRenderer.MinimumWidth * (Height / ComponentRenderer.OverallHeight);
                    if (Width < minimumWidth)
                        Height = (int)(Height / (minimumWidth / Width) + 0.5f);
                }
                else
                {
                    var minimumHeight = ComponentRenderer.MinimumHeight * (Width / ComponentRenderer.OverallWidth);
                    if (Height < minimumHeight)
                        Width = (int)(Width / (minimumHeight / Height) + 0.5f);
                }
            }
            else
                OldSize++;
            if (OldSize >= 0)
                OldSize = currentSize;
        }

        private Image MakeScreenShot(bool transparent = false)
        {
            var image = new Bitmap(Width, Height);
            var graphics = Graphics.FromImage(image);

            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.FillRectangle(Brushes.Transparent, 0, 0, BackBuffer.Width, BackBuffer.Height);
            graphics.CompositingMode = CompositingMode.SourceOver;

            if (!transparent)
            {
                graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, Width, Height);
                graphics.FillRectangle(new SolidBrush(Layout.Settings.BackgroundColor), 0, 0, Width, Height);
            }

            PaintForm(graphics, new Region(new Rectangle(0, 0, Width, Height)));

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
                Settings.RegisterHotkeys(Hook);
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
            TimeSpan? currentSegmentRTA = TimeSpan.Zero;
            TimeSpan? previousSplitTimeRTA = TimeSpan.Zero;
            TimeSpan? currentSegmentGT = TimeSpan.Zero;
            TimeSpan? previousSplitTimeGT = TimeSpan.Zero;
            foreach (var split in CurrentState.Run)
            {
                if (split.SplitTime.RealTime != null && CurrentState.CurrentTimingMethod == TimingMethod.RealTime)
                {
                    currentSegmentRTA = split.SplitTime.RealTime - previousSplitTimeRTA;
                    previousSplitTimeRTA = split.SplitTime.RealTime;
                    if (split.BestSegmentTime.RealTime == null || currentSegmentRTA < split.BestSegmentTime.RealTime)
                    {
                        warnUser = true;
                        break;
                    }
                }
                if (split.SplitTime.GameTime != null && CurrentState.CurrentTimingMethod == TimingMethod.GameTime)
                {
                    currentSegmentGT = split.SplitTime.GameTime - previousSplitTimeGT;
                    previousSplitTimeGT = split.SplitTime.GameTime;
                    if (split.BestSegmentTime.GameTime == null || currentSegmentGT < split.BestSegmentTime.GameTime)
                    {
                        warnUser = true;
                        break;
                    }
                }
            }
            if (!warnUser && (CurrentState.Run.Last().SplitTime[CurrentState.CurrentTimingMethod] != null && CurrentState.Run.Last().PersonalBestSplitTime[CurrentState.CurrentTimingMethod] == null) || CurrentState.Run.Last().SplitTime[CurrentState.CurrentTimingMethod] < CurrentState.Run.Last().PersonalBestSplitTime[CurrentState.CurrentTimingMethod])
                warnUser = true;
            if (warnUser)
            {
                DontRedraw = true;
                var result =  MessageBox.Show(this, "You have beaten some of your best times.\r\nDo you want to update them?", "Update Times?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
            SpeedRunsLiveAPI.Instance.RefreshRacesListAsync();
        }

        private void resetMenuItem_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void pauseMenuItem_Click(object sender, EventArgs e)
        {
            Model.Pause();
        }

        private void hotkeysMenuItem_Click(object sender, EventArgs e)
        {
            if (hotkeysMenuItem.Checked)
                hotkeysMenuItem.Checked = Settings.GlobalHotkeysEnabled = false;
            else
                hotkeysMenuItem.Checked = Settings.GlobalHotkeysEnabled = true;
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
                if (Settings.ToggleGlobalHotkeys != null)
                {
                    TaskbarManager.Instance.SetProgressState(Settings.GlobalHotkeysEnabled ? TaskbarProgressBarState.Normal : TaskbarProgressBarState.Error);
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
            var allGenerators = new StandardComparisonGeneratorsFactory().Create(CurrentState.Run);
            foreach (var generator in Settings.ComparisonGeneratorStates)
            {
                var generatorExists = CurrentState.Run.ComparisonGenerators.Any(x => x.Name == generator.Key);
                if (generator.Value != generatorExists)
                {
                    if (generator.Value == true)
                        CurrentState.Run.ComparisonGenerators.Add(allGenerators.First(x => x.Name == generator.Key));
                    else
                        CurrentState.Run.ComparisonGenerators.Remove(CurrentState.Run.ComparisonGenerators.First(x => x.Name == generator.Key));
                }
            }
        }

        private void SwitchComparison(string name)
        {
            var generator = CurrentState.Run.ComparisonGenerators.FirstOrDefault(x => x.Name == name);
            if (generator == null && (string.IsNullOrEmpty(name) || !CurrentState.Run.CustomComparisons.Contains(name)))
                name = Run.PersonalBestComparisonName;
            CurrentState.CurrentComparison = name;
        }

        private void AddActionToComparisonsMenu(string name)
        {
            var menuItem = new ToolStripMenuItem(name);
            menuItem.Click += (s, e) => SwitchComparison(name);
            comparisonMenuItem.DropDownItems.Add(menuItem);
        }

        private void AddActionToControlMenu(string name, Action action)
        {
            var menuItem = new ToolStripMenuItem(name);
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
            pauseMenuItem});

            controlMenuItem.DropDownItems.Add(new ToolStripSeparator());
            controlMenuItem.DropDownItems.Add(hotkeysMenuItem);

            hotkeysMenuItem.Checked = Settings.GlobalHotkeysEnabled;

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
    }
}
