using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.Web;
using LiveSplit.Web.Share;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.View
{
    public partial class BrowseSpeedrunComDialog : Form
    {
        delegate void CategoryNodeAction();
        public IRun Run { get; protected set; }
        public string RunName { get; protected set; }

        struct TaggedRecord
        {
            public string GameID;
            public SpeedrunCom.Record Record;

            public TaggedRecord(string gameId, SpeedrunCom.Record record)
            {
                GameID = gameId;
                Record = record;
            }
        }

        public BrowseSpeedrunComDialog(bool isImporting = false, string gameName = null, string categoryName = null)
        {
            InitializeComponent();
            chkIncludeTimes.Visible = chkDownloadEmpty.Visible = !isImporting;

            if (!string.IsNullOrEmpty(gameName))
            {
                txtSearch.Text = gameName;
                btnSearch_Click(this, null);

                if (!string.IsNullOrEmpty(categoryName))
                {
                    if (splitsTreeView.Nodes.Count > 0)
                    {
                        var gameNode = splitsTreeView.Nodes[0];
                        gameNode.Expand();
                        var categoryNode = gameNode.Nodes.Cast<TreeNode>().FirstOrDefault(x => x.Text == categoryName);
                        if (categoryNode != null)
                        {
                            categoryNode.Expand();
                        }
                    }
                }
            }
        }

        private int getDigits(int n)
        {
            if (n == 0)
                return 1;

            return (int)Math.Floor(Math.Log10(n) + 1);
        }

        private string formatTime(Time time)
        {
            var formatter = new ShortTimeFormatter();

            if (time.RealTime.HasValue && !time.GameTime.HasValue)
                return formatter.Format(time.RealTime);
            else if (!time.RealTime.HasValue && time.GameTime.HasValue)
                return formatter.Format(time.GameTime);
            else
                return formatter.Format(time.RealTime) + " / " + formatter.Format(time.GameTime);
        }

        private string formatPlace(int? place)
        {
            if (place.HasValue)
            {
                var strPlace = place.Value.ToString(CultureInfo.InvariantCulture);
                var postfix = "th";

                if (place % 100 < 10 || place % 100 > 20)
                {
                    switch (place % 10)
                    {
                        case 1: postfix = "st"; break;
                        case 2: postfix = "nd"; break;
                        case 3: postfix = "rd"; break;
                    }
                }
                
                return " (" + strPlace + postfix + ")";
            }
            return "";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            splitsTreeView.Nodes.Clear();
            try
            {
                var fuzzyGameName = txtSearch.Text;
                if (!string.IsNullOrEmpty(fuzzyGameName))
                {
                    try
                    {
                        string actualGameName;
                        var leaderboards = SpeedrunCom.Instance.GetLeaderboards(fuzzyGameName, out actualGameName);
                        var gameId = SpeedrunCom.Instance.GetGameID(actualGameName);
                        var games = new[] { leaderboards };
                        foreach (var game in games)
                        {
                            var gameNode = new TreeNode(actualGameName);
                            gameNode.Tag = SpeedrunCom.Instance.GetGameUri(gameId);
                            var categories = game;
                            foreach (var category in categories)
                            {
                                var categoryNode = new TreeNode(category.Key);
                                categoryNode.Tag = SpeedrunCom.Instance.GetGameUri(gameId);
                                var records = category.Value;
                                var timingMethod = SpeedrunCom.Instance.GetLeaderboardTimingMethod(records);

                                foreach (var record in records)
                                {
                                    var place = record.Place.HasValue
                                        ? (record.Place.Value.ToString(CultureInfo.InvariantCulture).PadLeft(getDigits(records.Count())) + ". ")
                                        : "";
                                    var runText = place + (record.Time[timingMethod].HasValue ? new ShortTimeFormatter().Format(record.Time[timingMethod]) : "") + " by " + record.Runner;
                                    var runNode = new TreeNode(runText);
                                    runNode.Tag = new TaggedRecord(gameId, record);
                                    if (!record.RunAvailable)
                                        runNode.ForeColor = Color.Gray;
                                    categoryNode.Nodes.Add(runNode);
                                }
                                gameNode.Nodes.Add(categoryNode);
                            }
                            splitsTreeView.Nodes.Add(gameNode);
                        }
                    }
                    catch { }

                    try
                    {
                        var fuzzyUserName = txtSearch.Text;
                        var games = SpeedrunCom.Instance.GetPersonalBestList(fuzzyUserName);
                        var userName = fuzzyUserName;
                        var userNode = new TreeNode();
                        foreach (var game in games)
                        {
                            var gameNode = new TreeNode(game.Key);
                            var gameId = SpeedrunCom.Instance.GetGameID(game.Key);
                            gameNode.Tag = SpeedrunCom.Instance.GetGameUri(gameId);

                            foreach (var category in game.Value)
                            {
                                var categoryName = category.Key;
                                var record = category.Value;
                                userName = record.Runner;

                                var place = formatPlace(record.Place);
                                var runText = formatTime(record.Time) + " in " + categoryName + place;

                                var runNode = new TreeNode(runText);
                                runNode.Tag = new TaggedRecord(gameId, record);
                                if (!record.RunAvailable)
                                    runNode.ForeColor = Color.Gray;
                                gameNode.Nodes.Add(runNode);
                            }
                            userNode.Nodes.Add(gameNode);
                        }
                        userNode.Tag = SpeedrunCom.Instance.GetUserUri(userName);
                        userNode.Text = "@" + userName;
                        splitsTreeView.Nodes.Add(userNode);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show(this, "Search Failed!", "Search Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (splitsTreeView.SelectedNode != null && splitsTreeView.SelectedNode.Tag is TaggedRecord)
                {
                    var taggedRecord = (TaggedRecord)splitsTreeView.SelectedNode.Tag;
                    var record = taggedRecord.Record;
                    Run = record.Run.Value;
                    RunName = record.Runner;
                    var result = PostProcessRun(RunName);
                    if (result == DialogResult.OK)
                    {
                        DialogResult = result;
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show(this, "Download Failed!", "Download Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DialogResult PostProcessRun(string nodeText)
        {
            if (chkDownloadEmpty.Checked)
            {
                var name = nodeText;
                if (chkIncludeTimes.Checked)
                {
                    var succeededName = false;
                    do
                    {
                        var result = InputBox.Show(this, "Enter Comparison Name", "Name:", ref name);
                        if (result == DialogResult.Cancel)
                            return result;

                        if (name.StartsWith("[Race]"))
                        {
                            result = MessageBox.Show(this, "A Comparison name cannot start with [Race].", "Invalid Comparison Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                            if (result == DialogResult.Cancel)
                                return result;
                        }
                        else if (name == Model.Run.PersonalBestComparisonName)
                        {
                            result = MessageBox.Show(this, "A Comparison with this name already exists.", "Comparison Already Exists", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                            if (result == DialogResult.Cancel)
                                return result;
                        }
                        else
                            succeededName = true;
                    }
                    while (!succeededName);
                }
                Run.AttemptHistory.Clear();
                Run.AttemptCount = 0;
                Run.CustomComparisons.Clear();
                Run.CustomComparisons.Add(Model.Run.PersonalBestComparisonName);
                foreach (var segment in Run)
                {
                    segment.SegmentHistory.Clear();
                    var time = segment.PersonalBestSplitTime;
                    segment.Comparisons.Clear();
                    if (chkIncludeTimes.Checked)
                        segment.Comparisons[name] = time;
                    segment.PersonalBestSplitTime = default(Time);
                }
                if (chkIncludeTimes.Checked)
                {
                    Run.CustomComparisons.Add(name);
                    Run.FixSplits();
                }
            }
            return DialogResult.OK;
        }

        private void chkDownloadEmpty_CheckedChanged(object sender, EventArgs e)
        {
            chkIncludeTimes.Enabled = chkDownloadEmpty.Checked;
        }

        private void splitsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btnDownload.Enabled = e.Node.Tag is TaggedRecord && ((TaggedRecord)e.Node.Tag).Record.RunAvailable;
            btnShowOnSpeedrunCom.Enabled = e.Node.Tag is Uri || e.Node.Tag is TaggedRecord;
        }

        private void btnShowOnSpeedrunCom_Click(object sender, EventArgs e)
        {
            if (splitsTreeView.SelectedNode != null)
            {
                if (splitsTreeView.SelectedNode.Tag is Uri)
                {
                    var uri = (Uri)splitsTreeView.SelectedNode.Tag;
                    Process.Start(uri.AbsoluteUri);
                }
                else if (splitsTreeView.SelectedNode.Tag is TaggedRecord)
                {
                    var taggedRecord = (TaggedRecord)splitsTreeView.SelectedNode.Tag;
                    var uri = SpeedrunCom.Instance.GetRunUri(taggedRecord.GameID, taggedRecord.Record.ID);
                    Process.Start(uri.AbsoluteUri);
                }
            }
        }
    }
}
