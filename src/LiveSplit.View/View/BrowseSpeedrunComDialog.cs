using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.Web.Share;

using SpeedrunComSharp;

namespace LiveSplit.View;

public partial class BrowseSpeedrunComDialog : Form
{
    public IRun Run { get; protected set; }
    public string RunName { get; protected set; }
    private readonly bool isImporting;

    private readonly BackgroundWorker searchWorker = new()
    {
        WorkerSupportsCancellation = true,
    };

    public BrowseSpeedrunComDialog(bool isImporting = false, string gameName = null, string categoryName = null)
    {
        InitializeComponent();
        this.isImporting = isImporting;
        chkIncludeTimes.Visible = chkDownloadEmpty.Visible = !isImporting;
        searchWorker.DoWork += new DoWorkEventHandler(btnSearchWorkerTask);

        if (!string.IsNullOrEmpty(gameName))
        {
            txtSearch.Text = gameName;
            btnSearch_Click(this, null);

            if (!string.IsNullOrEmpty(categoryName))
            {
                if (splitsTreeView.Nodes.Count > 0)
                {
                    TreeNode gameNode = splitsTreeView.Nodes[0];
                    gameNode.Expand();
                    TreeNode categoryNode = gameNode.Nodes.Cast<TreeNode>().FirstOrDefault(x => x.Text == categoryName);
                    categoryNode?.Expand();
                }
            }
        }
    }

    private static int getDigits(int n)
    {
        if (n == 0)
        {
            return 1;
        }

        return (int)Math.Floor(Math.Log10(n) + 1);
    }

    private static string formatPlace(int? place)
    {
        if (place.HasValue)
        {
            string strPlace = place.Value.ToString(CultureInfo.InvariantCulture);
            string postfix = "th";

            if (place % 100 is < 10 or > 20)
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
        if (searchWorker.CancellationPending)
        {
            return;
        }

        if (searchWorker.IsBusy)
        {
            searchWorker.CancelAsync();
            while (searchWorker.CancellationPending)
            {
                Application.DoEvents();
            }
        }

        searchWorker.RunWorkerAsync();
    }

    private void btnSearchWorkerTask(object sender, EventArgs e)
    {
        splitsTreeView.Nodes.Clear();
        try
        {
            string fuzzyGameName = txtSearch.Text;
            if (!string.IsNullOrEmpty(fuzzyGameName))
            {
                try
                {
                    System.Collections.Generic.IEnumerable<Game> games = SpeedrunCom.Client.Games.GetGames(name: fuzzyGameName, embeds: new GameEmbeds(embedCategories: true));

                    foreach (Game game in games)
                    {
                        if (searchWorker.CancellationPending)
                        {
                            return;
                        }

                        var gameNode = new TreeNode(game.Name)
                        {
                            Tag = game.WebLink
                        };
                        System.Collections.Generic.IEnumerable<Category> categories = game.FullGameCategories;
                        var timeFormatter = new AutomaticPrecisionTimeFormatter();

                        bool anySplitsAvailableForGame = false;
                        foreach (Category category in categories)
                        {
                            if (searchWorker.CancellationPending)
                            {
                                return;
                            }

                            var categoryNode = new TreeNode(category.Name)
                            {
                                Tag = category.WebLink
                            };
                            Leaderboard leaderboard = category.Leaderboard;
                            System.Collections.ObjectModel.ReadOnlyCollection<Record> records = leaderboard.Records;

                            bool anySplitsAvailableForCategory = false;
                            foreach (Record record in records)
                            {
                                if (searchWorker.CancellationPending)
                                {
                                    return;
                                }

                                string place = record.Rank.ToString(CultureInfo.InvariantCulture).PadLeft(getDigits(records.Count())) + ".   ";
                                string runners = string.Join(" & ", record.Players.Select(x => x.Name));
                                TimeSpan? time = record.Times.Primary;
                                string runText = place + (time.HasValue ? timeFormatter.Format(time) : "") + " by " + runners;
                                var runNode = new TreeNode(runText)
                                {
                                    Tag = record
                                };
                                if (!record.SplitsAvailable)
                                {
                                    runNode.ForeColor = Color.Gray;
                                }
                                else
                                {
                                    anySplitsAvailableForCategory = true;
                                }

                                categoryNode.Nodes.Add(runNode);
                            }

                            if (!anySplitsAvailableForCategory)
                            {
                                categoryNode.ForeColor = Color.Gray;
                            }
                            else
                            {
                                anySplitsAvailableForGame = true;
                            }

                            gameNode.Nodes.Add(categoryNode);
                        }

                        if (!anySplitsAvailableForGame)
                        {
                            gameNode.ForeColor = Color.Gray;
                        }

                        splitsTreeView.Invoke((MethodInvoker)delegate
                        {
                            splitsTreeView.Nodes.Add(gameNode);
                        });
                    }
                }
                catch { }

                try
                {
                    string fuzzyUserName = txtSearch.Text.TrimStart('@');
                    System.Collections.Generic.IEnumerable<User> users = SpeedrunCom.Client.Users.GetUsersFuzzy(fuzzyName: fuzzyUserName);

                    foreach (User user in users)
                    {
                        if (searchWorker.CancellationPending)
                        {
                            return;
                        }

                        var userNode = new TreeNode("@" + user.Name)
                        {
                            Tag = user.WebLink
                        };
                        System.Collections.Generic.IEnumerable<IGrouping<string, Record>> recordsGroupedByGames = SpeedrunCom.Client.Users.GetPersonalBests(user.ID, embeds: new RunEmbeds(embedGame: true, embedCategory: true))
                            .GroupBy(x => x.Game.Name);

                        bool anySplitsAvailableForUser = false;
                        foreach (IGrouping<string, Record> recordsForGame in recordsGroupedByGames)
                        {
                            if (searchWorker.CancellationPending)
                            {
                                return;
                            }

                            string gameName = recordsForGame.Key;
                            var gameNode = new TreeNode(gameName);
                            Game game = recordsForGame.First().Game;
                            var timeFormatter = new AutomaticPrecisionTimeFormatter();
                            gameNode.Tag = game.WebLink;

                            bool anySplitsAvailableForGame = false;
                            foreach (Record record in recordsForGame)
                            {
                                if (searchWorker.CancellationPending)
                                {
                                    return;
                                }

                                string categoryName = record.Category.Name;

                                string place = formatPlace(record.Rank);
                                string coopRunners = record.Players.Count() > 1 ? " by " + string.Join(" & ", record.Players.Select(x => x.Name)) : "";
                                string recordText = timeFormatter.Format(record.Times.Primary) + " in " + categoryName + coopRunners + place;

                                var recordNode = new TreeNode(recordText)
                                {
                                    Tag = record
                                };
                                if (!record.SplitsAvailable)
                                {
                                    recordNode.ForeColor = Color.Gray;
                                }
                                else
                                {
                                    anySplitsAvailableForGame = true;
                                }

                                gameNode.Nodes.Add(recordNode);
                            }

                            if (!anySplitsAvailableForGame)
                            {
                                gameNode.ForeColor = Color.Gray;
                            }
                            else
                            {
                                anySplitsAvailableForUser = true;
                            }

                            userNode.Nodes.Add(gameNode);
                        }

                        if (!anySplitsAvailableForUser)
                        {
                            userNode.ForeColor = Color.Gray;
                        }

                        splitsTreeView.Invoke((MethodInvoker)delegate
                        {
                            splitsTreeView.Nodes.Add(userNode);
                        });
                    }
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
            if (splitsTreeView.SelectedNode != null && splitsTreeView.SelectedNode.Tag is Record record)
            {
                Run = record.GetRun();

                if (!isImporting)
                {
                    Run.PatchRun(record);
                }

                string runners = string.Join(" & ", record.Players.Select(x => x.Name));
                RunName = runners;
                DialogResult result = PostProcessRun(RunName);
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
            string name = nodeText;
            if (chkIncludeTimes.Checked)
            {
                bool succeededName = false;
                do
                {
                    DialogResult result = InputBox.Show(this, "Enter Comparison Name", "Name:", ref name);
                    if (result == DialogResult.Cancel)
                    {
                        return result;
                    }

                    if (name.StartsWith("[Race]"))
                    {
                        result = MessageBox.Show(this, "A Comparison name cannot start with [Race].", "Invalid Comparison Name", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                        if (result == DialogResult.Cancel)
                        {
                            return result;
                        }
                    }
                    else if (name == Model.Run.PersonalBestComparisonName)
                    {
                        result = MessageBox.Show(this, "A Comparison with this name already exists.", "Comparison Already Exists", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                        if (result == DialogResult.Cancel)
                        {
                            return result;
                        }
                    }
                    else
                    {
                        succeededName = true;
                    }
                }
                while (!succeededName);
            }

            Time[] pbTimes = Run.Select(x => x.PersonalBestSplitTime).ToArray();
            Run.ClearTimes();
            if (chkIncludeTimes.Checked)
            {
                for (int index = 0; index < Run.Count; index++)
                {
                    Run[index].Comparisons[name] = pbTimes[index];
                }

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
        btnDownload.Enabled = e.Node.Tag is Record record && record.SplitsAvailable;
        btnShowOnSpeedrunCom.Enabled = e.Node.Tag is Uri or Record;
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
            else if (splitsTreeView.SelectedNode.Tag is Record record)
            {
                Uri uri = record.WebLink;
                Process.Start(uri.AbsoluteUri);
            }
        }
    }
}
