using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
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
        public BrowseSplitsIODialog()
        {
            InitializeComponent();
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
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show("Download Failed!", "Download Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void splitsTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var node = e.Node;
            if (node.Tag is CategoryNodeAction)
            {
                ((CategoryNodeAction)node.Tag)();
                node.Tag = null;
            }
        }
    }
}
