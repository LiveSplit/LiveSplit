namespace LiveSplit.Server
{
    public interface IScript
    {
        dynamic this[string name] { get; set; }
        dynamic Run();
    }
}
