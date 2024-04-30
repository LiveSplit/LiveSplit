﻿using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public TcpListener Server { get; set; }
        public List<Connection> PipeConnections { get; set; }
        public List<TcpConnection> TcpConnections { get; set; }

        protected LiveSplitState State { get; set; }
        protected Form Form { get; set; }
        protected TimerModel Model { get; set; }
        protected ITimeFormatter TimeFormatter { get; set; }
        protected NamedPipeServerStream WaitingServerPipe { get; set; }

        protected bool AlwaysPauseGameTime { get; set; }

        public CommandServer(LiveSplitState state)
        {
            Model = new TimerModel();
            PipeConnections = new List<Connection>();
            TcpConnections = new List<TcpConnection>();
            TimeFormatter = new PreciseTimeFormatter();

            State = state;
            Form = state.Form;

            Model.CurrentState = State;
            State.OnStart += State_OnStart;
        }

        public void StartTcp()
        {
            StopTcp();
            Server = new TcpListener(IPAddress.Any, State.Settings.ServerPort);
            Server.Start();
            Server.BeginAcceptTcpClient(AcceptTcpClient, null);
        }

        public void StartNamedPipe()
        {
            StopPipe();
            WaitingServerPipe = CreateServerPipe();
            WaitingServerPipe.BeginWaitForConnection(AcceptPipeClient, null);
        }

        public void StopAll()
        {
            StopTcp();
            StopPipe();
        }

        public void StopTcp()
        {
            foreach (var connection in TcpConnections)
            {
                connection.Dispose();
            }

            TcpConnections.Clear();
            Server?.Stop();
        }

        public void StopPipe()
        {
            WaitingServerPipe?.Dispose();

            foreach (var connection in PipeConnections)
            {
                connection.Dispose();
            }

            PipeConnections.Clear();
        }

        public void AcceptTcpClient(IAsyncResult result)
        {
            try
            {
                var client = Server.EndAcceptTcpClient(result);
                var connection = new TcpConnection(client);
                connection.MessageReceived += connection_MessageReceived;
                connection.Disconnected += tcpConnection_Disconnected;
                TcpConnections.Add(connection);

                Server.BeginAcceptTcpClient(AcceptTcpClient, null);
            }
            catch { }
        }

        public void AcceptPipeClient(IAsyncResult result)
        {
            try
            {
                WaitingServerPipe.EndWaitForConnection(result);
                var connection = new Connection(WaitingServerPipe);
                connection.MessageReceived += connection_MessageReceived;
                connection.Disconnected += pipeConnection_Disconnected;
                PipeConnections.Add(connection);

                WaitingServerPipe = CreateServerPipe();
                WaitingServerPipe.BeginWaitForConnection(AcceptPipeClient, null);
            }
            catch { }
        }

        private void pipeConnection_Disconnected(object sender, EventArgs e)
        {
            var connection = (Connection)sender;
            connection.MessageReceived -= connection_MessageReceived;
            connection.Disconnected -= pipeConnection_Disconnected;
            PipeConnections.Remove(connection);
            connection.Dispose();
        }

        private NamedPipeServerStream CreateServerPipe()
        {
            var pipe = new NamedPipeServerStream("LiveSplit", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            return pipe;
        }

        TimeSpan? ParseTime(string timeString)
        {
            if (timeString == "-")
                return null;

            return TimeSpanParser.Parse(timeString);
        }

        void connection_MessageReceived(object sender, MessageEventArgs e)
        {
            ProcessMessage(e.Message, e.Connection);
        }

        private void ProcessMessage(string message, Connection clientConnection)
        {
            string response = null;
            var args = message.Split(new[] { ' ' }, 2);
            var command = args[0];
            switch (command)
            {
                case "startorsplit":
                    {
                        if (State.CurrentPhase == TimerPhase.Running)
                        {
                            Model.Split();
                        }
                        else
                        {
                            Model.Start();
                        }
                        break;
                    }
                case "split":
                    {
                        Model.Split();
                        break;
                    }
                case "undosplit":
                case "unsplit":
                    {
                        Model.UndoSplit();
                        break;
                    }
                case "skipsplit":
                    {
                        Model.SkipSplit();
                        break;
                    }
                case "pause":
                    {
                        if (State.CurrentPhase != TimerPhase.Paused)
                        {
                            Model.Pause();
                        }
                        break;
                    }
                case "resume":
                    {
                        if (State.CurrentPhase == TimerPhase.Paused)
                        {
                            Model.Pause();
                        }
                        break;
                    }
                case "reset":
                    {
                        Model.Reset();
                        break;
                    }
                case "start":
                case "starttimer":
                    {
                        Model.Start();
                        break;
                    }
                case "setgametime":
                    {
                        try
                        {
                            var time = ParseTime(args[1]);
                            State.SetGameTime(time);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                            Log.Error($"[Server] Failed to parse time while setting game time: {args[1]}");
                        }
                        break;
                    }
                case "setloadingtimes":
                    {
                        try
                        {
                            var time = ParseTime(args[1]);
                            State.LoadingTimes = time ?? TimeSpan.Zero;
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                            Log.Error($"[Server] Failed to parse time while setting loading times: {args[1]}");
                        }
                        break;
                    }
                case "addloadingtimes":
                    {
                        try
                        {
                            var time = ParseTime(args[1]);
                            State.LoadingTimes = State.LoadingTimes.Add(time ?? TimeSpan.Zero);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                            Log.Error($"[Server] Failed to parse time while adding loading times: {args[1]}");
                        }
                        break;
                    }
                case "pausegametime":
                    {
                        State.IsGameTimePaused = true;
                        break;
                    }
                case "unpausegametime":
                    {
                        AlwaysPauseGameTime = false;
                        State.IsGameTimePaused = false;
                        break;
                    }
                case "alwayspausegametime":
                    {
                        AlwaysPauseGameTime = true;
                        State.IsGameTimePaused = true;
                        break;
                    }
                case "getdelta":
                    {
                        var comparison = args.Length > 1 ? args[1] : State.CurrentComparison;
                        TimeSpan? delta = null;
                        if (State.CurrentPhase == TimerPhase.Running || State.CurrentPhase == TimerPhase.Paused)
                            delta = LiveSplitStateHelper.GetLastDelta(State, State.CurrentSplitIndex, comparison, State.CurrentTimingMethod);
                        else if (State.CurrentPhase == TimerPhase.Ended)
                            delta = State.Run.Last().SplitTime[State.CurrentTimingMethod] - State.Run.Last().Comparisons[comparison][State.CurrentTimingMethod];

                        // Defaults to "-" when delta is null, such as when State.CurrentPhase == TimerPhase.NotRunning
                        response = TimeFormatter.Format(delta);
                        break;
                    }
                case "getsplitindex":
                    {
                        var splitindex = State.CurrentSplitIndex;
                        response = splitindex.ToString();
                        break;
                    }
                case "getcurrentsplitname":
                    {
                        if (State.CurrentSplit != null)
                        {
                            response = State.CurrentSplit.Name;
                        }
                        else
                        {
                            response = "-";
                        }
                        break;
                    }
                case "getlastsplitname":
                case "getprevioussplitname":
                    {
                        if (State.CurrentSplitIndex > 0)
                        {
                            response = State.Run[State.CurrentSplitIndex - 1].Name;
                        }
                        else
                        {
                            response = "-";
                        }
                        break;
                    }
                case "getlastsplittime":
                case "getprevioussplittime":
                    {
                        if (State.CurrentSplitIndex > 0)
                        {
                            var time = State.Run[State.CurrentSplitIndex - 1].SplitTime[State.CurrentTimingMethod];
                            response = TimeFormatter.Format(time);
                        }
                        else
                        {
                            response = "-";
                        }
                        break;
                    }
                case "getcurrentsplittime":
                case "getcomparisonsplittime":
                    {
                        if (State.CurrentSplit != null)
                        {
                            var comparison = args.Length > 1 ? args[1] : State.CurrentComparison;
                            var time = State.CurrentSplit.Comparisons[comparison][State.CurrentTimingMethod];
                            response = TimeFormatter.Format(time);
                        }
                        else
                        {
                            response = "-";
                        }
                        break;
                    }
                case "getcurrentrealtime":
                    {
                        response = TimeFormatter.Format(GetCurrentTime(State, TimingMethod.RealTime));
                        break;
                    }
                case "getcurrentgametime":
                    {
                        var timingMethod = TimingMethod.GameTime;
                        if (!State.IsGameTimeInitialized)
                            timingMethod = TimingMethod.RealTime;
                        response = TimeFormatter.Format(GetCurrentTime(State, timingMethod));
                        break;
                    }
                case "getcurrenttime":
                    {
                        var timingMethod = State.CurrentTimingMethod;
                        if (timingMethod == TimingMethod.GameTime && !State.IsGameTimeInitialized)
                            timingMethod = TimingMethod.RealTime;
                        response = TimeFormatter.Format(GetCurrentTime(State, timingMethod));
                        break;
                    }
                case "getfinaltime":
                case "getfinalsplittime":
                    {
                        var comparison = args.Length > 1 ? args[1] : State.CurrentComparison;
                        var time = (State.CurrentPhase == TimerPhase.Ended)
                            ? State.CurrentTime[State.CurrentTimingMethod]
                            : State.Run.Last().Comparisons[comparison][State.CurrentTimingMethod];
                        response = TimeFormatter.Format(time);
                        break;
                    }
                case "getbestpossibletime":
                case "getpredictedtime":
                    {
                        string comparison;
                        if (command == "getbestpossibletime")
                            comparison = LiveSplit.Model.Comparisons.BestSegmentsComparisonGenerator.ComparisonName;
                        else
                            comparison = args.Length > 1 ? args[1] : State.CurrentComparison;
                        var prediction = PredictTime(State, comparison);
                        response = TimeFormatter.Format(prediction);
                        break;
                    }
                case "gettimerphase":
                case "getcurrenttimerphase":
                    {
                        response = State.CurrentPhase.ToString();
                        break;
                    }
                case "setcomparison":
                    {
                        State.CurrentComparison = args[1];
                        break;
                    }
                case "switchto":
                    {
                        switch (args[1])
                        {
                            case "gametime":
                                State.CurrentTimingMethod = TimingMethod.GameTime;
                                break;
                            case "realtime":
                                State.CurrentTimingMethod = TimingMethod.RealTime;
                                break;
                        }
                        break;
                    }
                case "setsplitname":
                case "setcurrentsplitname":
                    {
                        if (args.Length < 2)
                        {
                            Log.Error($"[Server] Command {command} incorrect usage: missing one or more arguments.");
                            break;
                        }

                        var index = State.CurrentSplitIndex;
                        var title = args[1];

                        if (command == "setsplitname")
                        {
                            var options = args[1].Split(new[] { ' ' }, 2);

                            if (options.Length < 2)
                            {
                                Log.Error($"[Server] Command {command} incorrect usage: missing one or more arguments.");
                                break;
                            }

                            if (!int.TryParse(options[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
                            {
                                Log.Error($"[Server] Could not parse {options[0]} as a split index while setting split name.");
                                break;
                            }

                            title = options[1];
                        }

                        if (index >= 0 && index < State.Run.Count)
                        {
                            State.Run[index].Name = title;
                            State.Run.HasChanged = true;
                        }
                        else
                        {
                            Log.Warning($"[Sever] Split index {index} out of bounds for command {command}");
                        }

                        break;
                    }
                case "ping":
                    {
                        response = "pong";
                        break;
                    }
                default:
                    {
                        Log.Error($"[Server] Invalid command: {message}");
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(response))
            {
                clientConnection.SendMessage(response);
            }
        }

        private void tcpConnection_Disconnected(object sender, EventArgs e)
        {
            var connection = (TcpConnection)sender;
            connection.MessageReceived -= connection_MessageReceived;
            connection.Disconnected -= tcpConnection_Disconnected;
            TcpConnections.Remove(connection);
            connection.Dispose();
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

        private TimeSpan? GetCurrentTime(LiveSplitState state, TimingMethod timingMethod)
        {
            if (state.CurrentPhase == TimerPhase.NotRunning)
                return state.Run.Offset;
            else
                return state.CurrentTime[timingMethod];
        }

        public void Dispose()
        {
            State.OnStart -= State_OnStart;
            StopAll();
            WaitingServerPipe.Dispose();
        }
    }
}
