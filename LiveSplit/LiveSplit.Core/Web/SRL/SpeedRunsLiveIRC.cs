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
using LiveSplit.Updates;

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

                StateChanged?.Invoke(this, RaceState);
            }
        }
        protected IrcClient Client { get; set; }
        public ITimerModel Model { get; set; }

        public bool IsConnected => Client.IsConnected;

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

        protected IrcChannel MainChannel => Client.Channels.FirstOrDefault(x => x.Name.Equals("#speedrunslive"));
        protected IrcChannel LiveSplitChannel => Client.Channels.FirstOrDefault(x => x.Name.EndsWith("-livesplit"));
        protected IrcChannel RaceChannel => Client.Channels.FirstOrDefault(x => x.Name.StartsWith("#srl") && !x.Name.EndsWith("-livesplit"));

        public string LiveSplitChannelName => LiveSplitChannel.Name;
        public string RaceChannelName => RaceChannel?.Name;

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
            if (e.ChannelUser.User.NickName == Client.LocalUser.NickName)
               Kicked?.Invoke(this, null);
        }

        void Client_Disconnected(object sender, EventArgs e)
        {
            Disconnected?.Invoke(this, null);
        }

        void Model_OnReset(object sender, TimerPhase e)
        {
            if (RaceState == RaceState.RaceStarted)
                QuitRace();
        }

        void RaiseUserListRefreshed(object sender, EventArgs e)
        {
            UserListRefreshed?.Invoke(this, null);
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

            ChannelJoined?.Invoke(this, e.Channel.Name);
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
            GoalChanged?.Invoke(null, null);
        }

        private static string Escape(string value)
        {
            // \ -> \\
            // " -> \.
            return value.Replace("\\", "\\\\").Replace("\"", "\\.");
        }

        private static string Unescape(string value)
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
                    if (Model.CurrentState.CurrentPhase == TimerPhase.Ended)
                    {
                        Client.LocalUser.SendMessage(LiveSplitChannel, $"!done RealTime {timeRTA}");
                        Client.LocalUser.SendMessage(LiveSplitChannel, $"!done GameTime {timeIGT}");
                    }
                    else
                    {
                        Client.LocalUser.SendMessage(LiveSplitChannel, $"!time RealTime \"{Escape(split.Name)}\" {timeRTA}");
                        Client.LocalUser.SendMessage(LiveSplitChannel, $"!time GameTime \"{Escape(split.Name)}\" {timeIGT}");
                    }
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
                if (Model.CurrentState.CurrentSplitIndex == Model.CurrentState.Run.Count - 1)
                {
                    Client.LocalUser.SendMessage(LiveSplitChannel, $"!done RealTime {time}");
                    Client.LocalUser.SendMessage(LiveSplitChannel, $"!done GameTime {time}");
                }
                else
                {
                    Client.LocalUser.SendMessage(LiveSplitChannel, $"!time RealTime \"{Escape(split.Name)}\" {time}");
                    Client.LocalUser.SendMessage(LiveSplitChannel, $"!time GameTime \"{Escape(split.Name)}\" {time}");
                }
            }
        }

        void Client_Registered(object sender, EventArgs e)
        {
            Client.LocalUser.SendMessage("NickServ", $"IDENTIFY {Password}");
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
                NicknameInUse?.Invoke(this, null);
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
                    PasswordIncorrect?.Invoke(this, null);
                }
            }

            if (e.Message.Command == "NOTICE"
                && e.Message.Source != null
                && e.Message.Source.Name == "RaceBot"
                && e.Message.Parameters.Count > 1)
            {
                var text = e.Message.Parameters[1];
                if (text != null && !text.Contains("#srl"))
                {
                    MessageReceived?.Invoke(this, new Tuple<string, SRLIRCUser, string>(RaceChannelName, new SRLIRCUser("RaceBot", SRLIRCRights.Operator), text));
                }
            }

            RawMessageReceived?.Invoke(this, $"{e.Message.Command} - {e.Message.Parameters.Where(x => x != null).Aggregate((a, b) => a + " " + b)}");
        }

        protected void ProcessSplit(string user, string segmentName, TimeSpan? time, TimingMethod method)
        {
            var run = Model.CurrentState.Run;
            var segment = GetMatchingSegment(run, user, segmentName, method, time.HasValue);
            if (segment != null)
                AddSplit(user, segment, time, method);
        }

        private static ISegment GetMatchingSegment(IRun run, string user, string segmentName, TimingMethod method, bool timeHasValue)
        {
            var comparisonName = SRLComparisonGenerator.GetRaceComparisonName(user);
            var trimmedSegmentName = segmentName.Trim().ToLower();

            if (timeHasValue)
            {
                return run.FirstOrDefault(x => x.Name.Trim().ToLower() == trimmedSegmentName && x.Comparisons[comparisonName][method] == null);
            }
            return run.LastOrDefault(x => x.Name.Trim().ToLower() == trimmedSegmentName && x.Comparisons[comparisonName][method] != null);
        }

        protected void ProcessFinalSplit(string user, TimeSpan? time, TimingMethod method)
        {
            var run = Model.CurrentState.Run;
            var segment = run.Last();
            AddSplit(user, segment, time, method);
        }

        protected void AddSplit(string user, ISegment segment, TimeSpan? time, TimingMethod method)
        {
            var comparisonName = SRLComparisonGenerator.GetRaceComparisonName(user);
            var newTime = new Time(segment.Comparisons[comparisonName]);
            newTime[method] = time;
            segment.Comparisons[comparisonName] = newTime;
        }

        protected void ProcessRaceChannelMessage(string user, string message)
        {
            if (user == "RaceBot")
            {
                if (message.Contains("GO!") && RaceState == RaceState.EnteredRaceAndReady)
                {
                    Model.Start();
                    RaceState = RaceState.RaceStarted;
                }
                else if (message == Client.LocalUser.NickName + " has been removed from the race.")
                {
                    RaceState = RaceState.NotInRace;
                }
                else if (message.Contains(" has been removed from the race."))
                {
                    var userName = message.Substring(0, message.IndexOf(" "));
                    var raceComparison = SRLComparisonGenerator.GetRaceComparisonName(userName);

                    if (Model.CurrentState.CurrentComparison.Equals(raceComparison))
                        Model.CurrentState.CurrentComparison = Run.PersonalBestComparisonName;
                    
                    var comparisonGenerator = Model.CurrentState.Run.ComparisonGenerators.FirstOrDefault(x => x.Name == raceComparison);
                    if (comparisonGenerator != null)
                        Model.CurrentState.Run.ComparisonGenerators.Remove(comparisonGenerator);
                    
                    foreach (var segment in Model.CurrentState.Run)
                        segment.Comparisons[raceComparison] = default(Time);
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
            var comparisonName = SRLComparisonGenerator.GetRaceComparisonName(userName);
            if (run.ComparisonGenerators.All(x => x.Name != comparisonName))
            {
                CompositeComparisons.AddShortComparisonName(comparisonName, userName);
                run.ComparisonGenerators.Add(new SRLComparisonGenerator(comparisonName));
            }
        }

        public void RemoveRaceComparisons()
        {
            if (SRLComparisonGenerator.IsRaceComparison(Model.CurrentState.CurrentComparison))
                Model.CurrentState.CurrentComparison = Run.PersonalBestComparisonName;
            for (var ind = 0; ind < Model.CurrentState.Run.ComparisonGenerators.Count; ind++)
            {
                if (SRLComparisonGenerator.IsRaceComparison(Model.CurrentState.Run.ComparisonGenerators[ind].Name))
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
                    if (SRLComparisonGenerator.IsRaceComparison(comparison.Key))
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
                if (message.StartsWith("!time ") || message.StartsWith("!done "))
                {
                    try
                    {
                        var method = message.Substring("!time ".Length).StartsWith("GameTime") ? TimingMethod.GameTime : TimingMethod.RealTime;
                        var finalSplit = message.StartsWith("!done ");
                        var arguments = message.Substring("!time RealTime ".Length);

                        if (finalSplit)
                        {
                            var time = ParseTime(arguments);
                            ProcessFinalSplit(user, time, method);
                        }
                        else
                        {
                            var timeIndex = arguments.LastIndexOf("\"");
                            var splitName = Unescape(arguments.Substring(1, timeIndex - 1));
                            var time = ParseTime(arguments.Substring(timeIndex + 2));
                            ProcessSplit(user, splitName, time, method);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
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
                RealName = UpdateHelper.UserAgent
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
