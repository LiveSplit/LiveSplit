namespace LiveSplit.Options;

public enum ServerStartupType
{
    Off = 0,
    TCP,
    Websocket,
    PreviousState
}

public enum ServerStateType
{
    Off = 0,
    TCP,
    Websocket
}
