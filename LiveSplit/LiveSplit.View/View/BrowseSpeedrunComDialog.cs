using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.Web;
using LiveSplit.Web.Share;
using System;
using System.Collections.Generic;
using System.Data;
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

        public BrowseSpeedrunComDialog(bool isImporting = false)
        {
            InitializeComponent();
            chkIncludeTimes.Visible = chkDownloadEmpty.Visible = !isImporting;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            splitsTreeView.Nodes.Clear();
            try
            {
                var searchText = txtSearch.Text;
                if (!string.IsNullOrEmpty(searchText))
                {
                    var leaderboards = SpeedrunCom.Instance.GetLeaderboards(searchText);
                    var games = new[] { leaderboards };
                    foreach (var game in games)
                    {
                        var gameNode = new TreeNode(searchText);
                        var categories = game;
                        foreach (var category in categories)
                        {
                            var categoryNode = new TreeNode(category.Key);
                            var records = category.Value;
                            foreach (var record in records)
                            {
                                var runText = record.Place + ". " + (record.Time.RealTime.HasValue ? new ShortTimeFormatter().Format(record.Time.RealTime) + " " : "") + "by " + record.Runner;
                                var runNode = new TreeNode(runText);
                                runNode.Tag = record;
                                if (!record.RunAvailable)
                                    runNode.ForeColor = Color.Gray;
                                categoryNode.Nodes.Add(runNode);
                            }
                            gameNode.Nodes.Add(categoryNode);
                        }
                        splitsTreeView.Nodes.Add(gameNode);
                    }

                    /*var user = SpeedrunCom.Instance.GetPersonalBestList(searchText);
                    if (user != null)
                    {
                        var userNode = new TreeNode("@" + user.name);
                        var runs = SplitsIO.Instance.GetRunsForUser((string)user.name);
                        runs = runs.OrderBy(run => (string)run.name).ThenBy(run => (double)run.time);
                        foreach (var run in runs)
                        {
                            var runText = run.name;
                            if (run.time != SplitsIO.NoTime)
                                runText += " in " + (new ShortTimeFormatter()).Format(TimeSpan.FromSeconds((double)run.time));
                            var runNode = new TreeNode(runText);
                            runNode.Tag = run;
                            userNode.Nodes.Add(runNode);
                        }
                        splitsTreeView.Nodes.Add(userNode);
                    }*/
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
                if (splitsTreeView.SelectedNode != null && splitsTreeView.SelectedNode.Tag is SpeedrunCom.Record)
                {
                    var record = (SpeedrunCom.Record)splitsTreeView.SelectedNode.Tag;
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

        private void splitsTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                var node = e.Node;
                if (node.Tag is CategoryNodeAction)
                {
                    ((CategoryNodeAction)node.Tag)();
                    node.Tag = null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                e.Cancel = true;
            }
        }

        private void chkDownloadEmpty_CheckedChanged(object sender, EventArgs e)
        {
            chkIncludeTimes.Enabled = chkDownloadEmpty.Checked;
        }

        private void splitsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btnDownload.Enabled = e.Node.Tag is DynamicJsonObject;
        }
    }
}
