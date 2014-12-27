using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.Web;
using LiveSplit.Web.Share;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.View
{
    public partial class BrowseSplitsIODialog : Form
    {
        delegate void CategoryNodeAction();
        public IRun Run { get; set; }
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
                if (!String.IsNullOrEmpty(searchText))
                {
                    var games = SplitsIO.Instance.SearchGame(searchText);
                    foreach (var game in games)
                    {
                        var gameNode = new TreeNode(game.name);
                        var categories = game.categories;
                        foreach (var category in categories)
                        {
                            var categoryNode = new TreeNode(category.name);
                            categoryNode.Nodes.Add(new TreeNode(""));
                            categoryNode.Tag = new CategoryNodeAction(() =>
                            {
                                categoryNode.Nodes.Clear();
                                var runs = SplitsIO.Instance.GetRunsForCategory(category.id);
                                foreach (var run in runs)
                                {
                                    var runText = (new ShortTimeFormatter()).Format(TimeSpan.FromSeconds(Double.Parse(run.time ?? "0.0")));
                                    if (run.user != null)
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
                    Run = SplitsIO.Instance.DownloadRunByPath((String)run.path);
                    var result = PostProcessRun(splitsTreeView.SelectedNode.Text);
                    if (result == System.Windows.Forms.DialogResult.OK)
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

        private DialogResult PostProcessRun(String nodeText)
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
                        if (result == System.Windows.Forms.DialogResult.Cancel)
                            return result;

                        if (name.StartsWith("[Race]"))
                        {
                            result = MessageBox.Show(this, "A Comparison name cannot start with [Race].", "Invalid Comparison Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                            if (result == System.Windows.Forms.DialogResult.Cancel)
                                return result;
                        }
                        else if (name == LiveSplit.Model.Run.PersonalBestComparisonName)
                        {
                            result = MessageBox.Show(this, "A Comparison with this name already exists.", "Comparison Already Exists", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                            if (result == System.Windows.Forms.DialogResult.Cancel)
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
                Run.CustomComparisons.Add(LiveSplit.Model.Run.PersonalBestComparisonName);
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
