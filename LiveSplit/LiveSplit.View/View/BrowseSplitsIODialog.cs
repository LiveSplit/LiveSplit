using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.Web;
using LiveSplit.Web.Share;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.View
{
    public partial class BrowseSplitsIODialog : Form
    {
        delegate void CategoryNodeAction();
        public IRun Run { get; protected set; }
        public string RunName { get; protected set; }

        public BrowseSplitsIODialog(bool isImporting = false)
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
                    var games = SplitsIO.Instance.SearchGame(searchText);
                    games = games.OrderBy(game => game.name);
                    foreach (var game in games)
                    {
                        var gameNode = new TreeNode(game.name);
                        IEnumerable<dynamic> categories = game.categories;
                        categories = categories.OrderBy(category => category.name);
                        foreach (var category in categories)
                        {
                            var categoryNode = new TreeNode(category.name);
                            categoryNode.Nodes.Add(new TreeNode(""));
                            categoryNode.Tag = new CategoryNodeAction(() =>
                            {
                                categoryNode.Nodes.Clear();
                                IEnumerable<dynamic> runs = SplitsIO.Instance.GetRunsForCategory(category.id);
                                runs = runs.OrderBy(x => x.time != SplitsIO.NoTime ? double.Parse(x.time, CultureInfo.InvariantCulture) : double.MaxValue);
                                foreach (var run in runs)
                                {
                                    var runText = run.time != SplitsIO.NoTime ? (new ShortTimeFormatter()).Format(TimeSpan.FromSeconds(double.Parse(run.time, CultureInfo.InvariantCulture))) : "No Final Time";
                                    if (run.user != null && !string.IsNullOrEmpty(run.user.name))
                                        runText += " by " + run.user.name;
                                    var runNode = new TreeNode(runText);
                                    runNode.Tag = run;
                                    categoryNode.Nodes.Add(runNode);
                                }
                            });
                            gameNode.Nodes.Add(categoryNode);
                        }
                        splitsTreeView.Nodes.Add(gameNode);
                    }

                    var users = SplitsIO.Instance.SearchUser(searchText);
                    users = users.OrderBy(user => user.name);
                    foreach (var user in users)
                    {
                        var userNode = new TreeNode("@" + user.name);
                        var runs = SplitsIO.Instance.GetRunsForUser((int)user.id);
                        runs = runs.OrderBy(run => run.name).ThenBy(run => double.Parse(run.time, CultureInfo.InvariantCulture));
                        foreach (var run in runs)
                        {
                            var runText = run.name;
                            if (run.time != SplitsIO.NoTime)
                                runText += " in " + (new ShortTimeFormatter()).Format(TimeSpan.FromSeconds(double.Parse(run.time, CultureInfo.InvariantCulture)));
                            var runNode = new TreeNode(runText);
                            runNode.Tag = run;
                            userNode.Nodes.Add(runNode);
                        }
                        splitsTreeView.Nodes.Add(userNode);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show("Search Failed!", "Search Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (splitsTreeView.SelectedNode != null && splitsTreeView.SelectedNode.Tag != null && splitsTreeView.SelectedNode.Tag is DynamicJsonObject)
                {
                    dynamic run = splitsTreeView.SelectedNode.Tag;
                    Run = SplitsIO.Instance.DownloadRunByPath((string)run.path);
                    RunName = splitsTreeView.SelectedNode.Text;
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
                MessageBox.Show("Download Failed!", "Download Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        var result = InputBox.Show("Enter Comparison Name", "Name:", ref name);
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
                Run.RunHistory.Clear();
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
    }
}
