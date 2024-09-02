namespace LiveSplit.Server;

public interface IConnection
{
    void SendMessage(string message);
}
