using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

using DarkUI.Forms;

using LiveSplit.Racetime.Controller;
using LiveSplit.Racetime.Model;

using Microsoft.Web.WebView2.Core;

using Ookii.Dialogs.WinForms;

namespace LiveSplit.Racetime.View;

public partial class ChannelForm : DarkForm
{
    public RacetimeChannel Channel { get; set; }

    public ChannelForm(RacetimeChannel channel, string channelId, bool alwaysOnTop = true)
    {
        Channel = channel;
        Channel.Disconnected += Channel_Disconnected;
        Channel.RaceChanged += Channel_RaceChanged;
        Channel.Authorized += Channel_Authorized;
        InitializeComponent();

        string exePath = Assembly.GetEntryAssembly().CodeBase[8..];
        Icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);

        Load += OnLoaded;
        TopMost = alwaysOnTop;
        Show();
        chatBox.Hide();
        Text = "Connecting to " + channelId[(channelId.IndexOf('/') + 1)..];
        Channel.Connect(channelId);

        chatBox.SourceChanged += OnSourceChanged;
        chatBox.CoreWebView2InitializationCompleted += OnCoreWebViewCreated;
    }

    private async void OnLoaded(object sender, EventArgs e)
    {
        try
        {
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(userDataFolder: "WebView2_cache");
            await chatBox.EnsureCoreWebView2Async(environment);
        }
        catch
        {
            // handled in OnCoreWebViewCreated
        }
    }

    private void OnCoreWebViewCreated(object sender, CoreWebView2InitializationCompletedEventArgs e)
    {
        if (!e.IsSuccess)
        {
            ShowWebView2DownloadDialog();
            return;
        }

        chatBox.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
        chatBox.CoreWebView2.AddWebResourceRequestedFilter(Channel.FullWebRoot + "*", CoreWebView2WebResourceContext.All);
    }

    private void OnSourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
    {
        if (chatBox.Source.ToString() != Channel.FullWebRoot + Channel.Race.Id + "/livesplit")
        {
            if (retries >= 5)
            {
                loadMessage.BeginInvoke((Action)(() => loadMessage.Text = "Error loading page."));
                chatBox = null;
            }
            else
            {
                Channel_RaceChanged(null, null);
            }

            if (LastRetry.AddSeconds(10) >= DateTime.Now)
            {
                LastRetry = DateTime.Now;
                retries++;
            }
        }
        else
        {
            chatBox.BeginInvoke(() => chatBox.Show());
            retries = 0;
        }
    }

    private int retries = 0;
    private DateTime LastRetry = DateTime.Now;

    private async void Channel_RaceChanged(object sender, EventArgs e)
    {
        try
        {
            if (!IsDisposed)
            {
                Text = $"{Channel.Race.Goal} [{Channel.Race.GameName}] - {Channel.Race.ChannelName}";

                if (chatBox.Created)
                {
                    await Task.Delay(3000);

                    if (retries <= 5)
                    {
                        if (Channel.Token != null)
                        {
                            chatBox.BeginInvoke((Action)(() => chatBox.Source = new Uri(Channel.FullWebRoot + Channel.Race.Id + "/livesplit")));
                        }
                    }
                }
            }
        }
        catch { }
    }

    private void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        // Channel.Token.AccessToken
        if (!string.IsNullOrEmpty(Channel.Token.AccessToken))
        {
            CoreWebView2HttpRequestHeaders headers = e.Request.Headers;
            if (e.Request.Uri.ToLower().Contains(Properties.Resources.PROTOCOL_REST.ToLower() + "://" + Properties.Resources.DOMAIN.ToLower()))
            {
                headers.SetHeader("Authorization", $"Bearer {Channel.Token.AccessToken}");
            }
        }
    }

    private void Channel_Authorized(object sender, EventArgs e)
    {
        chatBox.BeginInvoke((Action)(() => Focus()));
    }

    private void Channel_Disconnected(object sender, EventArgs e)
    {
        if (!IsDisposed)
        {
            Text = "Disconnected";
        }
    }

    private void ChannelForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (Channel.Race?.State == RaceState.Started && Channel.PersonalStatus == UserStatus.Racing)
        {
            DialogResult r = MessageBox.Show("Do you want to FORFEIT before closing the window?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (r == DialogResult.Yes)
            {
                Channel.Forfeit();
            }
            else if (r == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
        }

        Channel.RemoveRaceComparisons();
        Channel.Authorized -= Channel_Authorized;
        Channel.RaceChanged -= Channel_RaceChanged;
        Channel.Disconnect();
    }

    private void ShowWebView2DownloadDialog()
    {
        if (InvokeRequired)
        {
            Invoke(() => ShowWebView2DownloadDialog());
            return;
        }

        var downloadButton = new TaskDialogButton("Download") { CommandLinkNote = "This will open in your default web browser." };
        var closeButton = new TaskDialogButton("Close") { CommandLinkNote = "LiveSplit.Racetime will not work until runtimes are installed." };

        var dialog = new TaskDialog(Container);
        dialog.CustomMainIcon = dialog.WindowIcon = Icon;
        dialog.MainInstruction = dialog.WindowTitle = "Microsoft Edge WebView2 Runtime Required";
        dialog.Content = "LiveSplit.Racetime requires the Microsoft Edge WebView2 Runtime to be installed on your machine in order to function. This should be included with Microsoft Edge, but we couldn't find it. Do you want to download it now?";
        dialog.Buttons.Add(downloadButton);
        dialog.Buttons.Add(closeButton);
        dialog.ButtonStyle = TaskDialogButtonStyle.CommandLinks;
        if (downloadButton == dialog.Show())
        {
            Process.Start("https://aka.ms/winui2/webview2download");
        }

        Close();
    }
}
