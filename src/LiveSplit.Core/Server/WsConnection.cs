using WebSocketSharp.Server;

namespace LiveSplit.Server;

internal class WsConnection : WebSocketBehavior, IConnection
{
    private readonly MessageEventHandler _eventHandler;

    internal WsConnection(MessageEventHandler eventHandler) : base()
    {
        _eventHandler = eventHandler;
    }

    protected override void OnMessage(WebSocketSharp.MessageEventArgs e)
    {
        _eventHandler.Invoke(this, new MessageEventArgs(this, e.Data));
    }

    public void SendMessage(string message)
    {
        Send(message);
    }
}
