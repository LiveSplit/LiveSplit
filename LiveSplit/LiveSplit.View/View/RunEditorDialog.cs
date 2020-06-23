using LiveSplit.Model;
using LiveSplit.Model.RunImporters;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.Web;
using LiveSplit.Utils;
using LiveSplit.Web.Share;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using LiveSplit.Web.SRL;

namespace LiveSplit.View
{
    public partial class RunEditorDialog : Form
    {
        private const int ICONINDEX = 0;
        private const int SEGMENTNAMEINDEX = 1;
        private const int SPLITTIMEINDEX = 2;
        private const int SEGMENTTIMEINDEX = 3;
        private const int BESTSEGMENTINDEX = 4;
        private const int CUSTOMCOMPARISONSINDEX = 5;

        public IRun Run { get; set; }
        public LiveSplitState CurrentState { get; set; }
        protected BindingList<ISegment> SegmentList { get; set; }
        protected ITimeFormatter TimeFormatter { get; set; }
        protected IList<TimeSpan?> SegmentTimeList { get; set; }
        protected bool IsInitialized = false;
        protected Time PreviousPersonalBestTime;

        protected bool IsGridTab { get { return tabControl.SelectedTab == RealTime || tabControl.SelectedTab == GameTime; } }
        protected bool IsMetadataTab { get { return tabControl.SelectedTab == Metadata; } }

        public List<Image> ImagesToDispose { get; set; }

        protected TimingMethod SelectedMethod
        {
            get { return tabControl.SelectedTab.Text == "Real Time" ? TimingMethod.RealTime : TimingMethod.GameTime; }
            set { tabControl.SelectTab(value.ToString()); }
        }

        public int CurrentSplitIndexOffset { get; set; }

        public bool AllowChangingSegments { get; set; }

        public event EventHandler RunEdited;
        public event EventHandler ComparisonRenamed;
        public event EventHandler SegmentRemovedOrAdded;

        private Control eCtl;

        private string[] gameNames;
        private IEnumerable<IGrouping<string, string>> abbreviations;

        public Image GameIcon { get { return Run.GameIcon ?? Properties.Resources.DefaultGameIcon; } set { Run.GameIcon = value; } }
        public string GameName
        {
            get { return Run.GameName; }
            set
            {
                if (Run.GameName != value)
                {
                    Run.GameName = value;
                    if (IsMetadataTab)
                        metadataControl.RefreshInformation();
                    RefreshCategoryAutoCompleteList();
                    RaiseRunEdited();
                    Run.Metadata.RunID = null;
                }
            }
        }
        public string CategoryName
        {
            get { return Run.CategoryName; }
            set
            {
                if (Run.CategoryName != value)
                {
                    Run.CategoryName = value;
                    if (IsMetadataTab)
                        metadataControl.RefreshInformation();
                    RaiseRunEdited();
                    Run.Metadata.RunID = null;
                }
            }
        }
        public string Offset
        {
            get
            {
                return TimeFormatter.Format(Run.Offset);
            }
            set
            {
                if (Regex.IsMatch(value, "[^0-9:.\\-−]"))
                    return;

                try
                {
                    Run.Offset = TimeSpanParser.Parse(value);
                    Run.HasChanged = true;
                }
                catch { }
            }
        }
        public int AttemptCount
        {
            get { return Run.AttemptCount; }
            set { Run.AttemptCount = Math.Max(0, value); RaiseRunEdited(); }
        }

        private class ParsingResults
        {
            public bool Parsed { get; set; }
            public object Value { get; set; }
            
            public ParsingResults(bool parsed, object value)
            {
                Parsed = parsed;
                Value = value;
            }
        }

        public RunEditorDialog(LiveSplitState state)
        {
            InitializeComponent();
            CurrentState = state;
            Run = state.Run;
            Run.PropertyChanged += Run_PropertyChanged;
            PreviousPersonalBestTime = Run.Last().PersonalBestSplitTime;
            metadataControl.Metadata = Run.Metadata;
            metadataControl.MetadataChanged += metadataControl_MetadataChanged;
            CurrentSplitIndexOffset = 0;
            AllowChangingSegments = false;
            ImagesToDispose = new List<Image>();
            SegmentTimeList = new List<TimeSpan?>();
            TimeFormatter = new ShortTimeFormatter();
            SegmentList = new BindingList<ISegment>(Run);
            SegmentList.AllowNew = true;
            SegmentList.AllowRemove = true;
            SegmentList.AllowEdit = true;
            SegmentList.ListChanged += SegmentList_ListChanged;
            runGrid.AutoGenerateColumns = false;
            runGrid.AutoSize = true;
            runGrid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            runGrid.DataSource = SegmentList;
            runGrid.CellDoubleClick += runGrid_CellDoubleClick;
            runGrid.CellFormatting += runGrid_CellFormatting;
            runGrid.CellParsing += runGrid_CellParsing;
            runGrid.CellValidating += runGrid_CellValidating;
            runGrid.CellEndEdit += runGrid_CellEndEdit;
            runGrid.SelectionChanged += runGrid_SelectionChanged;

            var iconColumn = new DataGridViewImageColumn() { ImageLayout = DataGridViewImageCellLayout.Zoom };
            iconColumn.Name = "Icon";
            iconColumn.Width = 50;
            iconColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            iconColumn.DefaultCellStyle.NullValue = new Bitmap(1, 1);
            runGrid.Columns.Add(iconColumn);

            var column = new DataGridViewTextBoxColumn();
            column.Name = "Segment Name";
            column.MinimumWidth = 120;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            runGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Split Time";
            column.Width = 100;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            runGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Segment Time";
            column.Width = 100;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            runGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Best Segment";
            column.Width = 100;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            runGrid.Columns.Add(column);

            runGrid.EditingControlShowing += runGrid_EditingControlShowing;

            cbxGameName.DataBindings.Add("Text", this, "GameName");
            cbxRunCategory.DataBindings.Add("Text", this, "CategoryName");
            tbxTimeOffset.DataBindings.Add("Text", this, "Offset");
            tbxAttempts.DataBindings.Add("Text", this, "AttemptCount");

            picGameIcon.Image = GameIcon;
            removeIconToolStripMenuItem.Enabled = state.Run.GameIcon != null;

            cbxGameName.GetAllItemsForText = x => new string[0];

            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        gameNames = CompositeGameList.Instance.GetGameNames().ToArray();
                        abbreviations = gameNames
                        .Select(x => x.GetAbbreviations()
                            .Select(y => new KeyValuePair<string, string>(x, y)))
                        .SelectMany(x => x)
                        .GroupBy(x => x.Value, x => x.Key);
                        cbxGameName.GetAllItemsForText = x => SearchForGameName(x);
                        this.InvokeIfRequired(() =>
                        {
                            try
                            {
                                cbxGameName.Items.AddRange(gameNames);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                });


            cbxRunCategory.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbxRunCategory.Items.AddRange(new[] { "Any%", "Low%", "100%" });
            cbxRunCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            SelectedMethod = state.CurrentTimingMethod;

            RefreshCategoryAutoCompleteList();
            UpdateSegmentList();
            RefreshAutoSplittingUI();
            SetClickEvents(this);
        }

        private IEnumerable<string> SearchForGameName(string name)
        {
            name = name.ToLowerInvariant();
            return abbreviations
                .Where(x => x.Key.ToLowerInvariant().Contains(name))
                .OrderBy(x => x.Key.ToLowerInvariant().Similarity(name))
                .SelectMany(x => x)
                .Distinct()
                .Take(10);
        }

        void Run_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GameName")
                cbxGameName.Text = Run.GameName;
            else if (e.PropertyName == "CategoryName")
                cbxRunCategory.Text = Run.CategoryName;
        }

        void metadataControl_MetadataChanged(object sender, EventArgs e)
        {
            var args = (MetadataChangedEventArgs)e;
            if (args.ClearRunID)
            {
                Run.Metadata.RunID = null;
                metadataControl.RefreshAssociateButton();
            }
            RaiseRunEdited();
        }

        void cbxGameName_TextChanged(object sender, EventArgs e)
        {
            if (IsInitialized)
            {
                DeactivateAutoSplitter();
                var splitter = AutoSplitterFactory.Instance.Create(cbxGameName.Text);
                Run.AutoSplitter = splitter;
                if (splitter != null && CurrentState.Settings.ActiveAutoSplitters.Contains(cbxGameName.Text))
                {
                    splitter.Activate(CurrentState);
                    SetAutoSplitterSettings();
                }
                RefreshAutoSplittingUI();
            }
        }

        private void DeactivateAutoSplitter()
        {
            if (Run.IsAutoSplitterActive())
                Run.AutoSplitter.Deactivate();
        }

        void RefreshCategoryAutoCompleteList()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    string[] categoryNames = new[] { "Any%", "Low%", "100%" };
                    try
                    {
                        var game = Run.Metadata.Game;
                        if (game != null)
                            categoryNames = game.FullGameCategories.Select(x => x.Name).ToArray();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                    this.InvokeIfRequired(() =>
                    {
                        try
                        {
                            cbxRunCategory.Items.Clear();
                            cbxRunCategory.Items.AddRange(categoryNames);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });
        }

        void runGrid_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonsStatus();
        }

        void SegmentList_ListChanged(object sender, ListChangedEventArgs e)
        {
            TimesModified();
            UpdateButtonsStatus();
        }

        private void UpdateButtonsStatus()
        {
            if (!AllowChangingSegments)
            {
                btnAdd.Enabled = false;
                btnRemove.Enabled = false;
                btnInsert.Enabled = false;
                btnMoveDown.Enabled = false;
                btnMoveUp.Enabled = false;
                btnOther.Enabled = false;
            }
            else
            {
                btnRemove.Enabled = SegmentList.Count > 1;
                List<DataGridViewCell> selectedCells = runGrid.SelectedCells.Cast<DataGridViewCell>().OrderBy(o => o.RowIndex).ToList();

                if (selectedCells.FirstOrDefault() != null)
                {
                    btnMoveUp.Enabled = selectedCells.First().RowIndex > 0;
                    btnMoveDown.Enabled = selectedCells.Last().RowIndex < SegmentList.Count - 1;
                }
                else
                {
                    btnMoveUp.Enabled = false;
                    btnMoveDown.Enabled = false;
                }
            }
        }

        void runGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            eCtl = e.Control;
            eCtl.TextChanged -= new EventHandler(eCtl_TextChanged);
            eCtl.KeyPress -= new KeyPressEventHandler(eCtl_KeyPress);
            eCtl.TextChanged += new EventHandler(eCtl_TextChanged);
            eCtl.KeyPress += new KeyPressEventHandler(eCtl_KeyPress);
        }

        private void eCtl_TextChanged(object sender, EventArgs e)
        {
            if (runGrid.CurrentCell.ColumnIndex == SPLITTIMEINDEX || runGrid.CurrentCell.ColumnIndex == BESTSEGMENTINDEX || runGrid.CurrentCell.ColumnIndex == SEGMENTTIMEINDEX || runGrid.CurrentCell.ColumnIndex >= CUSTOMCOMPARISONSINDEX)
            {
                if (Regex.IsMatch(eCtl.Text, "[^0-9:.]"))
                {
                    eCtl.Text = Regex.Replace(eCtl.Text, "[^0-9:.]", "");
                }
            }
        }

        private void eCtl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (runGrid.CurrentCell.ColumnIndex == SPLITTIMEINDEX || runGrid.CurrentCell.ColumnIndex == BESTSEGMENTINDEX || runGrid.CurrentCell.ColumnIndex == SEGMENTTIMEINDEX || runGrid.CurrentCell.ColumnIndex >= CUSTOMCOMPARISONSINDEX)
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ':' && e.KeyChar != '.' && e.KeyChar != ',')
                {
                    e.Handled = true;
                }
            }
        }

        void runGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            runGrid.Rows[e.RowIndex].ErrorText = "";
        }

        void runGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == SPLITTIMEINDEX || e.ColumnIndex == BESTSEGMENTINDEX || e.ColumnIndex == SEGMENTTIMEINDEX || e.ColumnIndex >= CUSTOMCOMPARISONSINDEX)
            {
                if (string.IsNullOrWhiteSpace(e.FormattedValue.ToString()))
                    return;

                try
                {
                    TimeSpanParser.Parse(e.FormattedValue.ToString());
                }
                catch
                {
                    e.Cancel = true;
                    runGrid.Rows[e.RowIndex].ErrorText = "Invalid Time";
                }
            }
        }

        void runGrid_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            var parsingResults = ParseCell(e.Value, e.RowIndex, e.ColumnIndex, true);
            if (parsingResults.Parsed)
            {
                e.ParsingApplied = true;
                e.Value = parsingResults.Value;
            }
            else
                e.ParsingApplied = false;
        }

        private ParsingResults ParseCell(object value, int rowIndex, int columnIndex, bool shouldFix)
        {
            if (columnIndex == SEGMENTNAMEINDEX)
            {
                Run[rowIndex].Name = value.ToString();
                RaiseRunEdited();
                return new ParsingResults(true, value);
            }

            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                value = null;
                if (columnIndex == SPLITTIMEINDEX)
                {
                    var time = new Time(Run[rowIndex].PersonalBestSplitTime);
                    time[SelectedMethod] = null;
                    Run[rowIndex].PersonalBestSplitTime = time;
                }
                if (columnIndex == BESTSEGMENTINDEX)
                {
                    var time = new Time(Run[rowIndex].BestSegmentTime);
                    time[SelectedMethod] = null;
                    Run[rowIndex].BestSegmentTime = time;
                }
                if (columnIndex == SEGMENTTIMEINDEX)
                {
                    SegmentTimeList[rowIndex] = null;
                    if (shouldFix)
                        FixSplitsFromSegments();
                }
                if (columnIndex >= CUSTOMCOMPARISONSINDEX)
                {
                    var time = new Time(Run[rowIndex].Comparisons[runGrid.Columns[columnIndex].Name]);
                    time[SelectedMethod] = null;
                    Run[rowIndex].Comparisons[runGrid.Columns[columnIndex].Name] = time;
                }
                if (shouldFix)
                    Fix();
                TimesModified();
                return new ParsingResults(true, value);
            }

            try
            {
                value = TimeSpanParser.Parse(value.ToString());
                if (columnIndex == SEGMENTTIMEINDEX)
                {
                    SegmentTimeList[rowIndex] = (TimeSpan)value;
                    if (shouldFix)
                        FixSplitsFromSegments();
                }
                if (columnIndex >= CUSTOMCOMPARISONSINDEX)
                {
                    var time = new Time(Run[rowIndex].Comparisons[runGrid.Columns[columnIndex].Name]);
                    time[SelectedMethod] = (TimeSpan)value;
                    Run[rowIndex].Comparisons[runGrid.Columns[columnIndex].Name] = time;
                }
                if (columnIndex == SPLITTIMEINDEX)
                {
                    var time = new Time(Run[rowIndex].PersonalBestSplitTime);
                    time[SelectedMethod] = (TimeSpan)value;
                    Run[rowIndex].PersonalBestSplitTime = time;
                }
                if (columnIndex == BESTSEGMENTINDEX)
                {
                    var time = new Time(Run[rowIndex].BestSegmentTime);
                    time[SelectedMethod] = (TimeSpan)value;
                    Run[rowIndex].BestSegmentTime = time;
                }
                if (shouldFix)
                    Fix();
                TimesModified();
                return new ParsingResults(true, value);
            }
            catch { }

            return new ParsingResults(false, null);
        }

        void runGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < Run.Count)
            {
                if (e.ColumnIndex == SPLITTIMEINDEX)
                {
                    var comparisonValue = Run[e.RowIndex].PersonalBestSplitTime[SelectedMethod];
                    if (comparisonValue == null)
                    {
                        e.Value = "";
                        e.FormattingApplied = false;
                    }
                    else
                    {
                        e.Value = TimeFormatter.Format(comparisonValue);
                        e.FormattingApplied = true;
                    }
                }
                else if (e.ColumnIndex == BESTSEGMENTINDEX)
                {
                    var comparisonValue = Run[e.RowIndex].BestSegmentTime[SelectedMethod];
                    if (comparisonValue == null)
                    {
                        e.Value = "";
                        e.FormattingApplied = false;
                    }
                    else
                    {
                        e.Value = TimeFormatter.Format(comparisonValue);
                        e.FormattingApplied = true;
                    }
                }
                else if (e.ColumnIndex >= CUSTOMCOMPARISONSINDEX)
                {
                    var comparisonValue = Run[e.RowIndex].Comparisons[runGrid.Columns[e.ColumnIndex].Name][SelectedMethod];
                    if (comparisonValue == null)
                    {
                        e.Value = "";
                        e.FormattingApplied = false;
                    }
                    else
                    {
                        e.Value = TimeFormatter.Format(comparisonValue);
                        e.FormattingApplied = true;
                    }
                }
                else if (e.ColumnIndex == SEGMENTTIMEINDEX)
                {
                    if (SegmentTimeList[e.RowIndex] == null)
                    {
                        e.Value = "";
                        e.FormattingApplied = false;
                    }
                    else
                    {
                        e.Value = TimeFormatter.Format(SegmentTimeList[e.RowIndex].Value);
                        e.FormattingApplied = true;
                    }
                }
                else if (e.ColumnIndex == SEGMENTNAMEINDEX)
                {
                    e.Value = Run[e.RowIndex].Name;
                }
                else if (e.ColumnIndex == ICONINDEX)
                {
                    e.Value = Run[e.RowIndex].Icon;
                }
            }
        }

        void runGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == ICONINDEX && e.RowIndex >= 0 && e.RowIndex < Run.Count)
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "Image Files|*.BMP;*.JPG;*.GIF;*.JPEG;*.PNG|All files (*.*)|*.*";
                var multiEdit = runGrid.SelectedCells.Count > 1;
                if (!string.IsNullOrEmpty(Run[e.RowIndex].Name) && !multiEdit)
                {
                    dialog.Title = "Set Icon for " + Run[e.RowIndex].Name + "...";
                }
                else
                {
                    dialog.Title = "Set Icon...";
                }
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    ChangeImage(e.RowIndex, dialog.FileName, multiEdit);
                }
            }
        }

        private void ChangeImage(int rowIndex, string fileName, bool multiEdit)
        {
            try
            {
                var image = Image.FromFile(fileName);

                if (!multiEdit)
                {
                    var oldImage = (Image)runGrid.Rows[rowIndex].Cells[ICONINDEX].Value;
                    if (oldImage != null)
                        ImagesToDispose.Add(oldImage);

                    Run[rowIndex].Icon = image;
                    runGrid.NotifyCurrentCellDirty(true);
                }
                else
                {
                    foreach (DataGridViewCell cell in runGrid.SelectedCells)
                    {
                        if (cell.ColumnIndex != ICONINDEX)
                            continue;

                        var oldImage = (Image)cell.Value;
                        if (oldImage != null)
                            ImagesToDispose.Add(oldImage);

                        Run[cell.RowIndex].Icon = (Image)image.Clone();
                        runGrid.UpdateCellValue(ICONINDEX, cell.RowIndex);
                    }
                }

                RaiseRunEdited();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show("Could not load image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Fix()
        {
            Run.FixSplits();
            UpdateSegmentList();
            runGrid.InvalidateColumn(SPLITTIMEINDEX);
            runGrid.InvalidateColumn(BESTSEGMENTINDEX);
            runGrid.InvalidateColumn(SEGMENTTIMEINDEX);
            for (var index = CUSTOMCOMPARISONSINDEX; index < runGrid.Columns.Count; index++)
                runGrid.InvalidateColumn(index);
        }

        private void SetGameIcon(Image icon)
        {
            ImagesToDispose.Add(GameIcon);
            GameIcon = icon;
            picGameIcon.Image = GameIcon;
            removeIconToolStripMenuItem.Enabled = GameIcon != null;
        }

        private void picGameIcon_DoubleClick(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (!string.IsNullOrEmpty(GameName))
            {
                dialog.Title = "Set Icon for " + GameName + "...";
            }
            else
            {
                dialog.Title = "Set Game Icon...";
            }
            dialog.Filter = "Image Files|*.BMP;*.JPG;*.GIF;*.JPEG;*.PNG|All files (*.*)|*.*";
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    var icon = Image.FromFile(dialog.FileName);
                    SetGameIcon(icon);
                    RaiseRunEdited();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    MessageBox.Show("Could not load image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

        private void UpdateSegmentList()
        {
            var previousTime = TimeSpan.Zero;
            SegmentTimeList.Clear();
            foreach (var curSeg in Run)
            {
                var splitTime = curSeg.PersonalBestSplitTime[SelectedMethod];

                SegmentTimeList.Add(splitTime - previousTime);

                if (splitTime != null)
                    previousTime = splitTime.Value;
            }
        }

        private void FixSplitsFromSegments()
        {
            var previousTime = TimeSpan.Zero;
            for (var index = 0; index < Run.Count; index++)
            {
                var curSegment = Run[index];
                var curSegTime = SegmentTimeList[index];

                var time = new Time(curSegment.PersonalBestSplitTime);
                time[SelectedMethod] = previousTime + curSegTime;
                curSegment.PersonalBestSplitTime = time;

                if (curSegTime != null)
                    previousTime = curSegment.PersonalBestSplitTime[SelectedMethod].Value;
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            var newSegment = new Segment("");
            Run.ImportBestSegment(runGrid.CurrentRow.Index);

            var maxIndex = Run.AttemptHistory.Select(x => x.Index).DefaultIfEmpty(0).Max();
            for (var x = Run.GetMinSegmentHistoryIndex(); x <= maxIndex; x++)
                newSegment.SegmentHistory.Add(x, default(Time));
            SegmentList.Insert(runGrid.CurrentRow.Index, newSegment);
            runGrid.ClearSelection();
            runGrid.CurrentCell = runGrid.Rows[runGrid.CurrentRow.Index - 1].Cells[runGrid.CurrentCell.ColumnIndex];
            runGrid.CurrentCell.Selected = true;

            Fix();

            SegmentRemovedOrAdded(null, null);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var newSegment = new Segment("");
            if (runGrid.CurrentRow.Index + 1 < Run.Count)
                Run.ImportBestSegment(runGrid.CurrentRow.Index + 1);
            var maxIndex = Run.AttemptHistory.Select(x => x.Index).DefaultIfEmpty(0).Max();
            for (var x = Run.GetMinSegmentHistoryIndex(); x <= maxIndex; x++)
                newSegment.SegmentHistory.Add(x, default(Time));
            SegmentList.Insert(runGrid.CurrentRow.Index + 1, newSegment);
            runGrid.ClearSelection();
            runGrid.CurrentCell = runGrid.Rows[runGrid.CurrentRow.Index + 1].Cells[runGrid.CurrentCell.ColumnIndex];
            runGrid.CurrentCell.Selected = true;

            Fix();

            SegmentRemovedOrAdded(null, null);
        }

        private void runGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (var selectedObject in runGrid.SelectedCells.OfType<DataGridViewCell>().Reverse())
                {
                    var selectedCell = selectedObject;

                    if (selectedCell.RowIndex >= Run.Count || selectedCell.RowIndex < 0)
                        continue;

                    if (selectedCell.ColumnIndex == SEGMENTNAMEINDEX)
                    {
                        Run[selectedCell.RowIndex].Name = "";
                        RaiseRunEdited();
                    }
                    else if (selectedCell.ColumnIndex == ICONINDEX)
                    {
                        if (Run[selectedCell.RowIndex].Icon != null)
                            ImagesToDispose.Add(Run[selectedCell.RowIndex].Icon);
                        Run[selectedCell.RowIndex].Icon = null;
                        RaiseRunEdited();
                    }
                    else
                    {
                        if (selectedCell.ColumnIndex == SPLITTIMEINDEX)
                        {
                            var time = new Time(Run[selectedCell.RowIndex].PersonalBestSplitTime);
                            time[SelectedMethod] = null;
                            Run[selectedCell.RowIndex].PersonalBestSplitTime = time;
                        }
                        else if (selectedCell.ColumnIndex == SEGMENTTIMEINDEX)
                        {
                            SegmentTimeList[selectedCell.RowIndex] = null;
                            FixSplitsFromSegments();
                        }
                        else if (selectedCell.ColumnIndex == BESTSEGMENTINDEX)
                        {
                            var time = new Time(Run[selectedCell.RowIndex].BestSegmentTime);
                            time[SelectedMethod] = null;
                            Run[selectedCell.RowIndex].BestSegmentTime = time;
                        }
                        else if (selectedCell.ColumnIndex >= CUSTOMCOMPARISONSINDEX)
                        {
                            var time = new Time(Run[selectedCell.RowIndex].Comparisons[selectedCell.OwningColumn.Name]);
                            time[SelectedMethod] = null;
                            Run[selectedCell.RowIndex].Comparisons[selectedCell.OwningColumn.Name] = time;
                        }
                        Fix();
                        TimesModified();
                    }
                }
                runGrid.Invalidate();
            }

            if (e.Control && e.KeyCode == Keys.V)
            {
                char[] rowSplitter = { '\n' };
                char[] columnSplitter = { '\t' };

                var dataInClipboard = Clipboard.GetDataObject();
                var stringInClipboard = (string)dataInClipboard.GetData(DataFormats.Text);

                if (stringInClipboard != null && runGrid.SelectedCells.Count > 0)
                {
                    var rowsInClipboard = stringInClipboard.Replace("\r\n", "\n").Split(rowSplitter);

                    var r = runGrid.SelectedCells[0].RowIndex;
                    var c = runGrid.SelectedCells[0].ColumnIndex;

                    var maxRow = Math.Min(rowsInClipboard.Length, runGrid.RowCount - r);

                    var splitsChanged = false;
                    var segmentsChanged = false;
                    var shouldFix = false;

                    for (int iRow = r; iRow < r + maxRow; iRow++)
                    {
                        string[] valuesInRow = rowsInClipboard[iRow - r].Split(columnSplitter);

                        for (int iCol = c; iCol < c + valuesInRow.Length; iCol++)
                        {
                            if (runGrid.ColumnCount - 1 >= iCol)
                            {
                                if (iCol == SEGMENTTIMEINDEX)
                                    segmentsChanged = true;
                                else if (iCol == SPLITTIMEINDEX)
                                    splitsChanged = true;
                                if (iCol != SEGMENTNAMEINDEX)
                                    shouldFix = true;

                                var cell = runGrid.Rows[iRow].Cells[iCol];
                                var parsingResults = ParseCell(valuesInRow[iCol - c], iRow, iCol, false);
                                if (parsingResults.Parsed)
                                {
                                    cell.Value = parsingResults.Value;
                                    runGrid.InvalidateCell(cell);
                                }
                            }
                        }
                    }

                    if (shouldFix)
                    {
                        if (segmentsChanged && !splitsChanged)
                            FixSplitsFromSegments();
                        Fix();
                    }
                }
            }
        }

        private void RemoveSelected()
        {
            foreach (var selectedObject in runGrid.SelectedCells)
            {
                var selectedCell = (DataGridViewCell)selectedObject;
                var selectedIndex = selectedCell.RowIndex;

                if (Run.Count <= 1 || selectedIndex >= Run.Count || selectedIndex < 0)
                    continue;
                FixAfterDeletion(selectedIndex);
                if (selectedIndex == Run.Count - 1 && selectedIndex == runGrid.CurrentRow.Index)
                {
                    runGrid.ClearSelection();
                    runGrid.CurrentCell = runGrid.Rows[runGrid.CurrentRow.Index - 1].Cells[runGrid.CurrentCell.ColumnIndex];
                    runGrid.CurrentCell.Selected = true;
                }
                if (Run[selectedIndex].Icon != null)
                    ImagesToDispose.Add(Run[selectedIndex].Icon);
                SegmentList.RemoveAt(selectedIndex);
            }

            Fix();
            runGrid.Invalidate();

            SegmentRemovedOrAdded(null, null);
        }

        private void SwitchSegments(int segIndex)
        {
            var firstSegment = SegmentList.ElementAt(segIndex);
            var secondSegment = SegmentList.ElementAt(segIndex + 1);

            var maxIndex = Run.AttemptHistory.Select(x => x.Index).DefaultIfEmpty(0).Max();
            for (var runIndex = Run.GetMinSegmentHistoryIndex(); runIndex <= maxIndex; runIndex++)
            {
                //Remove both segment history elements if one of them has a null time and the other has has a non null time
                Time firstHistory;
                Time secondHistory;
                var firstExists = firstSegment.SegmentHistory.TryGetValue(runIndex, out firstHistory);
                var secondExists = secondSegment.SegmentHistory.TryGetValue(runIndex, out secondHistory);

                if (firstExists && secondExists
                    && (firstHistory[TimingMethod.RealTime].HasValue != secondHistory[TimingMethod.RealTime].HasValue
                    || firstHistory[TimingMethod.GameTime].HasValue != secondHistory[TimingMethod.GameTime].HasValue))
                {
                    firstSegment.SegmentHistory.Remove(runIndex);
                    secondSegment.SegmentHistory.Remove(runIndex);
                }
            }

            var comparisonKeys = new List<string>(firstSegment.Comparisons.Keys);
            foreach (var comparison in comparisonKeys)
            {
                //Fix the comparison times based on the new positions of the two segments
                var previousTime = segIndex > 0 ? SegmentList.ElementAt(segIndex - 1).Comparisons[comparison] : new Time(TimeSpan.Zero, TimeSpan.Zero);
                var firstSegmentTime = firstSegment.Comparisons[comparison] - previousTime;
                var secondSegmentTime = secondSegment.Comparisons[comparison] - firstSegment.Comparisons[comparison];
                secondSegment.Comparisons[comparison] = new Time(previousTime + secondSegmentTime);
                firstSegment.Comparisons[comparison] = new Time(secondSegment.Comparisons[comparison] + firstSegmentTime);
            }

            SegmentList.Remove(secondSegment);
            SegmentList.Insert(segIndex, secondSegment);

            UpdateButtonsStatus();
        }

        private void FixAfterDeletion(int index)
        {
            FixWithTimingMethod(index, TimingMethod.RealTime);
            FixWithTimingMethod(index, TimingMethod.GameTime);
        }

        private void FixWithTimingMethod(int index, TimingMethod method)
        {
            var curIndex = index + 1;

            if (curIndex < Run.Count)
            {
                var maxIndex = Run.AttemptHistory.Select(x => x.Index).DefaultIfEmpty(0).Max();
                for (var runIndex = Run.GetMinSegmentHistoryIndex(); runIndex <= maxIndex; runIndex++)
                {
                    curIndex = index + 1;
                    //If a history element isn't there in the segment that's deleted, remove it from the next segment's history as well
                    Time segmentHistoryElement;
                    if (!Run[index].SegmentHistory.TryGetValue(runIndex, out segmentHistoryElement))
                    {
                        if (Run[curIndex].SegmentHistory.ContainsKey(runIndex))
                            Run[curIndex].SegmentHistory.Remove(runIndex);
                        continue;
                    }

                    var curSegment = segmentHistoryElement[method];
                    while (curSegment != null && curIndex < Run.Count)
                    {
                        //Add the removed segment's history times to the next non null times
                        Time segment;
                        if (Run[curIndex].SegmentHistory.TryGetValue(runIndex, out segment) && segment[method] != null)
                        {
                            segment[method] = curSegment + segment[method];
                            Run[curIndex].SegmentHistory[runIndex] = segment;
                            break;
                        }
                        curIndex++;
                    }
                }

                curIndex = index + 1;

                //Set the new Best Segment time to be the sum of the two Best Segments
                var minBestSegment = Run[index].BestSegmentTime[method] + Run[curIndex].BestSegmentTime[method];
                //Use any element in the history that has a lower time than this sum
                foreach (var history in Run[curIndex].SegmentHistory)
                {
                    if (history.Value[method] < minBestSegment)
                        minBestSegment = history.Value[method];
                }
                var newTime = Run[curIndex].BestSegmentTime;
                newTime[method] = minBestSegment;
                Run[curIndex].BestSegmentTime = newTime;
            }
        }

        private void TimesModified()
        {
            if (Run.Last().PersonalBestSplitTime.RealTime != PreviousPersonalBestTime.RealTime
                || Run.Last().PersonalBestSplitTime.GameTime != PreviousPersonalBestTime.GameTime)
            {
                Run.Metadata.RunID = null;
                PreviousPersonalBestTime = Run.Last().PersonalBestSplitTime;
            }
            RaiseRunEdited();
        }

        private void RaiseRunEdited()
        {
            if (IsInitialized)
            {
                Run.HasChanged = true;
                RunEdited?.Invoke(this, null);
            }
        }

        private void RunEditorDialog_Load(object sender, EventArgs e)
        {
            RebuildComparisonColumns();
            IsInitialized = true;
            UpdateButtonsStatus();
            cbxGameName.UpdateUI();
        }

        private void picGameIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RemoveIconMenu.Show(MousePosition);
            }
        }

        private void removeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImagesToDispose.Add(GameIcon);
            SetGameIcon(null);
            RaiseRunEdited();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            picGameIcon_DoubleClick(null, null);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            RemoveSelected();
        }

        private void downloadIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var game = Run.Metadata.Game;
                var cover = game.Assets.GetIconImage();
                SetGameIcon(cover);
                RaiseRunEdited();
                return;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show("Could not download the icon of the game!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void downloadBoxartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var game = Run.Metadata.Game;
                var cover = game.Assets.GetBoxartImage();
                SetGameIcon(cover);
                RaiseRunEdited();
                return;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            try
            {
                var gameId = PBTracker.Instance.GetGameIdByName(cbxGameName.Text);
                var cover = PBTracker.Instance.GetGameBoxArt(gameId);
                SetGameIcon(cover);
                RaiseRunEdited();
                return;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            try
            {
                var cover = Twitch.Instance.GetGameBoxArt(cbxGameName.Text);
                SetGameIcon(cover);
                RaiseRunEdited();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show("Could not download the box art of the game!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openFromURLMenuItem_Click(object sender, EventArgs e)
        {
            string url = null;

            if (DialogResult.OK == InputBox.Show("Open Game Icon from URL", "URL:", ref url))
            {
                try
                {
                    var uri = new Uri(url);

                    var request = (HttpWebRequest)WebRequest.Create(uri);

                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        try
                        {
                            var icon = Image.FromStream(stream);
                            SetGameIcon(icon);
                            RaiseRunEdited();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                            MessageBox.Show("The URL was not recognized as an image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    MessageBox.Show("The Game Icon couldn't be downloaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RebuildComparisonColumns()
        {
            while (runGrid.Columns.Count > CUSTOMCOMPARISONSINDEX)
            {
                runGrid.Columns.RemoveAt(CUSTOMCOMPARISONSINDEX);
            }
            foreach (var comparison in Run.CustomComparisons)
            {
                if (comparison != Model.Run.PersonalBestComparisonName)
                    AddComparisonColumn(comparison);
            }
        }

        private void AddComparisonColumn(string name)
        {
            var column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.Width = Math.Max(100, column.GetPreferredWidth(DataGridViewAutoSizeColumnMode.ColumnHeader, true));
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            var rightClickMenu = new ContextMenuStrip();
            var renameItem = new ToolStripMenuItem("Rename");
            renameItem.Click += (s, e) =>
            {
                RenameComparison(column);
            };
            var removeItem = new ToolStripMenuItem("Remove");
            removeItem.Click += (s, e) =>
            {
                RemoveComparison(column);
            };
            rightClickMenu.Items.Add(renameItem);
            rightClickMenu.Items.Add(removeItem);
            column.HeaderCell.ContextMenuStrip = rightClickMenu;
            runGrid.Columns.Add(column);
            RaiseRunEdited();
        }

        private void RenameComparison(DataGridViewTextBoxColumn column)
        {
            var name = column.Name;
            var newName = name;
            var dialogResult = InputBox.Show("Rename Comparison", "Comparison Name:", ref newName);
            if (dialogResult == DialogResult.OK)
            {
                if (!Run.Comparisons.Contains(newName))
                {
                    if (!SRLComparisonGenerator.IsRaceComparison(newName))
                    {
                        column.Name = newName;
                        column.Width = Math.Max(100, column.GetPreferredWidth(DataGridViewAutoSizeColumnMode.ColumnHeader, true));
                        if (CurrentState.CurrentComparison == name)
                            CurrentState.CurrentComparison = newName;
                        Run.CustomComparisons[Run.CustomComparisons.IndexOf(name)] = newName;
                        foreach (var segment in Run)
                        {
                            segment.Comparisons[newName] = segment.Comparisons[name];
                            segment.Comparisons.Remove(name);
                        }
                        var args = new RenameEventArgs();
                        args.OldName = name;
                        args.NewName = newName;
                        ComparisonRenamed(this, args);
                    }
                    else
                    {
                        var result = MessageBox.Show(this, "A Comparison name cannot start with [Race].", "Invalid Comparison Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                        if (result == DialogResult.Retry)
                            RenameComparison(column);
                    }
                }
                else if (newName != name)
                {
                    var result = MessageBox.Show(this, "A Comparison with this name already exists.", "Comparison Already Exists", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (result == DialogResult.Retry)
                        RenameComparison(column);
                }
            }
            RaiseRunEdited();
        }

        private void RemoveComparison(DataGridViewTextBoxColumn column)
        {
            var name = column.Name;
            runGrid.Columns.Remove(column);
            Run.CustomComparisons.Remove(name);

            if (CurrentState.CurrentComparison == name)
                CurrentState.CurrentComparison = Model.Run.PersonalBestComparisonName;

            var args = new RenameEventArgs();
            args.OldName = name;
            args.NewName = "Current Comparison";
            ComparisonRenamed(this, args);

            foreach (var segment in Run)
                segment.Comparisons.Remove(name);
            RaiseRunEdited();
        }

        private void btnAddComparison_Click(object sender, EventArgs e)
        {
            var name = "";
            var result = InputBox.Show("New Comparison", "Comparison Name:", ref name);
            if (result == DialogResult.OK)
            {
                if (!Run.Comparisons.Contains(name))
                {
                    if (!SRLComparisonGenerator.IsRaceComparison(name))
                    {
                        AddComparisonColumn(name);
                        Run.CustomComparisons.Add(name);
                    }
                    else
                    {
                        result = MessageBox.Show(this, "A Comparison name cannot start with [Race].", "Invalid Comparison Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                        if (result == DialogResult.Retry)
                            btnAddComparison_Click(sender, e);
                    }
                }
                else
                {
                    result = MessageBox.Show(this, "A Comparison with this name already exists.", "Comparison Already Exists", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (result == DialogResult.Retry)
                        btnAddComparison_Click(sender, e);
                }
            }
        }

        private void TabSelected(object sender, TabControlEventArgs e)
        {
            var margin = tabControl.Margin;
            if (IsGridTab)
            {
                margin.Bottom = 0;
                tabControl.Margin = margin;
                UpdateSegmentList();
                tableLayoutPanel1.SetRowSpan(tabControl, 1);
                runGrid.Visible = true;
                runGrid.Invalidate();
            }
            else
            {
                runGrid.Visible = false;
                tableLayoutPanel1.SetRowSpan(tabControl, 9);
                metadataControl.RefreshInformation();
                margin.Bottom = 10;
                tabControl.Margin = margin;
            }
        }

        protected void RefreshAutoSplittingUI()
        {
            lblDescription.Text = Run.AutoSplitter == null
                ? "There is no Auto Splitter available for this game."
                : Run.AutoSplitter.Description;
            btnActivate.Text = Run.IsAutoSplitterActive()
                ? "Deactivate"
                : "Activate";
            btnActivate.Enabled = Run.AutoSplitter != null;
            btnSettings.Enabled = Run.IsAutoSplitterActive() && Run.AutoSplitter.Component.GetSettingsControl(LayoutMode.Vertical) != null;
            btnWebsite.Visible = Run.AutoSplitter != null && Run.AutoSplitter.Website != null;

            if (Run.AutoSplitter != null && Run.AutoSplitter.Website == null)
            {
                tableLayoutPanel1.SetColumnSpan(lblDescription, 5);
                tableLayoutPanel1.SetColumn(flowLayoutPanel1, 6);
                tableLayoutPanel1.SetColumnSpan(flowLayoutPanel1, 2);
            }
            else
            {
                tableLayoutPanel1.SetColumnSpan(lblDescription, 4);
                tableLayoutPanel1.SetColumn(flowLayoutPanel1, 5);
                tableLayoutPanel1.SetColumnSpan(flowLayoutPanel1, 3);
            }
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            if (Run.AutoSplitter.IsActivated)
            {
                CurrentState.Settings.ActiveAutoSplitters.Remove(Run.GameName);
                Run.AutoSplitter.Deactivate();
            }
            else
            {
                CurrentState.Settings.ActiveAutoSplitters.Add(Run.GameName);
                Run.AutoSplitter.Activate(CurrentState);
                SetAutoSplitterSettings();
            }
            RefreshAutoSplittingUI();
        }

        private void SetAutoSplitterSettings()
        {
            if (Run.AutoSplitter.IsActivated
            && Run.AutoSplitterSettings != null
            && Run.AutoSplitterSettings.GetAttribute("gameName") == cbxGameName.Text)
                Run.AutoSplitter.Component.SetSettings(Run.AutoSplitterSettings);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var dialog = new ComponentSettingsDialog(Run.AutoSplitter.Component);
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var document = new XmlDocument();
                var autoSplitterSettings = document.CreateElement("AutoSplitterSettings");
                autoSplitterSettings.InnerXml = Run.AutoSplitter.Component.GetSettings(document).InnerXml;
                autoSplitterSettings.Attributes.Append(SettingsHelper.ToAttribute(document, "gameName", Run.GameName));
                Run.AutoSplitterSettings = autoSplitterSettings;
            }
        }

        private void btnWebsite_Click(object sender, EventArgs e)
        {
            Uri url = new Uri(Run.AutoSplitter.Website);
            if (url.Scheme == "http" || url.Scheme == "https")
            {
                try
                {
                    Process.Start(Run.AutoSplitter.Website);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            List<DataGridViewCell> selectedCells = runGrid.SelectedCells.Cast<DataGridViewCell>().OrderBy(o => o.RowIndex).ToList();

            var selectedInd = selectedCells.First().RowIndex;
            bool currCell = false;

            if (selectedCells != null)
            {
                foreach (DataGridViewCell selectedCell in selectedCells)
                {
                    selectedInd = selectedCell.RowIndex;
                    if (selectedInd > 0)
                    {
                        SwitchSegments(selectedInd - 1);

                        if (!currCell)
                        {
                            runGrid.CurrentCell = runGrid.Rows[selectedInd - 1].Cells[runGrid.CurrentCell.ColumnIndex];
                            currCell = true;
                        }

                        runGrid.Rows[selectedInd - 1].Cells[runGrid.CurrentCell.ColumnIndex].Selected = true;
                    }
                }
            }

            Fix();
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            List<DataGridViewCell> selectedCells = runGrid.SelectedCells.Cast<DataGridViewCell>().OrderByDescending(o => o.RowIndex).ToList();

            var selectedInd = selectedCells.First().RowIndex;
            bool currCell = false;

            if (selectedCells != null)
            {
                foreach (DataGridViewCell selectedCell in selectedCells)
                {
                    selectedInd = selectedCell.RowIndex;
                    if (selectedInd < SegmentList.Count - 1)
                    {
                        SwitchSegments(selectedInd);

                        if(!currCell)
                        {
                            runGrid.CurrentCell = runGrid.Rows[selectedInd + 1].Cells[runGrid.CurrentCell.ColumnIndex];
                            currCell = true;
                        }

                        runGrid.Rows[selectedInd + 1].Cells[runGrid.CurrentCell.ColumnIndex].Selected = true;
                    }
                }
            }

            Fix();
        }

        private void btnImportComparison_Click(object sender, EventArgs e)
        {
            ImportComparisonMenu.Show(MousePosition);
        }

        private void fromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var runImporter = new FileRunImporter();
            ImportClick(runImporter);
        }

        private void fromURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var runImporter = new URLRunImporter();
            ImportClick(runImporter);
        }

        private void fromSpeedruncomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var runImporter = new SpeedrunComRunImporter();
            ImportClick(runImporter);
        }

        private void ImportClick(IRunImporter importer)
        {
            var name = importer.ImportAsComparison(Run, this);
            if (name != null)
                AddComparisonColumn(name);
        }

        private void btnOther_Click(object sender, EventArgs e)
        {
            OtherMenu.Show(MousePosition);
        }

        private void clearHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Run.ClearHistory();
            Fix();
            RaiseRunEdited();
        }

        private void clearTimesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbxAttempts.Text = "0";
            Run.ClearTimes();
            RebuildComparisonColumns();
            Fix();
            TimesModified();
        }

        private void cleanSumOfBestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var alwaysCancel = false;
            var pastResponses = new Dictionary<string, bool>();
            SumOfBest.CleanUpCallback callback = parameters =>
                {
                    if (!alwaysCancel)
                    {
                        var formatter = new ShortTimeFormatter();
                        var messageText = formatter.Format(parameters.timeBetween) + " between "
                            + (parameters.startingSegment != null ? parameters.startingSegment.Name : "the start of the run") + " and " + parameters.endingSegment.Name
                            + (parameters.combinedSumOfBest != null ? ", which is faster than the Combined Best Segments of " + formatter.Format(parameters.combinedSumOfBest) : "");
                        if (parameters.attempt.Ended.HasValue)
                        {
                            messageText += " in a run on " + parameters.attempt.Ended.Value.Time.ToLocalTime().ToString("M/d/yyyy");
                        }

                        if (!pastResponses.ContainsKey(messageText))
                        {
                            var result = MessageBox.Show(this, "You had a " + (parameters.method == TimingMethod.RealTime ? "Real Time" : "Game Time") + " segment time of " + messageText + ". Do you think that this segment time is inaccurate and should be removed?", "Remove Time From Segment History?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            if (result == System.Windows.Forms.DialogResult.Yes)
                            {
                                pastResponses.Add(messageText, true);
                                return true;
                            }
                            else if (result == System.Windows.Forms.DialogResult.No)
                            {
                                pastResponses.Add(messageText, false);
                                return false;
                            }
                            alwaysCancel = true;
                        }
                        else
                            return pastResponses[messageText];
                    }
                    return false;
                };
            SumOfBest.Clean(Run, callback);
            RaiseRunEdited();
        }

        private void ClickControl(object sender, EventArgs e)
        {
            cbxGameName.CloseDropDown();
        }

        private void SetClickEvents(Control control)
        {
            foreach (Control childControl in control.Controls)
            {
                if (childControl is Label || childControl is TableLayoutPanel || childControl is PictureBox || childControl is FlowLayoutPanel)
                    SetClickEvents(childControl);
            }
            control.Click += ClickControl;
        }

        private void runGrid_DragDrop(object sender, DragEventArgs e)
        {
            if (DragIsValid(e))
            {
                Point dgvPos = runGrid.PointToClient(new Point(e.X, e.Y));
                DataGridView.HitTestInfo info = runGrid.HitTest(dgvPos.X, dgvPos.Y);
                var imagePath = (e.Data.GetData(DataFormats.FileDrop) as string[])?[0];
                ChangeImage(info.RowIndex, imagePath, false);
                runGrid.Refresh();
            }
        }

        private void runGrid_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragIsValid(e) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private bool DragIsValid(DragEventArgs e)
        {
            Point dgvPos = runGrid.PointToClient(new Point(e.X, e.Y));
            DataGridView.HitTestInfo info = runGrid.HitTest(dgvPos.X, dgvPos.Y);
            var imagePath = (e.Data.GetData(DataFormats.FileDrop) as string[])?[0];

            return e.Data.GetDataPresent(DataFormats.FileDrop)
                   && info.ColumnIndex != -1
                   && runGrid.Columns[info.ColumnIndex] is DataGridViewImageColumn
                   && info.Type == DataGridViewHitTestType.Cell;
        }
    }

    public class CustomAutoCompleteComboBox : ComboBox
    {
        private IList<string> _autoCompleteSource = null;
        private ToolStripDropDown _dropDown = null;
        private ListBox _box = null;
        private Form form;
        private SemaphoreSlim refreshDropDown;
        private string currentText = "";
        private string previousText = "";
        private bool taskCanceled = false;

        public IList<string> MyAutoCompleteSource
        {
            get { return _autoCompleteSource; }
            set { _autoCompleteSource = value; }
        }

        public Func<string, IEnumerable<string>> GetAllItemsForText { get; set; }

        public CustomAutoCompleteComboBox(Form controlForm) : base()
        {
            form = controlForm;
            form.FormClosed += Form_FormClosed;
            refreshDropDown = new SemaphoreSlim(0, 1);
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            taskCanceled = true;
            TryReleaseRefreshDropDown();
        }

        public void UpdateUI()
        {
            previousText = currentText;

            Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        refreshDropDown.Wait();
                        if (taskCanceled || IsDisposed)
                            return;

                        var text = currentText;
                        var legalStrings = GetAllItemsForText(text);

                        if (currentText == text && previousText != text)
                        {
                            if (text != "" && legalStrings.Count() > 0 && text != legalStrings.First())
                            {
                                form.InvokeIfRequired(() =>
                                {
                                    _box.Items.Clear();
                                    foreach (string str in legalStrings)
                                        _box.Items.Add(str);
                                    DroppedDown = false;
                                    _dropDown.Show(this, new Point(0, Height));
                                    previousText = text;
                                });

                            }
                            else
                            {
                                form.InvokeIfRequired(() =>
                                {
                                    CloseDropDown();
                                });
                            }
                        }
                    }
                });
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            _dropDown.AutoClose = false;

            currentText = Text;
            TryReleaseRefreshDropDown();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (!_dropDown.Focused)
                CloseDropDown();
        }

        private void TryReleaseRefreshDropDown()
        {
            if (refreshDropDown.CurrentCount == 0)
                refreshDropDown.Release();
        }

        public void CloseDropDown()
        {
            _dropDown.Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (_box.Items.Count > 0)
                    SelectedItem = _box.Items[0];
                CloseDropDown();
            }
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                e.SuppressKeyPress = true;
            base.OnKeyDown(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _dropDown = new ToolStripDropDown();
            _box = new ListBox();
            _box.Width = Width;
            _box.Click += (sender, arg) =>
            { Text = _box.SelectedItem as string; _dropDown.Close(); };
            ToolStripControlHost host = new ToolStripControlHost(_box);
            host.AutoSize = false;
            host.Margin = Padding.Empty;
            host.Padding = Padding.Empty;
            _dropDown.Items.Add(host);
            _dropDown.Height = _box.Height;
            _dropDown.AutoSize = false;
            _dropDown.Margin = Padding.Empty;
            _dropDown.Padding = Padding.Empty;
            _dropDown.Size = host.Size = _box.Size;
        }
    }
}
