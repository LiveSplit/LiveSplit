﻿using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.Web.Share;
using SpeedrunComSharp;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.View
{
    public partial class BrowseSpeedrunComDialog : Form
    {
        public IRun Run { get; protected set; }
        public string RunName { get; protected set; }
        private bool isImporting;

        private readonly BackgroundWorker searchWorker = new BackgroundWorker
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
            RefreshText();
        }

        private static int getDigits(int n)
        {
            if (n == 0)
                return 1;

            return (int)Math.Floor(Math.Log10(n) + 1);
        }

        private static string formatTime(Time time)
        {
            var formatter = new ShortTimeFormatter();

            if (time.RealTime.HasValue && !time.GameTime.HasValue)
                return formatter.Format(time.RealTime);

            if (!time.RealTime.HasValue && time.GameTime.HasValue)
                return formatter.Format(time.GameTime);

            return formatter.Format(time.RealTime) + " / " + formatter.Format(time.GameTime);
        }

        private static string formatPlace(int? place)
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


        private void btnSearchWorkerTask(object sender, EventArgs e) {
            splitsTreeView.Nodes.Clear();
            try
            {
                var fuzzyGameName = txtSearch.Text;
                if (!string.IsNullOrEmpty(fuzzyGameName))
                {
                    try
                    {
                        var games = SpeedrunCom.Client.Games.GetGames(name: fuzzyGameName, embeds: new GameEmbeds(embedCategories: true));

                        foreach (var game in games)
                        {
                            if (searchWorker.CancellationPending)
                            {
                                return;
                            }
                            var gameNode = new TreeNode(game.Name);
                            gameNode.Tag = game.WebLink;
                            var categories = game.FullGameCategories;
                            var timeFormatter = new AutomaticPrecisionTimeFormatter();

                            var anySplitsAvailableForGame = false;
                            foreach (var category in categories)
                            {
                                if (searchWorker.CancellationPending)
                                {
                                    return;
                                }
                                var categoryNode = new TreeNode(category.Name);
                                categoryNode.Tag = category.WebLink;
                                var leaderboard = category.Leaderboard;
                                var records = leaderboard.Records;

                                var anySplitsAvailableForCategory = false;
                                foreach (var record in records)
                                {
                                    if (searchWorker.CancellationPending)
                                    {
                                        return;
                                    }
                                    var place = record.Rank.ToString(CultureInfo.InvariantCulture).PadLeft(getDigits(records.Count())) + ".   ";
                                    var runners = string.Join(" & ", record.Players.Select(x => x.Name));
                                    var time = record.Times.Primary;
                                    var runText = place + (time.HasValue ? timeFormatter.Format(time) : "") + " by " + runners;
                                    var runNode = new TreeNode(runText);
                                    runNode.Tag = record;
                                    if (!record.SplitsAvailable)
                                        runNode.ForeColor = Color.Gray;
                                    else
                                        anySplitsAvailableForCategory = true;
                                    categoryNode.Nodes.Add(runNode);
                                }

                                if (!anySplitsAvailableForCategory)
                                    categoryNode.ForeColor = Color.Gray;
                                else 
                                    anySplitsAvailableForGame = true;

                                gameNode.Nodes.Add(categoryNode);
                            }
                            if (!anySplitsAvailableForGame)
                                gameNode.ForeColor = Color.Gray;
                            splitsTreeView.Invoke((MethodInvoker)delegate {
                                splitsTreeView.Nodes.Add(gameNode);
                            });
                        }
                    }
                    catch { }

                    try
                    {
                        var fuzzyUserName = txtSearch.Text.TrimStart('@');
                        var users = SpeedrunCom.Client.Users.GetUsersFuzzy(fuzzyName: fuzzyUserName);

                        foreach (var user in users)
                        {
                            if (searchWorker.CancellationPending)
                            {
                                return;
                            }
                            var userNode = new TreeNode("@" + user.Name);
                            userNode.Tag = user.WebLink;
                            var recordsGroupedByGames = SpeedrunCom.Client.Users.GetPersonalBests(user.ID, embeds: new RunEmbeds(embedGame: true, embedCategory: true))
                                .GroupBy(x => x.Game.Name);

                            var anySplitsAvailableForUser = false;
                            foreach (var recordsForGame in recordsGroupedByGames)
                            {
                                if (searchWorker.CancellationPending)
                                {
                                    return;
                                }
                                var gameName = recordsForGame.Key;
                                var gameNode = new TreeNode(gameName);
                                var game = recordsForGame.First().Game;
                                var timeFormatter = new AutomaticPrecisionTimeFormatter();
                                gameNode.Tag = game.WebLink;

                                var anySplitsAvailableForGame = false;
                                foreach (var record in recordsForGame)
                                {
                                    if (searchWorker.CancellationPending)
                                    {
                                        return;
                                    }
                                    var categoryName = record.Category.Name;

                                    var place = formatPlace(record.Rank);
                                    var coopRunners = record.Players.Count() > 1 ? " by " + string.Join(" & ", record.Players.Select(x => x.Name)) : "";
                                    var recordText = timeFormatter.Format(record.Times.Primary) + " in " + categoryName + coopRunners + place;

                                    var recordNode = new TreeNode(recordText);
                                    recordNode.Tag = record;
                                    if (!record.SplitsAvailable)
                                        recordNode.ForeColor = Color.Gray;
                                    else
                                        anySplitsAvailableForGame = true;
                                    gameNode.Nodes.Add(recordNode);
                                }

                                if (!anySplitsAvailableForGame)
                                    gameNode.ForeColor = Color.Gray;
                                else
                                    anySplitsAvailableForUser = true;
                                userNode.Nodes.Add(gameNode);
                            }
                            if (!anySplitsAvailableForUser)
                                userNode.ForeColor = Color.Gray;
                            splitsTreeView.Invoke((MethodInvoker)delegate {
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
                MessageBox.Show(
                    this, 
                    Languages.Instance.GetText("SearchFailedText", "Search Failed!"), 
                    Languages.Instance.GetText("SearchFailedCaption", "Search Failed"), 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (splitsTreeView.SelectedNode != null && splitsTreeView.SelectedNode.Tag is Record)
                {
                    var record = (Record)splitsTreeView.SelectedNode.Tag;
                    Run = record.GetRun();
                    
                    if (!isImporting)
                        Run.PatchRun(record);
                    
                    var runners = string.Join(" & ", record.Players.Select(x => x.Name));
                    RunName = runners;
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
                MessageBox.Show(
                    this, 
                    Languages.Instance.GetText("DownloadFailedText", "Download Failed!"), 
                    Languages.Instance.GetText("DownloadFailedCaption", "Download Failed"), 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
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
                        var result = InputBox.Show(
                            this, 
                            Languages.Instance.GetText("EnterComparisonName", "Enter Comparison Name"), 
                            Languages.Instance.GetText("Name", "Name:"), 
                            ref name);
                        if (result == DialogResult.Cancel)
                            return result;

                        if (name.StartsWith("[Race]"))
                        {
                            result = MessageBox.Show(
                                this, 
                                Languages.Instance.GetText("ComparisonText", "A Comparison name cannot start with [Race]."), 
                                Languages.Instance.GetText("InvalidComparisonName", "Invalid Comparison Name"), 
                                MessageBoxButtons.RetryCancel, 
                                MessageBoxIcon.Error);
                            if (result == DialogResult.Cancel)
                                return result;
                        }
                        else if (name == Model.Run.PersonalBestComparisonName)
                        {
                            result = MessageBox.Show(
                                this, 
                                Languages.Instance.GetText("ComparisonText_1", "A Comparison with this name already exists."), 
                                Languages.Instance.GetText("ComparisonAlreadyExists", "Comparison Already Exists"), 
                                MessageBoxButtons.RetryCancel, 
                                MessageBoxIcon.Error);
                            if (result == DialogResult.Cancel)
                                return result;
                        }
                        else
                            succeededName = true;
                    }
                    while (!succeededName);
                }
                var pbTimes = Run.Select(x => x.PersonalBestSplitTime).ToArray();
                Run.ClearTimes();
                if (chkIncludeTimes.Checked)
                {
                    for (var index = 0; index < Run.Count; index++)
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
            btnDownload.Enabled = e.Node.Tag is Record && ((Record)e.Node.Tag).SplitsAvailable;
            btnShowOnSpeedrunCom.Enabled = e.Node.Tag is Uri || e.Node.Tag is Record;
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
                else if (splitsTreeView.SelectedNode.Tag is Record)
                {
                    var record = (Record)splitsTreeView.SelectedNode.Tag;
                    var uri = record.WebLink;
                    Process.Start(uri.AbsoluteUri);
                }
            }
        }

        private void RefreshText ()
        {
            btnSearch.Text = Languages.Instance.GetText("btnSearch", "Search");
            btnDownload.Text = Languages.Instance.GetText("btnDownload", "Download");
            chkDownloadEmpty.Text = Languages.Instance.GetText("chkDownloadEmpty", "Download Empty");
            chkIncludeTimes.Text = Languages.Instance.GetText("chkIncludeTimes", "Include Times as Comparison");
            btnShowOnSpeedrunCom.Text = Languages.Instance.GetText("btnShowOnSpeedrunCom", "Show on Speedrun.com");
            Text = Languages.Instance.GetText("BrowseSpeedrunComDialog", "Browse Speedrun.com");
        }
    }
}
