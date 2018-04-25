using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.Server
{
    public class CommandServer
    {
        public List<Connection> Connections { get; set; }

        protected LiveSplitState State { get; set; }
        protected Form Form { get; set; }
        protected TimerModel Model { get; set; }
        protected ITimeFormatter DeltaFormatter { get; set; }
        protected ITimeFormatter SplitTimeFormatter { get; set; }
        protected NamedPipeServerStream WaitingServerPipe { get; set; }

        protected bool AlwaysPauseGameTime { get; set; }

        public CommandServer(LiveSplitState state)
        {
            Model = new TimerModel();
            Connections = new List<Connection>();

            DeltaFormatter = new DeltaTimeFormatter()
            {
                Accuracy = TimeAccuracy.Hundredths,
                DropDecimals = false
            };
            SplitTimeFormatter = new RegularTimeFormatter(TimeAccuracy.Hundredths);

            State = state;
            Form = state.Form;

            Model.CurrentState = State;
            State.OnStart += State_OnStart;
        }

        public void Start()
        {
            CloseAllConnections();

            WaitingServerPipe = CreateServerPipe();
            WaitingServerPipe.BeginWaitForConnection(AcceptPipeClient, null);
        }

        public void Stop()
        {
            CloseAllConnections();
        }

        protected void CloseAllConnections()
        {
            if (WaitingServerPipe != null)
                WaitingServerPipe.Dispose();
            foreach (var connection in Connections)
            {
                connection.Dispose();
            }
            Connections.Clear();
        }

        public void AcceptPipeClient(IAsyncResult result)
        {
            try
            {
                var waitingPipe = WaitingServerPipe;
                waitingPipe.EndWaitForConnection(result);

                Form.BeginInvoke(new Action(() => Connect(waitingPipe)));

                WaitingServerPipe = CreateServerPipe();
                WaitingServerPipe.BeginWaitForConnection(AcceptPipeClient, null);
            }
            catch { }
        }

        private NamedPipeServerStream CreateServerPipe()
        {
            var pipe = new NamedPipeServerStream("LiveSplit", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            return pipe;
        }

        private void Connect(Stream stream)
        {
            var connection = new Connection(stream);
            connection.MessageReceived += connection_MessageReceived;
            connection.ScriptReceived += connection_ScriptReceived;
            connection.Disconnected += connection_Disconnected;
            Connections.Add(connection);
        }

        TimeSpan? parseTime(string timeString)
        {
            if (timeString == "-")
                return null;

            return TimeSpanParser.Parse(timeString);
        }

        void connection_ScriptReceived(object sender, ScriptEventArgs e)
        {
            Form.BeginInvoke(new Action(() => ProcessScriptRequest(e.Script, e.Connection)));
        }

        private void ProcessScriptRequest(IScript script, Connection clientConnection)
        {
            try
            {
                script["state"] = State;
                script["model"] = Model;
                script["sendMessage"] = new Action<string>(x => clientConnection.SendMessage(x));
                var result = script.Run();
                if (result != null)
                    clientConnection.SendMessage(result.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                clientConnection.SendMessage(ex.Message);
            }
        }

        void connection_MessageReceived(object sender, MessageEventArgs e)
        {
            Form.BeginInvoke(new Action(() => ProcessMessage(e.Message, e.Connection)));
        }

        private void ProcessMessage(String message, Connection clientConnection)
        {
            try
            {
                if (message == "startorsplit")
                {
                    if (State.CurrentPhase == TimerPhase.Running)
                    {
                        Model.Split();
                    }
                    else
                    {
                        Model.Start();
                    }
                }
                else if (message == "split")
                {
                    Model.Split();
                }
                else if (message == "undosplit" || message == "unsplit")
                {
                    Model.UndoSplit();
                }
                else if (message == "skipsplit")
                {
                    Model.SkipSplit();
                }
                else if (message == "pause" && State.CurrentPhase != TimerPhase.Paused)
                {
                    Model.Pause();
                }
                else if (message == "resume" && State.CurrentPhase == TimerPhase.Paused)
                {
                    Model.Pause();
                }
                else if (message == "reset")
                {
                    Model.Reset();
                }
                else if (message == "start" || message == "starttimer")
                {
                    Model.Start();
                }
                else if (message.StartsWith("setgametime "))
                {
                    var value = message.Split(' ')[1];
                    var time = parseTime(value);
                    State.SetGameTime(time);
                }
                else if (message.StartsWith("setloadingtimes "))
                {
                    var value = message.Split(' ')[1];
                    var time = parseTime(value);
                    State.LoadingTimes = time ?? TimeSpan.Zero;
                }
                else if (message == "pausegametime")
                {
                    State.IsGameTimePaused = true;
                }
                else if (message == "unpausegametime")
                {
                    AlwaysPauseGameTime = false;
                    State.IsGameTimePaused = false;
                }
                else if (message == "alwayspausegametime")
                {
                    AlwaysPauseGameTime = true;
                    State.IsGameTimePaused = true;
                }
                else if (message == "getdelta" || message.StartsWith("getdelta "))
                {
                    var comparison = State.CurrentComparison;
                    if (message.Contains(" "))
                        comparison = message.Split(new char[] { ' ' }, 2)[1];
                    var delta = LiveSplitStateHelper.GetLastDelta(State, State.CurrentSplitIndex, comparison, State.CurrentTimingMethod);
                    var response = DeltaFormatter.Format(delta);
                    clientConnection.SendMessage(response);
                }
                else if (message == "getsplitindex")
                {
                    var splitindex = State.CurrentSplitIndex;
                    var response = splitindex.ToString();
                    clientConnection.SendMessage(response);
                }
                else if (message == "getcurrentsplitname")
                {
                    var splitindex = State.CurrentSplitIndex;
                    var currentsplitname = State.CurrentSplit.Name;
                    var response = currentsplitname;
                    clientConnection.SendMessage(response);
                }
                else if (message == "getprevioussplitname")
                {
                    var previoussplitindex = State.CurrentSplitIndex - 1;
                    var previoussplitname = State.Run[previoussplitindex].Name;
                    var response = previoussplitname;
                    clientConnection.SendMessage(response);
                }
                else if (message == "getlastsplittime" && State.CurrentSplitIndex > 0)
                {
                    var splittime = State.Run[State.CurrentSplitIndex - 1].SplitTime[State.CurrentTimingMethod];
                    var response = SplitTimeFormatter.Format(splittime);
                    clientConnection.SendMessage(response);
                }
                else if (message == "getcomparisonsplittime")
                {
                    var splittime = State.CurrentSplit.Comparisons[State.CurrentComparison][State.CurrentTimingMethod];
                    var response = SplitTimeFormatter.Format(splittime);
                    clientConnection.SendMessage(response);
                }
                else if (message == "getcurrenttime")
                {
                    var timingMethod = State.CurrentTimingMethod;
                    if (timingMethod == TimingMethod.GameTime && !State.IsGameTimeInitialized)
                        timingMethod = TimingMethod.RealTime;
                    var time = State.CurrentTime[timingMethod];
                    var response = SplitTimeFormatter.Format(time);
                    clientConnection.SendMessage(response);
                }
                else if (message == "getfinaltime" || message.StartsWith("getfinaltime "))
                {
                    var comparison = State.CurrentComparison;
                    if (message.Contains(" "))
                    {
                        comparison = message.Split(new char[] { ' ' }, 2)[1];
                    }
                    var time = (State.CurrentPhase == TimerPhase.Ended)
                        ? State.CurrentTime[State.CurrentTimingMethod]
                        : State.Run.Last().Comparisons[comparison][State.CurrentTimingMethod];
                    var response = SplitTimeFormatter.Format(time);
                    clientConnection.SendMessage(response);
                }
                else if (message.StartsWith("getpredictedtime "))
                {
                    var comparison = message.Split(new char[] { ' ' }, 2)[1];
                    var prediction = PredictTime(State, comparison);
                    var response = SplitTimeFormatter.Format(prediction);
                    clientConnection.SendMessage(response);
                }
                else if (message == "getbestpossibletime")
                {
                    var comparison = LiveSplit.Model.Comparisons.BestSegmentsComparisonGenerator.ComparisonName;
                    var prediction = PredictTime(State, comparison);
                    var response = SplitTimeFormatter.Format(prediction);
                    clientConnection.SendMessage(response);
                }
                else if (message == "getcurrenttimerphase")
                {
                    var response = State.CurrentPhase.ToString();
                    clientConnection.SendMessage(response);
                }
                else if (message.StartsWith("setcomparison "))
                {
                    var comparison = message.Split(new char[] { ' ' }, 2)[1];
                    State.CurrentComparison = comparison;
                }
                else if (message == "switchto realtime")
                {
                    State.CurrentTimingMethod = TimingMethod.RealTime;
                }
                else if (message == "switchto gametime")
                {
                    State.CurrentTimingMethod = TimingMethod.GameTime;
                }
                else if (message.StartsWith("setsplitname "))
                {
                    int index = Convert.ToInt32(message.Split(new char[] { ' ' }, 3)[1]);
                    string title = message.Split(new char[] { ' ' }, 3)[2];
                    State.Run[index].Name = title;
                    State.Run.HasChanged = true;
                }
                else if (message.StartsWith("setcurrentsplitname "))
                {
                    string title = message.Split(new char[] { ' ' }, 2)[1];
                    State.Run[State.CurrentSplitIndex].Name = title;
                    State.Run.HasChanged = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void connection_Disconnected(object sender, EventArgs e)
        {
            Form.BeginInvoke(new Action(() =>
            {
                var connection = (Connection)sender;
                Connections.Remove(connection);
                connection.Dispose();
            }));
        }

        private void State_OnStart(object sender, EventArgs e)
        {
            if (AlwaysPauseGameTime)
                State.IsGameTimePaused = true;
        }

        private TimeSpan? PredictTime(LiveSplitState state, string comparison)
        {
            if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                TimeSpan? delta = LiveSplitStateHelper.GetLastDelta(state, state.CurrentSplitIndex, comparison, State.CurrentTimingMethod) ?? TimeSpan.Zero;
                var liveDelta = state.CurrentTime[State.CurrentTimingMethod] - state.CurrentSplit.Comparisons[comparison][State.CurrentTimingMethod];
                if (liveDelta > delta)
                    delta = liveDelta;
                return delta + state.Run.Last().Comparisons[comparison][State.CurrentTimingMethod];
            }
            else if (state.CurrentPhase == TimerPhase.Ended)
            {
                return state.Run.Last().SplitTime[State.CurrentTimingMethod];
            }
            else
            {
                return state.Run.Last().Comparisons[comparison][State.CurrentTimingMethod];
            }
        }

        public void Dispose()
        {
            State.OnStart -= State_OnStart;
            CloseAllConnections();
        }
    }
}
