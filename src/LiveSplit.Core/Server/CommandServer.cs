﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;

using WebSocketSharp.Server;

namespace LiveSplit.Server;

public class CommandServer
{
    public TcpListener Server { get; set; }
    public WebSocketServer WsServer { get; set; }
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
        PipeConnections = [];
        TcpConnections = [];
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

    public void StartWs()
    {
        StopWs();
        WsServer = new WebSocketServer(State.Settings.ServerPort);
        WsServer.AddWebSocketService("/livesplit", () => new WsConnection(connection_MessageReceived));
        WsServer.Start();
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
        StopWs();
    }

    public void StopTcp()
    {
        foreach (TcpConnection connection in TcpConnections)
        {
            connection.Dispose();
        }

        TcpConnections.Clear();
        Server?.Stop();
    }

    public void StopWs()
    {
        WsServer?.Stop();
    }

    public void StopPipe()
    {
        WaitingServerPipe?.Dispose();

        foreach (Connection connection in PipeConnections)
        {
            connection.Dispose();
        }

        PipeConnections.Clear();
    }

    public void AcceptTcpClient(IAsyncResult result)
    {
        try
        {
            TcpClient client = Server.EndAcceptTcpClient(result);
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

    private TimeSpan? ParseTime(string timeString)
    {
        if (timeString == "-")
        {
            return null;
        }

        return TimeSpanParser.Parse(timeString);
    }

    private void connection_MessageReceived(object sender, MessageEventArgs e)
    {
        ProcessMessage(e.Message, e.Connection);
    }

    private (string requestId, string commandText) ParseCommand(string commandString)
    {
        commandString = commandString.Trim();
        string[] parts = commandString.Split(['|'], 2);
        return parts.Length != 2
            ? ("", "")
            : (parts[0], parts[1]);
    }

    private void SetExceptionResponse(CommandResponse commandResponse, Exception e, string customMessage)
    {
        commandResponse.Success = false;
        commandResponse.State = "Exception";
        commandResponse.Data.Add("message", e.Message);
        if (!string.IsNullOrEmpty(customMessage))
        {
            commandResponse.Data.Add("customMessage", customMessage);
        }
    }

    private void SetFailedResponse(CommandResponse commandResponse, string message)
    {
        commandResponse.Success = false;
        commandResponse.State = "Failed";
        commandResponse.Data.Add("message", message);
    }
    private void ProcessMessage(string message, IConnection clientConnection)
    {
        string response = null;
        (string requestId, string commandText) = ParseCommand(message);
        string[] args = commandText.Split([' '], 2);
        string command = args[0];
        CommandResponse commandResponse = new CommandResponse();
        commandResponse.Command = command;
        commandResponse.RequestId = requestId;
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
                    TimeSpan? time = ParseTime(args[1]);
                    State.SetGameTime(time);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    string errMsg = $"Failed to parse time while setting game time: {args[1]}";
                    Log.Error($"[Server] {errMsg}");
                    SetExceptionResponse(commandResponse, e, errMsg);
                }

                break;
            }
            case "setloadingtimes":
            {
                try
                {
                    TimeSpan? time = ParseTime(args[1]);
                    State.LoadingTimes = time ?? TimeSpan.Zero;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    string errMsg = $"Failed to parse time while setting loading times: {args[1]}";
                    Log.Error($"[Server] {errMsg}");
                    SetExceptionResponse(commandResponse, e, errMsg);

                }

                break;
            }
            case "addloadingtimes":
            {
                try
                {
                    TimeSpan? time = ParseTime(args[1]);
                    State.LoadingTimes = State.LoadingTimes.Add(time ?? TimeSpan.Zero);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    string errMsg = $"Failed to parse time while adding loading times: {args[1]}";
                    Log.Error($"[Server] {errMsg}");
                    SetExceptionResponse(commandResponse, e, errMsg);
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
                string comparison = args.Length > 1 ? args[1] : State.CurrentComparison;
                TimeSpan? delta = null;
                if (State.CurrentPhase is TimerPhase.Running or TimerPhase.Paused)
                {
                    delta = LiveSplitStateHelper.GetLastDelta(State, State.CurrentSplitIndex, comparison, State.CurrentTimingMethod);
                }
                else if (State.CurrentPhase == TimerPhase.Ended)
                {
                    delta = State.Run.Last().SplitTime[State.CurrentTimingMethod] - State.Run.Last().Comparisons[comparison][State.CurrentTimingMethod];
                }

                // Defaults to "-" when delta is null, such as when State.CurrentPhase == TimerPhase.NotRunning
                commandResponse.Data.Add("time", TimeFormatter.Format(delta));

                break;
            }
            case "getsplitindex":
            {
                int splitindex = State.CurrentSplitIndex;
                commandResponse.Data.Add("index", splitindex.ToString(CultureInfo.InvariantCulture));
                break;
            }
            case "getcurrentsplitname":
            {
                if (State.CurrentSplit != null)
                {
                    commandResponse.Data.Add("name", State.CurrentSplit.Name);
                }
                else
                {
                    //commandResponse.Data.Add("name", "-");
                    commandResponse.State = "NoData";
                }

                break;
            }
            case "getlastsplitname":
            case "getprevioussplitname":
            {
                if (State.CurrentSplitIndex > 0)
                {
                    commandResponse.Data.Add("name", State.Run[State.CurrentSplitIndex - 1].Name);
                }
                else
                {
                    //commandResponse.Data.Add("name", "-");
                    commandResponse.State = "NoData";
                }

                break;
            }
            case "getlastsplittime":
            case "getprevioussplittime":
            {
                if (State.CurrentSplitIndex > 0)
                {
                    TimeSpan? time = State.Run[State.CurrentSplitIndex - 1].SplitTime[State.CurrentTimingMethod];
                    commandResponse.Data.Add("time", TimeFormatter.Format(time));
                }
                else
                {
                    //response = "-";
                    commandResponse.State = "NoData";
                }

                break;
            }
            case "getcurrentsplittime":
            case "getcomparisonsplittime":
            {
                if (State.CurrentSplit != null)
                {
                    string comparison = args.Length > 1 ? args[1] : State.CurrentComparison;
                    TimeSpan? time = State.CurrentSplit.Comparisons[comparison][State.CurrentTimingMethod];
                    commandResponse.Data.Add("time", TimeFormatter.Format(time));
                }
                else
                {
                    //response = "-";
                    commandResponse.State = "NoData";
                }

                break;
            }
            case "getcurrentrealtime":
            {
                commandResponse.Data.Add("time", TimeFormatter.Format(GetCurrentTime(State, TimingMethod.RealTime)));
                break;
            }
            case "getcurrentgametime":
            {
                TimingMethod timingMethod = TimingMethod.GameTime;
                if (!State.IsGameTimeInitialized)
                {
                    timingMethod = TimingMethod.RealTime;
                }

                commandResponse.Data.Add("time", TimeFormatter.Format(GetCurrentTime(State, timingMethod)));
                break;
            }
            case "getcurrenttime":
            {
                TimingMethod timingMethod = State.CurrentTimingMethod;
                if (timingMethod == TimingMethod.GameTime && !State.IsGameTimeInitialized)
                {
                    timingMethod = TimingMethod.RealTime;
                }

                commandResponse.Data.Add("time", TimeFormatter.Format(GetCurrentTime(State, timingMethod)));
                break;
            }
            case "getfinaltime":
            case "getfinalsplittime":
            {
                string comparison = args.Length > 1 ? args[1] : State.CurrentComparison;
                TimeSpan? time = (State.CurrentPhase == TimerPhase.Ended)
                    ? State.CurrentTime[State.CurrentTimingMethod]
                    : State.Run.Last().Comparisons[comparison][State.CurrentTimingMethod];
                commandResponse.Data.Add("time", TimeFormatter.Format(time));
                break;
            }
            case "getbestpossibletime":
            case "getpredictedtime":
            {
                string comparison;
                if (command == "getbestpossibletime")
                {
                    comparison = LiveSplit.Model.Comparisons.BestSegmentsComparisonGenerator.ComparisonName;
                }
                else
                {
                    comparison = args.Length > 1 ? args[1] : State.CurrentComparison;
                }

                TimeSpan? prediction = PredictTime(State, comparison);
                commandResponse.Data.Add("prediction", TimeFormatter.Format(prediction));
                break;
            }
            case "gettimerphase":
            case "getcurrenttimerphase":
            {
                commandResponse.Data.Add("phase", State.CurrentPhase.ToString());
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
                    string errMsg = $"Command {command} incorrect usage: missing one or more arguments.";
                    Log.Error($"[Server] {errMsg}");
                    SetFailedResponse(commandResponse, errMsg);
                    break;
                }

                int index = State.CurrentSplitIndex;
                string title = args[1];

                if (command == "setsplitname")
                {
                    string[] options = args[1].Split([' '], 2);

                    if (options.Length < 2)
                    {
                        string errMsg = $"Command {command} incorrect usage: missing one or more arguments.";
                        Log.Error($"[Server] {errMsg}");
                        SetFailedResponse(commandResponse, errMsg);
                        break;
                    }

                    if (!int.TryParse(options[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
                    {
                        string errMsg = $"Could not parse {options[0]} as a split index while setting split name.";
                        Log.Error($"[Server] {errMsg}");
                        SetFailedResponse(commandResponse, errMsg);
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
                    string warnMsg = $"Split index {index} out of bounds for command {command}";
                    Log.Warning($"[Sever] {warnMsg}");
                    commandResponse.State = "Warning";
                    commandResponse.Data.Add("message", warnMsg);
                }

                break;
            }
            case "getcustomvariablevalue":
            {
                string value = State.Run.Metadata.CustomVariableValue(args[1]);
                // make sure response isn't null or empty, and doesn't contain line endings
                response = string.IsNullOrEmpty(value) ? "-" : Regex.Replace(value, @"\r\n?|\n", " ");
                break;
            }
            case "ping":
            {
                commandResponse.State = "Pong";
                break;
            }
            case "getattemptcount":
            {
                response = Model.CurrentState.Run.AttemptCount.ToString();
                break;
            }
			case "getcompletedcount":
            {
                response = State.Run.AttemptHistory.Count(x => x.Time.RealTime != null).ToString();
                break;
            }
            default:
            {
                string errMsg = $"Invalid command: {command}";
                Log.Error($"[Server] {errMsg}");
                SetFailedResponse(commandResponse, errMsg);
                break;
            }
        }

        response = commandResponse.GetResponse();
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
        {
            State.IsGameTimePaused = true;
        }
    }

    private TimeSpan? PredictTime(LiveSplitState state, string comparison)
    {
        if (state.CurrentPhase is TimerPhase.Running or TimerPhase.Paused)
        {
            TimeSpan? delta = LiveSplitStateHelper.GetLastDelta(state, state.CurrentSplitIndex, comparison, State.CurrentTimingMethod) ?? TimeSpan.Zero;
            TimeSpan? liveDelta = state.CurrentTime[State.CurrentTimingMethod] - state.CurrentSplit.Comparisons[comparison][State.CurrentTimingMethod];
            if (liveDelta > delta)
            {
                delta = liveDelta;
            }

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
        {
            return state.Run.Offset;
        }
        else
        {
            return state.CurrentTime[timingMethod];
        }
    }

    public void Dispose()
    {
        State.OnStart -= State_OnStart;
        StopAll();
        WaitingServerPipe.Dispose();
    }
}
