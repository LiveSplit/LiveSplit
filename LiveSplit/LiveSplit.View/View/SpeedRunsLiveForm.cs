using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.Utils;
using LiveSplit.Web;
using LiveSplit.Web.Share;
using LiveSplit.Web.SRL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.View
{
    public partial class SpeedRunsLiveForm : Form
    {
        SpeedRunsLiveIRC SRLClient { get; set; }

        protected bool FormIsClosing { get; set; }

        protected string RaceId { get; set; }
        protected string GameId { get; set; }
        protected string GameCategory { get; set; }

        public SpeedRunsLiveForm(LiveSplitState state, ITimerModel model, string raceId)
        {
            DownloadAllEmotes();
            RaceId = raceId;
            GameCategory = null;
            var raceChannel = string.Format("#srl-{0}", raceId);
            var liveSplitChannel = string.Format("{0}-livesplit", raceChannel);
            SRLClient = new SpeedRunsLiveIRC(state, model, new[] { "#speedrunslive", raceChannel, liveSplitChannel });
            SRLClient.ChannelJoined += SRLClient_ChannelJoined;
            SRLClient.RawMessageReceived += SRLClient_RawMessageReceived;
            SRLClient.MessageReceived += SRLClient_MessageReceived;
            SRLClient.StateChanged += SRLClient_StateChanged;
            SRLClient.UserListRefreshed += SRLClient_UserListRefreshed;
            SRLClient.GoalChanged += SRLClient_GoalChanged;
            SRLClient.PasswordIncorrect += SRLClient_PasswordIncorrect;
            SRLClient.NicknameInUse += SRLClient_NicknameInUse;
            SRLClient.Disconnected += SRLClient_Disconnected;
            SRLClient.Kicked += SRLClient_Kicked;
            InitializeComponent();
            SRLClient_StateChanged(null, RaceState.NotInRace);
            btnJoinQuit.Enabled = false;
            FormIsClosing = false;
        }

        void SRLClient_Kicked(object sender, EventArgs e)
        {
            if (IsDisposed)
                return;

            this.InvokeIfRequired(() =>
            {
                if (!FormIsClosing)
                {
                    MessageBox.Show(this, "You have been kicked from the IRC Channel.", "Kicked From Channel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            });
        }

        void SRLClient_Disconnected(object sender, EventArgs e)
        {
            if (IsDisposed)
                return;

            try
            {
                this.InvokeIfRequired(() =>
                {
                    if (!FormIsClosing)
                    {
                        MessageBox.Show(this, "You have been disconnected from the IRC server.", "Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Close();
                    }
                });
            }
            catch { }
        }

        void SRLClient_NicknameInUse(object sender, EventArgs e)
        {
            if (IsDisposed)
                return;

            FormIsClosing = true;

            this.InvokeIfRequired(() =>
            {
                MessageBox.Show(this, "Your nickname is already in use.", "Nickname In Use", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            });
        }

        void SRLClient_PasswordIncorrect(object sender, EventArgs e)
        {
            if (IsDisposed)
                return;

            FormIsClosing = true;

            this.InvokeIfRequired(() =>
            {
                MessageBox.Show(this, "The password is incorrect.", "Password Incorrect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            });
        }

        public SpeedRunsLiveForm(LiveSplitState state, ITimerModel model, string gameName, string gameID, string gameCategory)
        {
            DownloadAllEmotes();
            GameId = gameID;
            GameCategory = gameCategory;
            SRLClient = new SpeedRunsLiveIRC(state, model, new[] { "#speedrunslive" });
            SRLClient.GameName = gameName;
            SRLClient.ChannelJoined += SRLClient_ChannelJoined;
            SRLClient.RawMessageReceived += SRLClient_RawMessageReceived;
            SRLClient.MessageReceived += SRLClient_MessageReceived;
            SRLClient.StateChanged += SRLClient_StateChanged;
            SRLClient.UserListRefreshed += SRLClient_UserListRefreshed;
            SRLClient.GoalChanged += SRLClient_GoalChanged;
            SRLClient.PasswordIncorrect += SRLClient_PasswordIncorrect;
            SRLClient.NicknameInUse += SRLClient_NicknameInUse;
            SRLClient.Disconnected += SRLClient_Disconnected;
            SRLClient.Kicked += SRLClient_Kicked;
            InitializeComponent();
            SRLClient_StateChanged(null, RaceState.NotInRace);
            btnJoinQuit.Enabled = false;
            FormIsClosing = false;
        }

        protected void DownloadAllEmotes()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    this.InvokeIfRequired(TwitchEmoteResolver.DownloadTwitchEmotesList);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });
        }

        void SRLClient_GoalChanged(object sender, EventArgs e)
        {
            if (IsDisposed)
                return;

            this.InvokeIfRequired(() =>
            {
                Text = SRLClient.ChannelTopic;
            });
        }

        void SRLClient_UserListRefreshed(object sender, EventArgs e)
        {
            if (IsDisposed)
                return;

            RebuildUserList();
        }

        void SRLClient_StateChanged(object sender, RaceState state)
        {
            if (IsDisposed)
                return;

            this.InvokeIfRequired(() =>
            {
                if (state == RaceState.EnteredRace)
                {
                    chkReady.Enabled = true;
                    chkReady.Checked = false;
                    btnJoinQuit.Text = "Quit Race";
                    btnJoinQuit.Enabled = true;
                }
                else if (state == RaceState.EnteredRaceAndReady)
                {
                    chkReady.Enabled = true;
                    chkReady.Checked = true;
                    btnJoinQuit.Text = "Quit Race";
                    btnJoinQuit.Enabled = true;
                }
                else if (state == RaceState.NotInRace)
                {
                    chkReady.Enabled = false;
                    chkReady.Checked = false;
                    btnJoinQuit.Text = "Enter Race";
                    btnJoinQuit.Enabled = true;
                }
                else if (state == RaceState.RaceEnded)
                {
                    chkReady.Enabled = false;
                    chkReady.Checked = true;
                    btnJoinQuit.Text = "Rejoin Race";
                    btnJoinQuit.Enabled = true;
                }
                else if (state == RaceState.RaceStarted)
                {
                    chkReady.Enabled = false;
                    chkReady.Checked = true;
                    btnJoinQuit.Text = "Forfeit Race";
                    btnJoinQuit.Enabled = true;
                }
            });
        }

        void ExitMessageReceived(object sender, Tuple<string, SRLIRCUser, string> e)
        {
            if (IsDisposed)
                return;

            if (e.Item1 == SRLClient.RaceChannelName
                && e.Item2.Name == "RaceBot"
                && (e.Item3 == SRLClient.GetUser().Name + " has been removed from the race."
                || e.Item3 == SRLClient.GetUser().Name + " has forfeited from the race."))
            {
                SRLClient.RaceState = RaceState.NotInRace;
                SRLClient.MessageReceived -= ExitMessageReceived;
                if (InvokeRequired)
                    Invoke(new Action(Close));
                else
                    Close();
            }
        }

        void UndoneMessageReceived(object sender, Tuple<string, SRLIRCUser, string> e)
        {
            if (IsDisposed)
                return;

            if (e.Item1 == SRLClient.RaceChannelName
                && e.Item2.Name == "RaceBot"
                && e.Item3 == SRLClient.GetUser().Name + " has been undone from the race.")
            {
                SRLClient.RaceState = RaceState.RaceStarted;
                SRLClient.MessageReceived -= UndoneMessageReceived;
                if (InvokeRequired)
                    Invoke(new Action(Close));
                else
                    Close();
            }
        }

        void SRLClient_MessageReceived(object sender, Tuple<string, SRLIRCUser, string> e)
        {
            if (IsDisposed)
                return;

            RebuildUserList();
            if (e.Item1 == SRLClient.RaceChannelName)
            {
                var colorCode = GetColorCodeFromRights(e.Item2.Rights);
                ChatBoxAppend((char)3 + colorCode + (char)2 + e.Item2.Name + (char)2 + (char)3 + "1: " + e.Item3, Color.Black);
            }
        }

        void SRLClient_RawMessageReceived(object sender, string e)
        {
        }

        void SRLClient_ChannelJoined(object sender, string e)
        {
            if (IsDisposed)
                return;

            if (e == SRLClient.RaceChannelName)
            {
                RefreshRaceStateBasedOnAPI();
                if (GameCategory != null)
                {
                    var timer = new System.Timers.Timer(3000);
                    timer.Enabled = true;
                    timer.Elapsed += timer_Elapsed;
                }
                EnableJoinButton();
            }
            if (e == "#speedrunslive" && RaceId == null)
            {
                SRLClient.SendMainChannelMessage(".startrace " + GameId);
                SRLClient.RaceBotResponseTimer = new System.Timers.Timer(10000);
                SRLClient.RaceBotResponseTimer.Enabled = true;
                SRLClient.RaceBotResponseTimer.Elapsed += RaceBotResponseTimer_Elapsed;
            }
            RebuildUserList();
        }

        void RefreshRaceStateBasedOnAPI()
        {
            try
            {
                var race = SpeedRunsLiveAPI.Instance.GetRace(RaceId);
                var user = ((IDictionary<string, dynamic>)race.entrants.Properties).FirstOrDefault(x => x.Key.ToLower() == SRLClient.Username.ToLower()).Value;
                if (user != null)
                {
                    if (user.statetext == "Finished" || user.statetext == "Forfeit")
                        SRLClient.RaceState = RaceState.RaceEnded;
                    else if (race.statetext == "In Progress")
                        SRLClient.RaceState = RaceState.RaceStarted;
                    else if (user.statetext == "Ready")
                        SRLClient.RaceState = RaceState.EnteredRaceAndReady;
                    else
                        SRLClient.RaceState = RaceState.EnteredRace;
                    SRLClient_StateChanged(this, SRLClient.RaceState);
                }
            }
            catch { }
        }

        void RaceBotResponseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SRLClient.RaceBotResponseTimer.Enabled = false;
            FormIsClosing = true;

            this.InvokeIfRequired(() =>
            {
                MessageBox.Show(this, "RaceBot did not respond to your message. If you created a race within the last 5 minutes, you are not allowed to create another race.", "RaceBot Did Not Respond", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            });
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SRLClient.SendRaceChannelMessage(".setgoal " + GameCategory);
            ((System.Timers.Timer)sender).Enabled = false;
        }

        private void EnableJoinButton()
        {
            this.InvokeIfRequired(() =>
            {
                btnJoinQuit.Enabled = true;
            });
        }

        private void SpeedRunsLiveForm_Load(object sender, EventArgs e)
        {
            btnJoinQuit.Enabled = false;
            var authDialog = new AuthenticationDialog();

            var credentials = WebCredentials.SpeedRunsLiveIRCCredentials;
            authDialog.Username = credentials.Username ?? "";
            authDialog.Password = credentials.Password ?? "";
            authDialog.RememberPassword = !string.IsNullOrEmpty(authDialog.Password);

            if (authDialog.ShowDialog(this) == DialogResult.OK)
            {
                var username = authDialog.Username;
                var password = authDialog.Password;

                WebCredentials.SpeedRunsLiveIRCCredentials = new SRLCredentials(username, authDialog.RememberPassword ? password : "");

                SRLClient.Connect(username, password);
            }
            else
            {
                Close();
            }
        }

        private void SpeedRunsLiveForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (SRLClient.RaceState == RaceState.RaceEnded)
                {
                    SpeedRunsLiveAPI.Instance.RefreshRacesList();
                    var race = SpeedRunsLiveAPI.Instance.GetRace(RaceId);
                    if (race.statetext == "In Progress" && ((IDictionary<string, dynamic>)race.entrants.Properties).First(x => x.Key.ToLower() == SRLClient.Username.ToLower()).Value.statetext == "Finished")
                    {
                        var result = MessageBox.Show(this, "Due to SpeedRunsLive rules, you need to confirm that you have completed the race before leaving an unfinished race. Do you confirm that you legitimately finished the race?", "Confirmation Required", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (result == DialogResult.Cancel)
                        {
                            e.Cancel = true;
                            return;
                        }
                        else if (result == DialogResult.No)
                        {
                            SRLClient.Undone();
                            SRLClient.MessageReceived += UndoneMessageReceived;
                            e.Cancel = true;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            SRLClient.RemoveRaceComparisons();

            if (SRLClient.IsConnected
                && !FormIsClosing
                && (SRLClient.RaceState == RaceState.EnteredRace
                    || SRLClient.RaceState == RaceState.EnteredRaceAndReady
                    || SRLClient.RaceState == RaceState.RaceStarted))
            {
                e.Cancel = true;
                FormIsClosing = true;
                SRLClient.MessageReceived += ExitMessageReceived;
                SRLClient.QuitRace();
            }
        }

        private void SpeedRunsLiveForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormIsClosing = true;
            if (SRLClient.IsConnected)
                SRLClient.Disconnect();
            SRLClient.Dispose();
        }

        private static string GetColorCodeFromRights(SRLIRCRights rights)
        {
            string colorCode = "12";
            if (rights == SRLIRCRights.Operator)
                colorCode = "4";
            else if (rights == SRLIRCRights.Voice)
                colorCode = "3";
            return colorCode;
        }

        private static Color GetColorFromRights(SRLIRCRights rights)
        {
            return GetColorByCode(int.Parse(GetColorCodeFromRights(rights)));
        }

        private static Color GetColorByCode(int colorCode)
        {
            switch (colorCode)
            {
                case 0: return Color.White;
                case 1:
                default:
                    return Color.Black;
                case 2: return Color.DarkBlue;
                case 3: return Color.Green;
                case 4: return Color.Red;
                case 5: return Color.DarkRed;
                case 6: return Color.Purple;
                case 7: return Color.Orange;
                case 8: return Color.Yellow;
                case 9: return Color.LightGreen;
                case 10: return Color.Turquoise;
                case 11: return Color.LightBlue;
                case 12: return Color.Blue;
                case 13: return Color.Pink;
                case 14: return Color.Gray;
                case 15: return Color.LightGray;
            }
        }

        private void ChatBoxAppend(string message, Color color)
        {
            chatBox.InvokeIfRequired(() =>
            {
                int begin = chatBox.Text.Length;
                chatBox.AppendText(DateTime.Now.ToString("HH:mm "));
                chatBox.Select(begin, chatBox.Text.Length);
                chatBox.SelectionColor = Color.Gray;

                bool colorSplit = message[0] == 3;
                Color origColor = color;
                string colorlessMessage = "";
                int actualBegin = chatBox.Text.Length;
                foreach (var split in message.Split(new[] { (char)3 }, StringSplitOptions.RemoveEmptyEntries))
                {
                    begin = chatBox.Text.Length;
                    string useSplit = split;
                    if (colorSplit && split.Length > 1 && split[1] >= '0' && split[1] <= '9')
                    {
                        int code = -1;
                        bool parsed = int.TryParse(split.Substring(0, 2), out code);
                        color = parsed ? GetColorByCode(code) : origColor;
                        useSplit = split.Substring(2);
                    }
                    else if (colorSplit)
                    {
                        int code = -1;
                        bool parsed = int.TryParse(split.Substring(0, 1), out code);
                        color = parsed ? GetColorByCode(code) : origColor;
                        if (parsed)
                            useSplit = split.Substring(1);
                    }
                    var replacedText = useSplit.Replace(((char)2).ToString(), "");
                    bool isFirst = true;
                    foreach (var word in replacedText.Split(' '))
                    {
                        if (TwitchEmoteResolver.IsEmote(word))
                        {
                            chatBox.AppendText((isFirst ? "" : " "));
                            var image = TwitchEmoteResolver.Resolve(word);
                            var whiteImage = new Bitmap(image.Width, image.Height);
                            var g = Graphics.FromImage(whiteImage);
                            g.FillRectangle(Brushes.White, 0, 0, image.Width, image.Height);
                            g.DrawImage(image, 0, 0, image.Width, image.Height);
                            Clipboard.SetDataObject(whiteImage);
                            chatBox.ReadOnly = false;
                            chatBox.Paste();
                            chatBox.ReadOnly = true;
                        }
                        else
                        {
                            chatBox.AppendText((isFirst ? "" : " ") + word);
                        }
                        isFirst = false;
                    }
                    colorlessMessage += useSplit;
                    chatBox.Select(begin, chatBox.Text.Length);
                    chatBox.SelectionColor = color;
                    colorSplit = true;
                }
                bool bold = false;
                int boldBeginPosition = actualBegin;
                foreach (string boldSplit in colorlessMessage.Split((char)2))
                {
                    if (bold)
                    {
                        chatBox.Select(boldBeginPosition, boldSplit.Length);
                        chatBox.SelectionFont = new Font(chatBox.SelectionFont, FontStyle.Bold);
                    }
                    bold = !bold;
                    boldBeginPosition += boldSplit.Length;
                }

                chatBox.AppendText("\r\n");
                chatBox.Select(chatBox.TextLength, 0);
                chatBox.ScrollToCaret();
            });
        }

        private void Append(RichTextBox tbx, string text, Color? color = null)
        {
            int length = tbx.TextLength;
            tbx.AppendText(text);
            tbx.SelectionStart = length;
            tbx.SelectionLength = text.Length;
            tbx.SelectionColor = color ?? Color.Black;
            tbx.SelectionLength = 0;
        }

        private void btnJoinQuit_Click(object sender, EventArgs e)
        {
            if (SRLClient.RaceState == RaceState.EnteredRaceAndReady
                || SRLClient.RaceState == RaceState.EnteredRace
                || SRLClient.RaceState == RaceState.RaceStarted)
            {
                SRLClient.QuitRace();
            }
            else if (SRLClient.RaceState == RaceState.NotInRace)
            {
                SRLClient.JoinRace();
            }
            else if (SRLClient.RaceState == RaceState.RaceEnded)
            {
                SRLClient.Undone();
            }
        }

        private void chkReady_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void chatBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void lstUsers_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                //
                // Draw the background of the ListBox control for each item.
                // Create a new Brush and initialize to a Black colored brush
                // by default.
                //
                e.DrawBackground();
                Brush myBrush = (((ListBox)sender).Items[e.Index] as UserListItem).Brush;
                //
                // Draw the current item text based on the current 
                // Font and the custom brush settings.
                //
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                    e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
                //
                // If the ListBox has focus, draw a focus rectangle 
                // around the selected item.
                //
                e.DrawFocusRectangle();
            }
        }

        private void RebuildUserList()
        {
            Action action = () =>
                {
                    lstUsers.Items.Clear();
                    foreach (var user in SRLClient.GetRaceChannelUsers())
                    {
                        lstUsers.Items.Add(new UserListItem(user.Name, GetColorFromRights(user.Rights)));
                    }
                };

            try
            {
                if (!Disposing && !IsDisposed)
                {
                    this.InvokeIfRequired(action);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtMessage.Text.StartsWith("\r\n"))
                txtMessage.Clear();

            if (e.KeyCode == Keys.Enter && txtMessage.Text.Length != 0)
            {
                string message = txtMessage.Text;
                SRLClient.SendRaceChannelMessage(message);
                SRLClient_MessageReceived(this, new Tuple<string, SRLIRCUser, string>(SRLClient.RaceChannelName, SRLClient.GetUser(), message));
                txtMessage.Clear();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void chkReady_Click(object sender, EventArgs e)
        {
            if (chkReady.Checked)
            {
                SRLClient.Ready();
            }
            else
            {
                SRLClient.Unready();
            }
        }
    }

    internal class UserListItem
    {
        public string Value { get; set; }
        public Color Color { get; set; }
        public Brush Brush { get { return new SolidBrush(Color); } }

        public UserListItem(string value, Color color)
        {
            Value = value;
            Color = color;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
