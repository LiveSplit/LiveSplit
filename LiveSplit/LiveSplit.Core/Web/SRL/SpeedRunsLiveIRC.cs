using IrcDotNet;
using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Model.Input;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveSplit.Web.SRL
{
    public class SpeedRunsLiveIRC : IDisposable
    {
        private RaceState _State;

        public RaceState RaceState
        {
            get { return _State; }
            set
            {
                _State = value;

                if (StateChanged != null)
                    StateChanged(this, RaceState);
            }
        }
        protected IrcClient Client { get; set; }
        public ITimerModel Model { get; set; }

        public bool IsConnected { get { return Client.IsConnected; } }

        public event EventHandlerT<string> ChannelJoined;
        public event EventHandlerT<string> RawMessageReceived;
        public event EventHandlerT<Tuple<string, SRLIRCUser, string>> MessageReceived;
        public event EventHandlerT<RaceState> StateChanged;
        public event EventHandler UserListRefreshed;
        public event EventHandler GoalChanged;
        public event EventHandler PasswordIncorrect;
        public event EventHandler NicknameInUse;
        public event EventHandler Disconnected;
        public event EventHandler Kicked;

        public System.Timers.Timer RaceBotResponseTimer { get; set; }

        public string Username { get; protected set; }
        protected string Password { get; set; }
        protected IList<string> ChannelsToJoin { get; set; }
        public string GameName { get; set; }
        public string ChannelTopic { get; set; }

        protected IrcChannel MainChannel { get { return Client.Channels.FirstOrDefault(x => x.Name.Equals("#speedrunslive")); } }
        protected IrcChannel LiveSplitChannel { get { return Client.Channels.FirstOrDefault(x => x.Name.EndsWith("-livesplit")); } }
        protected IrcChannel RaceChannel { get { return Client.Channels.FirstOrDefault(x => x.Name.StartsWith("#srl") && !x.Name.EndsWith("-livesplit")); } }

        public string LiveSplitChannelName { get { return LiveSplitChannel.Name; } }
        public string RaceChannelName { get { return RaceChannel == null ? null : RaceChannel.Name; } }

        public SpeedRunsLiveIRC(LiveSplitState state, ITimerModel model, IEnumerable<string> channels)
        {
            ChannelsToJoin = channels.ToList();
            Client = new IrcClient();
            Client.ConnectFailed += Client_ConnectFailed;
            Client.Connected += Client_Connected;
            Client.Registered += Client_Registered;
            Client.RawMessageReceived += Client_RawMessageReceived;
            Client.ConnectFailed += Client_ConnectFailed;
            Client.Disconnected += Client_Disconnected;
            Model = model;
            state.OnSplit += Model_OnSplit;
            state.OnUndoSplit += Model_OnUndoSplit;
            state.OnReset += Model_OnReset;
            RaceState = RaceState.NotInRace;
        }

        void RaceChannel_UserKicked(object sender, IrcChannelUserEventArgs e)
        {
            if (e.ChannelUser.User.NickName == Client.LocalUser.NickName && Kicked != null)
               Kicked(this, null);
        }

        void Client_Disconnected(object sender, EventArgs e)
        {
            if (Disconnected != null)
                Disconnected(this, null);
        }

        void Model_OnReset(object sender, TimerPhase e)
        {
            if (RaceState == RaceState.RaceStarted)
                QuitRace();
        }

        void RaiseUserListRefreshed(object sender, EventArgs e)
        {
            if (UserListRefreshed != null)
                UserListRefreshed(this, null);
        }

        void LocalUser_JoinedChannel(object sender, IrcChannelEventArgs e)
        {
            e.Channel.MessageReceived += SpeedRunsLive_MessageReceived;

            if (e.Channel == RaceChannel)
            {
                e.Channel.ModesChanged += RaiseUserListRefreshed;
                e.Channel.UserJoined += RaiseUserListRefreshed;
                e.Channel.UserKicked += RaiseUserListRefreshed;
                e.Channel.UserLeft += RaiseUserListRefreshed;
                e.Channel.UsersListReceived += RaiseUserListRefreshed;
                e.Channel.UsersListReceived += Channel_UsersListReceived;
                RaceChannel.TopicChanged += RaceChannel_TopicChanged;
                RaceChannel.UserKicked += RaceChannel_UserKicked;
            }

            if (e.Channel == LiveSplitChannel)
            {
                e.Channel.UsersListReceived += Channel_UsersListReceived;
            }

            if (ChannelJoined != null)
                ChannelJoined(this, e.Channel.Name);
        }

        void Channel_UsersListReceived(object sender, EventArgs e)
        {
            try
            {
                if (RaceChannel != null && LiveSplitChannel != null)
                {
                    RemoveRaceComparisons();
                    foreach (var user in GetRaceChannelUsers().Where(x => x.Rights.HasFlag(SRLIRCRights.Voice) && x.Name != Username))
                    {
                        if (LiveSplitChannel.Users.Select(x => x.User.NickName).Contains(user.Name))
                            AddComparison(user.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        void RaceChannel_TopicChanged(object sender, EventArgs e)
        {
            ChannelTopic = RaceChannel.Topic;
            if (GoalChanged != null)
                GoalChanged(null, null);
        }

        private string Escape(string value)
        {
            // \ -> \\
            // " -> \.
            return value.Replace("\\", "\\\\").Replace("\"", "\\.");
        }

        private string Unescape(string value)
        {
            // \. -> "
            // \\ -> \
            return value.Replace("\\.", "\"").Replace("\\\\", "\\");
        }

        void Model_OnSplit(object sender, EventArgs e)
        {
            var timeFormatter = new RegularTimeFormatter(TimeAccuracy.Hundredths);

            if (LiveSplitChannel != null && (RaceState == RaceState.RaceStarted || RaceState == RaceState.RaceEnded))
            {
                if (Model.CurrentState.CurrentSplitIndex > 0)
                {
                        var split = Model.CurrentState.Run[Model.CurrentState.CurrentSplitIndex - 1];
                        var timeRTA = "-";
                        var timeIGT = "-";
                        if (split.SplitTime.RealTime != null)
                            timeRTA = timeFormatter.Format(split.SplitTime.RealTime);
                        if (split.SplitTime.GameTime != null)
                            timeIGT = timeFormatter.Format(split.SplitTime.GameTime);
                        Client.LocalUser.SendMessage(LiveSplitChannel, string.Format(".time \"{0}\" {1}", Escape(split.Name), timeRTA));
                        Client.LocalUser.SendMessage(LiveSplitChannel, string.Format(".timeGT \"{0}\" {1}", Escape(split.Name), timeIGT));
                }
            }

            if (RaceChannel != null)
            {
                if (Model.CurrentState.CurrentPhase == TimerPhase.Ended && RaceState == RaceState.RaceStarted)
                    Client.LocalUser.SendMessage(RaceChannel, ".done");
            }
        }

        void Model_OnUndoSplit(object sender, EventArgs e)
        {
            if (LiveSplitChannel != null && RaceState == RaceState.RaceEnded
                && Model.CurrentState.CurrentSplitIndex == Model.CurrentState.Run.Count - 1)
                Undone();

            if (LiveSplitChannel != null && (RaceState == RaceState.RaceStarted || RaceState == RaceState.RaceEnded))
            {
                var split = Model.CurrentState.CurrentSplit;
                var time = "-";
                Client.LocalUser.SendMessage(LiveSplitChannel, string.Format(".time \"{0}\" {1}", Escape(split.Name), time));
                Client.LocalUser.SendMessage(LiveSplitChannel, string.Format(".timeGT \"{0}\" {1}", Escape(split.Name), time));
            }
        }

        void Client_Registered(object sender, EventArgs e)
        {
            Client.LocalUser.SendMessage("NickServ", string.Format("IDENTIFY {0}", Password));
            Client.LocalUser.JoinedChannel += LocalUser_JoinedChannel;
        }

        void Client_RawMessageReceived(object sender, IrcRawMessageEventArgs e)
        {
            if (e.Message.Command == "010")
            {
                Client.Disconnect();
                Connect("irc2.speedrunslive.com", Username, Password);
                return;
            }

            if (e.Message.Command == "433")
            {
                if (NicknameInUse != null)
                    NicknameInUse(this, null);
            }

            if (e.Message.Source != null
                && e.Message.Source.Name == "NickServ"
                && e.Message.Command == "NOTICE")
            {
                if (e.Message.Parameters[1] == "Password accepted - you are now recognized.")
                {
                    Task.Factory.StartNew(() =>
                    {
                        foreach (var channel in ChannelsToJoin)
                            Client.Channels.Join(channel);
                    });
                }
                else if (e.Message.Parameters[1] == "Password incorrect.")
                {
                    if (PasswordIncorrect != null)
                        PasswordIncorrect(this, null);
                }
            }
            

            if (RawMessageReceived != null)
                RawMessageReceived(this, string.Format("{0} - {1}", e.Message.Command, e.Message.Parameters.Where(x => x != null).Aggregate((a, b) => a + " " + b)));
        }

        protected void ProcessSplit(string user, string segmentName, TimeSpan? time, TimingMethod method)
        {
            var run = Model.CurrentState.Run;
            var comparisonName = "[Race] " + user;

            var segment = run.FirstOrDefault(x => x.Name == segmentName);
            if (segment != null)
            {
                var newTime = new Time(segment.Comparisons[comparisonName]);
                newTime[method] = time;
                segment.Comparisons[comparisonName] = newTime;
            }
        }

        protected void ProcessRaceChannelMessage(string user, string message)
        {
            if (user == "RaceBot")
            {
                if (message.Contains("GO!") && RaceState == RaceState.EnteredRaceAndReady)
                {
                    Model.Start();
                    RaceState = RaceState.RaceStarted;
                    //Client.LocalUser.SendMessage(LiveSplitChannel, ".enter");
                }
                else if (message == Client.LocalUser.NickName + " has been removed from the race.")
                {
                    RaceState = RaceState.NotInRace;
                }
                else if (message.Contains(" has been removed from the race."))
                {
                    var userName = message.Substring(0, message.IndexOf(" "));

                    if (Model.CurrentState.CurrentComparison.Equals("[Race] " + userName))
                        Model.CurrentState.CurrentComparison = Run.PersonalBestComparisonName;
                    
                    var comparisonGenerator = Model.CurrentState.Run.ComparisonGenerators.FirstOrDefault(x => x.Name == ("[Race] " + userName));
                    if (comparisonGenerator != null)
                        Model.CurrentState.Run.ComparisonGenerators.Remove(comparisonGenerator);
                    
                    foreach (var segment in Model.CurrentState.Run)
                        segment.Comparisons["[Race] " + userName] = default(Time);
                }
                else if (message.StartsWith(Client.LocalUser.NickName + " enters the race!"))
                {
                    RaceState = RaceState.EnteredRace;
                }
                else if (message.Contains(" enters the race!"))
                {
                    var userName = message.Substring(0, message.IndexOf(" "));
                    if (LiveSplitChannel.Users.Select(x => x.User.NickName).Contains(userName))
                        AddComparison(userName);
                }
                else if (message.StartsWith(Client.LocalUser.NickName + " is ready!"))
                {
                    RaceState = RaceState.EnteredRaceAndReady;
                }
                else if (message.StartsWith(Client.LocalUser.NickName + " isn't ready!"))
                {
                    RaceState = RaceState.EnteredRace;
                }
                else if (message.StartsWith(Client.LocalUser.NickName + " has forfeited from the race."))
                {
                    RaceState = RaceState.RaceEnded;
                }
                else if (message.StartsWith(Client.LocalUser.NickName + " has been undone from the race."))
                {
                    RaceState = RaceState.RaceStarted;
                }
                else if (message.StartsWith(Client.LocalUser.NickName + " has finished in"))
                {
                    RaceState = RaceState.RaceEnded;
                }
                else if (message == "Rematch!")
                {
                    RaceState = RaceState.NotInRace;
                    RemoveRaceComparisons();
                }
            }
        }

        protected void AddComparison(string userName)
        {
            var run = Model.CurrentState.Run;
            var comparisonName = "[Race] " + userName;
            if (run.ComparisonGenerators.All(x => x.Name != comparisonName))
            {
                CompositeComparisons.AddShortComparisonName(comparisonName, userName);
                run.ComparisonGenerators.Add(new SRLComparisonGenerator(comparisonName));
            }
        }

        public void RemoveRaceComparisons()
        {
            if (Model.CurrentState.CurrentComparison.StartsWith("[Race] "))
                Model.CurrentState.CurrentComparison = Run.PersonalBestComparisonName;
            for (var ind = 0; ind < Model.CurrentState.Run.ComparisonGenerators.Count; ind++)
            {
                if (Model.CurrentState.Run.ComparisonGenerators[ind].Name.StartsWith("[Race] "))
                {
                    Model.CurrentState.Run.ComparisonGenerators.RemoveAt(ind);
                    ind--;
                }
            }
            foreach (var segment in Model.CurrentState.Run)
            {
                for (var index = 0; index < segment.Comparisons.Count; index++)
                {
                    var comparison = segment.Comparisons.ElementAt(index);
                    if (comparison.Key.StartsWith("[Race] "))
                        segment.Comparisons[comparison.Key] = default(Time);
                }
            }
        }

        protected void ProcessMainChannelMessage(string user, string message)
        {
            if ((user == "RaceBot") && RaceChannel == null && message.StartsWith("Race initiated for " + GameName))
            {
                var index = message.IndexOf("#srl");
                var channel = message.Substring(index, 10);
                Client.Channels.Join(channel);
                Client.Channels.Join(channel + "-livesplit");
                RaceBotResponseTimer.Enabled = false;
            }
        }

        protected void ProcessLiveSplitChannelMessage(string user, string message)
        {
            if (RaceState == RaceState.RaceStarted || RaceState == RaceState.RaceEnded)
            {
                if (message.StartsWith(".time ") || message.StartsWith(".timeGT "))
                {
                    var method = message.StartsWith(".timeGT ") ? TimingMethod.GameTime : TimingMethod.RealTime;
                    var cutOff = message.Substring(".time \"".Length + (method == TimingMethod.GameTime ? "GT".Length : 0));
                    var index = cutOff.IndexOf("\"");
                    var splitName = Unescape(cutOff.Substring(0, index));
                    var timeString = cutOff.Substring(index + 2);
                    var time = ParseTime(timeString);

                    ProcessSplit(user, splitName, time, method);
                }
            }
        }

        protected TimeSpan? ParseTime(string timeString)
        {
            if (timeString == "-")
                return null;
            return TimeSpanParser.Parse(timeString);
        }

        void SpeedRunsLive_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            if (e.Targets.Count > 0 && e.Targets[0] == RaceChannel)
            {
                var realName = RaceChannel.Users.FirstOrDefault(x => x.User.NickName == e.Source.Name).User.RealName;
                ProcessRaceChannelMessage(e.Source.Name, e.Text);
            }
            else if (e.Targets.Count > 0 && e.Targets[0] == LiveSplitChannel)
            {
                ProcessLiveSplitChannelMessage(e.Source.Name, e.Text);
            }
            else if (e.Targets.Count > 0 && e.Targets[0] == MainChannel)
            {
                ProcessMainChannelMessage(e.Source.Name, e.Text);
            }

            if (MessageReceived != null)
            {
                var rights = SRLIRCRights.Normal;
                if (e.Targets[0] is IrcChannel)
                {
                    var target = e.Targets[0] as IrcChannel;
                    var source = target.Users.FirstOrDefault(x => x.User.NickName == e.Source.Name);
                    if (source != null)
                    {
                        rights = SRLIRCRightsHelper.FromIrcChannelUser(source);
                    }
                }
                MessageReceived(this, new Tuple<string, SRLIRCUser, string>(e.Targets[0].Name, new SRLIRCUser(e.Source.Name, rights), e.Text));
            }
        }

        private void Connect(string server, string username, string password)
        {
            Username = username;
            Password = password;
            Client.Connect(server, 6667, new IrcUserRegistrationInfo()
            {
                UserName = username,
                NickName = username,
#if DEBUG            
                RealName = "xd"
#else
                RealName = "LiveSplit " + UpdateHelper.Version
#endif
            });
        }

        public void Connect(string username, string password)
        {
            Connect("irc2.speedrunslive.com", username, password);
        }

        void Client_Connected(object sender, EventArgs e)
        {   
        }

        void Client_ConnectFailed(object sender, IrcErrorEventArgs e)
        {
        }

        public void Disconnect()
        {
            try
            {
                Client.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void QuitRace()
        {
            try
            {
                Client.LocalUser.SendMessage(RaceChannel, ".quit");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void JoinRace()
        {
            try
            {
                Client.LocalUser.SendMessage(RaceChannel, ".join");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void Ready()
        {
            try
            {
                Client.LocalUser.SendMessage(RaceChannel, ".ready");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void Unready()
        {
            try
            {
                Client.LocalUser.SendMessage(RaceChannel, ".unready");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void Undone()
        {
            try
            {
                Client.LocalUser.SendMessage(RaceChannel, ".undone");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public IEnumerable<SRLIRCUser> GetRaceChannelUsers()
        {
            if (RaceChannel == null)
                return new SRLIRCUser[0];

            return RaceChannel.Users
                .Select(x => 
                new SRLIRCUser(
                    x.User.NickName,
                    SRLIRCRightsHelper.FromIrcChannelUser(x)
                    )
                )
                .OrderBy(x => 
                    ((x.Rights == SRLIRCRights.Operator)
                    ? "0"
                    : (x.Rights == SRLIRCRights.Voice)
                    ? "1"
                    : "2")
                    + x.Name
                );
        }

        public SRLIRCUser GetUser()
        {
            return GetRaceChannelUsers().FirstOrDefault(x => x.Name == Client.LocalUser.NickName);
        }

        public void SendRaceChannelMessage(string message)
        {
            if (RaceChannel != null)
            {
                Client.LocalUser.SendMessage(RaceChannel, message);
            }
        }

        public void SendMainChannelMessage(string message)
        {
            Client.LocalUser.SendMessage("#speedrunslive", message);
        }

        public void Dispose()
        {
            Client.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
